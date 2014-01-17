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
		/// <para>This method:</para>
		/// <list type="items">
		/// <item>Checks if <see cref="PreventShutdown"/> was called and abort the shutdown if it was.</item>
		/// <item>Prompts the user to save the open files. The user has the option to cancel the shutdown at that point.</item>
		/// <item>Closes the solution.</item>
		/// <item>Signals the <see cref="ShutdownToken"/>.</item>
		/// <item>Disposes pads</item>
		/// <item>Waits for background tasks (<see cref="AddBackgroundTask"/>) to finish.</item>
		/// <item>Disposes services</item>
		/// <item>Saves the PropertyService</item>
		/// </list>
		/// <para>
		/// This method must be called on the main thread.
		/// </para>
		/// </remarks>
		bool Shutdown();
		
		/// <summary>
		/// Prevents shutdown with the following reason.
		/// Dispose the returned value to allow shutdown again.
		/// </summary>
		/// <param name="reason">The reason, will be displayed to the user.</param>
		/// <exception cref="InvalidOperationException">Shutdown is already in progress</exception>
		/// <remarks>This method is thread-safe.</remarks>
		IDisposable PreventShutdown(string reason);
		
		/// <summary>
		/// Gets the current reason that prevents shutdown.
		/// If there isn't any reason, returns null.
		/// If there are multiple reasons, only one of them is returned.
		/// </summary>
		/// <remarks>This property is thread-safe.</remarks>
		string CurrentReasonPreventingShutdown { get; }
		
		/// <summary>
		/// Gets a cancellation token that gets signalled when SharpDevelop is shutting down.
		/// 
		/// This cancellation token may be used to stop background calculations.
		/// </summary>
		/// <remarks>This property is thread-safe.</remarks>
		CancellationToken ShutdownToken { get; }
		
		/// <summary>
		/// Gets a cancellation token that gets signalled a couple of seconds after the ShutdownToken.
		/// 
		/// This cancellation token may be used to stop background calculations that should run
		/// for a limited time after SharpDevelop is closed (e.g. saving state in caches
		/// - work that should better run even though we're shutting down, but shouldn't take too long either)
		/// </summary>
		/// <remarks>This property is thread-safe.</remarks>
		CancellationToken DelayedShutdownToken { get; }
		
		/// <summary>
		/// Adds a background task on which SharpDevelop should wait on shutdown.
		/// 
		/// Use this method for tasks that asynchronously write state to disk and should not be
		/// interrupted by SharpDevelop closing down.
		/// </summary>
		/// <remarks>This method is thread-safe.</remarks>
		void AddBackgroundTask(Task task);
	}
}
