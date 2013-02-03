// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using System.Reflection;
using ICSharpCode.AddInManager2;
using ICSharpCode.AddInManager2.Model;
using ICSharpCode.AddInManager2.Tests.Fakes;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using NuGet;
using NUnit.Framework;

namespace ICSharpCode.AddInManager2.Tests
{
	[TestFixture]
	[Category("AddInSetupTests")]
	public class AddInSetupTests
	{
		AddInManagerEvents _events;
		FakeNuGetPackageManager _nuGet;
		FakeSDAddInManagement _sdAddInManagement;
		AddInSetup _addInSetup;
		
		public AddInSetupTests()
		{
		}
		
		private void PrepareAddInSetup()
		{
			_events = new AddInManagerEvents();
			_nuGet = new FakeNuGetPackageManager();
			_sdAddInManagement = new FakeSDAddInManagement();
			_addInSetup = new AddInSetup(_events, _nuGet, _sdAddInManagement);
		}
		
		private AddIn CreateAddIn()
		{
			// Create AddIn object from an *.addin file available in this assembly's resources
			FakeAddInTree _addInTree = new FakeAddInTree();
			Stream resourceStream =
				Assembly.GetExecutingAssembly().GetManifestResourceStream("ICSharpCode.AddInManager2.Tests.TestResources.AddInManager2Test.addin");
			using (StreamReader streamReader = new StreamReader(resourceStream))
			{
				return AddIn.Load(_addInTree, streamReader);
			}
		}
		
		[Test, Description("")]
		public void InstallValidAddIn()
		{
			AddIn addIn = CreateAddIn();
			
			// Create a fake package
			FakePackage package = new FakePackage()
			{
				Id = addIn.Name,
				Version = new SemanticVersion(addIn.Version)
			};
			
			PrepareAddInSetup();
			
			
		}
	}
}
