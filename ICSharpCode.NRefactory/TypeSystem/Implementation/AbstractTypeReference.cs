// Copyright (c) 2010 AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under MIT X11 license (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;

namespace ICSharpCode.NRefactory.TypeSystem.Implementation
{
	public abstract class AbstractTypeReference : Immutable, ITypeReference
	{
		public abstract IType Resolve(ITypeResolveContext context);
		
		public virtual IType GetBaseType(ITypeResolveContext context)
		{
			return Resolve(context).GetBaseType(context);
		}
		
		public virtual IList<IType> GetNestedTypes(ITypeResolveContext context)
		{
			return Resolve(context).GetNestedTypes(context);
		}

		public virtual IList<IMethod> GetMethods(ITypeResolveContext context)
		{
			return Resolve(context).GetMethods(context);
		}
		
		public virtual IList<IProperty> GetProperties(ITypeResolveContext context)
		{
			return Resolve(context).GetProperties(context);
		}
		
		public virtual IList<IField> GetFields(ITypeResolveContext context)
		{
			return Resolve(context).GetFields(context);
		}
		
		public virtual IList<IEvent> GetEvents(ITypeResolveContext context)
		{
			return Resolve(context).GetEvents(context);
		}
	}
}
