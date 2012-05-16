// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.ComponentModel;
using System.IO;
using ICSharpCode.Core;
using SD = ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class ProjectItem : MarshalByRefObject
	{
		SD.FileProjectItem projectItem;
		
		public const string CopyToOutputDirectoryPropertyName = "CopyToOutputDirectory";
		public const string CustomToolPropertyName = "CustomTool";
		public const string FullPathPropertyName = "FullPath";
		
		public ProjectItem(Project project, FileProjectItem projectItem)
		{
			this.projectItem = projectItem;
			this.ContainingProject = project;
			this.ProjectItems = new DirectoryProjectItems(this);
			CreateProperties();
			Kind = Constants.VsProjectItemKindPhysicalFile;
		}
		
		public ProjectItem()
		{
		}
		
		void CreateProperties()
		{
			var propertyFactory = new ProjectItemPropertyFactory(this);
			Properties = new Properties(propertyFactory);
		}
		
		public virtual string Name {
			get { return Path.GetFileName(projectItem.Include); }
		}
		
		public virtual string Kind { get; set; }
		
		public Project SubProject {
			get { return null; }
		}
		
		public virtual Properties Properties { get; private set; }
		public virtual Project ContainingProject  { get; private set; }
		public virtual ProjectItems ProjectItems { get; private set; }
		
		internal virtual object GetProperty(string name)
		{
			if (name == CopyToOutputDirectoryPropertyName) {
				return GetCopyToOutputDirectory();
			} else if (name == CustomToolPropertyName) {
				return projectItem.CustomTool;
			} else if (name == FullPathPropertyName) {
				return projectItem.FileName;
			}
			return String.Empty;
		}
		
		UInt32 GetCopyToOutputDirectory()
		{
			return (UInt32)projectItem.CopyToOutputDirectory;
		}
		
		internal virtual void SetProperty(string name, object value)
		{
			if (name == CopyToOutputDirectoryPropertyName) {
				SetCopyToOutputDirectory(value);
			} else if (name == CustomToolPropertyName) {
				projectItem.CustomTool = value as string;
			}
		}
		
		void SetCopyToOutputDirectory(object value)
		{
			CopyToOutputDirectory copyToOutputDirectory = ConvertToCopyToOutputDirectory(value);
			projectItem.CopyToOutputDirectory = copyToOutputDirectory;
		}
		
		CopyToOutputDirectory ConvertToCopyToOutputDirectory(object value)
		{
			string valueAsString = value.ToString();
			return (CopyToOutputDirectory)Enum.Parse(typeof(CopyToOutputDirectory), valueAsString);
		}
		
		internal virtual bool IsMatchByName(string name)
		{
			return String.Equals(this.Name, name, StringComparison.InvariantCultureIgnoreCase);
		}
		
		internal virtual bool IsChildItem(SD.ProjectItem msbuildProjectItem)
		{
			string directory = Path.GetDirectoryName(msbuildProjectItem.Include);
			return IsMatchByName(directory);
		}
		
		internal virtual ProjectItemRelationship GetRelationship(SD.ProjectItem msbuildProjectItem)
		{
			return new ProjectItemRelationship(this, msbuildProjectItem);
		}
		
		public void Delete()
		{
			ContainingProject.DeleteFile(projectItem.FileName);
			ContainingProject.Save();
		}
	}
}
