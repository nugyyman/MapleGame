﻿using System.Collections.Generic;
using Loki.Maple.Data;
using Loki.Maple.Life;
using Loki.Maple.Characters;

namespace Loki.Maple.Maps
{
	public class WorldCharactersHelper : EnumerationHelper<int, Character>
	{
		public WorldCharactersHelper() : base() { }

		public override IEnumerator<Character> GetEnumerator()
		{
			foreach (Map loopMap in World.Maps)
			{
				lock (loopMap.Characters)
				{
					foreach (Character loopCharacter in loopMap.Characters)
					{
						yield return loopCharacter;
					}
				}
			}
		}

		public override int GetKeyForObject(Character item)
		{
			return item.ID;
		}

		public Character this[string name]
		{
			get
			{
				foreach (Character loopCharacter in this)
				{
					if (loopCharacter.Name.ToLower() == name.ToLower())
					{
						return loopCharacter;
					}
				}

				throw new KeyNotFoundException();
			}
		}
	}

	public class WorldNpcsHelper : EnumerationHelper<int, Npc>
	{
		public WorldNpcsHelper() : base() { }

		public override IEnumerator<Npc> GetEnumerator()
		{
			foreach (Map loopMap in World.Maps)
			{
				lock (loopMap.Npcs)
				{
					foreach (Npc loopNpc in loopMap.Npcs)
					{
						yield return loopNpc;
					}
				}
			}
		}

		public override int GetKeyForObject(Npc item)
		{
			return item.MapleID;
		}
	}
}
