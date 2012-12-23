// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class CodeProperty : CodeElement, global::EnvDTE.CodeProperty
	{
		public CodeProperty()
		{
		}
		
		public CodeProperty(IProperty property)
			: base(property)
		{
			this.Property = property;
		}
		
		protected IProperty Property { get; private set; }
		
		public override global::EnvDTE.vsCMElement Kind {
			get { return global::EnvDTE.vsCMElement.vsCMElementProperty; }
		}
		
		public virtual global::EnvDTE.vsCMAccess Access {
			get { return GetAccess(); }
			set { }
		}
		
		public virtual global::EnvDTE.CodeClass Parent {
			get { return new CodeClass(Property.ProjectContent, Property.DeclaringType); }
		}
		
		public virtual global::EnvDTE.CodeElements Attributes {
			get { return new CodeAttributes(Property); }
		}
		
		public virtual global::EnvDTE.CodeTypeRef Type {
			get { return new CodeTypeRef2(Property.ProjectContent, this, Property.ReturnType); }
		}
		
		public virtual global::EnvDTE.CodeFunction Getter {
			get { return GetGetter(); }
		}
		
		CodeFunction GetGetter()
		{
			if (Property.CanGet) {
				return new CodeGetterFunction(Property);
			}
			return null;
		}
		
		public virtual global::EnvDTE.CodeFunction Setter {
			get { return GetSetter(); }
		}
		
		CodeFunction GetSetter()
		{
			if (Property.CanSet) {
				return new CodeSetterFunction(Property);
			}
			return null;
		}
	}
}
