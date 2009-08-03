// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.SharpDevelop.Dom;
using System;
using NUnit.Framework;

namespace ICSharpCode.XamlBinding.Tests
{
	public class TextEditorBasedTests
	{
		protected MockTextEditor CreateTextEditor(string fileContent, int line, int column)
		{
			MockTextEditor textEditor = new MockTextEditor();
			
			textEditor.Document.Text = fileContent;
			textEditor.Caret.Line = line;
			textEditor.Caret.Column = column;
			textEditor.CreateParseInformation();
			
			return textEditor;
		}
	}
}
