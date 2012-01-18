using System.Collections.Generic;
using Loki.Data;

namespace Loki.Maple.Data
{
    public class CharacterCreationData
    {
        public List<int> ExplorerMaleFace { get; set; }
        public List<int> ExplorerMaleHair { get; set; }
        public List<int> ExplorerMaleHairColor { get; set; }
        public List<int> ExplorerMaleSkin { get; set; }
        public List<int> ExplorerMaleTop { get; set; }
        public List<int> ExplorerMaleBottom { get; set; }
        public List<int> ExplorerMaleShoes { get; set; }
        public List<int> ExplorerMaleWeapon { get; set; }

        public List<int> ExplorerFemaleFace { get; set; }
        public List<int> ExplorerFemaleHair { get; set; }
        public List<int> ExplorerFemaleHairColor { get; set; }
        public List<int> ExplorerFemaleSkin { get; set; }
        public List<int> ExplorerFemaleTop { get; set; }
        public List<int> ExplorerFemaleBottom { get; set; }
        public List<int> ExplorerFemaleShoes { get; set; }
        public List<int> ExplorerFemaleWeapon { get; set; }

        public List<int> CygnusMaleFace { get; set; }
        public List<int> CygnusMaleHair { get; set; }
        public List<int> CygnusMaleHairColor { get; set; }
        public List<int> CygnusMaleSkin { get; set; }
        public List<int> CygnusMaleTop { get; set; }
        public List<int> CygnusMaleBottom { get; set; }
        public List<int> CygnusMaleShoes { get; set; }
        public List<int> CygnusMaleWeapon { get; set; }

        public List<int> CygnusFemaleFace { get; set; }
        public List<int> CygnusFemaleHair { get; set; }
        public List<int> CygnusFemaleHairColor { get; set; }
        public List<int> CygnusFemaleSkin { get; set; }
        public List<int> CygnusFemaleTop { get; set; }
        public List<int> CygnusFemaleBottom { get; set; }
        public List<int> CygnusFemaleShoes { get; set; }
        public List<int> CygnusFemaleWeapon { get; set; }

        public List<int> AranMaleFace { get; set; }
        public List<int> AranMaleHair { get; set; }
        public List<int> AranMaleHairColor { get; set; }
        public List<int> AranMaleSkin { get; set; }
        public List<int> AranMaleTop { get; set; }
        public List<int> AranMaleBottom { get; set; }
        public List<int> AranMaleShoes { get; set; }
        public List<int> AranMaleWeapon { get; set; }

        public List<int> AranFemaleFace { get; set; }
        public List<int> AranFemaleHair { get; set; }
        public List<int> AranFemaleHairColor { get; set; }
        public List<int> AranFemaleSkin { get; set; }
        public List<int> AranFemaleTop { get; set; }
        public List<int> AranFemaleBottom { get; set; }
        public List<int> AranFemaleShoes { get; set; }
        public List<int> AranFemaleWeapon { get; set; }

        public CharacterCreationData()
            : base()
        {
            this.ExplorerMaleFace = new List<int>();
            this.ExplorerMaleHair = new List<int>();
            this.ExplorerMaleHairColor = new List<int>();
            this.ExplorerMaleSkin = new List<int>();
            this.ExplorerMaleTop = new List<int>();
            this.ExplorerMaleBottom = new List<int>();
            this.ExplorerMaleShoes = new List<int>();
            this.ExplorerMaleWeapon = new List<int>();

            this.ExplorerFemaleFace = new List<int>();
            this.ExplorerFemaleHair = new List<int>();
            this.ExplorerFemaleHairColor = new List<int>();
            this.ExplorerFemaleSkin = new List<int>();
            this.ExplorerFemaleTop = new List<int>();
            this.ExplorerFemaleBottom = new List<int>();
            this.ExplorerFemaleShoes = new List<int>();
            this.ExplorerFemaleWeapon = new List<int>();

            this.CygnusMaleFace = new List<int>();
            this.CygnusMaleHair = new List<int>();
            this.CygnusMaleHairColor = new List<int>();
            this.CygnusMaleSkin = new List<int>();
            this.CygnusMaleTop = new List<int>();
            this.CygnusMaleBottom = new List<int>();
            this.CygnusMaleShoes = new List<int>();
            this.CygnusMaleWeapon = new List<int>();

            this.CygnusFemaleFace = new List<int>();
            this.CygnusFemaleHair = new List<int>();
            this.CygnusFemaleHairColor = new List<int>();
            this.CygnusFemaleSkin = new List<int>();
            this.CygnusFemaleTop = new List<int>();
            this.CygnusFemaleBottom = new List<int>();
            this.CygnusFemaleShoes = new List<int>();
            this.CygnusFemaleWeapon = new List<int>();

            this.AranMaleFace = new List<int>();
            this.AranMaleHair = new List<int>();
            this.AranMaleHairColor = new List<int>();
            this.AranMaleSkin = new List<int>();
            this.AranMaleTop = new List<int>();
            this.AranMaleBottom = new List<int>();
            this.AranMaleShoes = new List<int>();
            this.AranMaleWeapon = new List<int>();

            this.AranFemaleFace = new List<int>();
            this.AranFemaleHair = new List<int>();
            this.AranFemaleHairColor = new List<int>();
            this.AranFemaleSkin = new List<int>();
            this.AranFemaleTop = new List<int>();
            this.AranFemaleBottom = new List<int>();
            this.AranFemaleShoes = new List<int>();
            this.AranFemaleWeapon = new List<int>();

            using (Log.Load("Character creation data"))
            {
                foreach (dynamic dataDatum in new Datums("character_creation_data").Populate())
                {
                    switch ((string)dataDatum.object_type)
                    {
                        case "face":
                            switch ((string)dataDatum.character_type)
                            {
                                case "regular":
                                    if (dataDatum.gender.ToString().Equals("male"))
                                        this.ExplorerMaleFace.Add(dataDatum.objectid);
                                    else
                                        this.ExplorerFemaleFace.Add(dataDatum.objectid);
                                    break;
                                case "cygnus":
                                    if (dataDatum.gender.ToString().Equals("male"))
                                        this.CygnusMaleFace.Add(dataDatum.objectid);
                                    else
                                        this.CygnusFemaleFace.Add(dataDatum.objectid);
                                    break;
                                case "aran":
                                    if (dataDatum.gender.ToString().Equals("male"))
                                        this.AranMaleFace.Add(dataDatum.objectid);
                                    else
                                        this.AranFemaleFace.Add(dataDatum.objectid);
                                    break;
                            }
                            break;
                        case "hair":
                            switch ((string)dataDatum.character_type)
                            {
                                case "regular":
                                    if (dataDatum.gender.ToString().Equals("male"))
                                        this.ExplorerMaleHair.Add(dataDatum.objectid);
                                    else
                                        this.ExplorerFemaleHair.Add(dataDatum.objectid);
                                    break;
                                case "cygnus":
                                    if (dataDatum.gender.ToString().Equals("male"))
                                        this.CygnusMaleHair.Add(dataDatum.objectid);
                                    else
                                        this.CygnusFemaleHair.Add(dataDatum.objectid);
                                    break;
                                case "aran":
                                    if (dataDatum.gender.ToString().Equals("male"))
                                        this.AranMaleHair.Add(dataDatum.objectid);
                                    else
                                        this.AranFemaleHair.Add(dataDatum.objectid);
                                    break;
                            }
                            break;
                        case "haircolor":
                            switch ((string)dataDatum.character_type)
                            {
                                case "regular":
                                    if (dataDatum.gender.ToString().Equals("male"))
                                        this.ExplorerMaleHairColor.Add(dataDatum.objectid);
                                    else
                                        this.ExplorerFemaleHairColor.Add(dataDatum.objectid);
                                    break;
                                case "cygnus":
                                    if (dataDatum.gender.ToString().Equals("male"))
                                        this.CygnusMaleHairColor.Add(dataDatum.objectid);
                                    else
                                        this.CygnusFemaleHairColor.Add(dataDatum.objectid);
                                    break;
                                case "aran":
                                    if (dataDatum.gender.ToString().Equals("male"))
                                        this.AranMaleHairColor.Add(dataDatum.objectid);
                                    else
                                        this.AranFemaleHairColor.Add(dataDatum.objectid);
                                    break;
                            }
                            break;
                        case "skin":
                            switch ((string)dataDatum.character_type)
                            {
                                case "regular":
                                    if (dataDatum.gender.ToString().Equals("male"))
                                        this.ExplorerMaleSkin.Add(dataDatum.objectid);
                                    else
                                        this.ExplorerFemaleSkin.Add(dataDatum.objectid);
                                    break;
                                case "cygnus":
                                    if (dataDatum.gender.ToString().Equals("male"))
                                        this.CygnusMaleSkin.Add(dataDatum.objectid);
                                    else
                                        this.CygnusFemaleSkin.Add(dataDatum.objectid);
                                    break;
                                case "aran":
                                    if (dataDatum.gender.ToString().Equals("male"))
                                        this.AranMaleSkin.Add(dataDatum.objectid);
                                    else
                                        this.AranFemaleSkin.Add(dataDatum.objectid);
                                    break;
                            }
                            break;
                        case "top":
                            switch ((string)dataDatum.character_type)
                            {
                                case "regular":
                                    if (dataDatum.gender.ToString().Equals("male"))
                                        this.ExplorerMaleTop.Add(dataDatum.objectid);
                                    else
                                        this.ExplorerFemaleTop.Add(dataDatum.objectid);
                                    break;
                                case "cygnus":
                                    if (dataDatum.gender.ToString().Equals("male"))
                                        this.CygnusMaleTop.Add(dataDatum.objectid);
                                    else
                                        this.CygnusFemaleTop.Add(dataDatum.objectid);
                                    break;
                                case "aran":
                                    if (dataDatum.gender.ToString().Equals("male"))
                                        this.AranMaleTop.Add(dataDatum.objectid);
                                    else
                                        this.AranFemaleTop.Add(dataDatum.objectid);
                                    break;
                            }
                            break;
                        case "bottom":
                            switch ((string)dataDatum.character_type)
                            {
                                case "regular":
                                    if (dataDatum.gender.ToString().Equals("male"))
                                        this.ExplorerMaleBottom.Add(dataDatum.objectid);
                                    else
                                        this.ExplorerFemaleBottom.Add(dataDatum.objectid);
                                    break;
                                case "cygnus":
                                    if (dataDatum.gender.ToString().Equals("male"))
                                        this.CygnusMaleBottom.Add(dataDatum.objectid);
                                    else
                                        this.CygnusFemaleBottom.Add(dataDatum.objectid);
                                    break;
                                case "aran":
                                    if (dataDatum.gender.ToString().Equals("male"))
                                        this.AranMaleBottom.Add(dataDatum.objectid);
                                    else
                                        this.AranFemaleBottom.Add(dataDatum.objectid);
                                    break;
                            }
                            break;
                        case "shoes":
                            switch ((string)dataDatum.character_type)
                            {
                                case "regular":
                                    if (dataDatum.gender.ToString().Equals("male"))
                                        this.ExplorerMaleShoes.Add(dataDatum.objectid);
                                    else
                                        this.ExplorerFemaleShoes.Add(dataDatum.objectid);
                                    break;
                                case "cygnus":
                                    if (dataDatum.gender.ToString().Equals("male"))
                                        this.CygnusMaleShoes.Add(dataDatum.objectid);
                                    else
                                        this.CygnusFemaleShoes.Add(dataDatum.objectid);
                                    break;
                                case "aran":
                                    if (dataDatum.gender.ToString().Equals("male"))
                                        this.AranMaleShoes.Add(dataDatum.objectid);
                                    else
                                        this.AranFemaleShoes.Add(dataDatum.objectid);
                                    break;
                            }
                            break;
                        case "weapon":
                            switch ((string)dataDatum.character_type)
                            {
                                case "regular":
                                    if (dataDatum.gender.ToString().Equals("male"))
                                        this.ExplorerMaleWeapon.Add(dataDatum.objectid);
                                    else
                                        this.ExplorerFemaleWeapon.Add(dataDatum.objectid);
                                    break;
                                case "cygnus":
                                    if (dataDatum.gender.ToString().Equals("male"))
                                        this.CygnusMaleWeapon.Add(dataDatum.objectid);
                                    else
                                        this.CygnusFemaleWeapon.Add(dataDatum.objectid);
                                    break;
                                case "aran":
                                    if (dataDatum.gender.ToString().Equals("male"))
                                        this.AranMaleWeapon.Add(dataDatum.objectid);
                                    else
                                        this.AranFemaleWeapon.Add(dataDatum.objectid);
                                    break;
                            }
                            break;
                    }
                }
            }
        }

        public bool checkData(int job, byte gender, int face, int hair, int haircolor, int skin, int top, int bottom, int shoes, int weapon)
        {
            switch (job)
            {
                case 0:
                    if (gender == 0)
                    {
                        if (!(this.CygnusMaleFace.Contains(face) && this.CygnusMaleHair.Contains(hair) && this.CygnusMaleHairColor.Contains(haircolor) && this.CygnusMaleSkin.Contains(skin) && this.CygnusMaleTop.Contains(top) && this.CygnusMaleBottom.Contains(bottom) && this.CygnusMaleShoes.Contains(shoes) && this.CygnusMaleWeapon.Contains(weapon)))
                            return false;
                    }
                    else if (gender == 1)
                    {
                        if (!(this.CygnusFemaleFace.Contains(face) && this.CygnusFemaleHair.Contains(hair) && this.CygnusFemaleHairColor.Contains(haircolor) && this.CygnusFemaleSkin.Contains(skin) && this.CygnusFemaleTop.Contains(top) && this.CygnusFemaleBottom.Contains(bottom) && this.CygnusFemaleShoes.Contains(shoes) && this.CygnusFemaleWeapon.Contains(weapon)))
                            return false;
                    }
                    else
                    {
                        return false;
                    }
                    break;
                case 1:
                    if (gender == 0)
                    {
                        if (!(this.ExplorerMaleFace.Contains(face) && this.ExplorerMaleHair.Contains(hair) && this.ExplorerMaleHairColor.Contains(haircolor) && this.ExplorerMaleSkin.Contains(skin) && this.ExplorerMaleTop.Contains(top) && this.ExplorerMaleBottom.Contains(bottom) && this.ExplorerMaleShoes.Contains(shoes) && this.ExplorerMaleWeapon.Contains(weapon)))
                            return false;
                    }
                    else if (gender == 1)
                    {
                        if (!(this.ExplorerFemaleFace.Contains(face) && this.ExplorerFemaleHair.Contains(hair) && this.ExplorerFemaleHairColor.Contains(haircolor) && this.ExplorerFemaleSkin.Contains(skin) && this.ExplorerFemaleTop.Contains(top) && this.ExplorerFemaleBottom.Contains(bottom) && this.ExplorerFemaleShoes.Contains(shoes) && this.ExplorerFemaleWeapon.Contains(weapon)))
                            return false;
                    }
                    else
                    {
                        return false;
                    }
                    break;
                case 2:
                    if (gender == 0)
                    {
                        if (!(this.AranMaleFace.Contains(face) && this.AranMaleHair.Contains(hair) && this.AranMaleHairColor.Contains(haircolor) && this.AranMaleSkin.Contains(skin) && this.AranMaleTop.Contains(top) && this.AranMaleBottom.Contains(bottom) && this.AranMaleShoes.Contains(shoes) && this.AranMaleWeapon.Contains(weapon)))
                            return false;
                    }
                    else if (gender == 1)
                    {
                        if (!(this.AranFemaleFace.Contains(face) && this.AranFemaleHair.Contains(hair) && this.AranFemaleHairColor.Contains(haircolor) && this.AranFemaleSkin.Contains(skin) && this.AranFemaleTop.Contains(top) && this.AranFemaleBottom.Contains(bottom) && this.AranFemaleShoes.Contains(shoes) && this.AranFemaleWeapon.Contains(weapon)))
                            return false;
                    }
                    else
                    {
                        return false;
                    }
                    break;
            }
            return true;
        }
    }
}
