using System.Net.Sockets;
using Loki.IO;
using Loki.Maple;
using Loki.Maple.Characters;
using Loki.Maple.CashShop;

namespace Loki.Net
{
	public class ChannelClientHandler : MapleClientHandler
	{
		public Character Character { get; private set; }

		public ChannelClientHandler(Socket socket) : base(socket) { }

		private void Initialize(ByteBuffer reader)
		{
			int characterId = reader.ReadInt();

			this.Character = new Character(characterId, this);
			this.Character.Load();
			this.Character.IsMaster = ChannelServer.LoginServerConnection.IsMaster(this.Character.AccountID);
			this.Character.IsLoggedIn = true;
			this.Character.Initialize();

			this.Title = this.Character.Name;
		}

		protected override void Register()
		{
			ChannelServer.Clients.Add(this);
		}

		protected override void Terminate()
		{
			if (this.Character != null)
			{
				this.Character.IsLoggedIn = false;
				this.Character.Save();
				this.Character.LastNpc = null;
				this.Character.Map.Characters.Remove(this.Character);
			}
		}

		protected override void Unregister()
		{
			ChannelServer.Clients.Remove(this);
		}

		protected override bool IsServerAlive
		{
			get
			{
				return ChannelServer.IsAlive;
			}
		}

		protected override void Dispatch(Packet inPacket)
		{
			switch ((MapleClientOperationCode)inPacket.OperationCode)
			{
				case MapleClientOperationCode.PlayerLoggedIn:
					this.Initialize(inPacket);
					break;

				case MapleClientOperationCode.MovePlayer:
					this.Character.Move(inPacket);
					break;

				case MapleClientOperationCode.FaceExpression:
					this.Character.Express(inPacket);
					break;

				case MapleClientOperationCode.GeneralChat:
					this.Character.Talk(inPacket);
					break;

				case MapleClientOperationCode.PlayerUpdate:
					//this.Character.Save(); Commented until moved to another thread
					break;

				case MapleClientOperationCode.CharacterInformation:
					this.Character.InformOnCharacter(inPacket);
					break;

				case MapleClientOperationCode.ItemMove:
					this.Character.Items.Handle(inPacket);
					break;

				case MapleClientOperationCode.ItemPickup:
					this.Character.Items.Pickup(inPacket);
					break;

				case MapleClientOperationCode.ChangeChannel:
					this.ChangeChannel(inPacket);
					break;

				case MapleClientOperationCode.ChangeMap:
					this.Character.ChangeMap(inPacket);
					break;

				case MapleClientOperationCode.ChangeMapSpecial:
					this.Character.ChangeMapSpecial(inPacket);
					break;

				case MapleClientOperationCode.NpcTalk:
					this.Character.Converse(inPacket);
					break;

				case MapleClientOperationCode.NpcResult:
					this.Character.LastNpc.HandleResult(this.Character, inPacket);
					break;

				case MapleClientOperationCode.DistributeAP:
					this.Character.DistributeAP(inPacket);
					break;

				case MapleClientOperationCode.DistributeSP:
					this.Character.DistributeSP(inPacket);
					break;

				case MapleClientOperationCode.HealOverTime:
					this.Character.HealOverTime(inPacket);
					break;

				case MapleClientOperationCode.MesoDrop:
					this.Character.DropMeso(inPacket);
					break;

				case MapleClientOperationCode.NpcShop:
					this.Character.LastNpc.Shop.Handle(this.Character, inPacket);
					break;

				case MapleClientOperationCode.NpcAction:
					this.Character.ControlledNpcs.Act(inPacket);
					break;

				case MapleClientOperationCode.MoveLife:
					this.Character.ControlledMobs.Move(inPacket);
					break;

				case MapleClientOperationCode.CloseRangeAttack:
					this.Character.Attack(inPacket, AttackType.CloseRange);
					break;

				case MapleClientOperationCode.RangedAttack:
					this.Character.Attack(inPacket, AttackType.Ranged);
					break;

				case MapleClientOperationCode.MagicAttack:
					this.Character.Attack(inPacket, AttackType.Magic);
					break;

				case MapleClientOperationCode.QuestAction:
					this.Character.Quests.Handle(inPacket);
					break;

				case MapleClientOperationCode.PlayerInteraction:
					this.Character.Interact(inPacket);
					break;

				case MapleClientOperationCode.TakeDamage:
					this.Character.Damage(inPacket);
					break;

				case MapleClientOperationCode.SpecialMove:
					this.Character.UseSpecialMove(inPacket);
					break;

				case MapleClientOperationCode.EnergyOrbAttack:
					this.Character.HandleEnergyOrbAttack(inPacket);
					break;

                case MapleClientOperationCode.CancelBuff:
                    this.Character.Buffs.Cancel(inPacket);
                    break;

                case MapleClientOperationCode.SkillEffect:
                    this.Character.HandleSkillEffect(inPacket);
                    break;

                case MapleClientOperationCode.ChangeKeymap:
                    this.Character.KeyMap.Update(inPacket);
                    break;

                case MapleClientOperationCode.UseCashItem:
                    UseCashItem.Handle(this.Character, inPacket);
                    break;
			}
		}

		public void ChangeChannel(Packet inPacket)
		{
			byte channelID = inPacket.ReadByte();

			using (Packet outPacket = new Packet(MapleServerOperationCode.ChangeChannel))
			{
				outPacket.WriteBool(true); // UNK: What does false do?
				outPacket.WriteBytes(ChannelServer.RemoteEndPoint.Address.GetAddressBytes());
				outPacket.WriteShort(ChannelServer.LoginServerConnection.GetChannelPort(channelID));

				this.Send(outPacket);
			}
		}
	}
}
