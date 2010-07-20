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
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.AvalonEdit;
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
		ContextActionsPopup popup = new ContextActionsPopup() { StaysOpen = true };
		
		/// <summary>
		/// Delays the available actions resolution so that it does not get called too often when user holds an arrow.
		/// </summary>
		DispatcherTimer delayMoveTimer;
		const int delayMoveMilliseconds = 100;
		
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
		}

		void ScrollChanged(object sender, EventArgs e)
		{
			this.popup.Close();
		}

		void TimerMoveTick(object sender, EventArgs e)
		{
			this.delayMoveTimer.Stop();
			
			var availableActions = ContextActionsService.Instance.GetAvailableActions(this.Editor);
			var availableActionsVM = new ObservableCollection<ContextActionViewModel>(
				availableActions.Select(a => new ContextActionViewModel(a)));
			if (availableActionsVM.Count == 0)
				return;
			
			this.popup.Actions = new ContextActionsViewModel {
				Title = "Actions",
				Actions = availableActionsVM
			};
			this.popup.OpenAtLineStart(this.Editor);
		}

		void CaretPositionChanged(object sender, EventArgs e)
		{
			this.popup.Close();
			this.popup.Actions = null;
			this.delayMoveTimer.Stop();
			this.delayMoveTimer.Start();
		}
	}
}
