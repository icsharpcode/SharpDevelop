// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using NUnit.Framework;
using PythonBinding.Tests.Utils;
using UnitTesting.Tests.Utils;

namespace PythonBinding.Tests.Resolver
{
	/// <summary>
	/// Given code:
	/// 
	/// a = Class1()
	/// 
	/// Check that the type of "a" can be obtained by the resolver.
	/// </summary>
	[TestFixture]
	public class ResolveLocalClassInstanceTests
	{
		PythonResolverTestsHelper resolverHelper;
		MockClass testClass;
		
		[SetUp]
		public void Init()
		{
			resolverHelper = new PythonResolverTestsHelper();
			
			testClass = resolverHelper.CreateClass("Test.Test1");
			resolverHelper.ProjectContent.ClassesInProjectContent.Add(testClass);			
			resolverHelper.ProjectContent.SetClassToReturnFromGetClass("Test.Test1", testClass);

			string python =
				"a = Test.Test1()\r\n" +
				"a";
			
			resolverHelper.Resolve("a", python);			
		}

		[Test]
		public void ResolveResultVariableName()
		{
			string name = resolverHelper.LocalResolveResult.VariableName;
			Assert.AreEqual("a", name);
		}
	}
}
