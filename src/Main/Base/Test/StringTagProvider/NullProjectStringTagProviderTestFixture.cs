// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.SharpDevelop.Commands;
using ICSharpCode.SharpDevelop.Project;
using NUnit.Framework;

namespace ICSharpCode.SharpDevelop.Tests.StringTagProvider
{
	/// <summary>
	/// Tests the SharpDevelopStringTagProvider when there is no active project.
	/// </summary>
	[TestFixture]
	public class NullProjectStringTagProviderTestFixture
	{
		SharpDevelopStringTagProvider tagProvider;
		
		[SetUp]
		public void Init()
		{
			ProjectService.CurrentProject = null;
			tagProvider = new SharpDevelopStringTagProvider();
		}
		
		[Test]
		public void ConvertCurrentProjectName()
		{
			Assert.AreEqual("<no current project>", tagProvider.Convert("CurrentProjectName"));
		}
		
		[Test]
		public void ConvertTargetPath()
		{
			Assert.AreEqual(String.Empty, tagProvider.Convert("TargetPath"));
		}		

		[Test]
		public void ConvertTargetDir()
		{
			Assert.AreEqual(String.Empty, tagProvider.Convert("TargetDir"));
		}		

		[Test]
		public void ConvertTargetName()
		{
			Assert.AreEqual(String.Empty, tagProvider.Convert("TargetName"));
		}		
		
		[Test]
		public void ConvertTargetExt()
		{
			Assert.AreEqual(String.Empty, tagProvider.Convert("TargetExt"));
		}		

		[Test]
		public void ConvertProjectDir()
		{
			Assert.AreEqual(String.Empty, tagProvider.Convert("ProjectDir"));
		}		
		
		[Test]
		public void ConvertProjectFileName()
		{
			Assert.AreEqual(String.Empty, tagProvider.Convert("ProjectFileName"));
		}		
	}
}
