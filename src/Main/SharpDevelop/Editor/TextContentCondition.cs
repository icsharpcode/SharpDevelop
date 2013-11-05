// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.Editor
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
	sealed class TextContentConditionEvaluator : IConditionEvaluator
	{
		public bool IsValid(object caller, Condition condition)
		{
			string textcontent = condition.Properties["textcontent"];
			var editor = SD.GetActiveViewContentService<ICSharpCode.AvalonEdit.TextEditor>();
			if (editor != null && editor.SyntaxHighlighting != null) {
				return string.Equals(textcontent, editor.SyntaxHighlighting.Name, StringComparison.OrdinalIgnoreCase);
			}
			return false;
		}
	}
}
