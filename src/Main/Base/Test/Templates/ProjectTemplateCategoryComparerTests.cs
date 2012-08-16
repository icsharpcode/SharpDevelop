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
	public class ProjectTemplateCategoryComparerTests
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
			NewProjectDialog.Category category1 = new NewProjectDialog.Category("aa");
			NewProjectDialog.Category category2 = new NewProjectDialog.Category("aa");
			
			Assert.AreEqual(0, comparer.Compare(category1, category2));
		}
		
		[Test]
		public void NameNotEqual1()
		{
			NewProjectDialog.Category category1 = new NewProjectDialog.Category("aa");
			NewProjectDialog.Category category2 = new NewProjectDialog.Category("bb");
			
			Assert.AreEqual(-1, comparer.Compare(category1, category2));
		}
		
		[Test]
		public void NameNotEqual2()
		{
			NewProjectDialog.Category category1 = new NewProjectDialog.Category("bb");
			NewProjectDialog.Category category2 = new NewProjectDialog.Category("aa");
			
			Assert.AreEqual(1, comparer.Compare(category1, category2));
		}
		
		[Test]
		public void IndexNotEqual1()
		{
			NewProjectDialog.Category category1 = new NewProjectDialog.Category("zz", 0);
			NewProjectDialog.Category category2 = new NewProjectDialog.Category("zz", 1);
			
			Assert.AreEqual(-1, comparer.Compare(category1, category2));
		}
		
		[Test]
		public void IndexNotEqual2()
		{
			NewProjectDialog.Category category1 = new NewProjectDialog.Category("zz", 1);
			NewProjectDialog.Category category2 = new NewProjectDialog.Category("zz", 0);
			
			Assert.AreEqual(1, comparer.Compare(category1, category2));
		}
		
		[Test]
		public void IndexEqual1()
		{
			NewProjectDialog.Category category1 = new NewProjectDialog.Category("aa", 0);
			NewProjectDialog.Category category2 = new NewProjectDialog.Category("bb", 0);
			
			Assert.AreEqual(-1, comparer.Compare(category1, category2));
		}
		
		[Test]
		public void OneIndexNotSet1()
		{
			NewProjectDialog.Category category1 = new NewProjectDialog.Category("zz", 0);
			NewProjectDialog.Category category2 = new NewProjectDialog.Category("aa");
			
			Assert.AreEqual(-1, comparer.Compare(category1, category2));
		}
		
		[Test]
		public void OneIndexNotSet2()
		{
			NewProjectDialog.Category category1 = new NewProjectDialog.Category("aa");
			NewProjectDialog.Category category2 = new NewProjectDialog.Category("zz", 0);
			
			Assert.AreEqual(1, comparer.Compare(category1, category2));
		}
	}
}
