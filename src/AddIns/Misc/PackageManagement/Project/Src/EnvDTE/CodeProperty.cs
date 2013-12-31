// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
