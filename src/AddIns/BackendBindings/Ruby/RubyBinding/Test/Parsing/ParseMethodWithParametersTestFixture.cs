// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.RubyBinding;
using ICSharpCode.SharpDevelop.Dom;
using NUnit.Framework;

namespace RubyBinding.Tests.Parsing
{
	[TestFixture]
	public class ParseMethodWithParametersTestFixture
	{
		IMethod method;
		IParameter senderParameter;
		IParameter eventArgsParameter;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			string Ruby = "class Test\r\n" +
							"\tdef foo(sender, e)\r\n" +
							"\tend\r\n" +
							"end";
			
			DefaultProjectContent projectContent = new DefaultProjectContent();
			RubyParser parser = new RubyParser();
			ICompilationUnit compilationUnit = parser.Parse(projectContent, @"C:\test.rb", Ruby);			
			if (compilationUnit.Classes.Count > 0) {
				IClass c = compilationUnit.Classes[0];
				method = c.Methods[0];
				if (method.Parameters.Count > 1) {
					senderParameter = method.Parameters[0];
					eventArgsParameter = method.Parameters[1];
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
			Assert.AreEqual("sender", senderParameter.Name);
		}
		
		[Test]
		public void SecondParameterIsEventArgs()
		{
			Assert.AreEqual("e", eventArgsParameter.Name);
		}
	}
}
