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
using ICSharpCode.NRefactory.TypeSystem;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class CodeProperty : CodeElement, global::EnvDTE.CodeProperty
	{
		protected readonly IProperty property;
		
		public CodeProperty()
		{
		}
		
		public CodeProperty(CodeModelContext context, IProperty property)
			: base(context, property)
		{
			this.property = property;
		}
		
		public override global::EnvDTE.vsCMElement Kind {
			get { return global::EnvDTE.vsCMElement.vsCMElementProperty; }
		}
		
		public virtual global::EnvDTE.vsCMAccess Access {
			get { return property.Accessibility.ToAccess(); }
			set { }
		}
		
		public virtual global::EnvDTE.CodeClass Parent {
			get { return new CodeClass(context, property.DeclaringTypeDefinition); }
		}
		
		public virtual global::EnvDTE.CodeElements Attributes {
			get { return GetAttributes(property); }
		}
		
		public virtual global::EnvDTE.CodeTypeRef Type {
			get { return new CodeTypeRef2(context, this, property.ReturnType); }
		}
		
		public virtual global::EnvDTE.CodeFunction Getter {
			get { return GetGetter(); }
		}
		
		CodeFunction GetGetter()
		{
			if (property.CanGet) {
				return new CodeFunction2(context, property.Getter);
			}
			return null;
		}
		
		public virtual global::EnvDTE.CodeFunction Setter {
			get { return GetSetter(); }
		}
		
		CodeFunction GetSetter()
		{
			if (property.CanSet) {
				return new CodeFunction2(context, property.Setter);
			}
			return null;
		}
	}
}
