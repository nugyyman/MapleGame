using System;
using System.Collections.Generic;
using Loki.Collections;
using Loki.Net;

namespace Loki.Interoperability
{
    public class ChannelsHelper : EnumerationHelper<int, ChannelServerHandler>
    {
        public ChannelsHelper() : base() { }

        public override IEnumerator<ChannelServerHandler> GetEnumerator()
        {
            foreach (World loopWorld in LoginServer.Worlds)
            {
                foreach (ChannelServerHandler loopChannel in loopWorld)
                {
                    yield return loopChannel;
                }
            }
        }

        public override int GetKeyForObject(ChannelServerHandler item)
        {
            return item.InternalID;
        }
    }
}
