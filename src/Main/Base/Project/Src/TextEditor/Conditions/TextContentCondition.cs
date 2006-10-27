// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.Core;
using ICSharpCode.TextEditor;

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
			if (caller is TextEditorControl) {
				TextEditorControl ctrl = (TextEditorControl)caller;
				if (ctrl.Document != null && ctrl.Document.HighlightingStrategy != null) {
					return string.Equals(textcontent, ctrl.Document.HighlightingStrategy.Name, StringComparison.OrdinalIgnoreCase);
				}
			}
			return false;
		}
	}
}
