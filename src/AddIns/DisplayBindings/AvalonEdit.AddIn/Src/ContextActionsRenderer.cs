// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Konicek" email="martin.konicek@gmail.com"/>
//     <version>$Revision: $</version>
// </file>
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Threading;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.AvalonEdit;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Refactoring;

namespace ICSharpCode.AvalonEdit.AddIn
{
	/// <summary>
	/// Renders Popup with context actions on the left side of the current line in the editor.
	/// </summary>
	public class ContextActionsRenderer
	{
		readonly CodeEditorView editorView;
		ITextEditor Editor { get { return this.editorView.Adapter; } }
		/// <summary>
		/// This popup is reused (closed and opened again).
		/// </summary>
		ContextActionsBulbPopup popup = new ContextActionsBulbPopup();
		
		/// <summary>
		/// Delays the available actions resolution so that it does not get called too often when user holds an arrow.
		/// </summary>
		DispatcherTimer delayMoveTimer;
		const int delayMoveMilliseconds = 100;
		
		public bool IsEnabled
		{
			get {
				try {
					string fileName = this.Editor.FileName;
					return fileName.EndsWith(".cs") || fileName.EndsWith(".vb");
				} catch {
					return false;
				}
			}
		}
		
		public ContextActionsRenderer(CodeEditorView editor)
		{
			if (editor == null)
				throw new ArgumentNullException("editor");
			this.editorView = editor;
			
			this.editorView.TextArea.Caret.PositionChanged += CaretPositionChanged;
			
			editor.TextArea.TextView.ScrollOffsetChanged += ScrollChanged;
			this.delayMoveTimer = new DispatcherTimer() { Interval = TimeSpan.FromMilliseconds(delayMoveMilliseconds) };
			this.delayMoveTimer.Stop();
			this.delayMoveTimer.Tick += TimerMoveTick;
			WorkbenchSingleton.Workbench.ViewClosed += WorkbenchSingleton_Workbench_ViewClosed;
			WorkbenchSingleton.Workbench.ActiveViewContentChanged += WorkbenchSingleton_Workbench_ActiveViewContentChanged;
		}

		void WorkbenchSingleton_Workbench_ViewClosed(object sender, ViewContentEventArgs e)
		{
			try {
				// prevent memory leaks
				if (e.Content.PrimaryFileName == this.Editor.FileName) {
					WorkbenchSingleton.Workbench.ViewClosed -= WorkbenchSingleton_Workbench_ViewClosed;
					WorkbenchSingleton.Workbench.ActiveViewContentChanged -= WorkbenchSingleton_Workbench_ActiveViewContentChanged;
				}
			} catch {}
		}

		void WorkbenchSingleton_Workbench_ActiveViewContentChanged(object sender, EventArgs e)
		{
			ClosePopup();
			try {
				// open the popup again if in current file
				if (((IViewContent)WorkbenchSingleton.Workbench.ActiveContent).PrimaryFileName == this.Editor.FileName)
					CaretPositionChanged(this, EventArgs.Empty);
			} catch {}
		}

		void ScrollChanged(object sender, EventArgs e)
		{
			ClosePopup();
		}

		void TimerMoveTick(object sender, EventArgs e)
		{
			this.delayMoveTimer.Stop();
			if (!IsEnabled)
				return;
			ClosePopup();
			var availableActions = ContextActionsService.Instance.GetAvailableActions(this.Editor);
			var availableActionsVM = new ObservableCollection<ContextActionViewModel>(
				availableActions.Select(a => new ContextActionViewModel(a)));
			if (availableActionsVM.Count == 0)
				return;
			
			this.popup.Actions = new ContextActionsViewModel {
				//Image = ClassBrowserIconService.Class.ImageSource,
				Actions = availableActionsVM
			};
			this.popup.OpenAtLineStart(this.Editor);
		}

		void CaretPositionChanged(object sender, EventArgs e)
		{
			if (this.popup.IsOpen)
			{
				ClosePopup();
			}
			this.delayMoveTimer.Stop();
			this.delayMoveTimer.Start();
		}
		
		void ClosePopup()
		{
			this.popup.Close();
			this.popup.IsDropdownOpen = false;
			this.popup.Actions = null;
		}
	}
}
