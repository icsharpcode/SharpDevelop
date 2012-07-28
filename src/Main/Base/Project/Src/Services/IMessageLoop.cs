// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
	[SDService]
	public interface IMessageLoop
	{
		/// <summary>
		/// Gets the thread corresponding to this message loop.
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
		/// Invokes the specified callback.
		/// </summary>
		/// <returns>Returns a task that is signalled when the execution of the callback is completed.</returns>
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
	}
}
