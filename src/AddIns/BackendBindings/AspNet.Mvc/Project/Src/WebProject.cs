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
