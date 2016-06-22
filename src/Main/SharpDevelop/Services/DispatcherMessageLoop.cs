// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
