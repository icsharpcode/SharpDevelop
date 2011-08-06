// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.AspNet.Mvc;
using NUnit.Framework;

namespace AspNet.Mvc.Tests
{
	[TestFixture]
	public class MvcTextTemplateRepositoryTests
	{
		MvcTextTemplateRepository repository;
		
		void CreateRepositoryWithAspNetMvcAddInDirectory(string mvcAddInPath)
		{
			repository = new MvcTextTemplateRepository(mvcAddInPath);
		}
		
		[Test]
		public void GetMvcViewTextTemplateFileName_CSharpEmptyTemplateRequested_ReturnsFileNameToCSharpEmptyTextTemplate()
		{
			CreateRepositoryWithAspNetMvcAddInDirectory(@"C:\SD\AddIns\AspNet.Mvc");
			
			string fileName = repository.GetMvcViewTextTemplateFileName(MvcTextTemplateLanguage.CSharp, "Empty");
			
			string expectedFileName = 
				@"C:\SD\AddIns\AspNet.Mvc\ItemTemplates\CSharp\CodeTemplates\AddView\AspxCSharp\Empty.tt";
			
			Assert.AreEqual(expectedFileName, fileName);
		}
		
		[Test]
		public void GetMvcViewTextTemplateFileName_VisualBasicEmptyTemplateRequested_ReturnsFileNameToVisualBasicEmptyTextTemplate()
		{
			CreateRepositoryWithAspNetMvcAddInDirectory(@"C:\SD\AddIns\AspNet.Mvc");
			
			string fileName = repository.GetMvcViewTextTemplateFileName(MvcTextTemplateLanguage.VisualBasic, "Empty");
			
			string expectedFileName = 
				@"C:\SD\AddIns\AspNet.Mvc\ItemTemplates\VisualBasic\CodeTemplates\AddView\AspxVisualBasic\Empty.tt";
			
			Assert.AreEqual(expectedFileName, fileName);
		}
	}
}
