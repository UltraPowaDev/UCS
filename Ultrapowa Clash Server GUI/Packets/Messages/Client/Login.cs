namespace CRS.Packets
{
    #region Usings

    using System;
    using System.Linq;
    using System.Security.Cryptography;

    using CRS.Core;
    using CRS.Core.Settings;
    using CRS.Extensions.Binary;
    using CRS.Extensions.List;
    using CRS.Library.Blake2b;
    using CRS.Logic;
    using CRS.Logic.Enums;
    using CRS.Network;

    #endregion

    internal class Login : Message
    {
        public const ushort PacketID = 10101;

        private long accountId;
        private string passToken;
        private int[] Version = new int[3]; // mayor, minor, build
        private string resourceSha;
        private string UDID;
        private string openUdid;
        private string macAddress;
        private string device;
        private string advertisingGuid;
        private string osVersion;
        private bool isAndroid;
        private string[] UnknownString = new string[3];
        private string androidID;
        private string preferredDeviceLanguage;
        private byte UnknownByte;
        private byte preferredLanguage;
        private string facebookAttributionId;
        private byte advertisingEnabled;

        private string appleIFV;

        private int appStore;

        private string kunlunSSO;

        private string kunlunUID;
        private byte UnknownByte2;

        public Login(Client _Client, Reader Reader, int[] _Header)
            : base(_Client, Reader, _Header)
        {
            this.Client.State = (int)State.LOGIN;
            this.Decrypt2();
        }

        /// <summary>
        /// Decrypt this instance.
        /// </summary>
        public override void Decrypt()
        {
            byte[] _Payload = this.GetData();
            this.Client.CPublicKey = _Payload.Take(32).ToArray();

            Blake2BHasher _Blake = new Blake2BHasher();

            _Blake.Update(this.Client.CPublicKey);
            _Blake.Update(Keys.Sodium.PublicKey);

            this.Client.CRNonce = _Blake.Finish();

            byte[] _Decrypted = Library.Sodium.Sodium.Decrypt(_Payload.Skip(32).ToArray(), this.Client.CRNonce, Keys.Sodium.PrivateKey, this.Client.CPublicKey);

            this.Client.CSNonce = _Decrypted.Skip(24).Take(24).ToArray();
            this.SetData(_Decrypted.Skip(48).ToArray());

            this.Reader = new Reader(this.GetData());
        }

        /// <summary>
        ///     <see cref="Decode" /> this instance.
        /// </summary>
        public override void Decode()
        {
            this.accountId = this.Reader.ReadInt64(); // User ID
            this.passToken = this.Reader.ReadString(); // User Token
            this.Version[0] = this.Reader.ReadVInt(); // MajorVersion
            this.Version[1] = this.Reader.ReadVInt(); // MinorVersion
            this.Version[2] = this.Reader.ReadVInt(); // Build Version
            this.resourceSha = this.Reader.ReadString(); // MasterHash

            this.UDID = this.Reader.ReadString(); // Udid

            this.openUdid = this.Reader.ReadString(); // OpenUDID

            this.macAddress = this.Reader.ReadString(); // Mac Address

            this.device = this.Reader.ReadString(); // Device Model
            this.advertisingGuid = this.Reader.ReadString(); // AGUID

            this.osVersion = this.Reader.ReadString(); // OS Version

            this.isAndroid = this.Reader.ReadBoolean(); // Android != iPhone
            this.UnknownString[0] = this.Reader.ReadString(); // Unknown

            this.androidID = this.Reader.ReadString(); // ADID
            this.preferredDeviceLanguage = this.Reader.ReadString() ?? string.Empty; // Region

            this.UnknownByte = this.Reader.ReadByte(); // Unknown

            this.preferredLanguage = this.Reader.ReadByte(); // Unknown

            this.facebookAttributionId = this.Reader.ReadString();
            this.advertisingEnabled = this.Reader.ReadByte();
            this.appleIFV = this.Reader.ReadString();
            this.appStore = this.Reader.ReadVInt();
            this.kunlunSSO = this.Reader.ReadString();
            this.kunlunUID = this.Reader.ReadString();
            this.UnknownString[1] = this.Reader.ReadString(); // Unknown
            this.UnknownString[2] = this.Reader.ReadString(); // Unknown
            this.UnknownByte2 = this.Reader.ReadByte(); // Unknown

            Debug.Write("accountId: " + this.accountId);
            Debug.Write("passToken: " + this.passToken);
            Debug.Write("MajorVersion: " + this.Version[0]);
            Debug.Write("MinorVersion: " + this.Version[1]);
            Debug.Write("BuildVersion: " + this.Version[2]);
            Debug.Write("resourceSha: " + this.resourceSha);
            Debug.Write("UDID: " + this.UDID);
            Debug.Write("openUdid: " + this.openUdid);
            Debug.Write("macAddress: " + this.macAddress);
            Debug.Write("device: " + this.device);
            Debug.Write("advertisingGuid: " + this.advertisingGuid);
            Debug.Write("osVersion: " + this.osVersion);
            Debug.Write("isAndroid: " + this.isAndroid);
            Debug.Write("UnknownString[0] : " + this.UnknownString[0]);
            Debug.Write("androidID: " + this.androidID);
            Debug.Write("preferredDeviceLanguage: " + this.preferredDeviceLanguage);
            Debug.Write("UnknownByte: " + this.UnknownByte);
            Debug.Write("preferredLanguage: " + this.preferredLanguage);
            Debug.Write("facebookAttributionId: " + this.facebookAttributionId);
            Debug.Write("advertisingEnabled: " + this.advertisingEnabled);
            Debug.Write("appleIFV: " + this.appleIFV);
            Debug.Write("appStore: " + this.appStore);
            Debug.Write("kunlunSSO: " + this.kunlunSSO);
            Debug.Write("kunlunUID: " + this.kunlunUID);
            Debug.Write("UnknownString[1]: " + this.UnknownString[1]);
            Debug.Write("UnknownString[2]: " + this.UnknownString[2]);
            Debug.Write("UnknownByte2: " + this.UnknownByte2);
            Debug.Write("BaseStream Position: " + this.Reader.BaseStream.Position);
        }

        /// <summary>
        ///     <see cref="Process" /> this instance.
        /// </summary>
        /// <param name="level">
        /// </param>
        public override void Process(Level level)
        {
            level = ResourcesManager.GetPlayer(this.accountId);

            if (Convert.ToBoolean(Constants.Patching))
            {
                if (this.resourceSha != ObjectManager.FingerPrint.sha)
                {
                    var p = new Authentification_Failed(this.Client);
                    p.SetErrorCode(CodesFail.NEW_VERSION);
                    p.SetResourceFingerprintData(ObjectManager.FingerPrint.SaveToJson());
                    p.SetContentURL(Constants.PatchURL);
                    p.SetUpdateURL("https://ClashRoyaleSpain.es/");
                    PacketManager.Send(p);
                    return;
                }
            }

            if (level == null)
            {
                level = ObjectManager.CreateAvatar(0, null);
                if (string.IsNullOrEmpty(this.passToken))
                {
                    byte[] tokenSeed = new byte[20];
                    new Random().NextBytes(tokenSeed);
                    using (SHA1 sha = new SHA1CryptoServiceProvider())
                    {
                        this.passToken = BitConverter.ToString(sha.ComputeHash(tokenSeed)).Replace("-", string.Empty);
                    }
                }
                level.GetPlayerAvatar().SetToken(this.passToken);
                level.GetPlayerAvatar().SetRegion(this.preferredDeviceLanguage.ToUpper());
                level.GetPlayerAvatar().SetCreated();
                level.GetPlayerAvatar().SetIsAndroid(this.isAndroid);
                DatabaseManager.Singelton.Save(level);
            }
            else
            {
                if (!string.Equals(level.GetPlayerAvatar().GetToken(), this.passToken, StringComparison.Ordinal))
                {
                    var p = new Authentification_Failed(this.Client);
                    p.SetErrorCode(CodesFail.DATA_BAD);
                    p.SetReason("We have some Problems with your Account. Please clean your App Data. https://ultrapowa.com/forum");
                    PacketManager.Send(p);
                    return;
                }

                if (level.GetAccountStatus() == (int)Status.BANNED)
                {
                    var p = new Authentification_Failed(this.Client);
                    p.SetErrorCode(CodesFail.BANNED);
                    PacketManager.Send(p);
                    return;
                }
            }
            this.Client.ClientSeed = this.Version[0];
            ResourcesManager.LogPlayerIn(level, this.Client);
            level.Tick();

            if (level.GetAccountPrivileges() > (int)Rank.Player)
            {
                level.GetPlayerAvatar().SetClanId(21);
            }

            if (level.GetAccountPrivileges() >= (int)Rank.Moderator)
            {
                level.GetPlayerAvatar().SetClanId(22);
            }

            level.GetPlayerAvatar().SetIsAndroid(this.isAndroid);
            level.GetPlayerAvatar().SetRegion(this.preferredDeviceLanguage.ToUpper());
            level.GetPlayerAvatar().SetUpdate(DateTime.UtcNow);

            var LoginOk = new LoginOk(this.Client);
            LoginOk.SetAccountId(level.GetPlayerAvatar().GetId());
            LoginOk.SetPassToken(level.GetPlayerAvatar().GetToken());
            LoginOk.SetServerMajorVersion(this.Version[0]);
            LoginOk.SetServerBuild(this.Version[1]);
            LoginOk.SetContentVersion(this.Version[2]);
            LoginOk.SetServerEnvironment("prod");
            LoginOk.SetDaysSinceStartedPlaying(10);
            LoginOk.SetServerTime(Math.Round(level.GetTime().Subtract(new DateTime(1970, 1, 1)).TotalSeconds * 1000.0).ToString());
            LoginOk.SetAccountCreatedDate(level.GetPlayerAvatar().GetDonations().ToString());
            LoginOk.SetStartupCooldownSeconds(0);
            LoginOk.SetCountryCode(level.GetPlayerAvatar().GetRegion().ToUpper());
            PacketManager.Send(LoginOk);
            if (level.GetPlayerAvatar().GetBattleID() > 0)
            {
                if (ResourcesManager.Battles.ContainsKey(level.GetPlayerAvatar().GetBattleID()))
                {
                    var SectorPC = new Sector_PC(this.Client)
                    {
                        Battle = ResourcesManager.Battles[level.GetPlayerAvatar().GetBattleID()]
                    };
                    PacketManager.Send(SectorPC);
                    this.Client.State = (int)State.IN_BATTLE;
                }
                else
                {
                    level.GetPlayerAvatar().SetBattleID(0);
                }
            }
            else
            {
                PacketManager.Send(new OwnHomeData(this.Client, level));
                if (level.GetPlayerAvatar().GetClan())
                {
                    PacketManager.Send(new Clan_Data(this.Client));
                }
            }
        }
    }
}
