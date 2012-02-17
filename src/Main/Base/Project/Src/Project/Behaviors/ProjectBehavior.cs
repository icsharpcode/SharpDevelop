// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Linq;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Debugging;
using ICSharpCode.SharpDevelop.Gui.OptionPanels;
using ICSharpCode.SharpDevelop.Project.Converter;
using ICSharpCode.SharpDevelop.Util;

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
		
		public virtual IEnumerable<CompilerVersion> GetAvailableCompilerVersions()
		{
			if (this.next != null)
				return next.GetAvailableCompilerVersions();
			return Enumerable.Empty<CompilerVersion>();
		}
		
		public virtual void UpgradeProject(CompilerVersion newVersion, TargetFramework newFramework)
		{
			if (this.next != null)
				next.UpgradeProject(newVersion, newFramework);
		}
		
		public virtual Properties CreateMemento()
		{
			if (this.next != null)
				return next.CreateMemento();
			throw new InvalidOperationException();
		}
		
		public virtual void SetMemento(Properties memento)
		{
			if (this.next != null)
				next.SetMemento(memento);
		}
	}
	

	
	public class XamlBehavior : ProjectBehavior
	{
		public override ItemType GetDefaultItemType(string fileName)
		{
			if (".xaml".Equals(Path.GetExtension(fileName), StringComparison.OrdinalIgnoreCase))
				return ItemType.Page;

			return base.GetDefaultItemType(fileName);
		}
	}
	
	public class SilverlightBehavior : ProjectBehavior
	{
		public override bool IsStartable {
			get { return TestPageFileName.Length > 0; }
		}
		
		public override ProcessStartInfo CreateStartInfo()
		{
			string pagePath = "file:///" + Path.Combine(((CompilableProject)Project).OutputFullPath, TestPageFileName);
			return new  ProcessStartInfo(pagePath);
		}
		
		public string TestPageFileName {
			get { return ((MSBuildBasedProject)Project).GetEvaluatedProperty("TestPageFileName") ?? ""; }
			set { ((MSBuildBasedProject)Project).SetProperty("TestPageFileName", string.IsNullOrEmpty(value) ? null : value); }
		}
	}
}