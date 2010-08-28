// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;

namespace ICSharpCode.XmlEditor
{
	public class XmlLanguageBinding : DefaultLanguageBinding
	{
		XmlFoldingManager foldingManager;
		
		public override IFormattingStrategy FormattingStrategy {
			get { return new XmlFormattingStrategy(); }
		}
		
		public override void Attach(ITextEditor editor)
		{
			foldingManager = new XmlFoldingManager(editor);
			foldingManager.UpdateFolds();
			foldingManager.Start();
			
			base.Attach(editor);
		}
		
		public override void Detach()
		{
			foldingManager.Stop();
			foldingManager.Dispose();
			
			base.Detach();
		}
	}
}
