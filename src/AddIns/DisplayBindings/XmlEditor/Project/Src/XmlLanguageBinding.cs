// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
