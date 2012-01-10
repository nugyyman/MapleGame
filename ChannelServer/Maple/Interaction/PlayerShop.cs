using System.Collections.Generic;
using Loki.Maple.Characters;
using Loki.Maple.Maps;
using Loki.Net;

namespace Loki.Maple.Interaction
{
    public class PlayerShop : MapObject, ISpawnable
    {
        public Character Owner { get; private set; }
        public Character[] Visitors { get; private set; }
        public List<PlayerShopItem> Items { get; private set; }
        public string Description { get; private set; }
        public bool Opened { get; private set; }

        public PlayerShop(Character owner, string description)
        {
            this.Owner = owner;
            this.Description = description;
            this.Visitors = new Character[3];
            this.Items = new List<PlayerShopItem>();
            this.Opened = false;

            using (Packet outPacket = new Packet(MapleServerOperationCode.PlayerInteraction))
            {
                outPacket.WriteBytes(5, 4, 4);
                outPacket.WriteBool(false);
                outPacket.WriteByte(0);
                outPacket.WriteBytes(this.Owner.AppearanceToByteArray());
                outPacket.WriteString(this.Owner.Name);
                outPacket.WriteByte(0xFF);
                outPacket.WriteString(this.Description);
                outPacket.WriteByte(0x10);
                outPacket.WriteByte(0);

                this.Owner.Client.Send(outPacket);
            }
        }

        public void Handle(Character player, InteractionCode action, Packet inPacket)
        {
            switch (action)
            {
                case InteractionCode.Open:
                    {
                        this.Owner.Map.PlayerShops.Add(this);
                        this.Opened = true;
                    }

                    break;

                case InteractionCode.AddItem:
                    {
                        byte type = inPacket.ReadByte();
                        sbyte slot = (sbyte)inPacket.ReadShort();
                        short bundles = inPacket.ReadShort();
                        short perBundle = inPacket.ReadShort();
                        int price = inPacket.ReadInt();
                        short quantity = (short)(bundles * perBundle);

                        Item item = player.Items[type, slot];

                        if (perBundle < 0 || perBundle * bundles > 2000 || bundles < 0 || price < 0)
                        {
                            throw new HackException("Illegal quantity of items in player shop.");
                        }

                        if (quantity > item.Quantity)
                        {
                            throw new HackException("Trading more items than available.");
                        }

                        if (quantity < item.Quantity)
                        {
                            item.Quantity -= quantity;
                            item.Update();
                        }
                        else
                        {
                            player.Items.Remove(item, true);
                        }

                        PlayerShopItem shopItem = new PlayerShopItem(item.MapleID, bundles, quantity, price);
                        this.Items.Add(shopItem);
                        this.UpdateItems();
                    }

                    break;

                case InteractionCode.RemoveItem:

                    if (this.Owner == player)
                    {
                        PlayerShopItem targetItem = this.Items[inPacket.ReadShort()];

                        if (targetItem.Quantity > 0)
                        {
                            this.Owner.Items.Add(new Item(targetItem.MapleID, targetItem.Quantity));
                        }

                        this.Items.Remove(targetItem);
                        this.UpdateItems();
                    }
                    else
                    {
                        throw new HackException("Removing items from a foreign shop.");
                    }

                    break;

                case InteractionCode.Exit:

                    if (this.Owner == player)
                    {
                        this.Close();
                    }
                    else
                    {
                        this.RemoveVisitor(player);
                    }

                    break;

                case InteractionCode.Buy:
                    {
                        PlayerShopItem purchase = this.Items[inPacket.ReadByte()];
                        short quantity = inPacket.ReadShort();

                        if (this.Owner == player)
                        {
                            throw new HackException("Buying items from own shop.");
                        }

                        if (quantity > purchase.Quantity)
                        {
                            throw new HackException("Buying a higher ammount of items than allowed.");
                        }

                        if (player.Meso < purchase.MerchantPrice * quantity)
                        {
                            throw new HackException("Buying items without enough mesos.");
                        }

                        purchase.Quantity -= quantity;

                        player.Meso -= purchase.MerchantPrice * quantity;
                        this.Owner.Meso += purchase.MerchantPrice * quantity;

                        player.Items.Add(new Item(purchase.MapleID, quantity));

                        this.UpdateItems();

                        bool noItemLeft = true;

                        foreach (PlayerShopItem shopItem in this.Items)
                        {
                            if (shopItem.Quantity > 0)
                            {
                                noItemLeft = false;
                                break;
                            }
                        }

                        if (noItemLeft)
                        {
                            this.Close();
                        }
                    }

                    break;

                case InteractionCode.Chat:
                    {
                        string chat = inPacket.ReadString();

                        using (Packet outPacket = new Packet(MapleServerOperationCode.PlayerInteraction))
                        {
                            outPacket.WriteBytes(6, 8);

                            byte sender = 0;

                            for (int i = 0; i < 3; i++)
                            {
                                if (this.Visitors[i] == player)
                                {
                                    sender = (byte)(i + 1);
                                }
                            }

                            outPacket.WriteByte(sender);
                            outPacket.WriteString(player.Name + " : " + chat);

                            this.Broadcast(outPacket);
                        }
                    }

                    break;
            }
        }

        public void AddVisitor(Character player)
        {
            for (int i = 0; i < 3; i++)
            {
                if (this.Visitors[i] == null)
                {
                    using (Packet outPacket = new Packet(MapleServerOperationCode.PlayerInteraction))
                    {
                        outPacket.WriteByte(4);
                        outPacket.WriteByte((byte)(i + 1));
                        outPacket.WriteBytes(player.AppearanceToByteArray());
                        outPacket.WriteString(player.Name);

                        this.Broadcast(outPacket);
                    }

                    this.Visitors[i] = player;

                    using (Packet outPacket = new Packet(MapleServerOperationCode.PlayerInteraction))
                    {
                        outPacket.WriteBytes(5, 4, 4);
                        outPacket.WriteBool(true);
                        outPacket.WriteByte();
                        outPacket.WriteBytes(this.Owner.AppearanceToByteArray());
                        outPacket.WriteString(this.Owner.Name);

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
                        outPacket.WriteString(this.Description);
                        outPacket.WriteByte(0x10);
                        outPacket.WriteByte((byte)this.Items.Count);

                        foreach (PlayerShopItem item in this.Items)
                        {
                            outPacket.WriteShort(item.Bundles);
                            outPacket.WriteShort(item.Quantity);
                            outPacket.WriteInt(item.MerchantPrice);
                            outPacket.WriteBytes(item.ToByteArray(true, true));
                        }

                        player.Client.Send(outPacket);
                    }

                    break;
                }
            }
        }

        public void RemoveVisitor(Character player)
        {
            for (int i = 0; i < 3; i++)
            {
                if (this.Visitors[i] == player)
                {
                    this.Visitors[i] = null;

                    using (Packet outPacket = new Packet(MapleServerOperationCode.PlayerInteraction))
                    {
                        outPacket.WriteByte(0x0A);

                        if (i > 0)
                        {
                            outPacket.WriteByte((byte)(i + 1));
                        }

                        this.Broadcast(outPacket, false);
                    }

                    using (Packet outPacket = new Packet(MapleServerOperationCode.PlayerInteraction))
                    {
                        outPacket.WriteByte(0x0A);
                        outPacket.WriteByte((byte)(i + 1));

                        this.Owner.Client.Send(outPacket);
                    }

                    break;
                }
            }
        }

        public void Close()
        {
            foreach (PlayerShopItem shopItem in this.Items)
            {
                if (shopItem.Quantity > 0)
                {
                    this.Owner.Items.Add(new Item(shopItem.MapleID, shopItem.Quantity));
                }
            }

            if (this.Opened)
            {
                this.Map.PlayerShops.Remove(this);
            }

            this.Notify(10, 1);
            this.Owner.PlayerShop = null;

            for (int i = 0; i < 3; i++)
            {
                if (this.Visitors[i] != null)
                {
                    this.Visitors[i].PlayerShop = null;
                }
            }
        }

        public void UpdateItems()
        {
            using (Packet outPacket = new Packet(MapleServerOperationCode.PlayerInteraction))
            {
                outPacket.WriteByte(0x17);
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

        public void Notify(byte error, byte type)
        {
            using (Packet outPacket = new Packet(MapleServerOperationCode.PlayerInteraction))
            {
                outPacket.WriteByte(0x0A);
                outPacket.WriteByte(type);
                outPacket.WriteByte(error);

                this.Broadcast(outPacket);
            }
        }

        public void Broadcast(Packet outPacket, bool includeOwner = true)
        {
            if (includeOwner)
            {
                this.Owner.Client.Send(outPacket);
            }

            for (int i = 0; i < 3; i++)
            {
                if (this.Visitors[i] != null)
                {
                    this.Visitors[i].Client.Send(outPacket);
                }
            }
        }

        public Packet GetCreatePacket()
        {
            return this.GetSpawnPacket();
        }

        public Packet GetSpawnPacket()
        {
            Packet spawn = new Packet(MapleServerOperationCode.UpdateCharacterBox);

            spawn.WriteInt(this.Owner.ID);
            spawn.WriteByte(4);
            spawn.WriteInt(this.ObjectID);
            spawn.WriteString(this.Description);
            spawn.WriteBytes(0, 0, 1, 4, 0);

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
