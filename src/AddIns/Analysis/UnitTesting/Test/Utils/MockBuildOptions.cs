// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.UnitTesting;

namespace UnitTesting.Tests.Utils
{
	public class MockBuildOptions : IBuildOptions
	{
		bool showErrorListAfterBuild = true;
		
		public bool ShowErrorListAfterBuild { 
			get { return showErrorListAfterBuild; }
			set { showErrorListAfterBuild = value; }
		}
	}
}
