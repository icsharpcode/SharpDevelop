// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;

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
