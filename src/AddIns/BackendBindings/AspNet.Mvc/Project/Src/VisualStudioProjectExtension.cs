// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
