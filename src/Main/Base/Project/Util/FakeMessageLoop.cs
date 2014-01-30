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
