// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
