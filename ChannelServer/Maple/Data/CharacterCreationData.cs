using System.Collections.Generic;
using Loki.Data;

namespace Loki.Maple.Data
{
    public class CharacterCreationData : List<int>
    {
        public CharacterCreationData()
            : base()
        {
            using (Log.Load("Character Creation Data"))
            {
                foreach (dynamic dataDatum in new Datums("character_creation_data").Populate())
                {
                    this.Add(dataDatum.objectid);
                }
            }
        }

        public bool checkData(int job, byte gender, int face, int hair, int haircolor, int skin, int top, int bottom, int shoes, int weapon)
        {
            if (!(this.Contains(face) && this.Contains(hair) && this.Contains(haircolor) && this.Contains(skin) && this.Contains(top) && this.Contains(bottom) && this.Contains(shoes) && this.Contains(weapon)))
                return false;
            return true;
        }
    }
}
