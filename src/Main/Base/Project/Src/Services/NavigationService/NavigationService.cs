/*
 * Created by SharpDevelop.
 * User: David
 * Date: 23/06/2006
 * Time: 12:14 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Collections.Generic;

using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.TextEditor;

namespace ICSharpCode.Core
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
	/// scheme, <see cref="ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor.TextEditorNavigationPoint">
	/// TextEditorNavigationPoint</see>, that logs filename and line number.</para>
	/// <para>To implement your own navigation scheme, implement 
	/// <see cref="IViewContent"/> or derive from 
	/// <see cref="AbstractViewContent"/> and override the 
	/// <see cref="IViewContent.BuildNavigationPoint">BuildNavigationPoint</see> 
	/// method.</para>
	/// <para>
	/// <i>History logic based in part on Orlando Curioso's <i>Code Project</i> article:
	/// <see href="http://www.codeproject.com/cs/miscctrl/WinFormsHistory.asp">
	/// Navigational history (go back/forward) for WinForms controls</see></i>
	/// </para>	
	/// </remarks>
	public class NavigationService
	{
		#region Private members
		static LinkedList<INavigationPoint> history;
		static LinkedListNode<INavigationPoint> currentNode;
		static bool loggingSuspended;
		#endregion
		
		static NavigationService()
		{
			history = new LinkedList<INavigationPoint>();
			currentNode = null;
			loggingSuspended = false;
			
			WorkbenchSingleton.WorkbenchCreated += WorkbenchCreatedHandler;
			FileService.FileRenamed += FileService_FileRenamed;
		}
		
		#region Public Properties
		
		public static bool CanNavigateBack {
			get { return currentNode != history.First && currentNode != null;}
		}
		public static bool CanNavigateForwards {
			get {return currentNode != history.Last && currentNode != null;}
		}
		public static int Count {
			get { return history.Count; }
		}
		
		public static INavigationPoint CurrentPosition {
			get {
				return currentNode==null ? (INavigationPoint)null : currentNode.Value;
			}
			set {
				Log(value);
			}
		}
		
		public static bool IsLogging {
			get { return !loggingSuspended;}
		}
		#endregion

		#region Public Methods
	
		public static void ContentChanging(object sender, EventArgs e)
		{
			foreach (INavigationPoint p in history)
			{
				p.ContentChanging(sender, e);
			}
		}
		
		#region private helpers
		static void Log(IWorkbenchWindow window)
		{
			if (window==null) return;
			// TODO: Navigation - enable logging of subpanes via window.ActiveViewContent
			Log(window.ViewContent);
		}
		
		static void Log(IViewContent vc)
		{
			if (vc==null) return;
			Log(vc.BuildNavPoint());
		}
		#endregion
		
		public static void Log(INavigationPoint p)
		{
			if (loggingSuspended) {
				return;
			}
			LogInternal(p);
		}

		// refactoring this out of Log() allows the NavigationService
		// to call this and ensure it will work regardless of the
		// requested state of loggingSuspended
		private static void LogInternal(INavigationPoint p)
		{
			if (p == null
			   || p.FileName==null) { // HACK: why/how do we get here?
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
		
		// untested
		public static INavigationPoint Log()
		{
//			IWorkbenchWindow window = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;
//			if (window == null) {
//				return null;
//			}
//
//			IViewContent view = window.ViewContent;
//			if (view == null) {
//				return null;
//			}
//
//			return view.BuildNavPoint();
			return null;
		}
		
		public static List<INavigationPoint> GetListOfPoints()
		{
			return new List<INavigationPoint>(history);
		}
		
		public static void ClearHistory()
		{
			ClearHistory(false);
		}
		
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
		
		public static void Go(int n)
		{
			if (0 == n) {
				return;
			} else if (0>n) {
				// move backwards
				while (0>n && currentNode!=history.First) {
					currentNode = currentNode.Previous;
					n++;
				}
			} else {
				// move forwards
				while (0<n && currentNode!=history.Last) {
					currentNode = currentNode.Next;
					n--;
				}
			}
			
			SyncViewWithModel();
		}
		
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
				//currentNode = history.AddAfter(currentNode, target);
			}
			
			SyncViewWithModel();
		}
		
		private static void SyncViewWithModel()
		{
			//LoggingService.Info("suspend logging");
			SuspendLogging();
			if (CurrentPosition!=null) { 
				CurrentPosition.JumpTo();
			}
			//LoggingService.Info("resume logging");
			ResumeLogging();
		}
		

		
		public static void SuspendLogging()
		{
			loggingSuspended = true;
		}
		public static void ResumeLogging()
		{
			// ENH: possible enhancement: use int instead of bool so resume statements are incremental rather than definitive.
			loggingSuspended = false;
		}
		
		#endregion

		// the following code is not covered by Unit tests as i wasn't sure 
		// how to test code triggered by the user interacting with the workbench
		#region event trapping

		#region ViewContent events
		public static void WorkbenchCreatedHandler(object sender, EventArgs e)
		{
			WorkbenchSingleton.Workbench.ViewOpened += 
				new ViewContentEventHandler(ViewContentOpened);
			WorkbenchSingleton.Workbench.ViewClosed +=
				new ViewContentEventHandler(ViewContentClosed);
		}

		static void ViewContentOpened(object sender, ViewContentEventArgs e)
		{
			//Log(e.Content);
			e.Content.WorkbenchWindow.WindowSelected += WorkBenchWindowSelected;
		}
		
		static void ViewContentClosed(object sender, ViewContentEventArgs e)
		{
			e.Content.WorkbenchWindow.WindowSelected -= WorkBenchWindowSelected;
		}
		
		static IWorkbenchWindow lastSelectedWindow = null;
		static void WorkBenchWindowSelected(object sender, EventArgs e)
		{
			try {
				IWorkbenchWindow window = sender as IWorkbenchWindow;
				if (window == lastSelectedWindow) {
					return;
				}
				int n = NavigationService.Count;
				Log(window);
				//LoggingService.DebugFormatted("WorkbenchSelected: logging {0}", window.Title);

				// HACK: Navigation - for some reason, JumpToFilePosition returns _before_ this 
				//       gets fired, (not the behaviour i expected) so we need to remember the 
				//       previous selected window to ensure that we only log once per visit to 
				//       a given window.
				lastSelectedWindow = window;

			} catch (Exception ex) {
				LoggingService.ErrorFormatted("{0}:\n{1}",ex.Message, ex.StackTrace);
			}
		}
		#endregion
		
		static void FileService_FileRenamed(object sender, FileRenameEventArgs e)
		{
			foreach (INavigationPoint p in history) {
				if (p.FileName.Equals(e.SourceFile)) {
					p.FileNameChanged(e.TargetFile);
				}
			}
		}
		
		#endregion
		
		
		#region Public Events
		public static event System.EventHandler HistoryChanged;
		
		public static void OnHistoryChanged()
		{
			if (HistoryChanged!=null) {
				HistoryChanged(NavigationService.CurrentPosition, EventArgs.Empty);
			}
		}
		#endregion
	}
}
