// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Threading;
using System.Threading.Tasks;

namespace ICSharpCode.SharpDevelop.Workbench
{
	/// <summary>
	/// Service that manages the IDE shutdown, and any questions.
	/// </summary>
	public interface IShutdownService
	{
		/// <summary>
		/// Attemps to close the IDE.
		/// </summary>
		/// <remarks>
		/// This method will:
		/// - Check if <see cref="PreventShutdown"/> was called and abort the shutdown if it was.
		/// - Prompt the user to save the open files. The user has the option to cancel the shutdown at that point.
		/// - Closes the solution.
		/// - Signals the <see cref="ShutdownToken"/>.
		/// - Disposes pads
		/// - Wait for background tasks (<see cref="AddBackgroundTask"/>) to finish.
		/// - Disposes services
		/// - Saves the PropertyService
		/// 
		/// This method must be called on the main thread.
		/// </remarks>
		bool Shutdown();
		
		/// <summary>
		/// Prevents shutdown with the following reason.
		/// Dispose the returned value to allow shutdown again.
		/// </summary>
		/// <param name="reason">The reason. This parameter will be passed through the StringParser when the reason is displayed to the user.</param>
		/// <exception cref="InvalidOperationException">Shutdown is already in progress</exception>
		/// <remarks>This method is thread-safe.</remarks>
		IDisposable PreventShutdown(string reason);
		
		/// <summary>
		/// Gets the current reason that prevents shutdown.
		/// If there isn't any reason, returns null.
		/// If there are multiple reasons, this returns one of them.
		/// </summary>
		/// <remarks>This method is thread-safe.</remarks>
		string CurrentReasonPreventingShutdown { get; }
		
		/// <summary>
		/// Gets a cancellation token that gets signalled when SharpDevelop is shutting down.
		/// 
		/// This cancellation token may be used to stop background calculations.
		/// </summary>
		CancellationToken ShutdownToken { get; }
		
		/// <summary>
		/// Gets a cancellation token that gets signalled a couple of seconds after the ShutdownToken.
		/// 
		/// This cancellation token may be used to stop background calculations that should run
		/// for a limited time after SharpDevelop is closed (e.g. saving state in caches
		/// - work that should better run even though we're shutting down, but shouldn't take too long either)
		/// </summary>
		CancellationToken DelayedShutdownToken { get; }
		
		/// <summary>
		/// Adds a background task on which SharpDevelop should wait on shutdown.
		/// 
		/// Use this method for tasks that asynchronously write state to disk and should not be
		/// interrupted by SharpDevelop closing down.
		/// </summary>
		void AddBackgroundTask(Task task);
	}
}
