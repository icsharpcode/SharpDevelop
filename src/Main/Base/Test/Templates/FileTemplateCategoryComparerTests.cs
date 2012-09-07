// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project.Dialogs;
using NUnit.Framework;
using System;

namespace ICSharpCode.SharpDevelop.Tests.Templates
{
	[TestFixture]
	[SetCulture("en-US")]
	public class FileTemplateCategoryComparerTests
	{
		TemplateCategoryComparer comparer;

		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			comparer = new TemplateCategoryComparer();
		}
		
		[Test]
		public void NameEquals()
		{
			NewFileDialog.Category category1 = new NewFileDialog.Category("aa");
			NewFileDialog.Category category2 = new NewFileDialog.Category("aa");
			
			Assert.AreEqual(0, comparer.Compare(category1, category2));
		}
		
		[Test]
		public void NameNotEqual1()
		{
			NewFileDialog.Category category1 = new NewFileDialog.Category("aa");
			NewFileDialog.Category category2 = new NewFileDialog.Category("bb");
			
			Assert.AreEqual(-1, comparer.Compare(category1, category2));
		}
		
		[Test]
		public void NameNotEqual2()
		{
			NewFileDialog.Category category1 = new NewFileDialog.Category("bb");
			NewFileDialog.Category category2 = new NewFileDialog.Category("aa");
			
			Assert.AreEqual(1, comparer.Compare(category1, category2));
		}
		
		[Test]
		public void IndexNotEqual1()
		{
			NewFileDialog.Category category1 = new NewFileDialog.Category("zz", 0);
			NewFileDialog.Category category2 = new NewFileDialog.Category("zz", 1);
			
			Assert.AreEqual(-1, comparer.Compare(category1, category2));
		}
		
		[Test]
		public void IndexNotEqual2()
		{
			NewFileDialog.Category category1 = new NewFileDialog.Category("zz", 1);
			NewFileDialog.Category category2 = new NewFileDialog.Category("zz", 0);
			
			Assert.AreEqual(1, comparer.Compare(category1, category2));
		}
		
		[Test]
		public void IndexEqual1()
		{
			NewFileDialog.Category category1 = new NewFileDialog.Category("aa", 0);
			NewFileDialog.Category category2 = new NewFileDialog.Category("bb", 0);
			
			Assert.AreEqual(-1, comparer.Compare(category1, category2));
		}
		
		[Test]
		public void OneIndexNotSet1()
		{
			NewFileDialog.Category category1 = new NewFileDialog.Category("zz", 0);
			NewFileDialog.Category category2 = new NewFileDialog.Category("aa");
			
			Assert.AreEqual(-1, comparer.Compare(category1, category2));
		}
		
		[Test]
		public void OneIndexNotSet2()
		{
			NewFileDialog.Category category1 = new NewFileDialog.Category("aa");
			NewFileDialog.Category category2 = new NewFileDialog.Category("zz", 0);
			
			Assert.AreEqual(1, comparer.Compare(category1, category2));
		}
	}
}
