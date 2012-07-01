// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class CodeProperty : CodeElement
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
		
		public override vsCMElement Kind {
			get { return vsCMElement.vsCMElementProperty; }
		}
		
		public virtual vsCMAccess Access {
			get { return GetAccess(); }
			set { }
		}
		
		public virtual CodeClass Parent {
			get { return new CodeClass(Property.ProjectContent, Property.DeclaringType); }
		}
		
		public virtual CodeElements Attributes {
			get { return new CodeAttributes(Property); }
		}
		
		public virtual CodeTypeRef Type {
			get { return new CodeTypeRef2(Property.ProjectContent, this, Property.ReturnType); }
		}
		
		public virtual CodeFunction Getter {
			get { return GetGetter(); }
		}
		
		CodeFunction GetGetter()
		{
			if (Property.CanGet) {
				return new CodeGetterFunction(Property);
			}
			return null;
		}
		
		public virtual CodeFunction Setter {
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
