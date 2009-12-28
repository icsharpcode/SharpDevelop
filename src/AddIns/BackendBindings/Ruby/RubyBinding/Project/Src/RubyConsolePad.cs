// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Forms;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.TextEditor;

namespace ICSharpCode.RubyBinding
{
	public class RubyConsolePad : AbstractPadContent
	{
		TextEditor textEditor;
		TextEditorControl textEditorControl;
		RubyConsoleHost host;
		
		public RubyConsolePad()
		{
			textEditorControl = new TextEditorControl();
			textEditorControl.CreateControl();
			textEditor = new TextEditor(textEditorControl);
			host = new RubyConsoleHost(textEditor);
			host.Run();			
		}
		
		public override Control Control {
			get { return textEditorControl; }
		}		
		
		public override void Dispose()
		{
			host.Dispose();
			textEditorControl.Dispose();
		}
	}
}
