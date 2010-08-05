// Copyright (c) 2010 AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under MIT X11 license (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.NRefactory.TypeSystem.Implementation
{
	/// <summary>
	/// A simple constant value that is independent of the resolve context.
	/// </summary>
	public sealed class SimpleConstantValue : Immutable, IConstantValue
	{
		readonly ITypeReference type;
		readonly object value;
		
		public SimpleConstantValue(ITypeReference type, object value)
		{
			if (type == null)
				throw new ArgumentNullException("type");
			this.type = type;
			this.value = value;
		}
		
		public IType GetValueType(ITypeResolveContext context)
		{
			return type.Resolve(context);
		}
		
		public object GetValue(ITypeResolveContext context)
		{
			if (value is ITypeReference)
				return ((ITypeReference)value).Resolve(context);
			else
				return value;
		}
	}
}
