// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Linq;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.AspNet.Mvc
{
	public class MvcClass : IMvcClass
	{
		ITypeDefinition type;
		IMvcProject project;
		
		public MvcClass(ITypeDefinition type, IMvcProject project)
		{
			this.type = type;
			this.project = project;
		}
		
		public string FullName {
			get { return type.FullName; }
		}
		
		public string Name {
			get { return type.Name; }
		}
		
		public string Namespace {
			get { return type.Namespace; }
		}
		
		public string BaseClassFullName {
			get { return GetBaseClassFullName(); }
		}
		
		string GetBaseClassFullName()
		{
			IType baseClass = type.DirectBaseTypes.Where(t => t.Kind == TypeKind.Class).FirstOrDefault();
			if (baseClass != null) {
				return baseClass.FullName;
			}
			return String.Empty;
		}
		
		public string AssemblyLocation {
			get { return this.project.OutputAssemblyFullPath; }
		}
		
		public bool IsModelClass()
		{
			if (IsBaseClassMvcController()) {
				return false;
			} else if (IsHttpApplication()) {
				return false;
			} else if (IsVisualBasicClassFromMyNamespace()) {
				return false;
			}
			return true;
		}
		
		bool IsHttpApplication()
		{
			return BaseClassFullName == "System.Web.HttpApplication";
		}
		
		bool IsBaseClassMvcController()
		{
			return BaseClassFullName == "System.Web.Mvc.Controller";
		}
		
		bool IsVisualBasicClassFromMyNamespace()
		{
			if (project.IsVisualBasic()) {
				return FullName.Contains(".My.");
			}
			return false;
		}
	}
}
