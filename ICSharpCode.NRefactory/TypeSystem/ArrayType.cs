// Copyright (c) 2010 AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under MIT X11 license (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory.TypeSystem.Implementation;

namespace ICSharpCode.NRefactory.TypeSystem
{
	/// <summary>
	/// Represents an array type.
	/// </summary>
	public class ArrayType : TypeWithElementType
	{
		int dimensions;
		
		public ArrayType(IType elementType, int dimensions) : base(elementType)
		{
			if (dimensions <= 0)
				throw new ArgumentOutOfRangeException("dimensions", dimensions, "dimensions must be positive");
			this.dimensions = dimensions;
		}
		
		public int Dimensions {
			get { return dimensions; }
		}
		
		public override string NameSuffix {
			get {
				if (dimensions == 0)
					return "[]";
				else
					return "[" + new string(',', dimensions-1) + "]";
			}
		}
		
		public override Nullable<bool> IsReferenceType {
			get { return true; }
		}
		
		public override int GetHashCode()
		{
			return unchecked(GetElementType().GetHashCode() * 71681 + dimensions);
		}
		
		public override bool Equals(IType other)
		{
			ArrayType a = other as ArrayType;
			return a != null && elementType.Equals(a.elementType) && a.dimensions == dimensions;
		}
	}
	
	public class ArrayTypeReference : AbstractTypeReference
	{
		ITypeReference elementType;
		int dimensions;
		
		public ArrayTypeReference(ITypeReference elementType, int dimensions)
		{
			if (elementType == null)
				throw new ArgumentNullException("elementType");
			if (dimensions <= 0)
				throw new ArgumentOutOfRangeException("dimensions", dimensions, "dimensions must be positive");
			this.elementType = elementType;
			this.dimensions = dimensions;
		}
		
		public ITypeReference ElementType {
			get { return elementType; }
		}
		
		public int Dimensions {
			get { return dimensions; }
		}
		
		public override IType Resolve(ITypeResolveContext context)
		{
			return new ArrayType(elementType.Resolve(context), dimensions);
		}
		
		public override string ToString()
		{
			return elementType.ToString() + "[" + new string(',', dimensions - 1) + "]";
		}
		
		public static ITypeReference Create(ITypeReference elementType, int dimensions)
		{
			if (elementType is IType)
				return new ArrayType((IType)elementType, dimensions);
			else
				return new ArrayTypeReference(elementType, dimensions);
		}
	}
}
