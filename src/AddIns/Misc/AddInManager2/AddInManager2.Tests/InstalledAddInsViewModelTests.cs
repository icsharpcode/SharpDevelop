// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using ICSharpCode.AddInManager2.Model;
using ICSharpCode.AddInManager2.Tests.Fakes;
using ICSharpCode.AddInManager2.ViewModel;
using ICSharpCode.Core;
using NUnit.Framework;

namespace ICSharpCode.AddInManager2.Tests
{
	public class InstalledAddInsViewModelTests
	{
		FakeAddInManagerServices _services;
		
		AddIn _addIn1;
		AddIn _addIn1_new;
		AddIn _addIn2;
		AddIn _addIn2_new;
		
		InstalledAddInsViewModel _viewModel;
		
		public InstalledAddInsViewModelTests()
		{
		}
		
		private void CreateAddIns()
		{
			// Create AddIn objects from *.addin files available in this assembly's output directory
			FakeAddInTree _addInTree = new FakeAddInTree();

			using (StreamReader streamReader = new StreamReader(@"TestResources\AddInManager2Test.addin"))
			{
				_addIn1 = AddIn.Load(_addInTree, streamReader);
			}
			
			using (StreamReader streamReader = new StreamReader(@"TestResources\AddInManager2Test_New.addin"))
			{
				_addIn1_new = AddIn.Load(_addInTree, streamReader);
			}
			
			using (StreamReader streamReader = new StreamReader(@"TestResources\AddInManager2Test_2.addin"))
			{
				_addIn2 = AddIn.Load(_addInTree, streamReader);
			}
			
			using (StreamReader streamReader = new StreamReader(@"TestResources\AddInManager2Test_2_New.addin"))
			{
				_addIn2_new = AddIn.Load(_addInTree, streamReader);
			}
		}
		
		[SetUp]
		public void SetUp()
		{
			_services = new FakeAddInManagerServices();
			_services.FakeSDAddInManagement = new FakeSDAddInManagement();
			_services.Events = new AddInManagerEvents();
			_services.FakeSetup = new FakeAddInSetup(_services.SDAddInManagement);
			_services.FakeSettings = new FakeAddInManagerSettings();
			_services.FakeRepositories = new FakePackageRepositories();
			_services.FakeNuGet = new FakeNuGetPackageManager();
			
			_viewModel = new InstalledAddInsViewModel(_services);
		}
		
		[Test]
		public void ShowInstalledAddIns()
		{
			CreateAddIns();
			
			// Simulate list of AddIns
			_services.FakeSDAddInManagement.RegisteredAddIns.Add(_addIn1);
			_services.FakeSDAddInManagement.RegisteredAddIns.Add(_addIn2);
			
			_viewModel.ReadPackages();
		}
	}
}
