// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.SharpDevelop.Internal.Templates;
using System;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.WixBinding;
using NUnit.Framework;

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
			wixNodeBuilder = new WixProjectNodeBuilder();
			project = new MSBuildBasedProject(
				new ProjectCreateInformation {
					OutputProjectFileName = @"C:\Projects\Test\test.csproj",
					Solution = new Solution(),
					ProjectName = "test"
				}
			);
			project.IdGuid = "FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF";
		}
		
		[Test]
		public void CannotBuildNonWixProject()
		{
			Assert.IsFalse(wixNodeBuilder.CanBuildProjectTree(project));
		}
	}
}
