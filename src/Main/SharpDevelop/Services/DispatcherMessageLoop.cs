// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace ICSharpCode.SharpDevelop
{
	sealed class DispatcherMessageLoop : IMessageLoop, ISynchronizeInvoke
	{
		readonly Dispatcher dispatcher;
		readonly SynchronizationContext synchronizationContext;
		
		public DispatcherMessageLoop(Dispatcher dispatcher, SynchronizationContext synchronizationContext)
		{
			this.dispatcher = dispatcher;
			this.synchronizationContext = synchronizationContext;
		}
		
		public Thread Thread {
			get { return dispatcher.Thread; }
		}
		
		public Dispatcher Dispatcher {
			get { return dispatcher; }
		}
		
		public SynchronizationContext SynchronizationContext {
			get { return synchronizationContext; }
		}
		
		public ISynchronizeInvoke SynchronizingObject {
			get { return this; }
		}
		
		public bool InvokeRequired {
			get { return !dispatcher.CheckAccess(); }
		}
		
		public bool CheckAccess()
		{
			return dispatcher.CheckAccess();
		}
		
		public void VerifyAccess()
		{
			dispatcher.VerifyAccess();
		}
		
		public void InvokeIfRequired(Action callback)
		{
			if (dispatcher.CheckAccess())
				callback();
			else
				dispatcher.Invoke(callback);
		}
		
		public void InvokeIfRequired(Action callback, DispatcherPriority priority)
		{
			if (dispatcher.CheckAccess())
				callback();
			else
				dispatcher.Invoke(callback, priority);
		}
		
		public void InvokeIfRequired(Action callback, DispatcherPriority priority, CancellationToken cancellationToken)
		{
			if (dispatcher.CheckAccess())
				callback();
			else
				dispatcher.Invoke(callback, priority, cancellationToken);
		}
		
		public T InvokeIfRequired<T>(Func<T> callback)
		{
			if (dispatcher.CheckAccess())
				return callback();
			else
				return dispatcher.Invoke(callback);
		}
		
		public T InvokeIfRequired<T>(Func<T> callback, DispatcherPriority priority)
		{
			if (dispatcher.CheckAccess())
				return callback();
			else
				return dispatcher.Invoke(callback, priority);
		}
		
		public T InvokeIfRequired<T>(Func<T> callback, DispatcherPriority priority, CancellationToken cancellationToken)
		{
			if (dispatcher.CheckAccess())
				return callback();
			else
				return dispatcher.Invoke(callback, priority, cancellationToken);
		}
		
		public Task InvokeAsync(Action callback)
		{
			return dispatcher.InvokeAsync(callback).Task;
		}
		
		public Task InvokeAsync(Action callback, DispatcherPriority priority)
		{
			return dispatcher.InvokeAsync(callback, priority).Task;
		}
		
		public Task InvokeAsync(Action callback, DispatcherPriority priority, CancellationToken cancellationToken)
		{
			return dispatcher.InvokeAsync(callback, priority, cancellationToken).Task;
		}
		
		public Task<T> InvokeAsync<T>(Func<T> callback)
		{
			return dispatcher.InvokeAsync(callback).Task;
		}
		
		public Task<T> InvokeAsync<T>(Func<T> callback, DispatcherPriority priority)
		{
			return dispatcher.InvokeAsync(callback, priority).Task;
		}
		
		public Task<T> InvokeAsync<T>(Func<T> callback, DispatcherPriority priority, CancellationToken cancellationToken)
		{
			return dispatcher.InvokeAsync(callback, priority, cancellationToken).Task;
		}
		
		public void InvokeAsyncAndForget(Action callback)
		{
			dispatcher.BeginInvoke(callback);
		}
		
		public void InvokeAsyncAndForget(Action callback, DispatcherPriority priority)
		{
			dispatcher.BeginInvoke(callback, priority);
		}
		
		public async void CallLater(TimeSpan delay, Action method)
		{
			await Task.Delay(delay).ConfigureAwait(false);
			InvokeAsyncAndForget(method);
		}
		
		IAsyncResult ISynchronizeInvoke.BeginInvoke(Delegate method, object[] args)
		{
			return dispatcher.BeginInvoke(method, args).Task;
		}
		
		object ISynchronizeInvoke.EndInvoke(IAsyncResult result)
		{
			return ((Task<object>)result).Result;
		}
		
		object ISynchronizeInvoke.Invoke(Delegate method, object[] args)
		{
			return dispatcher.Invoke(method, args);
		}
	}
}
