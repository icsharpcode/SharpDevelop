// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Konicek" email="martin.konicek@gmail.com"/>
//     <version>$Revision: $</version>
// </file>
using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.Refactoring
{
	/// <summary>
	/// Description of ContextActionsPopup.
	/// </summary>
	public class ContextActionsPopup : ContextActionsPopupBase
	{
		/// <summary>
		/// DOM Entity (IClass, IMember etc.) for which this popup is displayed.
		/// </summary>
		public IEntity Symbol { get; set; }
		
		public ContextActionsPopup()
		{
			// Close on lost focus
			this.StaysOpen = false;
			this.AllowsTransparency = true;
			this.ActionsControl = new ContextActionsHeaderedControl();
			// Close when any action excecuted
			this.ActionsControl.ActionExecuted += delegate { this.Close(); };
		}
		
		public ContextActionsHeaderedControl ActionsControl
		{
			get { return (ContextActionsHeaderedControl)this.Child; }
			set { this.Child = value; }
		}
		
		public ContextActionsViewModel Actions
		{
			get { return (ContextActionsViewModel)ActionsControl.DataContext; }
			set {
				ActionsControl.DataContext = value;
			}
		}
		
		public new void Focus()
		{
			this.ActionsControl.Focus();
		}
		
		public void OpenAtCaretAndFocus()
		{
			ITextEditor currentEditor = GetCurrentEditor();
			if (currentEditor == null) {
				OpenAtMousePosition();
				return;
			}
			// Should look if symbol under caret is the same as this.Symbol, so that when opened from class browser, popup opens at mouse pos
			//var rr = ParserService.Resolve(currentEditor.Caret.Offset, currentEditor.Document, currentEditor.FileName);
			OpenAtPosition(currentEditor, currentEditor.Caret.Line, currentEditor.Caret.Column, true);
			this.Focus();
		}

		ITextEditor GetCurrentEditor()
		{
			if (WorkbenchSingleton.Workbench == null)
				return null;
			var activeViewContent = WorkbenchSingleton.Workbench.ActiveViewContent as ITextEditorProvider;
			if (activeViewContent == null)
				return null;
			return activeViewContent.TextEditor;
		}
		
		void OpenAtMousePosition()
		{
			this.Placement = PlacementMode.MousePoint;
			this.Open();
		}
	}
}
