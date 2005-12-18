// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Drawing;
using System.CodeDom.Compiler;
using System.IO;
using System.Diagnostics;
using ICSharpCode.Core;


namespace ICSharpCode.SharpDevelop.Gui
{
	public delegate void TextEventHandler(object sender, TextEventArgs e);
	
	public class TextEventArgs : EventArgs
	{
		string text;
		
		public string Text {
			get {
				return text;
			}
		}
		
		public TextEventArgs(string text)
		{
			this.text = text;
		}
	}
}
