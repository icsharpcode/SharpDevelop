// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.AspNet.Mvc
{
	public class MvcClass : IMvcClass
	{
		IClass c;
		
		public MvcClass(IClass c)
		{
			this.c = c;
		}
		
		public string FullName {
			get { return c.FullyQualifiedName; }
		}
		
		public string Name {
			get { return c.Name; }
		}
		
		public string Namespace {
			get { return c.Namespace; }
		}
		
		public string BaseClassFullName {
			get { return GetBaseClassFullName(); }
		}
		
		string GetBaseClassFullName()
		{
			IClass baseClass = c.BaseClass;
			if (baseClass != null) {
				return baseClass.FullyQualifiedName;
			}
			return String.Empty;
		}
		
		public string AssemblyLocation {
			get { return GetAssemblyLocation(); }
		}
		
		string GetAssemblyLocation()
		{
			IProject project = GetProject();
			return project.OutputAssemblyFullPath;
		}
		
		IProject GetProject()
		{
			return c.ProjectContent.Project as IProject;
		}
	}
}
