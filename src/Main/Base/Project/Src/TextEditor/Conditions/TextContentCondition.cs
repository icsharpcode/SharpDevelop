// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.SharpDevelop.Gui;
using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Editor;

namespace ICSharpCode.SharpDevelop.DefaultEditor.Conditions
{
	/// <summary>
	/// Tests the name of the highlighting strategy of the text editor.
	/// </summary>
	/// <attribute name="textcontent">
	/// The name of the highlighting strategy that should be active.
	/// </attribute>
	/// <example title="Test if any XML file is being edited">
	/// &lt;Condition name = "TextContent" textcontent="XML"&gt;
	/// </example>
	public class TextContentConditionEvaluator : IConditionEvaluator
	{
		public bool IsValid(object caller, Condition condition)
		{
			string textcontent = condition.Properties["textcontent"];
			if (caller is WpfWorkbench) {
				IViewContent content = (caller as WpfWorkbench).ActiveViewContent;
				if (content is ITextEditorProvider) {
					var ctrl = (content as ITextEditorProvider).TextEditor.GetService(typeof(AvalonEdit.TextEditor)) as AvalonEdit.TextEditor;
					if (ctrl != null && ctrl.SyntaxHighlighting.Name != null) {
						return string.Equals(textcontent, ctrl.SyntaxHighlighting.Name, StringComparison.OrdinalIgnoreCase);
					}
				}
			}
			return false;
		}
	}
}
