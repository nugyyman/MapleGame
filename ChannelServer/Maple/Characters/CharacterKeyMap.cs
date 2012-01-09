using System.Collections.Generic;
using Loki.Data;
using Loki.Net;

namespace Loki.Maple.Characters
{
	public class CharacterKeyMap : Dictionary<int, Shortcut>
	{
		public Character Parent { get; private set; }

		public CharacterKeyMap(Character parent)
			: base()
		{
			this.Parent = parent;
		}

		public void Load()
		{
			foreach (dynamic datum in new Datums("keymaps").Populate("CharacterID = '{0}'", this.Parent.ID))
			{
				this.Add(datum.KeyID, new Shortcut(datum.Type, datum.Action));
			}
		}

		public void Delete()
		{
			Database.Delete("keymaps", "CharacterID = '{0}'", this.Parent.ID);
		}

		public void Save()
		{
			this.Delete();

			foreach (KeyValuePair<int, Shortcut> loopKeyMap in this)
			{
				dynamic datum = new Datum("keymaps");

				datum.CharacterID = this.Parent.ID;
				datum.KeyID = loopKeyMap.Key;
				datum.Type = loopKeyMap.Value.Type;
				datum.Action = loopKeyMap.Value.Action;

				datum.Insert();
			}
		}

		public void Update(Packet inPacket)
		{
			if (inPacket.Remaining != 8)
			{
				inPacket.ReadInt();
				int amountChanges = inPacket.ReadInt();

				for (int i = 0; i < amountChanges; i++)
				{
					int keyId = inPacket.ReadInt();
					byte type = inPacket.ReadByte();
					int action = inPacket.ReadInt();

					if (type != 0)
					{
						if (this.ContainsKey(keyId))
						{
							this[keyId] = new Shortcut(type, action);
						}
						else
						{
							this.Add(keyId, new Shortcut(type, action));
						}
					}
					else
					{
						this.Remove(keyId);
					}
				}
			}
		}

		public void Send()
		{
			using (Packet outPacket = new Packet(MapleServerOperationCode.KeyMap))
			{
				outPacket.WriteByte();

				for (int i = 0; i < 90; i++)
				{
					if (this.ContainsKey(i))
					{
						outPacket.WriteByte(this[i].Type);
						outPacket.WriteInt(this[i].Action);
					}
					else
					{
						outPacket.WriteByte(0);
						outPacket.WriteInt(0);
					}
				}

				this.Parent.Client.Send(outPacket);
			}
		}
	}
}
