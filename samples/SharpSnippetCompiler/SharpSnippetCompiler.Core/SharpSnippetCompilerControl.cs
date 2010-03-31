// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
// </file>

using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Debugging;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;

namespace ICSharpCode.SharpSnippetCompiler.Core
{
	public partial class SharpSnippetCompilerControl : UserControl
	{
		SharpDevelopTextAreaControl textEditor;
		
		public SharpSnippetCompilerControl()
		{
			InitializeComponent();
			
			textEditor = new SharpDevelopTextAreaControl();
			textEditor.Dock = DockStyle.Fill;
			this.Controls.Add(textEditor);
		}
		
		public TextEditorControl TextEditor {
			get { return textEditor; }
		}
				
		public void LoadFile(string fileName)
		{
			textEditor.LoadFile(fileName);
		}
		
		public void Save()
		{
			textEditor.SaveFile(textEditor.FileName);
		}
	}
}
