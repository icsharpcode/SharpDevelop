// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
		IMember method;
		List<IProject> projects = new List<IProject>();
		
		public SelectedTests(IProject project, string namespaceFilter, IClass c, IMember method)
		{
			this.namespaceFilter = namespaceFilter;
			this.c = c;
			this.method = method;
			
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
			
			method = TestableCondition.GetMember(owner);
			c = GetClass(method, owner);
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
		
		public IList<IProject> Projects {
			get { return projects; }
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
		
		public IMember Method {
			get { return method; }
		}
	}
}
