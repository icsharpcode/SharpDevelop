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
using System.Collections.Generic;

using System.Windows.Input;

using ICSharpCode.Core;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Parser;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Workbench;

namespace ICSharpCode.SharpDevelop
{

	/// <summary>
	/// Provides the infrastructure to handle generalized code navigation.
	/// </summary>
	/// <remarks>
	/// <para>This service is not limited to navigating code; rather, it
	/// integrates with SharpDevelop's extendable <see cref="IViewContent"/>
	/// interface so that each window has the opportunity to implement a
	/// contextually appropriate navigation scheme.</para>
	/// <para>The default scheme, <see cref="DefaultNavigationPoint"/>, is
	/// created automatically in the <see cref="AbstractViewContent"/>
	/// implementation.  This scheme supports the basic function of logging a
	/// filename and returning to that file's default view.</para>
	/// <para>The default text editor provides a slightly more sophisticated
	/// scheme, <see cref="ICSharpCode.SharpDevelop.TextNavigationPoint">
	/// TextEditorNavigationPoint</see>, that logs filename and line number.</para>
	/// <para>To implement your own navigation scheme, implement
	/// <see cref="IViewContent"/> or derive from
	/// <see cref="AbstractViewContent"/> and override the
	/// <see cref="IViewContent.BuildNavPoint">BuildNavigationPoint</see>
	/// method.</para>
	/// <para>
	/// <i>History logic based in part on Orlando Curioso's <i>Code Project</i> article:
	/// <see href="http://www.codeproject.com/cs/miscctrl/WinFormsHistory.asp">
	/// Navigational history (go back/forward) for WinForms controls</see></i>
	/// </para>
	/// </remarks>
	public sealed class NavigationService
	{
		#region Private members
		static LinkedList<INavigationPoint> history = new LinkedList<INavigationPoint>();
		static LinkedListNode<INavigationPoint> currentNode;
		static int loggingSuspendedCount;
		static bool serviceInitialized;
		#endregion
		
		/// <summary>
		/// Keeps .NET compiler from autogenerating a public constructor that breaks an FxCop rule #CA1053
		/// </summary>
		private NavigationService()
		{
		}
		
		/// <summary>
		/// Initializes the NavigationService.
		/// </summary>
		/// <remarks>Must be called after the Workbench has been initialized and after the ProjectService has been initialized.</remarks>
		/// <exception cref="InvalidOperationException">The <see cref="WorkbenchSingleton"/> has not yet been initialized and <see cref="WorkbenchSingleton.Workbench">Workbench</see> is <value>null</value></exception>
		public static void InitializeService()
		{
			if (!serviceInitialized) {
				if (SD.Workbench == null) {
					throw new InvalidOperationException("Initializing the NavigationService requires that the WorkbenchSingleton has already created a Workbench.");
				}
				// trap changes in the secondary tab via the workbench's ActiveViewContentChanged event
				SD.Workbench.ActiveViewContentChanged += ActiveViewContentChanged;
				
				SD.FileService.FileRenamed += FileService_FileRenamed;
				SD.ProjectService.SolutionClosed += ProjectService_SolutionClosed;
				serviceInitialized = true;
			}
		}
		
		/// <summary>
		/// Unloads the <see cref="NavigationService"/>
		/// </summary>
		public static void Unload()
		{
			// perform any necessary cleanup
			HistoryChanged = null;
			ClearHistory(true);
			serviceInitialized = false;
		}
		
		#region Public Properties
		
		/// <summary>
		/// <b>true</b> if we can navigate back to a previous point; <b>false</b>
		/// if there are no points in the history.
		/// </summary>
		public static bool CanNavigateBack {
			get { return currentNode != history.First && currentNode != null;}
		}
		
		/// <summary>
		/// <b>true</b> if we can navigate forwards to a point prevously left
		/// via navigating backwards; <b>false</b> if all the points in the
		/// history are in the "past".
		/// </summary>
		public static bool CanNavigateForwards {
			get {return currentNode != history.Last && currentNode != null;}
		}
		
		/// <summary>
		/// Gets the number of points in the navigation history.
		/// </summary>
		/// <remarks>
		/// <b>Note:</b> jumping forwards or backwards requires at least
		/// two points in the history; otherwise navigating has no meaning.
		/// </remarks>
		public static int Count {
			get { return history.Count; }
		}
		
		/// <summary>
		/// Gets or sets the "current" position as tracked by the service.
		/// </summary>
		public static INavigationPoint CurrentPosition {
			get {
				return currentNode==null ? (INavigationPoint)null : currentNode.Value;
			}
			set {
				Log(value);
			}
		}
		
		/// <summary>
		/// <b>true</b> when the service is logging points; <b>false</b> when
		/// logging is suspended.
		/// </summary>
		public static bool IsLogging {
			get { return loggingSuspendedCount == 0;}
		}
		#endregion

		#region Public Methods
		
		// TODO: FxCop says "find another way to do this" (ReviewVisibleEventHandlers)
		// we'd have to ask each point that cares to subscribe to the appropriate event
		// listeners in their respective IViewContent implementation's underlying models.
		public static void ContentChanging(object sender, EventArgs e)
		{
			foreach (INavigationPoint p in history)
			{
				p.ContentChanging(sender, e);
			}
		}
		
		/*		static void Log(IWorkbenchWindow window)
		{
			if (window==null) return;
			Log(window.ActiveViewContent);
		}
		 */

		
		/// <summary>
		/// Asks an <see cref="IViewContent"/> implementation to build an
		/// <see cref="INavigationPoint"/> and then logs it.
		/// </summary>
		/// <param name="vc"></param>
		public static void Log(IViewContent vc)
		{
			if (vc==null) return;
			Log(vc.BuildNavPoint());
		}
		
		/// <summary>
		/// Adds an <see cref="INavigationPoint"/> to the history.
		/// </summary>
		/// <param name="pointToLog">The point to store.</param>
		public static void Log(INavigationPoint pointToLog)
		{
			if (loggingSuspendedCount > 0) {
				return;
			}
			LogInternal(pointToLog);
		}

		/// <summary>
		/// Adds an <see cref="INavigationPoint"/> to the history.
		/// </summary>
		/// <param name="p">The <see cref="INavigationPoint"/> to add.</param>
		/// <remarks>
		/// Refactoring this out of Log() allows the NavigationService
		/// to call this and ensure it will work regardless of the
		/// requested state of loggingSuspended, as in
		/// <see cref="ClearHistory()"/> where we want to log
		/// the current position after clearing the
		/// history.
		/// </remarks>
		private static void LogInternal(INavigationPoint p)
		{
			if (p == null
			    || String.IsNullOrEmpty(p.FileName)
			   )
			{
				return;
			}
			if (currentNode==null) {
				currentNode = history.AddFirst(p);
			} else if (p.Equals(currentNode.Value)) {
				// replace it
				currentNode.Value = p;
			} else {
				currentNode = history.AddAfter(currentNode, p);
			}
			OnHistoryChanged();
		}
		
		/// <summary>
		/// Gets a <see cref="List{T}"/> of the <see cref="INavigationPoint">INavigationPoints</see> that
		/// are currently in the collection.
		/// </summary>
		public static ICollection<INavigationPoint> Points
		{
			get {
				return new List<INavigationPoint>(history);
			}
		}
		
		/// <summary>
		/// Clears the navigation history (except for the current position).
		/// </summary>
		public static void ClearHistory()
		{
			ClearHistory(false);
		}
		
		/// <summary>
		/// Clears the navigation history and optionally clears the current position.
		/// </summary>
		/// <param name="clearCurrentPosition">Do we clear the current position as well as the rest of the history?</param>
		/// <remarks>
		/// <para>The current position is often used to "seed" the next history to ensure
		/// that the first significant movement after clearing the history allows
		/// us to jump "back" immediately.</para>
		/// <para>Remembering the current position across requests to clear the history
		/// does not always make sense, however, such as when a solution is closing,
		/// hence the ability to explicitly control it's retention.</para>
		/// </remarks>
		public static void ClearHistory(bool clearCurrentPosition)
		{
			INavigationPoint currentPosition = CurrentPosition;
			history.Clear();
			currentNode = null;
			if (!clearCurrentPosition) {
				LogInternal(currentPosition);
			}
			OnHistoryChanged();
		}
		
		/// <summary>
		/// Navigates to an <see cref="INavigationPoint"/> that is an arbitrary
		/// number of points away from the <see cref="CurrentPosition"/>.
		/// </summary>
		/// <param name="delta">Number of points to move; negative deltas move
		/// backwards while positive deltas move forwards through the history.</param>
		public static void Go(int delta)
		{
			if (0 == delta) {
				// no movement required
				return;
			} else if (0>delta) {
				// move backwards
				while (0>delta && currentNode!=history.First) {
					currentNode = currentNode.Previous;
					delta++;
				}
			} else {
				// move forwards
				while (0<delta && currentNode!=history.Last) {
					currentNode = currentNode.Next;
					delta--;
				}
			}
			
			SyncViewWithModel();
		}
		
		/// <summary>
		/// Jump to a specific <see cref="INavigationPoint"/> in the history;
		/// if the point is not in the history, we log it internally, regardless
		/// of whether logging is currently suspended or not.
		/// </summary>
		/// <param name="target">The <see cref="INavigationPoint"/> to jump</param>
		public static void Go(INavigationPoint target)
		{
			if (target==null) {
				return;
			}
			
			LinkedListNode<INavigationPoint> targetNode;
			targetNode = history.Find(target);
			if (targetNode!=null) {
				currentNode = targetNode;
			} else {
				LoggingService.ErrorFormatted("Logging additional point: {0}", target);
				LogInternal(target);
			}
			
			SyncViewWithModel();
		}
		
		/// <summary>
		/// Navigates the view (i.e. the workbench) to whatever
		/// <see cref="INavigationPoint"/> is the current
		/// position in the internal model.
		/// </summary>
		/// <remarks>Factoring this out of code that manipulates
		/// the history allows to make multiple changes to the
		/// history while only updating the view once we are
		/// finished.</remarks>
		private static void SyncViewWithModel()
		{
			SuspendLogging();
			if (CurrentPosition!=null) {
				CurrentPosition.JumpTo();
			}
			ResumeLogging();
		}
		
		/// <summary>
		/// Suspends logging of navigation so that we don't log intermediate points
		/// while opening a file, for example.
		/// </summary>
		/// <remarks>
		/// Suspend/resume statements are incremental, i.e. you must call
		/// <see cref="ResumeLogging"/> once for each time you call
		/// <see cref="SuspendLogging"/> to resume logging.
		/// </remarks>
		public static void SuspendLogging()
		{
			LoggingService.Debug("NavigationService -- suspend logging");
			System.Threading.Interlocked.Increment(ref loggingSuspendedCount);
		}
		
		/// <summary>
		/// Resumes logging after suspending it via <see cref="SuspendLogging"/>.
		/// </summary>
		public static void ResumeLogging()
		{
			LoggingService.Debug("NavigationService -- resume logging");
			if (System.Threading.Interlocked.Decrement(ref loggingSuspendedCount) < 0) {
				System.Threading.Interlocked.Increment(ref loggingSuspendedCount);
				LoggingService.Warn("NavigationService -- ResumeLogging called without corresponding SuspendLogging");
			}
		}
		
		#endregion

		// the following code is not covered by Unit tests as i wasn't sure
		// how to test code triggered by the user interacting with the workbench
		#region event trapping

		/// <summary>
		/// Respond to changes in the <see cref="IWorkbench.ActiveViewContent">
		/// ActiveViewContent</see> by logging the new <see cref="IViewContent"/>.
		/// </summary>
		static void ActiveViewContentChanged(object sender, EventArgs e)
		{
			IViewContent vc = SD.Workbench.ActiveViewContent;
			if (vc == null) return;
			LoggingService.DebugFormatted("NavigationService\n\tActiveViewContent: {0}\n\t          Subview: {1}",
			                              vc.TitleName,
			                              vc.TabPageText);
			Log(vc);
		}
		
		/// <summary>
		/// Respond to changes in filenames by updating points in the history
		/// to reflect the change.
		/// </summary>
		/// <param name="sender"/>
		/// <param name="e"><see cref="FileRenameEventArgs"/> describing
		/// the file rename.</param>
		static void FileService_FileRenamed(object sender, FileRenameEventArgs e)
		{
			foreach (INavigationPoint p in history) {
				if (p.FileName.Equals(e.SourceFile)) {
					p.FileNameChanged(e.TargetFile);
				}
			}
		}

		/// <summary>
		/// Responds to the <see cref="ProjectService"/>.<see cref="ProjectService.SolutionClosed"/> event.
		/// </summary>
		static void ProjectService_SolutionClosed(object sender, EventArgs e)
		{
			NavigationService.ClearHistory(true);
		}
		
		#endregion
		
		#region Public Events

		/// <summary>
		/// Fires whenever the navigation history has changed.
		/// </summary>
		public static event System.EventHandler HistoryChanged;
		
		/// <summary>
		/// Used internally to call the <see cref="HistoryChanged"/> event delegates.
		/// </summary>
		static void OnHistoryChanged()
		{
			if (HistoryChanged!=null) {
				HistoryChanged(NavigationService.CurrentPosition, EventArgs.Empty);
			}
			CommandManager.InvalidateRequerySuggested();
		}
		#endregion
		
		#region Navigate To Entity
		public static bool NavigateTo(IEntity entity)
		{
			if (entity == null)
				throw new ArgumentNullException("entity");
			var region = entity.Region;
			
			if (region.IsEmpty || string.IsNullOrEmpty(region.FileName)) {
				foreach (var item in AddInTree.BuildItems<INavigateToEntityService>("/SharpDevelop/Services/NavigateToEntityService", null, false)) {
					if (item.NavigateToEntity(entity))
						return true;
				}
				return false;
			}
			
			return FileService.JumpToFilePosition(region.FileName, region.BeginLine, region.BeginColumn) != null;
		}
		#endregion
	}
	
	/// <summary>
	/// Called by <see cref="NavigationService.NavigateTo"/> when the entity is not defined in source code.
	/// </summary>
	/// <remarks>
	/// Loaded from addin tree path "/SharpDevelop/Services/NavigateToEntityService"
	/// </remarks>
	public interface INavigateToEntityService
	{
		bool NavigateToEntity(IEntity entity);
	}
}
