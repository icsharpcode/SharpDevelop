// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.RubyBinding;
using ICSharpCode.SharpDevelop.Dom;
using NUnit.Framework;

namespace RubyBinding.Tests.Parsing
{
	/// <summary>
	/// Tests that import statements are added to the compilation 
	/// unit's Using property.
	/// </summary>
	[TestFixture]
	public class ParseRequireTestFixture
	{
		ICompilationUnit compilationUnit;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			string Ruby = "require \"System.Windows.Forms, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"";
			
			DefaultProjectContent projectContent = new DefaultProjectContent();
			RubyParser parser = new RubyParser();
			compilationUnit = parser.Parse(projectContent, @"C:\test.rb", Ruby);	
		}

		[Test]
		public void OneUsing()
		{
			Assert.AreEqual(1, compilationUnit.UsingScope.Usings.Count);
		}
		
		[Test]
		public void UsingSystem()
		{
			Assert.AreEqual("System.Windows.Forms", compilationUnit.UsingScope.Usings[0].Usings[0]);
		}
	}
}
