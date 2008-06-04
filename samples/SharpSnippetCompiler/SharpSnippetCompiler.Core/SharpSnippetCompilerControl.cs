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
			
			textEditor.ActiveTextAreaControl.TextArea.IconBarMargin.MouseDown += MarginMouseDown;
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
		
		static void MarginMouseDown(AbstractMargin iconBar, Point mousepos, MouseButtons mouseButtons)
		{
			if (mouseButtons != MouseButtons.Left) return;
			
			Rectangle viewRect = iconBar.TextArea.TextView.DrawingPosition;
			TextLocation logicPos = iconBar.TextArea.TextView.GetLogicalPosition(0, mousepos.Y - viewRect.Top);
			
			if (logicPos.Y >= 0 && logicPos.Y < iconBar.TextArea.Document.TotalNumberOfLines) {
				DebuggerService.ToggleBreakpointAt(iconBar.TextArea.Document, iconBar.TextArea.MotherTextEditorControl.FileName, logicPos.Y);
				iconBar.TextArea.Refresh(iconBar);
			}
		}		
	}
}
