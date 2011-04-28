// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.ComponentModel;
using System.IO;

using SD = ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class ProjectItem
	{
		SD.FileProjectItem projectItem;
		public const string CopyToOutputDirectoryPropertyName = "CopyToOutputDirectory";
		
		public ProjectItem(Project project, SD.FileProjectItem projectItem)
		{
			this.projectItem = projectItem;
			this.ContainingProject = project;
			CreateProperties();
		}
		
		void CreateProperties()
		{
			var propertyFactory = new ProjectItemPropertyFactory(this);
			Properties = new Properties(propertyFactory);			
		}
		
		public string Name {
			get { return Path.GetFileName(projectItem.Include); }
		}
		
		public Properties Properties { get; private set; }
		public Project ContainingProject  { get; private set; }
		
		internal object GetProperty(string name)
		{
			if (name == CopyToOutputDirectoryPropertyName) {
				return GetCopyToOutputDirectory();
			}
			return String.Empty;
		}
		
		UInt32 GetCopyToOutputDirectory()
		{
			return (UInt32)projectItem.CopyToOutputDirectory;
		}
		
		internal void SetProperty(string name, object value)
		{
			if (name == CopyToOutputDirectoryPropertyName) {
				SetCopyToOutputDirectory(value);
			}
		}
		
		void SetCopyToOutputDirectory(object value)
		{
			SD.CopyToOutputDirectory copyToOutputDirectory = ConvertToCopyToOutputDirectory(value);
			projectItem.CopyToOutputDirectory = copyToOutputDirectory;
		}
		
		SD.CopyToOutputDirectory ConvertToCopyToOutputDirectory(object value)
		{
			string valueAsString = value.ToString();
			return (SD.CopyToOutputDirectory)Enum.Parse(typeof(SD.CopyToOutputDirectory), valueAsString);
		}
	}
}
