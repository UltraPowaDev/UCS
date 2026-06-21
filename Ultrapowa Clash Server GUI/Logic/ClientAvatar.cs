namespace CRS.Logic
{
    #region Usings

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using CRS.Core.Settings;
    using CRS.Extensions.List;
    using CRS.Logic.Enums;
    using CRS.Logic.Slots;
    using CRS.Logic.Slots.Items;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    using Achievements = Slots.Achievements;
    using Resource = Slots.Items.Resource;
    using Shop = Slots.Items.Shop;

    #endregion

    internal class ClientAvatar : Avatar
    {
        //public struct AttackInfo
        //{
        //    public Level Defender;

        //    public Level Attacker;

        //    public int Lost;

        //    public int Reward;
        //}

        private Arena m_Arena;

        private bool m_Banned;

        private DateTime m_BanTime;

        private int m_BattleID;

        private byte m_Changes;

        private bool m_Clan;

        private long m_ClanID;

        private DateTime m_Created;

        private int m_CurrentGems;

        private int m_Donations;

        private int m_Experience;

        private int m_FreeGems;

        private long m_Id;

        private bool m_IsAndroid;

        private int m_League;

        private int m_Legendary_Trophies;

        private int m_Level;

        private int m_Loses;

        private bool m_Muted;

        private DateTime m_MuteTime;

        private string m_Name;

        private string m_Pass;

        private TimeSpan m_PlayTime;

        private string m_Region;

        private int m_Report;

        private int m_Score;

        private string m_Token;

        private int m_Trophies;

        private DateTime m_Update;

        private int m_Wins;

        private byte m_vNameChangingLeft;

        private byte m_vnameChosenByUser;

        //public ClientAvatar() : base()
        //{
        //this.Achievements = new Achievements();
        //this.Resources = new Resources();
        //this.Boutique = new Boutique();
        //this.Chests = new Chests();
        //this.Deck = new Deck();

        // this.AllianceUnits = new List<DataSlot>();
        // this.NpcStars = new List<DataSlot>();
        // this.NpcLootedGold = new List<DataSlot>();
        // this.NpcLootedElixir = new List<DataSlot>();
        // this.Arena = new List<DataSlot>();
        // m_LeagueId = 9;
        //}

        public ClientAvatar()
        {
            //Achievements = new List<DataSlot>();
            //AchievementsUnlocked = new List<DataSlot>();
            //AllianceUnits = new List<TroopDataSlot>();
            //NpcStars = new List<DataSlot>();
            //NpcLootedGold = new List<DataSlot>();
            //NpcLootedElixir = new List<DataSlot>();
            //BookmarkedClan = new List<BookmarkSlot>();
            //DonationSlot = new List<DonationSlot>();
            //QuickTrain1 = new List<DataSlot>();
            //QuickTrain2 = new List<DataSlot>();
            //QuickTrain3 = new List<DataSlot>();
            //AttackingInfo = new Dictionary<long, AttackInfo>(); 
        }

        /// <summary>
        ///     Initialize a new instance of the <see cref="ClientAvatar" />
        ///     class.
        /// </summary>
        /// <param name="Id">The player identifier.</param>
        /// <param name="token"></param>
        public ClientAvatar(long Id, string token) : this()
        {
            this.m_Id = Id;
            this.m_Token = token;
            this.LastUpdate = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
            this.Login = Id + ((int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds).ToString();
            this.m_vnameChosenByUser = 0x00;
            this.m_vNameChangingLeft = 2;
            this.m_Level = Settings.StartingLevel;
            this.m_Experience = Settings.StartingExperience;
            this.m_ClanID = 0;
            this.m_Trophies = Settings.StartingTrophies;
            //this.TutorialStepsCount = 0x00; // 0x0A 10
            this.TutorialStepsCount = 10u;
            this.m_Name = "NoNameYet";
            this.m_Arena = Arena.ARENA_L;
            this.m_Changes = 0x00;

            this.Resources.Set(Enums.Resource.MAX_TROPHIES, this.m_Trophies);
            this.Resources.Set(Enums.Resource.CARD_COUNT, this.Deck.Count);
            //this.SetResourceCount(ObjectManager.DataTables.GetResourceByName("Gold"), Settings.StartingGold);
            //this.SetResourceCount(ObjectManager.DataTables.GetResourceByName("Diamonds"), Settings.StartingGems);
            this.Boutique.Add(new Shop(1, 0, 26, 39, 0, DateTime.Today.AddDays(1)));
            this.Boutique.Add(new Shop(4, 0, 26, 0, 0, DateTime.Today.AddDays(1)));
            this.Boutique.Add(new Shop(1, 0, 26, 1, 0, DateTime.Today.AddDays(1)));
            this.Boutique.Add(new Shop(1, 0, 26, 2, 0, DateTime.Today.AddDays(1)));
            this.Boutique.Add(new Shop(4, 1, 26, 3, 0, DateTime.Today.AddDays(1)));
            this.Boutique.Add(new Shop(1, 2, 26, 4, 0, DateTime.Today.AddDays(1)));
            this.Boutique.Add(new Shop(3, 0, 0, 0, 0, DateTime.Today.AddDays(1)));
        }

        public Achievements Achievements = new Achievements();

        public Resources Resources = new Resources();

        public Boutique Boutique = new Boutique();

        public Chests Chests = new Chests();

        public Deck Deck = new Deck();

        //public Achievements Achievements { get; set; }

        //public Resources Resources { get; set; }

        //public Boutique Boutique { get; set; }

        //public Chests Chests { get; set; }

        //public Deck Deck { get; set; }

        public int LastUpdate { get; set; }

        public string Login { get; set; }

        public uint TutorialStepsCount { get; set; }

        //public Dictionary<long, AttackInfo> AttackingInfo { get; set; }

        //public int State { get; set; }

        public byte[] Data_Part1()
        {
            int TimeStamp = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;

            List<byte> _Packet = new List<byte>();

            _Packet.AddLong(this.m_Id);

            _Packet.Add(16);
            _Packet.Add(0);
            _Packet.AddVInt(1698340);
            _Packet.AddVInt(1727920);
            _Packet.AddVInt(TimeStamp);
            _Packet.Add(0);

            _Packet.Add(1);
            {
                _Packet.AddVInt(8);

                foreach (Card _Card in this.Deck.GetRange(0, 8))
                {
                    _Packet.AddVInt(_Card.GlobalID);
                }
            }

            _Packet.Add(255);
            _Packet.AddRange(this.Deck.ToBytes());

            _Packet.AddVInt(this.Deck.Count - 8);
            foreach (Card _Card in this.Deck.Skip(8))
            {
                _Packet.AddVInt(_Card.Type);
                _Packet.AddVInt(_Card.ID);
                _Packet.AddVInt(_Card.Level);
                _Packet.AddVInt(0);
                _Packet.AddVInt(_Card.Count);
                _Packet.AddVInt(0);
                _Packet.AddVInt(0);
                _Packet.AddVInt(_Card.New);
            }

            _Packet.Add(0);
            _Packet.Add(4);

            _Packet.AddRange(this.Chests.Encode());

            _Packet.AddVInt(287600);
            _Packet.AddVInt(288000);

            _Packet.AddVInt(TimeStamp);

            _Packet.AddVInt(0);
            _Packet.AddVInt(0);
            _Packet.AddVInt(127);

            _Packet.AddVInt(0);
            _Packet.AddVInt(0);
            _Packet.AddVInt(0);

            _Packet.AddVInt(5); // Crown
            _Packet.AddVInt(0); // 0 = Unlocked    1 = locked
            _Packet.AddVInt(360 * 20); // Time from unlock crown chest

            _Packet.AddRange("A4F4D201".HexaToBytes());
            _Packet.AddVInt(TimeStamp);
            _Packet.AddVInt(0);

            _Packet.AddRange("A4F4D201".HexaToBytes());
            _Packet.AddVInt(TimeStamp);
            _Packet.AddVInt(0);

            _Packet.AddVInt(0);
            _Packet.AddVInt(0);
            _Packet.AddVInt(127);

            _Packet.AddVInt(1); // 0 = Tuto Upgrade Spell
            _Packet.AddVInt(0);
            _Packet.AddVInt(0);
            _Packet.AddVInt(0);
            _Packet.AddVInt(0);
            _Packet.AddVInt(0);
            _Packet.AddVInt(0);
            _Packet.AddVInt(0);
            _Packet.AddVInt(2); // 0, 1 = Animation Page Card (Tuto)

            _Packet.AddVInt(this.m_Level);
            _Packet.Add(0x36);
            _Packet.AddVInt((int)this.m_Arena);

            _Packet.AddVInt(736968123); // Shop ID
            _Packet.AddVInt((int)DateTime.UtcNow.DayOfWeek + 1);
            _Packet.AddVInt((int)this.m_Update.DayOfWeek + 1);

            int _Time = (int)(DateTime.UtcNow.AddDays(1) - DateTime.UtcNow).TotalSeconds;
            _Packet.AddVInt(20 * _Time);
            _Packet.AddVInt(20 * _Time);

            _Packet.AddVInt(TimeStamp);

            _Packet.AddRange(this.Boutique.EncodeCard());
            _Packet.AddRange(this.Boutique.EncodeOffer());
            _Packet.AddRange(new byte[]
            {
                0x00, 0x00, 0x7F,
                0x00, 0x00, 0x7F,
                0x00, 0x00, 0x7F
            });

            _Packet.AddInt(0);
            _Packet.AddInt(0);
            _Packet.AddInt(9);
            _Packet.AddInt(0);

            _Packet.AddRange("F801".HexaToBytes()); // Prefixe from Deck

            _Packet.AddRange(new byte[]
            {
                0x1A, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00,
                0x1A, 0x01, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00,
                0x1A, 0x0D, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00,
                0x1C, 0x01, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00,
                0x1C, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00,
                0x1A, 0x03, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00
            });

            _Packet.AddVInt(TimeStamp);
            _Packet.AddVInt(0);
            _Packet.AddVInt(0);
            _Packet.AddInt(1);
            _Packet.AddVInt(TimeStamp);

            return _Packet.ToArray();
        }

        /// <summary>
        /// Encode the second part.
        /// </summary>
        /// <returns>THe encoded second part.</returns>
        public byte[] Data_Part2()
        {
            List<byte> _Packet = new List<byte>();

            _Packet.AddVInt(this.m_Id);
            _Packet.AddVInt(this.m_Id);
            _Packet.AddVInt(this.m_Id);

            _Packet.AddString(this.m_Name);
            _Packet.AddVInt(this.m_Changes);
            _Packet.AddVInt(0x36); // Arena Data
            _Packet.AddVInt((int)this.m_Arena);
            _Packet.AddVInt(this.m_Trophies);

            _Packet.AddInt(0);

            _Packet.Add(0);
            _Packet.AddVInt(0); // Rank
            _Packet.AddVInt(this.m_Trophies);
            _Packet.AddVInt(this.m_Trophies); // Legendary Trophies

            _Packet.AddVInt(this.Resources.Count);
            _Packet.AddVInt(this.Resources.Count);

            foreach (Resource _Resource in this.Resources.OrderBy(r => r.Data))
            {
                _Packet.AddVInt(_Resource.Type);
                _Packet.AddVInt(_Resource.Data);
                _Packet.AddVInt(_Resource.Value);
            }

            _Packet.Add(0);

            _Packet.AddVInt(this.Achievements.Count);
            foreach (Achievement _Achievement in this.Achievements)
            {
                _Packet.AddVInt(_Achievement.Type);
                _Packet.AddVInt(_Achievement.Data);
                _Packet.AddVInt(_Achievement.Value);
            }

            _Packet.AddVInt(0); // Completed Achievements
            _Packet.AddVInt(0); // Unknown Count

            _Packet.Add(0);

            _Packet.Add(0);

            _Packet.AddVInt(this.Resources[0].Value);
            _Packet.AddVInt(this.Resources[0].Value);
            _Packet.AddVInt(this.m_Experience);
            _Packet.AddVInt(this.m_Level);

            _Packet.Add(0);

            if (this.m_Clan)
            {
                // 8 = Set name popup + clan
                // 9 = Name already set + clan
                // < 8 =  Set name popup

                _Packet.Add(9);

                _Packet.AddVInt(this.m_ClanID);
                _Packet.AddString("Clash");
                _Packet.AddVInt(0x10);
                _Packet.AddVInt(16);
            }
            else
            {
                _Packet.Add(0);
            }

            _Packet.Add(this.m_vnameChosenByUser);

            _Packet.Add(0);
            _Packet.Add(0);
            _Packet.Add(0);
            _Packet.Add(0);
            _Packet.Add(0);

            _Packet.AddVInt(this.TutorialStepsCount); //tutorial

            _Packet.Add(0);
            _Packet.Add(0);

            return _Packet.ToArray();
        }

        public void LoadFromJSON(string jsonString)
        {
            JObject _Json = JObject.Parse(jsonString);

            this.m_Id = _Json["player_id"]?.ToObject<long>() ?? 1L;
            this.m_Id = _Json["home_id"]?.ToObject<long>() ?? this.m_Id;
            this.m_Id = _Json["backup_id"]?.ToObject<long>() ?? this.m_Id;
            this.m_ClanID = _Json["clan_id"]?.ToObject<long>() ?? 1L;

            this.m_Token = _Json["token"].ToObject<string>();
            this.m_Pass = _Json["password"].ToObject<string>();
            this.m_Name = _Json["name"].ToObject<string>();
            this.m_Region = _Json["region"].ToObject<string>();

            this.m_Level = _Json["level"].ToObject<int>();
            this.m_Experience = _Json["experience"].ToObject<int>();
            this.m_Score = _Json["score"].ToObject<int>();
            this.m_CurrentGems = _Json["gems"].ToObject<int>();
            this.m_Wins = _Json["wins"].ToObject<int>();
            this.m_Loses = _Json["loses"].ToObject<int>();
            this.m_Report = _Json["report"].ToObject<int>();
            this.m_Donations = _Json["donations"].ToObject<int>();

            this.m_Arena = (Arena)_Json["arena"].ToObject<int>();

            this.TutorialStepsCount = _Json["tutorial"].ToObject<byte>();
            this.m_Changes = _Json["changes"].ToObject<byte>();
            this.m_vNameChangingLeft = _Json["nameChangesLeft"].ToObject<byte>();
            this.m_vnameChosenByUser = _Json["name_set"].ToObject<byte>();

            this.m_IsAndroid = _Json["android"].ToObject<bool>();
            this.m_Clan = _Json["clan"].ToObject<bool>();
            this.m_Banned = _Json["banned"].ToObject<bool>();
            this.m_Muted = _Json["muted"].ToObject<bool>();

            this.m_Update = _Json["update"].ToObject<DateTime>();
            this.m_Created = _Json["created"].ToObject<DateTime>();
            this.m_BanTime = _Json["ban_time"].ToObject<DateTime>();
            this.m_MuteTime = _Json["mute_time"].ToObject<DateTime>();

            //var jsonResources = (JArray)_Json["resources"];
            //foreach (JObject resource in jsonResources)
            //{
            //    var ds = new DataSlot(null, 0);
            //    ds.Load(resource);
            //    this.GetResources().Add(ds);
            //}
        }

        /// <summary>
        ///     Serialize this instance.
        /// </summary>
        /// <returns>
        ///     The player data in JSON.
        /// </returns>
        public string SaveToJSON()
        {
            JObject _JSON = new JObject();

            _JSON.Add("player_id", this.GetId());
            _JSON.Add("home_id", this.GetId());
            _JSON.Add("backup_id", this.GetId());
            _JSON.Add("clan_id", this.GetClanID());

            _JSON.Add("token", this.GetToken());
            _JSON.Add("password", this.GetPass());
            _JSON.Add("name", this.GetName());
            _JSON.Add("region", this.GetRegion());

            _JSON.Add("level", this.GetLevel());
            _JSON.Add("experience", this.GetExp());
            _JSON.Add("score", this.GetScore());
            _JSON.Add("gems", this.GetDiamonds());
            _JSON.Add("wins", this.GetWins());
            _JSON.Add("loses", this.GetLoses());
            _JSON.Add("report", this.GetReport());
            _JSON.Add("donations", this.GetDonations());

            _JSON.Add("arena", (int)this.GetArena());

            _JSON.Add("tutorial", this.GetTutorial());
            _JSON.Add("changes", this.GetChanges());
            _JSON.Add("nameChangesLeft", this.m_vNameChangingLeft);
            _JSON.Add("name_set", this.m_vnameChosenByUser);

            _JSON.Add("android", this.m_IsAndroid);
            _JSON.Add("clan", this.m_Clan);
            _JSON.Add("banned", this.m_Banned);
            _JSON.Add("muted", this.m_Muted);

            _JSON.Add("update", this.m_Update);
            _JSON.Add("created", this.m_Created);
            _JSON.Add("ban_time", this.m_BanTime);
            _JSON.Add("mute_time", this.m_MuteTime);

            //var jsonResourcesArray = new JArray();
            //foreach (var resource in this.GetResources())
            //    jsonResourcesArray.Add(resource.Save(new JObject()));
            //_JSON.Add("resources", jsonResourcesArray);

            return JsonConvert.SerializeObject(_JSON);
        }

        public long GetId() => this.m_Id;

        public void SetClanId(long id) => this.m_ClanID = id;

        public long GetClanID() => this.m_ClanID;

        public void SetBattleID(int battleID) => this.m_BattleID = battleID;

        public int GetBattleID() => this.m_BattleID;

        public void SetLevel(int level) => this.m_Level = level;

        public int GetLevel() => this.m_Level;

        //public void AddExperience(int exp)
        //{
        //    m_Experience += exp;
        //    var experienceCap =
        //        ((Exp_Levels)ObjectManager.DataTables.GetTable(13).GetDataByName(this.m_Level.ToString())).ExpToNextLevel;
        //    if (this.m_Experience >= experienceCap)
        //        if (ObjectManager.DataTables.GetTable(13).GetItemCount() > m_Level + 1)
        //        {
        //            m_Level += 1;
        //            m_Experience = m_Experience - experienceCap;
        //        }
        //        else
        //            m_Experience = 0;
        //}

        public int GetExp() => this.m_Experience;

        public void SetTrophies(int trophies) => this.m_Trophies = trophies;

        public int GetTrophies() => this.m_Trophies;

        public void SetLegenTrophies(int legenTrophies) => this.m_Legendary_Trophies = legenTrophies;

        public int GetLegenTrophies() => this.m_Legendary_Trophies;

        public void SetWins(int wins) => this.m_Wins = wins;

        public int GetWins() => this.m_Wins;

        public void SetLoses(int loses) => this.m_Loses = loses;

        public int GetLoses() => this.m_Loses;

        public void SetReport(int report) => this.m_Report = report;

        public int GetReport() => this.m_Report;

        public void SetDonations(int donations) => this.m_Donations = donations;

        public int GetDonations() => this.m_Donations;

        public void SetTutorial(byte tutorial) => this.TutorialStepsCount = tutorial;

        public uint GetTutorial() => this.TutorialStepsCount;

        public void SetChanges(byte changes) => this.m_Changes = changes;

        public int GetChanges() => this.m_Changes;

        public void SetArena(Arena arena) => this.m_Arena = arena;

        public Arena GetArena() => this.m_Arena;

        public void SetToken(string token) => this.m_Token = token;

        public string GetToken() => this.m_Token;

        public void SetPass(string token) => this.m_Pass = token;

        public string GetPass() => this.m_Pass;

        public void SetName(string name)
        {
            this.m_Name = name;
            this.TutorialStepsCount = 0;

            if (this.m_vnameChosenByUser == 0x01)
            {
                this.m_vNameChangingLeft = 0x01;
            }
            else
            {
                this.m_vNameChangingLeft = 0x02;
            }

            this.TutorialStepsCount = 0x0D; //13
        }

        public void SetNameSet(byte nameSet) => this.m_vnameChosenByUser = nameSet;

        public byte GetNameSet() => this.m_vnameChosenByUser;

        public string GetName() => this.m_Name;

        public void SetRegion(string region) => this.m_Region = region;

        public string GetRegion() => this.m_Region;

        public void SetIsAndroid(bool isAndroid) => this.m_IsAndroid = isAndroid;

        public bool GetIsAndroid() => this.m_IsAndroid;

        public void SetClan(bool clan) => this.m_Clan = clan;

        public bool GetClan() => this.m_Clan;

        public void SetBanned(bool banned) => this.m_Banned = banned;

        public bool GetBanned() => this.m_Banned;

        public void SetMuted(bool muted) => this.m_Muted = muted;

        public bool GetMuted() => this.m_Muted;

        public void SetCreated() => this.m_Created = DateTime.Now;

        public DateTime GetAccountCreationDate() => this.m_Created;

        public void SetDiamonds(int count) => this.m_CurrentGems = count;

        public int GetDiamonds() => this.m_CurrentGems;

        public void SetScore(int score) => this.m_Score = score;

        public int GetScore() => this.m_Score;

        public void UseDiamonds(int diamondCount) => this.m_CurrentGems -= diamondCount;

        public void SetUpdate(DateTime update) => this.m_Update = update;

        public DateTime GetUpdate() => this.m_Update;

        public int GetSecondsFromLastUpdate()
            => (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds - this.LastUpdate;

        public bool HasEnoughDiamonds(int diamondCount) => this.m_CurrentGems >= diamondCount;
    }
}
