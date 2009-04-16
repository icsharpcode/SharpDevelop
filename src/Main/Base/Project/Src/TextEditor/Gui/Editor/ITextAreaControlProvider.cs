// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.SharpDevelop.Editor;
using System;
using ICSharpCode.TextEditor;

namespace ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor
{
	public interface ITextEditorControlProvider : ITextEditorProvider
	{
		TextEditorControl TextEditorControl {
			get;
		}
	}
}
