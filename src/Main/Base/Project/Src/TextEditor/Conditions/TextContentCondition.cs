// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Xml;


using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.TextEditor;

namespace ICSharpCode.SharpDevelop.DefaultEditor.Conditions
{
	public class TextContentAuswerter : IAuswerter
	{
		public bool IsValid(object caller, Condition condition)
		{
			string textcontent = condition.Properties["textcontent"];
			if (caller is TextEditorControl) {
				TextEditorControl ctrl = (TextEditorControl)caller;
				if (ctrl.Document != null && ctrl.Document.HighlightingStrategy != null) {
					return textcontent == ctrl.Document.HighlightingStrategy.Name;
				}
			}
			return false;
		}
	}
}
