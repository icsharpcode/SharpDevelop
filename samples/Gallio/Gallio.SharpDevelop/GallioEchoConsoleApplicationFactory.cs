// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using ICSharpCode.UnitTesting;

namespace Gallio.SharpDevelop
{
	public class GallioEchoConsoleApplicationFactory
	{
		public static readonly string AddInTreePath = "/SharpDevelop/UnitTesting/GallioEchoApplication";
		
		string gallioEchoConsoleApplicationFileName = "Gallio.Echo.exe";
		
		public GallioEchoConsoleApplicationFactory()
			: this(new UnitTestAddInTree())
		{
		}
		
		public GallioEchoConsoleApplicationFactory(IAddInTree addInTree)
		{
			ReadFileName(addInTree);
		}
		
		void ReadFileName(IAddInTree addInTree)
		{
			List<string> items = addInTree.BuildItems<string>(AddInTreePath, this);
			gallioEchoConsoleApplicationFileName = items[0];
		}
		
		public GallioEchoConsoleApplication Create(SelectedTests selectedTests)
		{
			return new GallioEchoConsoleApplication(selectedTests, gallioEchoConsoleApplicationFileName);
		}
	}
}
