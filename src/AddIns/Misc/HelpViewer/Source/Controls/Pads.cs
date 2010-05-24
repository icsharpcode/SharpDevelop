using System;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;
using ICSharpCode.SharpDevelop.Gui;

namespace MSHelpSystem.Controls
{
	public class Help3TocPad : AbstractPadContent
	{
		public Help3TocPad()
		{
		}
		
		TocPadControl toc = new TocPadControl();
		
		public override object Control
		{
			get { return toc; }
		}		
	}
}
