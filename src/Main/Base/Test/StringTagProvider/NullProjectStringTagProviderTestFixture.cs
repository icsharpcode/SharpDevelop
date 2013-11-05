// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Commands;
using ICSharpCode.SharpDevelop.Project;
using NUnit.Framework;
using Rhino.Mocks;

namespace ICSharpCode.SharpDevelop.Tests.StringTagProvider
{
	/// <summary>
	/// Tests the SharpDevelopStringTagProvider when there is no active project.
	/// </summary>
	[TestFixture]
	public class NullProjectStringTagProviderTestFixture : SDTestFixtureBase
	{
		SharpDevelopStringTagProvider tagProvider;
		
		public override void FixtureSetUp()
		{
			base.FixtureSetUp();
			SD.Services.AddService(typeof(IProjectService), MockRepository.GenerateStrictMock<IProjectService>());
			SD.ProjectService.Stub(s => s.CurrentProject).Return(null);
			
			tagProvider = new SharpDevelopStringTagProvider();
		}
		
		[Test]
		public void ConvertCurrentProjectName()
		{
			Assert.AreEqual("<no current project>", tagProvider.ProvideString("CurrentProjectName"));
		}
		
		[Test]
		public void ConvertTargetPath()
		{
			Assert.AreEqual(String.Empty, tagProvider.ProvideString("TargetPath"));
		}		

		[Test]
		public void ConvertTargetDir()
		{
			Assert.AreEqual(String.Empty, tagProvider.ProvideString("TargetDir"));
		}		

		[Test]
		public void ConvertTargetName()
		{
			Assert.AreEqual(String.Empty, tagProvider.ProvideString("TargetName"));
		}		
		
		[Test]
		public void ConvertTargetExt()
		{
			Assert.AreEqual(String.Empty, tagProvider.ProvideString("TargetExt"));
		}		

		[Test]
		public void ConvertProjectDir()
		{
			Assert.AreEqual(String.Empty, tagProvider.ProvideString("ProjectDir"));
		}		
		
		[Test]
		public void ConvertProjectFileName()
		{
			Assert.AreEqual(String.Empty, tagProvider.ProvideString("ProjectFileName"));
		}		
	}
}
