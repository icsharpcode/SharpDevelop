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

using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop
{
	/// <summary>
	/// Represents a thread running a message loop.
	/// </summary>
	[SDService("SD.MainThread", FallbackImplementation = typeof(FakeMessageLoop))]
	public interface IMessageLoop
	{
		/// <summary>
		/// Gets the thread that runs this message loop.
		/// </summary>
		Thread Thread { get; }
		
		/// <summary>
		/// Gets the dispatcher for this message loop.
		/// </summary>
		Dispatcher Dispatcher { get; }
		
		/// <summary>
		/// Gets the synchronization context corresponding to this message loop.
		/// </summary>
		SynchronizationContext SynchronizationContext { get; }
		
		/// <summary>
		/// Gets the <see cref="ISynchronizeInvoke"/> implementation corresponding to this message loop.
		/// </summary>
		ISynchronizeInvoke SynchronizingObject { get; }
		
		/// <summary>
		/// Gets whether the current thread is different from the thread running this message loop.
		/// </summary>
		/// <remarks><c>InvokeRequired = !CheckAccess()</c></remarks>
		bool InvokeRequired { get; }
		
		/// <summary>
		/// Gets whether the current thread is the same as the thread running this message loop.
		/// </summary>
		/// <remarks><c>CheckAccess() = !InvokeRequired</c></remarks>
		bool CheckAccess();
		
		/// <summary>
		/// Throws an exception if the current thread is different from the thread running this message loop.
		/// </summary>
		void VerifyAccess();
		
		/// <summary>
		/// Invokes the specified callback on the message loop and waits for its completion.
		/// If the current thread is the thread running the message loop, executes the callback
		/// directly without pumping the message loop.
		/// </summary>
		void InvokeIfRequired(Action callback);
		/// <inheritdoc see="Invoke(Action)"/>
		void InvokeIfRequired(Action callback, DispatcherPriority priority);
		/// <inheritdoc see="Invoke(Action)"/>
		void InvokeIfRequired(Action callback, DispatcherPriority priority, CancellationToken cancellationToken);
		
		/// <summary>
		/// Invokes the specified callback, waits for its completion, and returns the result.
		/// If the current thread is the thread running the message loop, executes the callback
		/// directly without pumping the message loop.
		/// </summary>
		T InvokeIfRequired<T>(Func<T> callback);
		/// <inheritdoc see="Invoke{T}(Func{T})"/>
		T InvokeIfRequired<T>(Func<T> callback, DispatcherPriority priority);
		/// <inheritdoc see="Invoke{T}(Func{T})"/>
		T InvokeIfRequired<T>(Func<T> callback, DispatcherPriority priority, CancellationToken cancellationToken);
		
		/// <summary>
		/// Invokes the specified callback on the message loop.
		/// </summary>
		/// <returns>Returns a task that is signalled when the execution of the callback is completed.</returns>
		/// <remarks>
		/// If the callback method causes an exception; the exception gets stored in the task object.
		/// </remarks>
		Task InvokeAsync(Action callback);
		/// <inheritdoc see="InvokeAsync(Action)"/>
		Task InvokeAsync(Action callback, DispatcherPriority priority);
		/// <inheritdoc see="InvokeAsync(Action)"/>
		Task InvokeAsync(Action callback, DispatcherPriority priority, CancellationToken cancellationToken);
		
		/// <inheritdoc see="InvokeAsync(Action)"/>
		Task<T> InvokeAsync<T>(Func<T> callback);
		/// <inheritdoc see="InvokeAsync(Action)"/>
		Task<T> InvokeAsync<T>(Func<T> callback, DispatcherPriority priority);
		/// <inheritdoc see="InvokeAsync(Action)"/>
		Task<T> InvokeAsync<T>(Func<T> callback, DispatcherPriority priority, CancellationToken cancellationToken);
		
		/// <summary>
		/// Invokes the specified callback on the message loop.
		/// </summary>
		/// <remarks>
		/// This method does not wait for the callback complete execution.
		/// If this method is used on the main thread; the message loop must be pumped before the callback gets to run.
		/// If the callback causes an exception, it is left unhandled.
		/// </remarks>
		void InvokeAsyncAndForget(Action callback);
		/// <inheritdoc see="InvokeAsyncAndForget(Action)"/>
		void InvokeAsyncAndForget(Action callback, DispatcherPriority priority);
		
		/// <summary>
		/// Waits <paramref name="delay"/>, then executes <paramref name="method"/> on the message loop thread.
		/// </summary>
		void CallLater(TimeSpan delay, Action method);
	}
}
