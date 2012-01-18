using System.Collections.Generic;
using Loki.Data;

namespace Loki.Maple.Data
{
    public class ForbiddenNames : List<string>
    {
        public ForbiddenNames()
            : base()
        {
            using (Log.Load("ForbiddenNames"))
            {
                foreach (dynamic nameDatum in new Datums("character_forbidden_names").Populate())
                {
                    this.Add(nameDatum.forbidden_name);
                }
            }
        }
    }
}
