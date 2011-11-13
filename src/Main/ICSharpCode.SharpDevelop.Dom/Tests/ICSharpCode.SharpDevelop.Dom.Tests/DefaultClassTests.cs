// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Linq;
using NUnit.Framework;

namespace ICSharpCode.SharpDevelop.Dom.Tests
{
	[TestFixture]
	public class DefaultClassTests
	{
		DefaultClass defaultClass;
		DefaultCompilationUnit compilationUnit;
		DefaultProjectContent projectContent;
		
		void CreateDefaultClass(string fullyQualifiedName)
		{
			projectContent = new DefaultProjectContent();
			compilationUnit = new DefaultCompilationUnit(projectContent);
			defaultClass = new DefaultClass(compilationUnit, fullyQualifiedName);
		}
		
		[Test]
		public void AllMembers_PropertiesFieldsMethodsEventsAreAllNull_ReturnsNoItemsWithoutThrowingNullException()
		{
			CreateDefaultClass("TestClass");
			int count = defaultClass.AllMembers.Count();
			
			Assert.AreEqual(0, count);
		}
	}
}
