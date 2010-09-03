// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;

using MSBuild = Microsoft.Build.Evaluation;

namespace ICSharpCode.SharpDevelop.Project
{
	sealed class MSBuildItemWrapper : IProjectItemBackendStore
	{
		readonly MSBuildBasedProject project;
		MSBuild.ProjectItem item;
		
		public MSBuildItemWrapper(MSBuildBasedProject project, MSBuild.ProjectItem item)
		{
			this.project = project;
			this.item = item;
		}
		
		public MSBuild.ProjectItem MSBuildItem {
			get { return item; }
		}
		
		public IProject Project { 
			get { return project; }
		}
		
		public string UnevaluatedInclude { 
			get { return item.UnevaluatedInclude; }
			set { item.UnevaluatedInclude = value; }
		}
		public string EvaluatedInclude { 
			 get { return item.EvaluatedInclude; }
			 set { item.UnevaluatedInclude = MSBuildInternals.Escape(value); }
		}
		
		public ItemType ItemType { 
			get { return new ItemType(item.ItemType); }
			set { item.ItemType = value.ItemName; }
		}
		
		public string GetEvaluatedMetadata(string name)
		{
			return item.GetMetadataValue(name);
		}
		public string GetMetadata(string name)
		{
			var m = item.GetMetadata(name);
			return m != null ? m.UnevaluatedValue : null;
		}
		public bool HasMetadata(string name)
		{
			return item.HasMetadata(name);
		}
		public void RemoveMetadata(string name)
		{
			item.RemoveMetadata(name);
		}
		public void SetEvaluatedMetadata(string name, string value)
		{
			item.SetMetadataValue(name, MSBuildInternals.Escape(value));
		}
		public void SetMetadata(string name, string value)
		{
			item.SetMetadataValue(name, value);
		}
		public IEnumerable<string> MetadataNames {
			get { return item.DirectMetadata.Select(m => m.Name).ToList(); }
		}
	}
}
