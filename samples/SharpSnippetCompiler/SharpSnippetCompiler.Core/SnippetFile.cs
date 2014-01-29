// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
// </file>

using System;
using System.Collections.Generic;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpSnippetCompiler.Core
{
	public class SnippetFile : OpenedFile
	{
		public SnippetFile(string fileName)
		{
			this.FileName = new FileName(fileName);
			IsUntitled = false;
		}
			
		public override IList<IViewContent> RegisteredViewContents {
			get {
				throw new NotImplementedException();
			}
		}
		
		public override void RegisterView(IViewContent view)
		{
			throw new NotImplementedException();
		}
		
		public override void UnregisterView(IViewContent view)
		{
			throw new NotImplementedException();
		}
		
		public override event EventHandler FileClosed;
		
		protected virtual void OnFileClosed(EventArgs e)
		{
			if (FileClosed != null) {
				FileClosed(this, e);
			}
		}
	}
}
