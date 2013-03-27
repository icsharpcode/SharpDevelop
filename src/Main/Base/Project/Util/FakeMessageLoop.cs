// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace ICSharpCode.SharpDevelop
{
	/// <summary>
	/// A fake IMessageLoop implementation that always has <c>InvokeRequired=false</c> and synchronously invokes
	/// the callback passed to <c>InvokeIfRequired</c>.
	/// </summary>
	sealed class FakeMessageLoop : IMessageLoop
	{
		public Thread Thread {
			get {
				throw new NotImplementedException();
			}
		}
		
		public Dispatcher Dispatcher {
			get {
				throw new NotImplementedException();
			}
		}
		
		public SynchronizationContext SynchronizationContext {
			get {
				throw new NotImplementedException();
			}
		}
		
		public System.ComponentModel.ISynchronizeInvoke SynchronizingObject {
			get {
				throw new NotImplementedException();
			}
		}
		
		public bool InvokeRequired {
			get { return false; }
		}
		
		public bool CheckAccess()
		{
			return true;
		}
		
		public void VerifyAccess()
		{
		}
		
		public void InvokeIfRequired(Action callback)
		{
			callback();
		}
		
		public void InvokeIfRequired(Action callback, DispatcherPriority priority)
		{
			callback();
		}
		
		public void InvokeIfRequired(Action callback, DispatcherPriority priority, CancellationToken cancellationToken)
		{
			callback();
		}
		
		public T InvokeIfRequired<T>(Func<T> callback)
		{
			return callback();
		}
		
		public T InvokeIfRequired<T>(Func<T> callback, DispatcherPriority priority)
		{
			return callback();
		}
		
		public T InvokeIfRequired<T>(Func<T> callback, DispatcherPriority priority, CancellationToken cancellationToken)
		{
			return callback();
		}
		
		public Task InvokeAsync(Action callback)
		{
			throw new NotImplementedException();
		}
		
		public Task InvokeAsync(Action callback, DispatcherPriority priority)
		{
			throw new NotImplementedException();
		}
		
		public Task InvokeAsync(Action callback, DispatcherPriority priority, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}
		
		public Task<T> InvokeAsync<T>(Func<T> callback)
		{
			throw new NotImplementedException();
		}
		
		public Task<T> InvokeAsync<T>(Func<T> callback, DispatcherPriority priority)
		{
			throw new NotImplementedException();
		}
		
		public Task<T> InvokeAsync<T>(Func<T> callback, DispatcherPriority priority, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}
		
		public void InvokeAsyncAndForget(Action callback)
		{
			callback();
		}
		
		public void InvokeAsyncAndForget(Action callback, DispatcherPriority priority)
		{
			throw new NotImplementedException();
		}
		
		public void CallLater(TimeSpan delay, Action method)
		{
			throw new NotImplementedException();
		}
	}
}
