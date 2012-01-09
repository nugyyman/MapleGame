using System;
using System.Collections.Generic;
using System.Threading;

namespace Loki.Collections
{
	public class MultiThreadedDictionary<TKey, TReturn, TInput> : Dictionary<TKey, TReturn>, IDisposable
	{
		private int NeededResponses { get; set; }
		private ManualResetEvent CollectDone = new ManualResetEvent(false);

		public MultiThreadedDictionary(int neededResponses)
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

		public void AddFromThread(Func<TInput, TReturn> target, TKey key, TInput input)
		{
			new Thread(() =>
			{
				this.Add(key, target(input));

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
