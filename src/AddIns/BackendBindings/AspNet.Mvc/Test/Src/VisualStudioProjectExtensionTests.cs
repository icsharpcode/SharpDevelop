// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Xml.Linq;
using AspNet.Mvc.Tests.Helpers;
using ICSharpCode.AspNet.Mvc;
using ICSharpCode.SharpDevelop.Project;
using NUnit.Framework;

namespace AspNet.Mvc.Tests
{
	[TestFixture]
	public class VisualStudioProjectExtensionTests
	{
		VisualStudioProjectExtension extension;
		MSBuildBasedProject msbuildProject;
		
		void CreateExtension()
		{
			msbuildProject = MSBuildProjectHelper.CreateCSharpProject();
			CreateExtension(msbuildProject);
		}
		
		void CreateExtension(MSBuildBasedProject msbuildProject)
		{
			extension = new VisualStudioProjectExtension(msbuildProject);
		}
		
		void CreateMSBuildProject()
		{
			msbuildProject = MSBuildProjectHelper.CreateCSharpProject();
		}
		
		void CreateMSBuildProjectWithWebProjectProperties()
		{
			CreateMSBuildProject();
			var properties = new WebProjectProperties();
			var extension = new VisualStudioProjectExtension(properties);
			extension.Save(msbuildProject);
		}
		
		void AddVisualStudioExtensionWithChildElement(string childElementName)
		{
			msbuildProject.SaveProjectExtensions(VisualStudioProjectExtension.ProjectExtensionName, new XElement(childElementName));
		}
		
		[Test]
		public void ProjectContainsExtension_MSBuildProjectHasNoVisualStudioExtension_ReturnsFalse()
		{
			CreateMSBuildProject();
			
			bool exists = VisualStudioProjectExtension.ProjectContainsExtension(msbuildProject);
			
			Assert.IsFalse(exists);
		}
		
		[Test]
		public void ProjectContainsExtension_MSBuildProjectHasVisualStudioExtension_ReturnsTrue()
		{
			CreateMSBuildProject();
			AddVisualStudioExtensionWithChildElement("Test");
			
			bool exists = VisualStudioProjectExtension.ProjectContainsExtension(msbuildProject);
			
			Assert.IsTrue(exists);
		}
		

		[Test]
		public void ContainsWebProjectProperties_MSBuildProjectHasWebProjectProperties_ReturnsTrue()
		{
			CreateMSBuildProjectWithWebProjectProperties();
			CreateExtension(msbuildProject);
			
			bool contains = extension.ContainsWebProjectProperties();
			
			Assert.IsTrue(contains);
		}
		
		[Test]
		public void ContainsWebProjectProperties_MSBuildProjectDoesNotHaveWebProjectProperties_ReturnsFalse()
		{
			CreateMSBuildProject();
			CreateExtension();
			
			bool contains = extension.ContainsWebProjectProperties();
			
			Assert.IsFalse(contains);
		}
		
		[Test]
		public void ContainsWebProjectProperties_MSBuildProjectDoesNotHaveWebProjectPropertiesButHasVisualStudioExtension_ReturnsFalse()
		{
			CreateMSBuildProject();
			AddVisualStudioExtensionWithChildElement("Test");
			CreateExtension(msbuildProject); 
			
			bool contains = extension.ContainsWebProjectProperties();
			
			Assert.IsFalse(contains);
		}
	}
}
