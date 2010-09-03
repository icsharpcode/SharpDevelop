// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.RubyBinding;
using ICSharpCode.SharpDevelop.Dom;
using NUnit.Framework;

namespace RubyBinding.Tests.Parsing
{
	/// <summary>
	/// Tests that a base class is added to the class.
	/// </summary>
	[TestFixture]
	public class ParseClassWithBaseClassTestFixture
	{
		ICompilationUnit compilationUnit;
		IClass c;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			string Ruby = "class Test < Base\r\n" +
							"\tdef foo(i)\r\n" +
							"\tend\r\n" +
							"end";
			
			DefaultProjectContent projectContent = new DefaultProjectContent();
			RubyParser parser = new RubyParser();
			compilationUnit = parser.Parse(projectContent, @"C:\test.rb", Ruby);			
			if (compilationUnit.Classes.Count > 0) {
				c = compilationUnit.Classes[0];
			}
		}
		
		[Test]
		public void HasBaseClass()
		{
			IReturnType matchedBaseType = null;
			foreach (IReturnType baseType in c.BaseTypes) {
				if (baseType.Name == "Base") {
					matchedBaseType = baseType;
					break;
				}
			}
			Assert.IsNotNull(matchedBaseType);
		}
	}
}
