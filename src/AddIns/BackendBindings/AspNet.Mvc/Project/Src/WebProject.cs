// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.AspNet.Mvc
{
	public class WebProject
	{
		public static readonly WebProjectProperties DefaultProperties = new WebProjectProperties
		{
			DevelopmentServerPort = 8080,
			DevelopmentServerVPath = "/"
		};
		
		MSBuildBasedProject msbuildProject;
		
		public WebProject(MSBuildBasedProject msbuildProject)
		{
			this.msbuildProject = msbuildProject;
		}
		
		public string Name {
			get { return msbuildProject.Name; }
		}
		
		public string Directory {
			get { return msbuildProject.Directory; }
		}
		
		public bool HasWebProjectProperties()
		{
			if (VisualStudioProjectExtension.ProjectContainsExtension(msbuildProject)) {
				var projectExtension = new VisualStudioProjectExtension(msbuildProject);
				return projectExtension.ContainsWebProjectProperties();
			}
			return false;
		}
		
		public WebProjectProperties GetWebProjectProperties()
		{
			if (HasWebProjectProperties()) {
				var projectExtension = new VisualStudioProjectExtension(msbuildProject);
				WebProjectProperties properties = projectExtension.GetWebProjectProperties();
				properties.UseIISExpress = UseIISExpress;
				return properties;
			}
			return DefaultProperties;
		}
		
		public bool UseIISExpress
		{
			get { return GetUseIISExpress(); }
		}
		
		bool GetUseIISExpress()
		{
			string value = GetMSBuildProperty("UseIISExpress");
			bool result = false;
			Boolean.TryParse(value, out result);
			return result;
		}
		
		string GetMSBuildProperty(string propertyName)
		{
			string value = msbuildProject.GetEvaluatedProperty(propertyName);
			if (value != null) {
				return value;
			}
			return String.Empty;
		}
		
		public void UpdateWebProjectProperties(WebProjectProperties properties)
		{
			var projectExtension = new VisualStudioProjectExtension(properties);
			projectExtension.Save(msbuildProject);
			SetMSBuildProperty("UseIISExpress", properties.UseIISExpress);
		}
		
		void SetMSBuildProperty(string propertyName, bool value)
		{
			msbuildProject.SetProperty(propertyName, value.ToString());
		}
	}
}
