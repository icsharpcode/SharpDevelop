// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.AspNet.Mvc;
using NUnit.Framework;

namespace AspNet.Mvc.Tests
{
	[TestFixture]
	public class MvcProjectRazorFileTests
	{
		[Test]
		public void IsRazorFileName_NullFileNamePassed_ReturnsFalse()
		{
			bool result = MvcProjectRazorFile.IsRazorFile(null);
			
			Assert.IsFalse(result);
		}
		
		[Test]
		public void IsRazorFileName_RazorFileNamePassed_ReturnsTrue()
		{
			bool result = MvcProjectRazorFile.IsRazorFileName("layout.cshtml");
			
			Assert.IsTrue(result);
		}
		
		[Test]
		public void IsRazorFileName_UpperCaseRazorFileNamePassed_ReturnsTrue()
		{
			bool result = MvcProjectRazorFile.IsRazorFileName("test.CSHTML");
			
			Assert.IsTrue(result);
		}
		
		[Test]
		public void CreateMvcProjectRazorFile_NullProjectItemPassed_ReturnsNull()
		{
			MvcProjectFile file = MvcProjectRazorFile.CreateMvcProjectRazorFile(null);
			
			Assert.IsNull(file);
		}
	}
}
