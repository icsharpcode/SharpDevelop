// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.Scripting.Tests.Utils
{
	public class MockOpenedFile : OpenedFile
	{
		public MockOpenedFile()
		{
		}
		
		public MockOpenedFile(string fileName)
		{
			this.FileName = new ICSharpCode.Core.FileName(fileName);
		}
		
		public override IList<IViewContent> RegisteredViewContents {
			get {
				throw new NotImplementedException();
			}
		}
		
		public override void RegisterView(IViewContent view)
		{
		}
		
		public override void UnregisterView(IViewContent view)
		{
		}
		
		public override event EventHandler FileClosed { add {} remove {} }
	}
}
