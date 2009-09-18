// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Actions;
using ICSharpCode.TextEditor.Document;

namespace ICSharpCode.SharpDevelop.DefaultEditor.Commands
{
	public abstract class AbstractEditActionMenuCommand : AbstractMenuCommand
	{
		public abstract IEditAction EditAction {
			get;
		}
		
		public override void Run()
		{
			IViewContent viewContent = WorkbenchSingleton.Workbench.ActiveViewContent;
			
			if (viewContent == null || !(viewContent is ITextEditorControlProvider)) {
				return;
			}
			TextEditorControl textEditor = ((ITextEditorControlProvider)viewContent).TextEditorControl;
			EditAction.Execute(textEditor.ActiveTextAreaControl.TextArea);
		}
	}
	
	public class RemoveLeadingWS : AbstractEditActionMenuCommand
	{
		public override IEditAction EditAction {
			get {
				return new ICSharpCode.TextEditor.Actions.RemoveLeadingWS();
			}
		}
	}
	
	public class RemoveTrailingWS : AbstractEditActionMenuCommand
	{
		public override IEditAction EditAction {
			get {
				return new ICSharpCode.TextEditor.Actions.RemoveTrailingWS();
			}
		}
	}
	
	
	public class ToUpperCase : AbstractEditActionMenuCommand
	{
		public override IEditAction EditAction {
			get {
				return new ICSharpCode.TextEditor.Actions.ToUpperCase();
			}
		}
	}
	
	public class ToLowerCase : AbstractEditActionMenuCommand
	{
		public override IEditAction EditAction {
			get {
				return new ICSharpCode.TextEditor.Actions.ToLowerCase();
			}
		}
	}
	
	public class InvertCaseAction : AbstractEditActionMenuCommand
	{
		public override IEditAction EditAction {
			get {
				return new ICSharpCode.TextEditor.Actions.InvertCaseAction();
			}
		}
	}
	
	public class CapitalizeAction : AbstractEditActionMenuCommand
	{
		public override IEditAction EditAction {
			get {
				return new ICSharpCode.TextEditor.Actions.CapitalizeAction();
			}
		}
	}
	
	public class ConvertTabsToSpaces : AbstractEditActionMenuCommand
	{
		public override IEditAction EditAction {
			get {
				return new ICSharpCode.TextEditor.Actions.ConvertTabsToSpaces();
			}
		}
	}
	
	public class ConvertSpacesToTabs : AbstractEditActionMenuCommand
	{
		public override IEditAction EditAction {
			get {
				return new ICSharpCode.TextEditor.Actions.ConvertSpacesToTabs();
			}
		}
	}
	
	public class ConvertLeadingTabsToSpaces : AbstractEditActionMenuCommand
	{
		public override IEditAction EditAction {
			get {
				return new ICSharpCode.TextEditor.Actions.ConvertLeadingTabsToSpaces();
			}
		}
	}
	
	public class ConvertLeadingSpacesToTabs : AbstractEditActionMenuCommand
	{
		public override IEditAction EditAction {
			get {
				return new ICSharpCode.TextEditor.Actions.ConvertLeadingSpacesToTabs();
			}
		}
	}
	
	/// <summary>
	/// This is a sample editaction plugin, it indents the selected area.
	/// </summary>
	public class IndentSelection : AbstractEditActionMenuCommand
	{
		public override IEditAction EditAction {
			get {
				return new ICSharpCode.TextEditor.Actions.IndentSelection();
			}
		}
	}
}
