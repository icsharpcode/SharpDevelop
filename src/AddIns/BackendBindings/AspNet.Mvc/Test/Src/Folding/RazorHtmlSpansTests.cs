// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.AspNet.Mvc.Folding;
using NUnit.Framework;

namespace AspNet.Mvc.Tests.Folding
{
	[TestFixture]
	public class RazorHtmlSpansTests
	{
		RazorHtmlSpans htmlSpans;
		
		void CreateHtmlSpans(string fileExtension)
		{
			htmlSpans = new RazorHtmlSpans(String.Empty, fileExtension);
		}
		
		[Test]
		public void CodeLanguageName_CSharpRazorFileExtension_ReturnsCSharp()
		{
			CreateHtmlSpans(".cshtml");
			
			Assert.AreEqual("csharp", htmlSpans.CodeLanguageName);
		}
		
		[Test]
		public void CodeLanguageName_VisualBasicRazorFileExtension_ReturnsVisualBasicRazorFileExtensionWithoutDot()
		{
			CreateHtmlSpans(".vbhtml");
			
			Assert.AreEqual("vb", htmlSpans.CodeLanguageName);
		}
	}
}
