// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.UnitTesting
{
	public class SelectedTests
	{
		string namespaceFilter;
		IClass c;
		IMember member;
		List<IProject> projects = new List<IProject>();
		
		public SelectedTests(IProject project, string namespaceFilter, IClass c, IMember member)
		{
			this.namespaceFilter = namespaceFilter;
			this.c = c;
			this.member = member;
			
			if (project != null) {
				projects.Add(project);
			}
		}
		
		public SelectedTests(IProject project)
			: this(project, null,  null, null)
		{
		}
		
		public SelectedTests(object owner, IProject[] selectedProjects)
		{
			IProject project = TestableCondition.GetProject(owner);
			if (project != null) {
				projects.Add(project);
			} else {
				projects.AddRange(selectedProjects);
			}
			
			member = TestableCondition.GetMember(owner);
			c = GetClass(member, owner);
			namespaceFilter = TestableCondition.GetNamespace(owner);
		}
		
		static IClass GetClass(IMember member, Object owner)
		{
			if (member != null) {
				return member.DeclaringType;
			}
			return TestableCondition.GetClass(owner);
		}
		
		public bool HasProjects {
			get { return projects.Count > 0; }
		}
		
		public void RemoveFirstProject()
		{
			if (HasProjects) {
				projects.RemoveAt(0);
			}
		}
		
		public IEnumerable<IProject> Projects {
			get { return projects; }
		}
		
		public int ProjectsCount {
			get {return projects.Count;}
		}
		
		public IProject Project {
			get {
				if (projects.Count > 0) {
					return projects[0];
				}
				return null;
			}
		}
		
		public string NamespaceFilter {
			get { return namespaceFilter; }
		}
		
		public IClass Class {
			get { return c; }
		}
		
		public IMember Member {
			get { return member; }
		}
	}
}
