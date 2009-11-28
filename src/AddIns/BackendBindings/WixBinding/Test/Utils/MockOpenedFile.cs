// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;

namespace WixBinding.Tests.Utils
{
	public class MockOpenedFile : OpenedFile
	{
		public MockOpenedFile(string fileName, bool isUntitled)
		{
			base.FileName = FileName.Create(fileName);
			base.IsUntitled = isUntitled;
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
