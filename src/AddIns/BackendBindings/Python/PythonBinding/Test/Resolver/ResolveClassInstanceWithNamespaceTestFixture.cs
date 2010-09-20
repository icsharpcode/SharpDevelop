// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using ICSharpCode.PythonBinding;
using ICSharpCode.SharpDevelop.Dom;
using NUnit.Framework;
using PythonBinding.Tests;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Resolver
{
	/// <summary>
	/// Given code:
	/// 
	/// a = Test.Class1()
	/// 
	/// Where Test is the namespace of the class, check that the type of "a" can be obtained 
	/// by the resolver.
	/// </summary>
	[TestFixture]
	public class ResolveClassInstanceWithNamespaceTestFixture
	{		
		[Test]
		public void GetTypeOfInstance()
		{
			string code = "a = Test.Class1()";
			PythonVariableResolver resolver = new PythonVariableResolver();
			Assert.AreEqual("Test.Class1", resolver.Resolve("a", @"C:\Projects\Test\Test.py", code));
		}
		
		[Test]
		public void GetTypeOfInstanceWithTwoNamespaces()
		{
			string code = "a = Root.Test.Class1()";
			PythonVariableResolver resolver = new PythonVariableResolver();
			Assert.AreEqual("Root.Test.Class1", resolver.Resolve("a", @"C:\Projects\Test\Test.py", code));
		}		
	}
}
