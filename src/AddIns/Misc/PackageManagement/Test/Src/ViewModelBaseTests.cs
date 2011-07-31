// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement;
using NUnit.Framework;
using PackageManagement.Tests.Helpers;

namespace PackageManagement.Tests
{
	[TestFixture]
	public class ViewModelBaseTests
	{
		TestableViewModelBase viewModel;
		
		void CreateTestableViewModel()
		{
			viewModel = new TestableViewModelBase();
		}
		
		[Test]
		public void PropertyChangedFor_PropertySpecified_ReturnsPropertyName()
		{
			CreateTestableViewModel();
			string name = viewModel.PropertyChangedFor(m => m.MyProperty);
			
			Assert.AreEqual("MyProperty", name);
		}

		[Test]
		public void PropertyChangedFor_TypeSpecified_ReturnsEmptyString()
		{
			CreateTestableViewModel();
			string name = viewModel.PropertyChangedFor(m => m);
			
			Assert.AreEqual(String.Empty, name);
		}
		
		[Test]
		public void PropertyChangedFor_MethodSpecified_ReturnsEmptyString()
		{
			CreateTestableViewModel();
			string name = viewModel.PropertyChangedFor(m => m.ToString());
			
			Assert.AreEqual(String.Empty, name);
		}
	}
}
