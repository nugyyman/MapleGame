using System.Collections.Generic;
using Loki.Data;

namespace Loki.Maple.Data
{
    public class AvailableStyles
    {
        public List<byte> Skins { get; set; }
        public List<int> MaleHairs { get; set; }
        public List<int> FemaleHairs { get; set; }
        public List<int> MaleFaces { get; set; }
        public List<int> FemaleFaces { get; set; }

        public AvailableStyles()
            : base()
        {
            this.Skins = new List<byte>();

            this.MaleHairs = new List<int>();
            this.FemaleHairs = new List<int>();

            this.MaleFaces = new List<int>();
            this.FemaleFaces = new List<int>();

            using (Log.Load("Styles"))
            {
                foreach (dynamic datum in new Datums("character_skin_data").Populate())
                {
                    this.Skins.Add(datum.skinid);
                }

                foreach (dynamic datum in new Datums("character_hair_data").Populate())
                {
                    switch ((string)datum.gender)
                    {
                        case "male":
                            this.MaleHairs.Add(datum.hairid);
                            break;

                        case "female":
                            this.FemaleHairs.Add(datum.hairid);
                            break;
                    }
                }

                foreach (dynamic datum in new Datums("character_face_data").Populate())
                {
                    switch ((string)datum.gender)
                    {
                        case "male":
                            this.MaleFaces.Add(datum.faceid);
                            break;

                        case "female":
                            this.FemaleFaces.Add(datum.faceid);
                            break;
                    }
                }
            }
        }
    }
}
