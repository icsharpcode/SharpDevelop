// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.RubyBinding;
using ICSharpCode.SharpDevelop.Dom;
using NUnit.Framework;
using RubyBinding.Tests;

namespace RubyBinding.Tests.Parsing
{
	/// <summary>
	/// Tests that the RubyParser class has a class in the returned
	/// CompilationUnit.
	/// </summary>
	[TestFixture]
	public class ParseSingleClassTestFixture
	{
		ICompilationUnit compilationUnit;
		IClass c;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			string Ruby = "class Test\r\n" +
							"end";
			
			DefaultProjectContent projectContent = new DefaultProjectContent();
			RubyParser parser = new RubyParser();
			compilationUnit = parser.Parse(projectContent, @"C:\Projects\Test\test.rb", Ruby);
			if (compilationUnit.Classes.Count > 0) {
				c = compilationUnit.Classes[0];
			}
		}
		
		[Test]
		public void OneClass()
		{
			Assert.AreEqual(1, compilationUnit.Classes.Count);
		}
		
		[Test]
		public void ClassName()
		{
			Assert.AreEqual("Test", c.Name);
		}
		
		[Test]
		public void FullyQualifiedClassName()
		{
			Assert.AreEqual("Test", c.FullyQualifiedName);
		}
		
		[Test]
		public void CompilationUnitFileName()
		{
			Assert.AreEqual(@"C:\Projects\Test\test.rb", compilationUnit.FileName);
		}
		
		[Test]
		public void ClassBodyRegion()
		{
			int startLine = 1;
			int startColumn = 11;
			int endLine = 2;
			int endColumn = 4;
			DomRegion region = new DomRegion(startLine, startColumn, endLine, endColumn);
			Assert.AreEqual(region.ToString(), c.BodyRegion.ToString());
		}
		
		[Test]
		public void ClassRegion()
		{
			int startLine = 1;
			int startColumn = 1;
			int endLine = 2;
			int endColumn = 4;
			DomRegion region = new DomRegion(startLine, startColumn, endLine, endColumn);
			Assert.AreEqual(region.ToString(), c.Region.ToString());
		}		
	}
}
