namespace CRS.Core
{
    #region Usings

    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Validation;
    using System.Linq;

    using CRS.Database;
    using CRS.Logic;

    #endregion

    internal class DatabaseManager
    {
        private static DatabaseManager singelton;

        private readonly string m_vConnectionString;

        public static DatabaseManager Singelton
        {
            get
            {
                if (singelton == null)
                {
                    singelton = new DatabaseManager();
                }
                return singelton;
            }
        }

        public DatabaseManager()
        {
            this.m_vConnectionString = ConfigurationManager.AppSettings["databaseConnectionName"];
        }

        public void CreateAccount(Level l)
        {
            try
            {
                Debug.Write(
                    "[CRS]    Saving new account to database (player id: " + l.GetPlayerAvatar().GetId() + ")");
                using (ucsdbEntities ucsdbEntities = new ucsdbEntities())
                {
                    ucsdbEntities.player.Add(
                        new player
                            {
                                PlayerId = l.GetPlayerAvatar().GetId(),
                                AccountStatus = l.GetAccountStatus(),
                                AccountPrivileges = l.GetAccountPrivileges(),
                                LastUpdateTime = l.GetTime(),
                                IPAddress = l.GetIPAddress(),
                                Avatar = l.GetPlayerAvatar().SaveToJSON(),
                                GameObjects = l.SaveToJSON()
                            });
                    ucsdbEntities.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Debug.Write("[CRS]    An exception occured during CreateAccount processing :" + Debug.FlattenException(ex));
            }
        }

        public void CreateAlliance(Clan a)
        {
            try
            {
                using (ucsdbEntities ucsdbEntities = new ucsdbEntities())
                {
                    ucsdbEntities.clan.Add(
                        new clan { ClanId = a.GetAllianceId(), LastUpdateTime = DateTime.Now, Data = a.SaveToJSON() });
                    ucsdbEntities.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Debug.Write("[CRS]    An exception occured during CreateAlliance processing :" + Debug.FlattenException(ex));
            }
        }

        public Level GetAccount(long playerId)
        {
            Level level = null;
            try
            {
                using (ucsdbEntities ucsdbEntities = new ucsdbEntities())
                {
                    player player = ucsdbEntities.player.Find(playerId);
                    if (player != null)
                    {
                        level = new Level();
                        level.SetAccountStatus(player.AccountStatus);
                        level.SetAccountPrivileges(player.AccountPrivileges);
                        level.SetTime(player.LastUpdateTime);
                        level.SetIPAddress(player.IPAddress);
                        level.GetPlayerAvatar().LoadFromJSON(player.Avatar);
                        level.LoadFromJSON(player.GameObjects);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Write("[CRS]    An exception occured during GetAccount processing :" + ex);
            }
            return level;
        }

        public Clan GetAlliance(long allianceId)
        {
            Clan alliance = null;
            try
            {
                using (ucsdbEntities ucsdbEntities = new ucsdbEntities())
                {
                    clan clan = ucsdbEntities.clan.Find(allianceId);
                    if (clan != null)
                    {
                        alliance = new Clan();
                        alliance.LoadFromJSON(clan.Data);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Write("[CRS]    An exception occured during GetAlliance processing :" + Debug.FlattenException(ex));
            }
            return alliance;
        }

        public List<Clan> GetAllAlliances()
        {
            List<Clan> list = new List<Clan>();
            try
            {
                using (ucsdbEntities ucsdbEntities = new ucsdbEntities())
                {
                    foreach (clan current in ucsdbEntities.clan)
                    {
                        Clan alliance = new Clan();
                        alliance.LoadFromJSON(current.Data);
                        list.Add(alliance);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Write("[CRS]    An exception occured during GetAlliance processing:" + Debug.FlattenException(ex));
            }
            return list;
        }

        public long GetMaxAllianceId()
        {
            using (ucsdbEntities ucsdbEntities = new ucsdbEntities())
            {
                return ucsdbEntities.clan
                    .Select(a => (long?)a.ClanId)
                    .Max() ?? 0L;
            }
        }

        public List<long> GetAllPlayerIds()
        {
            List<long> list = new List<long>();
            using (ucsdbEntities ucsdbEntities = new ucsdbEntities())
            {
                foreach (player current in ucsdbEntities.player)
                {
                    list.Add(current.PlayerId);
                }
            }
            return list;
        }

        public ConcurrentDictionary<long, Level> GetAllPlayers()
        {
            ConcurrentDictionary<long, Level> players = new ConcurrentDictionary<long, Level>();
            try
            {
                using (ucsdbEntities db = new ucsdbEntities())
                {
                    DbSet<player> a = db.player;
                    int count = 0;
                    foreach (player c in a)
                    {
                        Level pl = new Level();
                        players.TryAdd(pl.GetPlayerAvatar().GetId(), pl);
                        if (count++ >= 100)
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLine("Error when try to get All Players: " + ex);
                Debug.Write("Error when try to get All Players: " + Debug.FlattenException(ex));
            }
            return players;
        }

        public long GetMaxPlayerId()
        {
            using (ucsdbEntities ucsdbEntities = new ucsdbEntities())
            {
                return ucsdbEntities.player
                    .Select(p => (long?)p.PlayerId)
                    .Max() ?? 0L;
            }
        }

        public void RemoveAlliance(Clan alliance)
        {
            long allianceId = alliance.GetAllianceId();
            using (ucsdbEntities ucsdbEntities = new ucsdbEntities())
            {
                ucsdbEntities.clan.Remove(ucsdbEntities.clan.Find((int)allianceId));
                ucsdbEntities.SaveChanges();
            }

            ObjectManager.RemoveInMemoryAlliance(allianceId);
        }

        public void Save(Level avatar)
        {
            ucsdbEntities ucsdbEntities = new ucsdbEntities();
            ucsdbEntities.Configuration.AutoDetectChangesEnabled = false;
            ucsdbEntities.Configuration.ValidateOnSaveEnabled = false;
            player player = ucsdbEntities.player.Find(avatar.GetPlayerAvatar().GetId());
            if (player != null)
            {
                player.AccountStatus = avatar.GetAccountStatus();
                player.AccountPrivileges = avatar.GetAccountPrivileges();
                player.LastUpdateTime = avatar.GetTime();
                player.IPAddress = avatar.GetIPAddress();
                player.Avatar = avatar.GetPlayerAvatar().SaveToJSON();
                player.GameObjects = avatar.SaveToJSON();
                ucsdbEntities.Entry(player).State = EntityState.Modified;
            }
            else
            {
                ucsdbEntities.player.Add(
                    new player
                        {
                            PlayerId = avatar.GetPlayerAvatar().GetId(),
                            AccountStatus = avatar.GetAccountStatus(),
                            AccountPrivileges = avatar.GetAccountPrivileges(),
                            LastUpdateTime = avatar.GetTime(),
                            IPAddress = avatar.GetIPAddress(),
                            Avatar = avatar.GetPlayerAvatar().SaveToJSON(),
                            GameObjects = avatar.SaveToJSON()
                        });
            }

            try
            {
                ucsdbEntities.SaveChanges();
            }
            catch (DbUpdateException dbUpdateException)
            {
                Debug.Write(Debug.FlattenException(dbUpdateException));
            }
            catch (DbEntityValidationException dbEntityValidationException)
            {
                Debug.Write(Debug.FlattenException(dbEntityValidationException));
            }
        }

        public void Save(List<Level> avatars)
        {
            try
            {
                using (ucsdbEntities ucsdbEntities = new ucsdbEntities())
                {
                    ucsdbEntities.Configuration.AutoDetectChangesEnabled = false;
                    ucsdbEntities.Configuration.ValidateOnSaveEnabled = false;
                    int num = 0;
                    foreach (Level current in avatars)
                    {
                        Level obj = current;
                        lock (obj)
                        {
                            player player = ucsdbEntities.player.Find(current.GetPlayerAvatar().GetId());
                            if (player != null)
                            {
                                player.AccountStatus = current.GetAccountStatus();
                                player.AccountPrivileges = current.GetAccountPrivileges();
                                player.LastUpdateTime = current.GetTime();
                                player.IPAddress = current.GetIPAddress();
                                player.Avatar = current.GetPlayerAvatar().SaveToJSON();
                                player.GameObjects = current.SaveToJSON();
                                ucsdbEntities.Entry(player).State = EntityState.Modified;
                            }
                            else
                            {
                                ucsdbEntities.player.Add(
                                    new player
                                        {
                                            PlayerId = current.GetPlayerAvatar().GetId(),
                                            AccountStatus = current.GetAccountStatus(),
                                            AccountPrivileges = current.GetAccountPrivileges(),
                                            LastUpdateTime = current.GetTime(),
                                            IPAddress = current.GetIPAddress(),
                                            Avatar = current.GetPlayerAvatar().SaveToJSON(),
                                            GameObjects = current.SaveToJSON()
                                        });
                            }
                        }
                    }
                    num++;
                    if (num >= 500)
                    {
                        ucsdbEntities.SaveChanges();
                        num = 0;
                    }
                    ucsdbEntities.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Debug.Write("[CRS]    An exception occured during Save processing for avatars :" + Debug.FlattenException(ex));
            }
        }

        public void Save(List<Clan> alliances)
        {
            try
            {
                using (ucsdbEntities ucsdbEntities = new ucsdbEntities())
                {
                    ucsdbEntities.Configuration.AutoDetectChangesEnabled = false;
                    ucsdbEntities.Configuration.ValidateOnSaveEnabled = false;
                    int num = 0;
                    foreach (Clan current in alliances)
                    {
                        Clan obj = current;
                        lock (obj)
                        {
                            clan clan = ucsdbEntities.clan.Find((int)current.GetAllianceId());
                            if (clan != null)
                            {
                                clan.LastUpdateTime = DateTime.Now;
                                clan.Data = current.SaveToJSON();
                                ucsdbEntities.Entry(clan).State = EntityState.Modified;
                            }
                            else
                            {
                                ucsdbEntities.clan.Add(
                                    new clan
                                        {
                                            ClanId = current.GetAllianceId(),
                                            LastUpdateTime = DateTime.Now,
                                            Data = current.SaveToJSON()
                                        });
                            }
                        }
                    }
                    num++;
                    if (num >= 500)
                    {
                        ucsdbEntities.SaveChanges();
                        num = 0;
                    }
                    ucsdbEntities.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Debug.Write("[CRS]    An exception occured during Save processing for alliances :" + Debug.FlattenException(ex));
            }
        }
    }
}