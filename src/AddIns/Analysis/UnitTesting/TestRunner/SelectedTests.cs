// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.UnitTesting
{
	public class SelectedTests
	{
		string namespaceFilter;
		TestClass c;
		TestMember member;
		List<IProject> projects = new List<IProject>();
		
		public SelectedTests(IProject project, string namespaceFilter, TestClass c, TestMember member)
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
			
			TestProject testProject = TestService.Solution.GetTestProject(project);
			if (testProject != null) {
				member = testProject.GetTestMember(TestableCondition.GetMember(owner));
				c = testProject.GetTestClass(TestableCondition.GetClassFromMemberOrCaller(owner));
			}
			namespaceFilter = TestableCondition.GetNamespace(owner);
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
		
		public TestClass Class {
			get { return c; }
		}
		
		public TestMember Member {
			get { return member; }
		}
	}
}
