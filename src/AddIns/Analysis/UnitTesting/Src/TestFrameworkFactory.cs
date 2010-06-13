// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.Core;

namespace ICSharpCode.UnitTesting
{
	public class TestFrameworkFactory : ITestFrameworkFactory
	{
		AddIn addin;
		
		public TestFrameworkFactory(AddIn addin)
		{
			this.addin = addin;
		}
		
		public ITestFramework Create(string className)
		{
			return addin.CreateObject(className) as ITestFramework;
		}
	}
}
