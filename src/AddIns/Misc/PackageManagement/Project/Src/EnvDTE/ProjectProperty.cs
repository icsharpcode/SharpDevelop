// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class ProjectProperty : Property
	{
		Project project;
		
		public ProjectProperty(Project project, string name)
			: base(name)
		{
			this.project = project;
		}
		
		protected override object GetValue()
		{
			string value = GetMSBuildProjectProperty(Name);
			if (value != null) {
				return value;
			}
			
			if (IsTargetFrameworkMoniker()) {
				return GetTargetFrameworkMoniker();
			} else if (IsFullPath()) {
				return GetFullPath();
			} else if (IsOutputFileName()) {
				return GetOutputFileName();
			} else if (IsDefaultNamespace()) {
				return GetDefaultNamespace();
			}
			return EmptyStringIfNull(value);
		}
		
		string GetMSBuildProjectProperty(string name)
		{
			return MSBuildProject.GetUnevalatedProperty(name);
		}
		
		bool IsTargetFrameworkMoniker()
		{
			return IsCaseInsensitiveMatch(Name, "TargetFrameworkMoniker");
		}
		
		bool IsFullPath()
		{
			return IsCaseInsensitiveMatch(Name, "FullPath");
		}
		
		bool IsOutputFileName()
		{
			return IsCaseInsensitiveMatch(Name, "OutputFileName");
		}
		
		bool IsCaseInsensitiveMatch(string a, string b)
		{
			return String.Equals(a, b, StringComparison.OrdinalIgnoreCase);
		}
		
		string GetTargetFrameworkMoniker()
		{
			var targetFramework = new ProjectTargetFramework(MSBuildProject);
			return targetFramework.TargetFrameworkName.ToString();
		}
		
		bool IsDefaultNamespace()
		{
			return IsCaseInsensitiveMatch(Name, "DefaultNamespace");
		}
		
		string GetDefaultNamespace()
		{
			return MSBuildProject.RootNamespace;
		}
		
		MSBuildBasedProject MSBuildProject {
			get { return project.MSBuildProject; }
		}
		
		string GetFullPath()
		{
			return MSBuildProject.Directory;
		}
		
		string GetOutputFileName()
		{
			return String.Format("{0}{1}", MSBuildProject.AssemblyName, GetOutputTypeFileExtension());
		}
		
		string GetOutputTypeFileExtension()
		{
			string outputTypeProperty = GetMSBuildProjectProperty("OutputType");
			OutputType outputType = GetOutputType(outputTypeProperty);
			return CompilableProject.GetExtension(outputType);
		}
		
		OutputType GetOutputType(string outputTypeProperty)
		{
			if (outputTypeProperty == null) {
				return OutputType.Exe;
			}
			
			OutputType outputType = OutputType.Exe;
			if (Enum.TryParse<OutputType>(outputTypeProperty, true, out outputType)) {
				return outputType;
			}
			return OutputType.Exe;
		}
		
		string EmptyStringIfNull(string value)
		{
			if (value != null) {
				return value;
			}
			return String.Empty;
		}
		
		protected override void SetValue(object value)
		{
			bool escapeValue = false;
			MSBuildProject.SetProperty(Name, value as string, escapeValue);
			project.Save();
		}
	}
}
