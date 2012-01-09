using System;
using System.Collections.Generic;
using Loki.Data;

namespace Loki
{
	public static class Strings
	{
		public static Dictionary<string, int> Items { get; private set; }
		public static Dictionary<string, int> Maps { get; private set; }
		public static Dictionary<string, int> Mobs { get; private set; }
		public static Dictionary<string, int> Npcs { get; private set; }
		public static Dictionary<string, int> Quests { get; private set; }

		public static void Load()
		{
			Strings.Items = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
			Strings.Maps = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
			Strings.Mobs = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
			Strings.Npcs = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
			Strings.Quests = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

			using (Log.Load("Strings"))
			{
				Strings.Maps.Add("Henesys", 100000000);
				Strings.Maps.Add("Ellinia", 101000000);
				Strings.Maps.Add("Perion", 102000000);
				Strings.Maps.Add("Kerning", 103000000);
				Strings.Maps.Add("Lith Harbor", 104000000);
				Strings.Maps.Add("Nautilus", 120000000);
				Strings.Maps.Add("Nautilus Port", 120000000);
				Strings.Maps.Add("GM", 180000000);
				Strings.Maps.Add("Staff", 180000000);
				Strings.Maps.Add("Master", 180000000);
				Strings.Maps.Add("Blank", 180000000);
				Strings.Maps.Add("Florina", 110000000);
				Strings.Maps.Add("Ereve", 130000000);
				Strings.Maps.Add("Happy Ville", 140000000);
				Strings.Maps.Add("HappyVille", 140000000);
				Strings.Maps.Add("Orbis", 200000000);
				Strings.Maps.Add("El Nath", 211000000);
				Strings.Maps.Add("ElNath", 211000000);
				Strings.Maps.Add("Ludibrium", 220000000);
				Strings.Maps.Add("Aquarium", 230000000);
				Strings.Maps.Add("Leafre", 240000000);
				Strings.Maps.Add("Omega", 221000000);
				Strings.Maps.Add("Omega Sector", 221000000);
				Strings.Maps.Add("KFT", 222000000);
				Strings.Maps.Add("Korean Folk Town", 222000000);
				Strings.Maps.Add("Mu Lung", 250000000);
				Strings.Maps.Add("Magatia", 261000000);
				Strings.Maps.Add("Ariant", 260000000);
				Strings.Maps.Add("Time Temple", 270000100);
				Strings.Maps.Add("Amoria", 680000000);
				Strings.Maps.Add("NLC", 600000000);
				Strings.Maps.Add("New Leaf City", 600000000);
				Strings.Maps.Add("Zipangu", 800000000);
				Strings.Maps.Add("Mushroom Shrine", 800000000);
				Strings.Maps.Add("Showa", 801000000);
				Strings.Maps.Add("Showa Town", 801000000);
				Strings.Maps.Add("Singapore", 540000000);
				Strings.Maps.Add("HHG", 104040000);
				Strings.Maps.Add("HHG1", 104040000);

				foreach (dynamic searchDatum in new Datums("strings").Populate())
				{
					searchDatum.label = searchDatum.label.Trim();

					try
					{
						if (searchDatum.label != string.Empty)
						{
							switch ((string)searchDatum.object_type)
							{
								case "item":
									Strings.Items.Add(searchDatum.label, searchDatum.objectid);
									break;

								case "map":

									if (searchDatum.label.Contains(" - "))
									{
										Strings.Maps.Add(searchDatum.label.Split(new[] { " - " }, StringSplitOptions.None)[1], searchDatum.objectid);
									}
									else
									{
										Strings.Maps.Add(searchDatum.label, searchDatum.objectid);
									}

									break;

								case "mob":
									Strings.Mobs.Add(searchDatum.label, searchDatum.objectid);
									break;

								case "npc":
									Strings.Npcs.Add(searchDatum.label, searchDatum.objectid);
									break;

								case "quest":
									Strings.Quests.Add(searchDatum.label, searchDatum.objectid);
									break;
							}
						}
					}
					catch (ArgumentException) { } // NOTE: Some labels are duplicates.
				}
			}
		}
	}
}
