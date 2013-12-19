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
        private bool isLoggedIn;
        public bool IsBanned { get; set; }
        public bool IsMaster { get; set; }
        public int MaplePoints { get; set; }
        public int PaypalNX { get; set; }
        public int CardNX { get; set; }
        public bool LoggedIn { get; set; }

        private bool Assigned { get; set; }

        public bool IsLoggedIn
        {
            get
            {
                return this.isLoggedIn && !LoginServer.LoggedIn.Contains(this.ID);
            }
            set
            {
                this.isLoggedIn = value;

                if (value)
                {
                    LoginServer.LoggedIn.Add(this.ID);
                }
                else
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
            this.IsLoggedIn = datum.IsLoggedIn;
            this.IsBanned = datum.IsBanned;
            this.IsMaster = datum.IsMaster;
            this.MaplePoints = datum.MaplePoints;
            this.PaypalNX = datum.PaypalNX;
            this.CardNX = datum.CardNX;

            this.LoggedIn = false;
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
            datum.IsLoggedIn = this.IsLoggedIn;
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