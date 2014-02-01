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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Linq;

using ICSharpCode.Core;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop.Debugging;
using ICSharpCode.SharpDevelop.Gui.OptionPanels;
using ICSharpCode.SharpDevelop.Project.Converter;

namespace ICSharpCode.SharpDevelop.Project
{
	public abstract class ProjectBehavior
	{
		ProjectBehavior next;
		protected IProject Project { get; private set; }
		
		public ProjectBehavior()
		{
			
		}
		
		protected ProjectBehavior(IProject project, ProjectBehavior next = null)
		{
			this.Project = project;
			this.next = next;
		}
		
		internal void SetProject(IProject project)
		{
			if (project == null)
				throw new ArgumentNullException("project");
			this.Project = project;
		}

		internal void SetNext(ProjectBehavior next)
		{
			if (next == null)
				throw new ArgumentNullException("next");
			this.next = next;
		}
		
		public virtual bool IsStartable {
			get {
				if (this.next != null)
					return next.IsStartable;
				return false;
			}
		}
		
		public virtual void Start(bool withDebugging)
		{
			if (this.next != null)
				next.Start(withDebugging);
		}
		
		public virtual ProcessStartInfo CreateStartInfo()
		{
			if (this.next != null)
				return next.CreateStartInfo();
			return null;
		}
		
		public virtual ItemType GetDefaultItemType(string fileName)
		{
			if (this.next != null)
				return next.GetDefaultItemType(fileName);
			return default(ItemType);
		}
		
		public virtual ProjectItem CreateProjectItem(IProjectItemBackendStore item)
		{
			if (this.next != null)
				return next.CreateProjectItem(item);
			return null;
		}
		
		public virtual void ProjectCreationComplete()
		{
			if (this.next != null)
				next.ProjectCreationComplete();
		}
		
		public virtual void ProjectLoaded()
		{
			if (this.next != null)
				next.ProjectLoaded();
		}
		
		public virtual IEnumerable<CompilerVersion> GetAvailableCompilerVersions()
		{
			if (this.next != null)
				return next.GetAvailableCompilerVersions();
			return Enumerable.Empty<CompilerVersion>();
		}
		
		public virtual IEnumerable<TargetFramework> GetAvailableTargetFrameworks()
		{
			if (this.next != null)
				return next.GetAvailableTargetFrameworks();
			return Enumerable.Empty<TargetFramework>();
		}
		
		public virtual CompilerVersion CurrentCompilerVersion {
			get {
				if (this.next != null)
					return next.CurrentCompilerVersion;
				throw new InvalidOperationException();
			}
		}
		
		public virtual TargetFramework CurrentTargetFramework {
			get {
				if (this.next != null)
					return next.CurrentTargetFramework;
				throw new InvalidOperationException();
			}
		}
		
		public virtual void UpgradeProject(CompilerVersion newVersion, TargetFramework newFramework)
		{
			if (this.next != null)
				next.UpgradeProject(newVersion, newFramework);
		}
		
		/// <summary>
		/// Saves project preferences (currently opened files, bookmarks etc.) to the
		/// a property container.
		/// </summary>
		public virtual void SavePreferences(Properties preferences)
		{
			if (this.next != null)
				next.SavePreferences(preferences);
		}
		
		public virtual Refactoring.ISymbolSearch PrepareSymbolSearch(ISymbol entity)
		{
			if (this.next != null)
				return this.next.PrepareSymbolSearch(entity);
			return null;
		}
	}
}
