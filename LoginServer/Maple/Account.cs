using System;
using System.Data;
using Loki.Collections;
using Loki.Data;
using Loki.Interoperability;
using Loki.Net;

namespace Loki.Maple
{
    public class Account
    {
        public MapleClientHandler Client { get; private set; }

        public int ID { get; private set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }
        public string Pin { get; set; }
        public string Pic { get; set; }
        public DateTime Birthday { get; set; }
        public DateTime Creation { get; set; }
        public bool IsBanned { get; set; }
        public bool IsMaster { get; set; }
        public int MaplePoints { get; set; }
        public int PaypalNX { get; set; }
        public int CardNX { get; set; }

        private bool Assigned { get; set; }

        public bool IsLoggedIn
        {
            get
            {
                if (LoginServer.LoggedIn.Contains(this.ID))
                {
                    return true;
                }
                else
                {
                    MultiThreadedCollection<bool, int> collection = new MultiThreadedCollection<bool, int>(LoginServer.Channels.Count);

                    foreach (ChannelServerHandler loopChannel in LoginServer.Channels)
                    {
                        collection.AddFromThread(new Func<int, bool>(loopChannel.IsLoggedIn), this.ID);
                    }

                    collection.WaitUntilDone();

                    foreach (bool value in collection)
                    {
                        if (value)
                        {
                            return true;
                        }
                    }

                    return false;
                }
            }
            set
            {
                if (value && !LoginServer.LoggedIn.Contains(this.ID))
                {
                    LoginServer.LoggedIn.Add(this.ID);
                }
                else if (!value && LoginServer.LoggedIn.Contains(this.ID))
                {
                    LoginServer.LoggedIn.Remove(this.ID);
                }
            }
        }

        public Account(MapleClientHandler client)
        {
            this.Client = client;
        }

        public void Load(string username)
        {
            dynamic datum = new Datum("accounts");

            try
            {
                datum.Populate("Username = '{0}'", username);
            }
            catch (RowNotInTableException)
            {
                throw new NoAccountException();
            }

            this.ID = datum.ID;
            this.Assigned = true;

            this.Username = datum.Username;
            this.Password = datum.Password;
            this.Salt = datum.Salt;
            this.Pin = datum.Pin;
            this.Pic = datum.Pic;
            this.Birthday = datum.Birthday;
            this.Creation = datum.Creation;
            this.IsBanned = datum.IsBanned;
            this.IsMaster = datum.IsMaster;
            this.MaplePoints = datum.MaplePoints;
            this.PaypalNX = datum.PaypalNX;
            this.CardNX = datum.CardNX;
        }

        public void Save()
        {
            dynamic datum = new Datum("accounts");

            datum.Username = this.Username;
            datum.Password = this.Password;
            datum.Salt = this.Salt;
            datum.Pin = this.Pin;
            datum.Pic = this.Pic;
            datum.Birthday = this.Birthday;
            datum.Creation = this.Creation;
            datum.IsBanned = this.IsBanned;
            datum.IsMaster = this.IsMaster;
            datum.MaplePoints = this.MaplePoints;
            datum.PaypalNX = this.PaypalNX;
            datum.CardNX = this.CardNX;

            if (this.Assigned)
            {
                datum.Update("ID = '{0}'", this.ID);
            }
            else
            {
                datum.Insert();

                this.ID = Database.Fetch("accounts", "ID", "Username = '{0}'", this.Username);
                this.Assigned = true;
            }

            Log.Inform("Saved account '{0}' to database.", this.Username);
        }
    }
}