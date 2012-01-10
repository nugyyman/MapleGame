using Loki.Maple.Characters;
using Loki.Maple.Maps;
using Loki.Net;

namespace Loki.Maple.Interaction
{
    public class Omok : MapObject, ISpawnable
    {
        public Character Owner { get; private set; }
        public Character Visitor { get; private set; }
        public string Description { get; private set; }
        public bool IsStarted { get; private set; }
        public bool IsReady { get; private set; }
        public int Type { get; private set; }
        public int Loser { get; private set; }
        public int Turn { get; private set; }
        public int[,] Pieces { get; private set; }

        public Omok(Character owner, string description, int type)
        {
            this.Owner = owner;
            this.Description = description;
            this.Type = type;
            this.Pieces = new int[15, 15];
            this.IsStarted = false;
            this.IsReady = false;
            this.Loser = 1;
            this.Turn = 0;

            using (Packet outPacket = new Packet(MapleServerOperationCode.PlayerInteraction))
            {
                outPacket.WriteBytes(05, 01, 02);
                outPacket.WriteBool(false);
                outPacket.WriteByte(0);
                outPacket.WriteBytes(this.Owner.AppearanceToByteArray());
                outPacket.WriteString(this.Owner.Name);
                outPacket.WriteByte(0xFF);
                outPacket.WriteByte(0);
                outPacket.WriteInt(1);
                outPacket.WriteInt(0); // Wins
                outPacket.WriteInt(0); // Ties
                outPacket.WriteInt(0); // Losses
                outPacket.WriteInt(2000);
                outPacket.WriteByte(0xFF);
                outPacket.WriteString(this.Description);
                outPacket.WriteByte((byte)this.Type);
                outPacket.WriteByte(0);

                this.Owner.Client.Send(outPacket);
            }

            this.Owner.Map.Omoks.Add(this);
        }

        public void Handle(Character player, InteractionCode action, Packet inPacket)
        {
            switch (action)
            {
                case InteractionCode.Start:

                    if (this.IsReady)
                    {
                        this.IsStarted = true;
                        this.IsReady = false;

                        using (Packet outPacket = new Packet(MapleServerOperationCode.PlayerInteraction))
                        {
                            outPacket.WriteByte((byte)InteractionCode.Start);
                            outPacket.WriteByte((byte)this.Loser);

                            this.Owner.Client.Send(outPacket);
                            this.Visitor.Client.Send(outPacket);
                        }
                    }

                    break;

                case InteractionCode.Exit:

                    if (this.Owner == player)
                    {
                        this.Close();
                    }
                    else
                    {
                        this.Visitor.Omok = null;
                        this.Visitor = null;
                    }

                    break;

                case InteractionCode.Chat:
                    string chat = inPacket.ReadString();

                    using (Packet outPacket = new Packet(MapleServerOperationCode.PlayerInteraction))
                    {
                        outPacket.WriteBytes(0x06, 0x08);
                        outPacket.WriteBool(player != this.Owner);
                        outPacket.WriteString(player.Name + " : " + chat);

                        this.Owner.Client.Send(outPacket);
                        this.Visitor.Client.Send(outPacket);
                    }

                    break;

                case InteractionCode.Skip:

                    using (Packet outPacket = new Packet(MapleServerOperationCode.PlayerInteraction))
                    {
                        outPacket.WriteByte((byte)InteractionCode.Skip);
                        outPacket.WriteBool(player == this.Owner);

                        this.Owner.Client.Send(outPacket);
                        this.Visitor.Client.Send(outPacket);
                    }

                    break;

                case InteractionCode.MoveOmok:
                    int x = inPacket.ReadInt();
                    int y = inPacket.ReadInt();
                    int type = inPacket.ReadByte(); // piece ( 1 or 2; Owner has one piece, visitor has another, it switches every game.)

                    //TODO: Check to see if its their turn

                    this.Pieces[x, y] = type;

                    using (Packet outPacket = new Packet(MapleServerOperationCode.PlayerInteraction))
                    {
                        outPacket.WriteByte((byte)InteractionCode.MoveOmok);
                        outPacket.WriteInt(x);
                        outPacket.WriteInt(y);
                        outPacket.WriteByte((byte)type);

                        this.Owner.Client.Send(outPacket);
                        this.Visitor.Client.Send(outPacket);
                    }

                    this.CheckForCombo(player, type);

                    break;

                case InteractionCode.Ready:
                    this.IsReady = true;

                    using (Packet outPacket = new Packet(MapleServerOperationCode.PlayerInteraction))
                    {
                        outPacket.WriteByte((byte)InteractionCode.Ready);

                        this.Owner.Client.Send(outPacket);
                        this.Visitor.Client.Send(outPacket);
                    }

                    break;

                case InteractionCode.UnReady:
                    this.IsReady = false;

                    using (Packet outPacket = new Packet(MapleServerOperationCode.PlayerInteraction))
                    {
                        outPacket.WriteByte((byte)InteractionCode.UnReady);

                        this.Owner.Client.Send(outPacket);
                        this.Visitor.Client.Send(outPacket);
                    }

                    break;

                case InteractionCode.RequestTie:

                    using (Packet outPacket = new Packet(MapleServerOperationCode.PlayerInteraction))
                    {
                        outPacket.WriteByte(0x2C);

                        if (player == this.Owner)
                        {
                            this.Visitor.Client.Send(outPacket);
                        }
                        else
                        {
                            this.Owner.Client.Send(outPacket);
                        }
                    }

                    break;

                case InteractionCode.AnswerTie:
                    int status = inPacket.ReadByte();

                    using (Packet outPacket = new Packet(MapleServerOperationCode.PlayerInteraction))
                    {
                        outPacket.WriteByte(0x2C);

                        if (player == this.Owner)
                        {
                            this.Visitor.Client.Send(outPacket);
                        }
                        else
                        {
                            this.Owner.Client.Send(outPacket);
                        }
                    }

                    break;

                case InteractionCode.GiveUp:

                    if (player == this.Owner)
                    {
                        this.EndGame(this.Visitor, false, true);
                    }
                    else
                    {
                        this.EndGame(this.Owner, false, true);
                    }

                    break;
            }
        }

        public void AddVisitor(Character player)
        {
            this.Visitor = player;

            using (Packet outPacket = new Packet(MapleServerOperationCode.PlayerInteraction))
            {
                outPacket.WriteByte(0x04);
                outPacket.WriteByte(0x01);
                outPacket.WriteBytes(this.Visitor.AppearanceToByteArray());
                outPacket.WriteString(this.Visitor.Name);
                outPacket.WriteInt(1);
                outPacket.WriteInt(0); // Wins
                outPacket.WriteInt(0); // Ties
                outPacket.WriteInt(0); // Losses
                outPacket.WriteInt(2000); // Points

                this.Owner.Client.Send(outPacket);
            }

            using (Packet outPacket = new Packet(MapleServerOperationCode.PlayerInteraction))
            {
                outPacket.WriteBytes(05, 01, 02);
                outPacket.WriteBool(true);
                outPacket.WriteByte();
                outPacket.WriteBytes(this.Owner.AppearanceToByteArray());
                outPacket.WriteString(this.Owner.Name);
                outPacket.WriteByte(1);
                outPacket.WriteBytes(this.Visitor.AppearanceToByteArray());
                outPacket.WriteString(this.Visitor.Name);
                outPacket.WriteByte(0xFF);

                outPacket.WriteBool(false);
                outPacket.WriteInt(1);
                outPacket.WriteInt(0); // Wins
                outPacket.WriteInt(0); // Ties
                outPacket.WriteInt(0); // Losses
                outPacket.WriteInt(2000); // Points

                outPacket.WriteBool(true);
                outPacket.WriteInt(1);
                outPacket.WriteInt(0); // Wins
                outPacket.WriteInt(0); // Ties
                outPacket.WriteInt(0); // Losses
                outPacket.WriteInt(2000); // Points

                outPacket.WriteByte(0xFF);
                outPacket.WriteString(this.Description);
                outPacket.WriteByte((byte)this.Type);
                outPacket.WriteByte();

                this.Visitor.Client.Send(outPacket);
            }
        }

        public void CheckForCombo(Character player, int type)
        {
            bool win = false;

            // Horizontal

            for (int i = 0; i < 15; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    if (this.Pieces[j, i] == type &&
                        this.Pieces[j + 1, i] == type &&
                        this.Pieces[j + 2, i] == type &&
                        this.Pieces[j + 3, i] == type &&
                        this.Pieces[j + 4, i] == type)
                    {
                        win = true;
                        break;
                    }
                }
            }

            // Vertical

            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 15; j++)
                {
                    if (this.Pieces[j, i] == type &&
                        this.Pieces[j, i + 1] == type &&
                        this.Pieces[j, i + 2] == type &&
                        this.Pieces[j, i + 3] == type &&
                        this.Pieces[j, i + 4] == type)
                    {
                        win = true;
                        break;
                    }
                }
            }

            //Diagonal Up

            for (int i = 4; i < 15; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    if (this.Pieces[j, i] == type &&
                        this.Pieces[j + 1, i - 1] == type &&
                        this.Pieces[j + 2, i - 2] == type &&
                        this.Pieces[j + 3, i - 3] == type &&
                        this.Pieces[j + 4, i - 4] == type)
                    {
                        win = true;
                        break;
                    }
                }
            }

            //Diagonal Down

            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    if (this.Pieces[j, i] == type &&
                        this.Pieces[j + 1, i + 1] == type &&
                        this.Pieces[j + 2, i + 2] == type &&
                        this.Pieces[j + 3, i + 3] == type &&
                        this.Pieces[j + 4, i + 4] == type)
                    {
                        win = true;
                        break;
                    }
                }
            }

            if (win)
            {
                if (this.Owner == player)
                {
                    this.EndGame(this.Owner, false, false);
                }
                else
                {
                    this.EndGame(this.Visitor, false, false);
                }
            }
        }

        public void EndGame(Character winner, bool tie, bool forfeit)
        {
            if (tie)
            {
                //this.Owner.OmokTies++;
                //this.Visitor.OmokTies++;
            }
            else
            {
                if (winner == this.Owner)
                {
                    //this.Owner.OmokWins++;
                    //this.Visitor.OmokLosses++;
                }
                else
                {
                    //this.Visitor.OmokWins++;
                    //this.Visitor.OmokLosses++;
                }
            }

            using (Packet outPacket = new Packet(MapleServerOperationCode.PlayerInteraction))
            {
                outPacket.WriteByte(0x38);

                if (!tie && !forfeit)
                {
                    outPacket.WriteByte(0);
                }
                else if (tie)
                {
                    outPacket.WriteByte(1);
                }
                else if (forfeit)
                {
                    outPacket.WriteByte(2);
                }

                outPacket.WriteByte();

                // TODO: Stats.

                outPacket.WriteInt(1); // Owner
                outPacket.WriteInt(0); // Wins
                outPacket.WriteInt(0); // Ties
                outPacket.WriteInt(0); // Losses
                outPacket.WriteInt(2000); // Points

                outPacket.WriteInt(1); // Visitor
                outPacket.WriteInt(0); // Wins
                outPacket.WriteInt(0); // Ties
                outPacket.WriteInt(0); // Losses
                outPacket.WriteInt(2000); // Points

                this.Owner.Client.Send(outPacket);
                this.Visitor.Client.Send(outPacket);
            }
        }

        public void Close()
        {
            using (Packet outPacket = new Packet(MapleServerOperationCode.PlayerInteraction))
            {
                outPacket.WriteBytes(0x0A, 0x01, 0x03);

                this.Visitor.Client.Send(outPacket);
            }

            this.Owner.Omok = null;
            this.Visitor.Omok = null;

            this.Map.Omoks.Remove(this);
        }

        public Packet GetCreatePacket()
        {
            return this.GetSpawnPacket();
        }

        public Packet GetSpawnPacket()
        {
            Packet spawn = new Packet(MapleServerOperationCode.UpdateCharacterBox);

            spawn.WriteInt(this.Owner.ID);
            spawn.WriteByte(1);
            spawn.WriteInt(this.ObjectID);
            spawn.WriteString(this.Description);
            spawn.WriteBytes(0, 0, 1, 2, 0);

            return spawn;
        }

        public Packet GetDestroyPacket()
        {
            Packet destroy = new Packet(MapleServerOperationCode.UpdateCharacterBox);

            destroy.WriteInt(this.Owner.ID);
            destroy.WriteByte();

            return destroy;
        }
    }
}
