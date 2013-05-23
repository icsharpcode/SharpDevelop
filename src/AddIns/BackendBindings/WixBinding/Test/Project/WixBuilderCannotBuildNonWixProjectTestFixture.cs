// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.WixBinding;
using NUnit.Framework;
using WixBinding.Tests.Utils;

namespace WixBinding.Tests.Project
{
	/// <summary>
	/// Tests that a non-Wix project is ignored by the WixProjectNodeBuilder.
	/// </summary>
	[TestFixture]
	public class WixBuilderCannotBuildNonWixProjectTestFixture
	{
		WixProjectNodeBuilder wixNodeBuilder;
		MSBuildBasedProject project;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			SD.InitializeForUnitTests();
			wixNodeBuilder = new WixProjectNodeBuilder();
			project = new MSBuildBasedProject(
				new ProjectCreateInformation(MockSolution.Create(), new FileName(@"C:\Projects\Test\test.csproj"))
			);
			project.IdGuid = Guid.Parse("FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF");
		}
		
		[Test]
		public void CannotBuildNonWixProject()
		{
			Assert.IsFalse(wixNodeBuilder.CanBuildProjectTree(project));
		}
	}
}
