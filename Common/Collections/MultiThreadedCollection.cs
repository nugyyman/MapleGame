using System;
using System.Collections.ObjectModel;
using System.Threading;

namespace Loki.Collections
{
	public class MultiThreadedCollection<TItem, TInput> : Collection<TItem>, IDisposable
	{
		private int NeededResponses { get; set; }
		private ManualResetEvent CollectDone = new ManualResetEvent(false);

		public MultiThreadedCollection(int neededResponses)
			: base()
		{
			this.NeededResponses = neededResponses;
		}

		public void WaitUntilDone()
		{
			if (this.NeededResponses > 0)
			{
				this.CollectDone.WaitOne();
			}
		}

		public void AddFromThread(Func<TInput, TItem> target, TInput input)
		{
			new Thread(delegate()
			{
				this.Add(target(input));

				if (this.Count == this.NeededResponses)
				{
					this.CollectDone.Set();
				}
			}).Start();
		}

		public void Dispose()
		{
			this.CollectDone.Dispose();
		}
	}
}
