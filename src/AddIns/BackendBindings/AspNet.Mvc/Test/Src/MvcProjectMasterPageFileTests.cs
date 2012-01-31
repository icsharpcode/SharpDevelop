// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.AspNet.Mvc;
using NUnit.Framework;

namespace AspNet.Mvc.Tests
{
	[TestFixture]
	public class MvcProjectMasterPageFileTests
	{
		[Test]
		public void IsMasterPageFileName_NullFileNamePassed_ReturnsFalse()
		{
			bool result = MvcProjectMasterPageFile.IsMasterPageFileName(null);
			
			Assert.IsFalse(result);
		}
		
		[Test]
		public void IsMasterPageFileName_MasterPageFileNamePassed_ReturnsTrue()
		{
			bool result = MvcProjectMasterPageFile.IsMasterPageFileName("Site.master");
			
			Assert.IsTrue(result);
		}
		
		[Test]
		public void IsMasterPageFileName_UpperCaseMasterPageFileNamePassed_ReturnsTrue()
		{
			bool result = MvcProjectMasterPageFile.IsMasterPageFileName("Site.MASTER");
			
			Assert.IsTrue(result);
		}
		
		[Test]
		public void CreateMvcProjectMasterPageFile_NullProjectItemPassed_ReturnsNull()
		{
			MvcProjectFile file = MvcProjectMasterPageFile.CreateMvcProjectMasterPageFile(null);
			
			Assert.IsNull(file);
		}
	}
}
