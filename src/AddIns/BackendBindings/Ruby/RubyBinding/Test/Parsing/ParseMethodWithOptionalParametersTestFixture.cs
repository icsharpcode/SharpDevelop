// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.RubyBinding;
using ICSharpCode.SharpDevelop.Dom;
using NUnit.Framework;

namespace RubyBinding.Tests.Parsing
{
	[TestFixture]
	public class ParseMethodWithOptionalParametersTestFixture
	{
		IMethod method;
		IParameter parameterA;
		IParameter parameterB;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			string Ruby = "class Test\r\n" +
							"\tdef foo(a = 1, b = 'test')\r\n" +
							"\tend\r\n" +
							"end";
			
			DefaultProjectContent projectContent = new DefaultProjectContent();
			RubyParser parser = new RubyParser();
			ICompilationUnit compilationUnit = parser.Parse(projectContent, @"C:\test.rb", Ruby);			
			if (compilationUnit.Classes.Count > 0) {
				IClass c = compilationUnit.Classes[0];
				method = c.Methods[0];
				if (method.Parameters.Count > 1) {
					parameterA = method.Parameters[0];
					parameterB = method.Parameters[1];
				}
			}
		}
		
		[Test]
		public void MethodHasTwoParameters()
		{
			Assert.AreEqual(2, method.Parameters.Count);
		}
		
		[Test]
		public void FirstParameterIsSender()
		{
			Assert.AreEqual("a", parameterA.Name);
		}
		
		[Test]
		public void SecondParameterIsEventArgs()
		{
			Assert.AreEqual("b", parameterB.Name);
		}
	}
}
