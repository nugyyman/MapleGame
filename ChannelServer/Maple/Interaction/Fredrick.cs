using System.Collections.Generic;
using Loki.Maple.Characters;
using Loki.Net;

namespace Loki.Maple.Interaction
{
    public class Fredrick
    {
        public Character Character { get; private set; }
        public int Meso { get; private set; }
        public List<Item> Items { get; private set; }

        public Fredrick(Character character)
        {
            this.Character = character;
            this.Items = new List<Item>();
            this.Meso = 0;

            this.Load();

            using (Packet outPacket = new Packet(MapleServerOperationCode.Ping)) //TODO: Fredrick Header
            {
                outPacket.WriteByte(0x23);
                outPacket.WriteInt(9030000); //Fredrick NPC ID
                outPacket.WriteInt(32272); // ID
                outPacket.WriteBytes(0, 0, 0, 0, 0);
                outPacket.WriteInt(this.Meso);
                outPacket.WriteByte();
                outPacket.WriteByte((byte)this.Items.Count);

                foreach (Item item in this.Items)
                {
                    outPacket.WriteBytes(item.ToByteArray(true, true));
                }

                outPacket.WriteBytes(0, 0, 0);
            }
        }

        public void Load()
        {
            // TODO: Fredrick load.
        }

        public void Delete()
        {

        }

        public Packet GetMessagePacket(byte type)
        {
            Packet outPacket = new Packet(MapleServerOperationCode.Ping); //TODO: Get Header

            outPacket.WriteByte(type);

            return outPacket;
        }

        public void Handle(Character player, Packet inPacket) // TODO: Not linked anywhere??
        {
            byte action = inPacket.ReadByte();

            switch (action)
            {
                case 0x19:
                    // TODO: What does that do?
                    break;

                case 0x1A:

                    if (this.Character.Items.CouldReceive(this.Items))
                    {
                        player.Meso += this.Meso;

                        foreach (Item loopItem in this.Items)
                        {
                            player.Items.Add(loopItem, false, true);
                        }

                        this.Delete();

                        using (Packet outPacket = this.GetMessagePacket(0x1E))
                        {
                            this.Character.Client.Send(outPacket);
                        }

                        // TODO: this.Character.Release()?? What did you mean Rob?

                        this.Meso = 0;
                        this.Items = null;
                        this.Character.Fredrick = null;
                        this.Character = null;
                    }
                    else
                    {
                        using (Packet outPacket = this.GetMessagePacket(0x21))
                        {
                            this.Character.Client.Send(outPacket);
                        }
                    }

                    break;
            }
        }
    }
}
