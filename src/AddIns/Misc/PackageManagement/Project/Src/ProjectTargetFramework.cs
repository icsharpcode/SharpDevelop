// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Runtime.Versioning;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.PackageManagement
{
	public class ProjectTargetFramework
	{
		MSBuildBasedProject project;
		FrameworkName targetFramework;
		
		public ProjectTargetFramework(MSBuildBasedProject project)
		{
			this.project = project;
			GetTargetFramework();
		}
		
		void GetTargetFramework()
		{
			string identifier = GetTargetFrameworkIdentifier();
			string version = GetTargetFrameworkVersion();
			string profile = GetTargetFrameworkProfile();
			
			GetTargetFramework(identifier, version, profile);
		}
		
		void GetTargetFramework(string identifier, string version, string profile)
		{
			string name = String.Format("{0}, Version={1}, Profile={2}", identifier, version, profile);
			targetFramework = new FrameworkName(name);
		}
		
		string GetTargetFrameworkIdentifier()
		{
			return GetEvaluatedPropertyOrDefault("TargetFrameworkIdentifier", ".NETFramework");
		}
		
		string GetTargetFrameworkVersion()
		{
			return project.GetEvaluatedProperty("TargetFrameworkVersion");
		}
		
		string GetTargetFrameworkProfile()
		{
			return GetEvaluatedPropertyOrDefault("TargetFrameworkProfile", String.Empty);
		}
		
		string GetEvaluatedPropertyOrDefault(string propertyName, string defaultPropertyValue)
		{
			string propertyValue = project.GetEvaluatedProperty(propertyName);
			if (String.IsNullOrEmpty(propertyValue)) {
				return defaultPropertyValue;
			}
			return propertyValue;
		}
		
		public FrameworkName TargetFrameworkName {
			get { return targetFramework; }
		}
	}
}
