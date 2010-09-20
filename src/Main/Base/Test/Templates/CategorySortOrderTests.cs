// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.SharpDevelop.Internal.Templates;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project.Dialogs;
using NUnit.Framework;
using System;
using System.IO;
using System.Xml;

namespace ICSharpCode.SharpDevelop.Tests.Templates
{
	[TestFixture]
	public class CategorySortOrderTests
	{
		TemplateCategorySortOrderFile sortOrderFile;
		int csharpSortOrder;
		int windowsAppsSortOrder;
		int miscSortOrder;
		int errorSortOrder;
		int misTypedNameSortOrder;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			string xml = "<Categories>\r\n" +
				"  <Category Name='C#'>\r\n" +
				"     <Category Name='Windows Applications' SortOrder='10'/>\r\n" +
				"  </Category>\r\n" +
				"  <Category Name='Misc' SortOrder='20'/>\r\n" +
				"  <Category Name='Error' SortOrder='A'/>\r\n" +
				"  <Category mis-typed-Name='Test' SortOrder='100'/>\r\n" +
				"</Categories>";
			
			sortOrderFile = new TemplateCategorySortOrderFile(new XmlTextReader(new StringReader(xml)));
			csharpSortOrder = sortOrderFile.GetCategorySortOrder("C#");
			windowsAppsSortOrder = sortOrderFile.GetCategorySortOrder("C#", "Windows Applications");
			miscSortOrder = sortOrderFile.GetCategorySortOrder("Misc");
			errorSortOrder = sortOrderFile.GetCategorySortOrder("Error");
			misTypedNameSortOrder = sortOrderFile.GetCategorySortOrder(String.Empty);
			
		}
		
		[Test]
		public void CSharpCategorySortOrder()
		{
			Assert.AreEqual(TemplateCategorySortOrderFile.UndefinedSortOrder, csharpSortOrder);
		}
		
		[Test]
		public void MiscCategorySortOrder()
		{
			Assert.AreEqual(20, miscSortOrder);
		}
		
		[Test]
		public void WindowsAppsSortOrder()
		{
			Assert.AreEqual(10, windowsAppsSortOrder);
		}
		
		[Test]
		public void InvalidSortOrder()
		{
			Assert.AreEqual(TemplateCategorySortOrderFile.UndefinedSortOrder, errorSortOrder);
		}
		
		[Test]
		public void InvalidAttributeName()
		{
			Assert.AreEqual(TemplateCategorySortOrderFile.UndefinedSortOrder, misTypedNameSortOrder);
		}
		
	}
}
