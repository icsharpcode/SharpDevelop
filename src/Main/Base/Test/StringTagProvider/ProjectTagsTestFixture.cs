// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using ICSharpCode.SharpDevelop.Commands;
using ICSharpCode.SharpDevelop.Project;
using NUnit.Framework;

namespace ICSharpCode.SharpDevelop.Tests.StringTagProvider
{
	/// <summary>
	/// Tests the SharpDevelopStringTagProvider when there is an active project.
	/// </summary>
	[TestFixture]
	public class ProjectTagsTestFixture
	{
		SharpDevelopStringTagProvider tagProvider;
		MockProjectForTagProvider project;
		
		[SetUp]
		public void Init()
		{
			project = new MockProjectForTagProvider();
			project.FileName = @"C:\Projects\MyProject\MyProject.csproj";
			project.Directory = @"C:\Projects\MyProject";
			project.OutputAssemblyFullPath = @"C:\Projects\MyProject\bin\Debug\MyProject.exe";
			project.Name = "MyProject";
			
			ProjectService.CurrentProject = project;
			tagProvider = new SharpDevelopStringTagProvider();
		}
		
		/// <summary>
		/// Sanity check the mock project implementation.
		/// </summary>
		[Test]
		public void MockProjectFileName()
		{
			Assert.AreEqual(@"C:\Projects\MyProject\MyProject.csproj", project.FileName);
		}

		/// <summary>
		/// Sanity check the mock project implementation.
		/// </summary>
		[Test]
		public void MockProjectDirectory()
		{
			Assert.AreEqual(@"C:\Projects\MyProject", project.Directory);
		}

		/// <summary>
		/// Sanity check the mock project implementation.
		/// </summary>
		[Test]
		public void MockProjectOutputAssemblyFullPath()
		{
			Assert.AreEqual(@"C:\Projects\MyProject\bin\Debug\MyProject.exe", project.OutputAssemblyFullPath);
		}
		
		/// <summary>
		/// Sanity check the mock project implementation.
		/// </summary>
		[Test]
		public void MockProjectName()
		{
			Assert.AreEqual("MyProject", project.Name);
		}		
		
		[Test]
		public void ConvertCurrentProjectName()
		{
			Assert.AreEqual(project.Name, tagProvider.Convert("CurrentProjectName"));
		}
		
		[Test]
		public void ConvertTargetPath()
		{
			Assert.AreEqual(project.OutputAssemblyFullPath, tagProvider.Convert("TargetPath"));
		}		

		[Test]
		public void ConvertTargetDir()
		{
			Assert.AreEqual(Path.GetDirectoryName(project.OutputAssemblyFullPath), tagProvider.Convert("TargetDir"));
		}		

		[Test]
		public void ConvertTargetName()
		{
			Assert.AreEqual("MyProject.exe", tagProvider.Convert("TargetName"));
		}		
		
		[Test]
		public void ConvertTargetExt()
		{
			Assert.AreEqual(".exe", tagProvider.Convert("TargetExt"));
		}		

		[Test]
		public void ConvertProjectDir()
		{
			Assert.AreEqual(project.Directory, tagProvider.Convert("ProjectDir"));
		}		
		
		[Test]
		public void ConvertProjectFileName()
		{
			Assert.AreEqual(Path.GetFileName(project.FileName), tagProvider.Convert("ProjectFileName"));
		}		
	}
}
