// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Debugging;

namespace ICSharpCode.SharpDevelop.Project
{
	public abstract class ProjectBehavior
	{
		ProjectBehavior next;
		protected IProject Project { get; private set; }
		
		public ProjectBehavior(IProject project, ProjectBehavior next = null)
		{
			if (project == null)
				throw new ArgumentNullException("project");
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
		
		public virtual ICollection<ItemType> AvailableFileItemTypes {
			get {
				if (this.next != null)
					return next.AvailableFileItemTypes;
				return null;
			}
		}
		
		public virtual void ProjectCreationComplete()
		{
			if (this.next != null)
				next.ProjectCreationComplete();
		}
	}
	
	sealed class DefaultProjectBehavior : ProjectBehavior
	{
		public DefaultProjectBehavior(IProject project)
			: base(project)
		{
		}
		
		public override bool IsStartable {
			get { return false; }
		}
		
		public override void Start(bool withDebugging)
		{
			ProcessStartInfo psi;
			try {
				if (!(Project is AbstractProject))
					return;
				psi = ((AbstractProject)Project).CreateStartInfo();
			} catch (ProjectStartException ex) {
				MessageService.ShowError(ex.Message);
				return;
			}
			if (withDebugging) {
				DebuggerService.CurrentDebugger.Start(psi);
			} else {
				DebuggerService.CurrentDebugger.StartWithoutDebugging(psi);
			}
		}
		
		public override ProcessStartInfo CreateStartInfo()
		{
			throw new NotSupportedException();
		}
		
		public override ItemType GetDefaultItemType(string fileName)
		{
			return ItemType.None;
		}
		
		public override ProjectItem CreateProjectItem(IProjectItemBackendStore item)
		{
			return new UnknownProjectItem(Project, item);
		}
		
		public override ICollection<ItemType> AvailableFileItemTypes {
			get { return ItemType.DefaultFileItems; }
		}
		
		public override void ProjectCreationComplete()
		{
			
		}
	}
}
