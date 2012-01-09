using System.Collections.Generic;
using Loki.Data;

namespace Loki.Maple.Data
{
	public class CachedSkills : Dictionary<int, Dictionary<byte, Skill>>
	{
		public CachedSkills()
			: base()
		{
			Dictionary<int, short> weaponType = new Dictionary<int, short>();

			using (Log.Load("Skills"))
			{
				foreach (dynamic skillDatum in new Datums("skill_player_data").Populate())
				{
					weaponType.Add(skillDatum.skillid, skillDatum.weapon);
					this.Add(skillDatum.skillid, new Dictionary<byte, Skill>());
				}

				foreach (dynamic skillLevelDatum in new Datums("skill_player_level_data").Populate())
				{
					this[skillLevelDatum.skillid].Add(skillLevelDatum.skill_level, new Skill(skillLevelDatum));
				}
			}
		}
	}
}
