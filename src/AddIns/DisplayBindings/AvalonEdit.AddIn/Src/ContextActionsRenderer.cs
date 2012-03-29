// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using System.Windows.Threading;
using ICSharpCode.Core;
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
	public sealed class ContextActionsRenderer : IDisposable
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
		const int delayMoveMilliseconds = 500;
		
		public bool IsEnabled
		{
			get {
				string fileName = this.Editor.FileName;
				if (String.IsNullOrEmpty(fileName))
					return false;
				return fileName.EndsWith(".cs", StringComparison.OrdinalIgnoreCase)
					|| fileName.EndsWith(".vb", StringComparison.OrdinalIgnoreCase);
			}
		}
		
		public ContextActionsRenderer(CodeEditorView editor)
		{
			if (editor == null)
				throw new ArgumentNullException("editor");
			this.editorView = editor;
			
			this.editorView.TextArea.Caret.PositionChanged += CaretPositionChanged;
			
			this.editorView.KeyDown += new KeyEventHandler(ContextActionsRenderer_KeyDown);
			
			editor.TextArea.TextView.ScrollOffsetChanged += ScrollChanged;
			this.delayMoveTimer = new DispatcherTimer() { Interval = TimeSpan.FromMilliseconds(delayMoveMilliseconds) };
			this.delayMoveTimer.Stop();
			this.delayMoveTimer.Tick += TimerMoveTick;
			WorkbenchSingleton.Workbench.ActiveViewContentChanged += WorkbenchSingleton_Workbench_ActiveViewContentChanged;
		}
		
		public void Dispose()
		{
			ClosePopup();
			WorkbenchSingleton.Workbench.ActiveViewContentChanged -= WorkbenchSingleton_Workbench_ActiveViewContentChanged;
		}
		
		void ContextActionsRenderer_KeyDown(object sender, KeyEventArgs e)
		{
			if (this.popup == null)
				return;
			if (e.Key == Key.T && Keyboard.Modifiers == ModifierKeys.Control) {
				if (popup.ViewModel != null && popup.ViewModel.Actions != null && popup.ViewModel.Actions.Count > 0) {
					popup.IsDropdownOpen = true;
					popup.Focus();
				} else {
					ClosePopup();
					// Popup is not shown but user explicitely requests it
					var popupVM = BuildPopupViewModel(this.Editor);
					popupVM.LoadHiddenActions();
					if (popupVM.Actions.Count == 0 && popupVM.HiddenActions.Count == 0)
						return;
					this.popup.ViewModel = popupVM;
					this.popup.IsDropdownOpen = true;
					this.popup.IsHiddenActionsExpanded = popupVM.Actions.Count == 0;
					this.popup.OpenAtLineStart(this.Editor);
					this.popup.Focus();
				}
			}
		}

		void ScrollChanged(object sender, EventArgs e)
		{
			StartTimer();
		}

		void TimerMoveTick(object sender, EventArgs e)
		{
			if (!delayMoveTimer.IsEnabled)
				return;
			ClosePopup();
			if (!IsEnabled)
				return;
			
			ContextActionsBulbViewModel popupVM = BuildPopupViewModel(this.Editor);
			//availableActionsVM.Title =
			//availableActionsVM.Image =
			if (popupVM.Actions.Count == 0)
				return;
			this.popup.ViewModel = popupVM;
			this.popup.OpenAtLineStart(this.Editor);
		}
		
		EditorActionsProvider lastActions;

		ContextActionsBulbViewModel BuildPopupViewModel(ITextEditor editor)
		{
			var actionsProvider = ContextActionsService.Instance.GetAvailableActions(editor);
			this.lastActions = actionsProvider;
			return new ContextActionsBulbViewModel(actionsProvider);
		}

		void CaretPositionChanged(object sender, EventArgs e)
		{
			StartTimer();
		}
		
		void StartTimer()
		{
			ClosePopup();
			IViewContent activeViewContent = WorkbenchSingleton.Workbench.ActiveViewContent;
			if (activeViewContent != null && activeViewContent.PrimaryFileName == this.Editor.FileName)
				delayMoveTimer.Start();
		}
		
		void ClosePopup()
		{
			this.delayMoveTimer.Stop();
			this.popup.Close();
			this.popup.IsDropdownOpen = false;
			this.popup.IsHiddenActionsExpanded = false;
			this.popup.ViewModel = null;
			if (this.lastActions != null) {
				// Clear the context to prevent memory leaks in case some users kept long-lived references to EditorContext
				this.lastActions.EditorContext.Clear();
				this.lastActions = null;
			}
		}
		void WorkbenchSingleton_Workbench_ActiveViewContentChanged(object sender, EventArgs e)
		{
			// open the popup again if in current file
			StartTimer();
		}
	}
}
