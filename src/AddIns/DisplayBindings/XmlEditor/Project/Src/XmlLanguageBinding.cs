// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;

namespace ICSharpCode.XmlEditor
{
	public class XmlLanguageBinding : DefaultLanguageBinding
	{
		public XmlLanguageBinding()
		{
			container.AddService(typeof(IFormattingStrategy), new XmlFormattingStrategy());
		}
	}
	
	public class XmlTextEditorExtension : ITextEditorExtension
	{
		XmlFoldingManager foldingManager;
		
		public virtual void Attach(ITextEditor editor)
		{
			foldingManager = new XmlFoldingManager(editor);
			foldingManager.UpdateFolds();
			foldingManager.Start();
		}
		
		public virtual void Detach()
		{
			foldingManager.Stop();
			foldingManager.Dispose();
		}
	}
}
