using System.Collections.Generic;
using Loki.Collections;
using Loki.Maple.Life;
using Loki.Net;

namespace Loki.Maple.Characters
{
    // NOTE: I removed interface inheritance because interfaces are a mess: We need multiple inheritance, but we don't have it.

    public class ControlledMobs : NumericalKeyedCollection<Mob>
    {
        public Character Parent { get; private set; }

        public ControlledMobs(Character parent)
        {
            this.Parent = parent;
        }

        protected override int GetKeyForItem(Mob item)
        {
            return item.ObjectID;
        }

        protected override void InsertItem(int index, Mob item)
        {
            lock (this)
            {
                if (this.Parent.Client.IsAlive)
                {
                    item.Controller = this.Parent;
                    base.InsertItem(index, item);

                    using (Packet requestControl = item.GetControlRequestPacket())
                    {
                        item.Controller.Client.Send(requestControl);
                    }
                }
                else
                {
                    item.AssignController();
                }
            }
        }

        protected override void RemoveItem(int index)
        {
            lock (this)
            {
                Mob item = this.GetAtIndex(index);

                if (item.Controller.Client.IsAlive)
                {
                    using (Packet cancelControl = item.GetControlCancelPacket())
                    {
                        item.Controller.Client.Send(cancelControl);
                    }
                }

                item.Controller = null;
                base.RemoveItem(index);
            }
        }

        protected override void ClearItems()
        {
            lock (this)
            {
                List<Mob> toRemove = new List<Mob>();

                foreach (Mob loopController in this)
                {
                    toRemove.Add(loopController);
                }

                foreach (Mob loopController in toRemove)
                {
                    this.Remove(loopController);
                }
            }
        }

        public void Move(Packet inPacket)
        {
            int objectId = inPacket.ReadInt();

            lock (this)
            {
                if (this.Contains(objectId))
                {
                    this[objectId].Move(inPacket);
                }
            }
        }
    }
}
