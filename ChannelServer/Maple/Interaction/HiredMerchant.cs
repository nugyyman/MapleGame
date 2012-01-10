using System.Collections.Generic;
using Loki.Maple.Characters;
using Loki.Maple.Maps;
using Loki.Net;

namespace Loki.Maple.Interaction
{
    public class HiredMerchant : MapObject, ISpawnable
    {
        public int ItemID { get; private set; }
        public int OwnerID { get; private set; }
        public string OwnerName { get; private set; }
        public string Description { get; private set; }
        public Character[] Visitors { get; private set; }
        public List<PlayerShopItem> Items { get; private set; }
        public int Meso { get; private set; }
        public bool Opened { get; private set; }
        public bool InMaintenance { get; private set; }

        public HiredMerchant(Character owner, string description, int itemId)
        {
            this.OwnerName = owner.Name;
            this.OwnerID = owner.ID;
            this.ItemID = itemId;
            this.Meso = 0;
            this.Description = description;
            this.Visitors = new Character[3];
            this.Items = new List<PlayerShopItem>();
            this.Opened = false;
            this.InMaintenance = false;
            this.Visitors[0] = owner;

            using (Packet outPacket = new Packet(MapleServerOperationCode.PlayerInteraction))
            {
                outPacket.WriteBytes(5, 5, 4, 0, 0);
                outPacket.WriteInt(this.ItemID);
                outPacket.WriteString("Hired Merchant");
                outPacket.WriteByte(0xFF);
                outPacket.WriteShort();
                outPacket.WriteString(this.OwnerName);
                outPacket.WriteInt(this.OwnerID);
                outPacket.WriteBool(true);
                outPacket.WriteByte();
                outPacket.WriteInt();
                outPacket.WriteString(this.Description);
                outPacket.WriteByte(0x10);
                outPacket.WriteInt();
                outPacket.WriteByte();
                outPacket.WriteByte();

                owner.Client.Send(outPacket);
            }
        }

        public void Handle(Character player, InteractionCode action, Packet inPacket)
        {
            switch (action)
            {
                case InteractionCode.Open:

                    if (player.ID != this.OwnerID)
                    {
                        throw new HackException("Opening foreign hired merchant.");
                    }
                    else if (!this.Opened)
                    {
                        this.Opened = true;
                        this.Position = player.Position;
                        player.Map.HiredMerchants.Add(this);
                    }

                    break;

                case InteractionCode.AddItem:
                case InteractionCode.PutItem:
                    {
                        Item item = player.Items[inPacket.ReadByte(), inPacket.ReadSByte()];
                        short bundles = inPacket.ReadShort();
                        short perBundle = inPacket.ReadShort();
                        int price = inPacket.ReadInt();
                        short quantity = (short)(bundles * perBundle);

                        if (perBundle < 0 || perBundle * bundles > 2000 || bundles < 0 || price < 0)
                        {
                            throw new HackException("Illegal quantity of items in hired merchant.");
                        }

                        if (quantity > item.Quantity)
                        {
                            return;
                        }
                        else if (quantity < item.Quantity)
                        {
                            item.Quantity -= quantity;
                            item.Update();
                        }
                        else if (quantity == item.Quantity)
                        {
                            player.Items.Remove(item, true);
                        }

                        this.Items.Add(new PlayerShopItem(item.MapleID, perBundle, quantity, price));
                        this.Update();
                    }

                    break;

                case InteractionCode.RemoveItem:
                case InteractionCode.TakeItemBack:

                    if (this.OwnerID == player.ID)
                    {
                        PlayerShopItem item = this.Items[inPacket.ReadShort()];

                        if (item.Quantity > 0)
                        {
                            player.Items.Add(new Item(item.MapleID, item.Quantity));
                        }

                        this.Items.Remove(item);
                        this.Update();
                    }
                    else
                    {
                        throw new HackException("Removing items from a foreign hired merchant.");
                    }

                    break;

                case InteractionCode.Exit:

                    if (this.Visitors[0] == player || this.Visitors[1] == player || this.Visitors[2] == player)
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            if (this.Visitors[i] == player)
                            {
                                this.Visitors[i] = null;
                                player.HiredMerchant = null;

                                using (Packet outPacket = new Packet(MapleServerOperationCode.PlayerInteraction))
                                {
                                    outPacket.WriteByte(0x0A);

                                    if (i > 0)
                                    {
                                        outPacket.WriteByte((byte)(i + 1));
                                    }

                                    this.Broadcast(outPacket);
                                }

                                break;
                            }
                        }
                    }

                    break;

                case InteractionCode.MaintenanceOff:

                    if (this.OwnerID == player.ID)
                    {
                        this.InMaintenance = false;
                    }
                    else
                    {
                        throw new HackException("Turning off maintenance from a foreign hired merchant.");
                    }

                    break;

                case InteractionCode.Buy:
                    {
                        PlayerShopItem item = this.Items[inPacket.ReadByte()];
                        short quantity = inPacket.ReadShort();

                        if (this.OwnerID == player.ID)
                        {
                            throw new HackException("Buying items from own hired merchant.");
                        }

                        if (quantity > item.Quantity || player.Meso < item.MerchantPrice * quantity)
                        {
                            return;
                        }

                        item.Quantity -= quantity;
                        this.Meso += item.MerchantPrice * quantity;
                        this.Update();

                        player.Meso -= item.MerchantPrice * quantity;
                        player.Items.Add(new Item(item.MapleID, item.Quantity));
                    }

                    break;

                case InteractionCode.Chat:
                    string chat = inPacket.ReadString();

                    using (Packet outPacket = new Packet(MapleServerOperationCode.PlayerInteraction))
                    {
                        outPacket.WriteBytes(6, 8);

                        for (int i = 0; i < 3; i++)
                        {
                            if (this.Visitors[i] == player)
                            {
                                outPacket.WriteByte((byte)(i + 1));
                                break;
                            }
                        }

                        outPacket.WriteString(player.Name + " : " + chat);

                        this.Broadcast(outPacket);
                    }

                    break;

                case InteractionCode.MerchantClose:

                    if (this.OwnerID == player.ID)
                    {
                        this.Close(player, true);
                    }
                    else
                    {
                        throw new HackException("Closing a foreign hired merchant.");
                    }

                    break;
            }
        }

        public void Update()
        {
            using (Packet outPacket = new Packet(MapleServerOperationCode.PlayerInteraction))
            {
                outPacket.WriteByte(0x17);
                outPacket.WriteInt(this.Meso);
                outPacket.WriteByte((byte)this.Items.Count);

                foreach (PlayerShopItem item in this.Items)
                {
                    outPacket.WriteShort(item.Bundles);
                    outPacket.WriteShort(item.Quantity);
                    outPacket.WriteInt(item.MerchantPrice);
                    outPacket.WriteBytes(item.ToByteArray(true, true));
                }

                this.Broadcast(outPacket);
            }
        }

        public void AddVisitor(Character player)
        {
            if (this.InMaintenance)
            {
                using (Packet outPacket = new Packet(MapleServerOperationCode.PlayerInteraction))
                {
                    outPacket.WriteBytes(5, 0, 18);

                    this.Broadcast(outPacket);
                }

                return;
            }

            if (this.OwnerID == player.ID)
            {
                this.InMaintenance = true;
            }

            for (int i = 0; i < 3; i++)
            {
                if (this.Visitors[i] == null)
                {
                    using (Packet outPacket = new Packet(MapleServerOperationCode.PlayerInteraction))
                    {
                        outPacket.WriteByte(0x04);
                        outPacket.WriteByte((byte)(i + 1));
                        outPacket.WriteBytes(player.AppearanceToByteArray());
                        outPacket.WriteString(player.Name);

                        this.Broadcast(outPacket);
                    }

                    this.Visitors[i] = player;

                    using (Packet outPacket = new Packet(MapleServerOperationCode.PlayerInteraction))
                    {
                        outPacket.WriteBytes(5, 5, 4, 0, 0);
                        outPacket.WriteInt(this.ItemID);
                        outPacket.WriteString("Hired Merchant");

                        for (int slot = 0; slot < 3; slot++)
                        {
                            if (this.Visitors[slot] != null)
                            {
                                outPacket.WriteByte((byte)(slot + 1));
                                outPacket.WriteBytes(this.Visitors[slot].AppearanceToByteArray());
                                outPacket.WriteString(this.Visitors[slot].Name);
                            }
                        }

                        outPacket.WriteByte(0xFF);

                        outPacket.WriteShort();
                        outPacket.WriteString(this.OwnerName);

                        if (this.OwnerID == player.ID)
                        {
                            outPacket.WriteInt(this.OwnerID);
                            outPacket.WriteBool(false); // UNK?
                            outPacket.WriteByte();
                            outPacket.WriteInt(this.Meso);
                        }

                        outPacket.WriteString(this.Description);
                        outPacket.WriteByte(0x10);
                        outPacket.WriteInt(this.Meso);
                        outPacket.WriteByte((byte)this.Items.Count);

                        foreach (PlayerShopItem loopItem in this.Items)
                        {
                            outPacket.WriteShort(loopItem.Bundles);
                            outPacket.WriteShort(loopItem.Quantity);
                            outPacket.WriteInt(loopItem.MerchantPrice);
                            outPacket.WriteBytes(loopItem.ToByteArray(true, true));
                        }

                        player.Client.Send(outPacket);
                    }

                    break;
                }
            }
        }

        public void Close(Character owner, bool manualClose)
        {
            if (manualClose)
            {
                if (owner.Items.CouldReceive(this.Items))
                {
                    owner.Items.AddRange(this.Items);
                }
                else
                {
                    this.Save(); // TODO: Pick the items you can, then rest to Fredrick.

                    owner.Notify("Inventory full: Items sent to Fredrick.", NoticeType.Popup); // TODO: Actual notice.
                }
            }
            else
            {
                this.Save();
            }

            for (int i = 0; i < 3; i++)
            {
                if (this.Visitors[i] != null)
                {
                    this.Visitors[i].HiredMerchant = null;
                    this.Visitors[i] = null;

                    // TODO: Send force close packet to all visitors.
                }
            }

            this.Items.Clear();

            this.Map.HiredMerchants.Remove(this);
        }

        public void Save()
        {
            foreach (PlayerShopItem merchantItem in this.Items)
            {
                // TODO: Save item to database. No table changes needed. I want to set the item position to a static number that can be used to determine if an item if saved from a merchant.
            }
        }

        public void Broadcast(Packet outPacket)
        {
            for (int i = 0; i < 3; i++)
            {
                if (this.Visitors[i] != null)
                {
                    this.Visitors[i].Client.Send(outPacket);
                }
            }
        }

        public Packet GetSpawnPacket()
        {
            Packet outPacket = new Packet(MapleServerOperationCode.SpawnHiredMerchant);

            outPacket.WriteInt(this.OwnerID);
            outPacket.WriteInt(this.ItemID);
            outPacket.WriteShort(this.Position.X);
            outPacket.WriteShort(this.Position.Y);
            outPacket.WriteShort();
            outPacket.WriteString(this.OwnerName);
            outPacket.WriteByte(5);
            outPacket.WriteInt(this.ObjectID);
            outPacket.WriteString(this.Description);
            outPacket.WriteByte((byte)(this.ItemID % 10));
            outPacket.WriteBytes(0, 4);

            return outPacket;
        }

        public Packet GetCreatePacket()
        {
            return this.GetSpawnPacket();
        }

        public Packet GetDestroyPacket()
        {
            Packet outPacket = new Packet(MapleServerOperationCode.DestroyHiredMerchant);

            outPacket.WriteInt(this.ObjectID);

            return outPacket;
        }
    }

    public class PlayerShopItem : Item // TODO: Own file.
    {
        public short Bundles { get; set; }
        public int MerchantPrice { get; private set; }

        public PlayerShopItem(int mapleId, short bundles, short quantity, int price)
            : base(mapleId, quantity)
        {
            this.Bundles = bundles;
            this.MerchantPrice = price;
        }
    }
}
