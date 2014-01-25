// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Xml.Linq;
using AspNet.Mvc.Tests.Helpers;
using ICSharpCode.AspNet.Mvc;
using ICSharpCode.SharpDevelop.Project;
using NUnit.Framework;

namespace AspNet.Mvc.Tests
{
	[TestFixture]
	public class VisualStudioProjectExtensionTests : MvcTestsBase
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
