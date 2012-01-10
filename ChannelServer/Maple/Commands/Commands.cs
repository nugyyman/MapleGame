using System.Collections.ObjectModel;

namespace Loki.Maple.Commands
{
    class Commands : KeyedCollection<string, Command>
    {
        protected override string GetKeyForItem(Command item)
        {
            return item.Name.ToLower();
        }
    }
}
