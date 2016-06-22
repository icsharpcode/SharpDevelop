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
