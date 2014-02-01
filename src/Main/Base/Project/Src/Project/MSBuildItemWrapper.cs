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
