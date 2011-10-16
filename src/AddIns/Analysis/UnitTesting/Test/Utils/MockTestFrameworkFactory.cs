// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.Core;
using ICSharpCode.UnitTesting;

namespace UnitTesting.Tests.Utils
{
	public class MockTestFrameworkFactory : ITestFrameworkFactory
	{
		Dictionary<string, ITestFramework> frameworks = new Dictionary<string, ITestFramework>();
		List<string> classNames = new List<string>();
		
		public void Add(string className, ITestFramework framework)
		{
			frameworks.Add(className, framework);
		}
		
		public ITestFramework Create(string className)
		{
			classNames.Add(className);
			
			ITestFramework framework;
			if (frameworks.TryGetValue(className, out framework)) {
				return framework;
			}
			return null;
		}
		
		public List<string> ClassNamesPassedToCreateMethod {
			get { return classNames; }
		}
		
		public MockTestFramework AddFakeTestFramework(string className)
		{
			var testFramework = new MockTestFramework();
			Add(className, testFramework);
			return testFramework;
		}
	}
}
