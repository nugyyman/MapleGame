using System;
using System.Collections.Generic;
using Loki.Maple.Characters;
using Loki.Net;

namespace Loki.Maple
{
	public class Trade
	{
		public Character Owner { get; private set; }
		public Character Visitor { get; private set; }
		public int OwnerMeso { get; private set; }
		public int VisitorMeso { get; private set; }
		public List<Item> OwnerItems { get; private set; }
		public List<Item> VisitorItems { get; private set; }
		public bool Started { get; private set; }
		public bool OwnerLocked { get; private set; }
		public bool VisitorLocked { get; private set; }
		
		public Trade(Character owner)
		{
			this.Owner = owner;
			this.OwnerItems = new List<Item>();
			this.VisitorItems = new List<Item>();
			this.OwnerMeso = 0;
			this.VisitorMeso = 0;
			this.Started = false;
			this.OwnerLocked = false;
			this.VisitorLocked = false;

			using (Packet outPacket = new Packet(MapleServerOperationCode.PlayerInteraction))
			{
				outPacket.WriteBytes(5, 3, 2);
				outPacket.WriteByte();
				outPacket.WriteByte();
				outPacket.WriteBytes(this.Owner.AppearanceToByteArray());
				outPacket.WriteString(this.Owner.Name);
				outPacket.WriteByte(0xFF);

				this.Owner.Client.Send(outPacket);
			}
		}

		public void Complete()
		{
			if (this.Owner.Items.CouldReceive(this.VisitorItems) && this.Visitor.Items.CouldReceive(this.OwnerItems))
			{
				this.Owner.Meso += this.VisitorMeso;
				this.Visitor.Meso += this.OwnerMeso;

				this.Visitor.Items.AddRange(this.OwnerItems);
				this.Owner.Items.AddRange(this.VisitorItems);
			}
			else
			{
				this.Owner.Notify("Trade was not successful.", NoticeType.Popup);
				this.Visitor.Notify("Trade was not successful.", NoticeType.Popup);

				// TODO: (Rob) Cannot trade, inventory would be full.
			}
		}

		public void Cancel()
		{
			this.Owner.Meso += this.OwnerMeso;
			this.Visitor.Meso += this.VisitorMeso;

			this.Owner.Items.AddRange(this.OwnerItems);
			this.Visitor.Items.AddRange(this.VisitorItems);
		}

		public void Handle(Character player, InteractionCode action, Packet inPacket)
		{
			switch (action)
			{
				case InteractionCode.Invite:
					Character invitee = this.Owner.Map.Characters[inPacket.ReadInt()];

					if (invitee.Trade != null)
					{
						using (Packet outPacket = new Packet(MapleServerOperationCode.PlayerInteraction))
						{
							outPacket.WriteByte(0xA);
							outPacket.WriteByte();
							outPacket.WriteBytes(PacketConstants.Trade);
							outPacket.WriteByte(2);
							this.Owner.Client.Send(outPacket);
						}

						player.Notify("This player is already in a trade.", NoticeType.Pink);
					}
					else
					{
						invitee.Trade = this;
						this.Visitor = invitee;

						using (Packet outPacket = new Packet(MapleServerOperationCode.PlayerInteraction))
						{
							outPacket.WriteBytes(2, 3);
							outPacket.WriteString(this.Owner.Name);
							outPacket.WriteBytes(PacketConstants.Trade);

							this.Visitor.Client.Send(outPacket);
						}
					}

					break;

				case InteractionCode.Decline:
					this.Visitor.Trade = null;
					this.Owner.Trade = null;

					using (Packet outPacket = new Packet(MapleServerOperationCode.PlayerInteraction))
					{
						outPacket.WriteByte(0xA);
						outPacket.WriteByte();
						outPacket.WriteBytes(PacketConstants.Trade);
						outPacket.WriteByte(2);
						this.Owner.Client.Send(outPacket);
					}

					this.Owner.Notify("The trade has been declined.", NoticeType.Popup);

					this.Owner = null;
					this.Visitor = null;

					break;

				case InteractionCode.Visit:

					if (this.Owner == null)
					{
						this.Visitor = null;
						player.Trade = null;

						player.Notify("The trade has already been closed.", NoticeType.Popup);
					}
					else
					{
						this.Started = true;

						using (Packet outPacket = new Packet(MapleServerOperationCode.PlayerInteraction))
						{
							outPacket.WriteBytes(4, 1);
							outPacket.WriteBytes(this.Visitor.AppearanceToByteArray());
							outPacket.WriteString(this.Visitor.Name);
							this.Owner.Client.Send(outPacket);
						}

						using (Packet outPacket = new Packet(MapleServerOperationCode.PlayerInteraction))
						{
							outPacket.WriteBytes(5, 3, 2, 1, 0);
							outPacket.WriteBytes(this.Owner.AppearanceToByteArray());
							outPacket.WriteString(this.Owner.Name);
							outPacket.WriteByte(1);
							outPacket.WriteBytes(this.Visitor.AppearanceToByteArray());
							outPacket.WriteString(this.Visitor.Name);
							outPacket.WriteByte(0xFF);

							this.Visitor.Client.Send(outPacket);
						}
					}

					break;

				case InteractionCode.SetItems:
					ItemType type = (ItemType)inPacket.ReadByte();
					sbyte slot = (sbyte)inPacket.ReadShort();
					short quantity = inPacket.ReadShort();
					byte targetSlot = inPacket.ReadByte();

					Item item = player.Items[type, slot];

					if (item.IsBlocked)
					{
						throw new HackException("Trading blocked item.");
					}
					else if (quantity > item.Quantity)
					{
						throw new HackException("Trading more items than available.");
					}
					else
					{
						if (quantity < item.Quantity)
						{
							item.Quantity -= quantity;
							item.Update();
							item = new Item(item.MapleID, quantity);
						}
						else
						{
							player.Items.Remove(item, true);
						}

						item.Slot = (sbyte)targetSlot;

						using (Packet outPacket = new Packet(MapleServerOperationCode.PlayerInteraction))
						{
							outPacket.WriteBytes(0xE, 0);
							outPacket.WriteBytes(item.ToByteArray());

							if (player == this.Owner)
							{
								this.OwnerItems.Add(item);
								this.Owner.Client.Send(outPacket);

							}
							else
							{
								this.VisitorItems.Add(item);
								this.Visitor.Client.Send(outPacket);
							}
						}

						using (Packet outPacket = new Packet(MapleServerOperationCode.PlayerInteraction))
						{
							outPacket.WriteBytes(0xE, 1);
							outPacket.WriteBytes(item.ToByteArray());

							if (player == this.Owner)
							{
								this.Visitor.Client.Send(outPacket);

							}
							else
							{
								this.Owner.Client.Send(outPacket);
							}
						}
					}

					break;

				case InteractionCode.SetMeso:
					int meso = inPacket.ReadInt();

					if (meso < 0 || meso > player.Meso)
					{
						throw new HackException("Invalid amount of meso in trade.");
					}
					else
					{
						using (Packet outPacket = new Packet(MapleServerOperationCode.PlayerInteraction))
						{
							outPacket.WriteBytes(0xF, 0);
							outPacket.WriteInt(meso);

							if (this.Owner == player)
							{
								if (this.OwnerLocked)
								{
									throw new InvalidOperationException("Adding mesos to locked trade.");
								}

								this.Owner.Client.Send(outPacket);
								this.OwnerMeso += meso;
								this.Owner.Meso -= meso;
							}
							else
							{
								if (this.VisitorLocked)
								{
									throw new InvalidOperationException("Adding mesos to locked trade.");
								}

								this.Visitor.Client.Send(outPacket);
								this.VisitorMeso += meso;
								this.Visitor.Meso -= meso;
							}
						}

						using (Packet outPacket = new Packet(MapleServerOperationCode.PlayerInteraction))
						{
							outPacket.WriteBytes(0x0F, 0x01);
							outPacket.WriteInt(meso);

							if (this.Owner == player)
							{
								this.Visitor.Client.Send(outPacket);
							}
							else
							{
								this.Owner.Client.Send(outPacket);
							}
						}
					}

					break;

				case InteractionCode.Exit:

					if (this.Started)
					{
						this.Cancel();

						using (Packet outPacket = new Packet(MapleServerOperationCode.PlayerInteraction))
						{
							outPacket.WriteBytes(0x0A, 0x00);
							outPacket.WriteByte(2);
							this.Owner.Client.Send(outPacket);
							this.Visitor.Client.Send(outPacket);
						}

						this.Owner.Trade = null;
						this.Visitor.Trade = null;
						this.Owner = null;
						this.Visitor = null;
					}
					else
					{
						using (Packet outPacket = new Packet(MapleServerOperationCode.PlayerInteraction))
						{
							outPacket.WriteBytes(0x0A, 0x00);
							outPacket.WriteBytes(PacketConstants.Trade);
							outPacket.WriteByte(2);
							this.Owner.Client.Send(outPacket);
						}

						this.Owner.Trade = null;
						this.Owner = null;
					}

					break;

				case InteractionCode.Confirm:

					using (Packet outPacket = new Packet(MapleServerOperationCode.PlayerInteraction))
					{
						outPacket.WriteByte(0x10);

						if (this.Owner == player)
						{
							this.OwnerLocked = true;
							this.Visitor.Client.Send(outPacket);
						}
						else
						{
							this.VisitorLocked = true;
							this.Owner.Client.Send(outPacket);
						}
					}

					if (this.OwnerLocked && this.VisitorLocked)
					{
						this.Complete();

						using (Packet outPacket = new Packet(MapleServerOperationCode.PlayerInteraction))
						{
							outPacket.WriteBytes(0x0A, 0x00);
							outPacket.WriteByte(6);
							this.Owner.Client.Send(outPacket);
							this.Visitor.Client.Send(outPacket);
						}

						this.Owner.Trade = null;
						this.Visitor.Trade = null;
						this.Owner = null;
						this.Visitor = null;
					}

					break;

				case InteractionCode.Chat:

					string chat = inPacket.ReadString();

					using (Packet outPacket = new Packet(MapleServerOperationCode.PlayerInteraction))
					{
						outPacket.WriteBytes(6, 8);
						outPacket.WriteBool(this.Owner != player);
						outPacket.WriteString(player.Name + " : " + chat);

						this.Owner.Client.Send(outPacket);
						this.Visitor.Client.Send(outPacket);
					}

					break;
			}
		}
	}
}
