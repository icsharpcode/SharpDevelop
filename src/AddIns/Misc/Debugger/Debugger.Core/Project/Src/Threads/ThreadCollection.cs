// <file>
//     <owner name="David Srbecký" email="dsrbecky@post.cz"/>
// </file>

using System;
using System.Collections;

using DebuggerInterop.Core;

namespace DebuggerLibrary
{
	public class ThreadCollection: ReadOnlyCollectionBase
	{
		internal ThreadCollection()
		{

		}

		public event ThreadEventHandler ThreadAdded;

		private void OnThreadAdded(Thread thread)
		{
			thread.ThreadStateChanged += new ThreadEventHandler(OnThreadStateChanged);
			if (ThreadAdded != null)
				ThreadAdded(this, new ThreadEventArgs(thread));
		}


		public event ThreadEventHandler ThreadRemoved;

		private void OnThreadRemoved(Thread thread)
		{
			thread.ThreadStateChanged -= new ThreadEventHandler(OnThreadStateChanged);
			if (ThreadRemoved != null)
				ThreadRemoved(this, new ThreadEventArgs(thread));
		}


		public event ThreadEventHandler ThreadStateChanged;

		private void OnThreadStateChanged(object sender, ThreadEventArgs e)
		{
			if (ThreadStateChanged != null)
				ThreadStateChanged(this, new ThreadEventArgs(e.Thread));
		}


		public Thread this[int index] {
			get {
				return (Thread) InnerList[index];
			}
		}

		internal Thread this[ICorDebugThread corThread]
		{
			get  
			{
				foreach(Thread thread in InnerList)
					if (thread.CorThread == corThread)
						return thread;

				throw new UnableToGetPropertyException(this, "this[ICorDebugThread]", "Thread is not in collection");
			}
		}


		internal void Clear()
		{
            foreach (Thread t in InnerList) {
				OnThreadRemoved(t);
			}
			InnerList.Clear();
		}

		internal void Add(Thread thread)
		{
			System.Diagnostics.Trace.Assert(thread != null);
			if (thread != null)
			{
				InnerList.Add(thread);
				OnThreadAdded(thread);
			}
		}

		internal void Add(ICorDebugThread corThread)
		{
			System.Diagnostics.Trace.Assert(corThread != null);
			if (corThread != null)
				Add(new Thread(corThread));
		}

		internal void Remove(Thread thread)
		{
			InnerList.Remove(thread);
			OnThreadRemoved(thread);
		}

		internal void Remove(ICorDebugThread corThread)
		{
			Remove(this[corThread]);
		}
	}
}
