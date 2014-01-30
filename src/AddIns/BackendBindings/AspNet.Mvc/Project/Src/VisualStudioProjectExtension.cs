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
using System.Linq;
using System.Xml.Linq;

using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.AspNet.Mvc
{
	public class VisualStudioProjectExtension
	{
		public static readonly string ProjectExtensionName = "VisualStudio";
		public static readonly string FlavorPropertiesName = "FlavorProperties";
		
		WebProjectProperties webProjectProperties;
		MSBuildBasedProject msbuildProject;
		
		public VisualStudioProjectExtension(WebProjectProperties properties)
		{
			this.webProjectProperties = properties;
		}
		
		public VisualStudioProjectExtension(MSBuildBasedProject msbuildProject)
		{
			this.msbuildProject = msbuildProject;
		}
		
		public string Name {
			get { return ProjectExtensionName; }
		}
		
		public XElement ToXElement()
		{	
			var flavorProperties = new XElement(FlavorPropertiesName);
			flavorProperties.SetAttributeValue("GUID", "{349C5851-65DF-11DA-9384-00065B846F21}");
			flavorProperties.Add(webProjectProperties.ToXElement());
			return flavorProperties;
		}
		
		public static bool ProjectContainsExtension(MSBuildBasedProject msbuildProject)
		{
			return msbuildProject.ContainsProjectExtension(ProjectExtensionName);
		}
		
		public WebProjectProperties GetWebProjectProperties()
		{
			XElement element = msbuildProject.LoadProjectExtensions(ProjectExtensionName);
			return new WebProjectProperties(element.Descendants().First());
		}
		
		public void Save(MSBuildBasedProject msbuildProject)
		{
			msbuildProject.SaveProjectExtensions(ProjectExtensionName, ToXElement());
		}
		
		public bool ContainsWebProjectProperties()
		{
			XElement element = msbuildProject.LoadProjectExtensions(ProjectExtensionName);
			return element.Element("WebProjectProperties") != null;
		}
	}
}
