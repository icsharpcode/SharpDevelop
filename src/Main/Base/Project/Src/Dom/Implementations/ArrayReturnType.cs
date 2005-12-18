// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Text;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Dom
{
	/// <summary>
	/// The ArrayReturnType wraps around another type, converting it into an array
	/// with the specified number of dimensions.
	/// The element type is only used as return type for the indexer; all methods and fields
	/// are retrieved from System.Array.
	/// </summary>
	public sealed class ArrayReturnType : ProxyReturnType
	{
		IReturnType elementType;
		int dimensions;
		
		public ArrayReturnType(IReturnType elementType, int dimensions)
		{
			if (dimensions <= 0)
				throw new ArgumentOutOfRangeException("dimensions", dimensions, "dimensions must be positive");
			if (elementType == null)
				throw new ArgumentNullException("elementType");
			this.elementType = elementType;
			this.dimensions = dimensions;
		}
		
		public override bool Equals(object o)
		{
			IReturnType rt = o as IReturnType;
			if (rt == null) return false;
			if (rt.ArrayDimensions != dimensions) return false;
			return elementType.Equals(rt.ArrayElementType);
		}
		
		public override int GetHashCode()
		{
			unchecked {
				return 2 * elementType.GetHashCode() + 27 * dimensions;
			}
		}
		
		public override IReturnType ArrayElementType {
			get {
				return elementType;
			}
		}
		
		public override string FullyQualifiedName {
			get {
				return elementType.FullyQualifiedName;
			}
		}
		
		public override string Name {
			get {
				return elementType.Name;
			}
		}
		
		public override string DotNetName {
			get {
				return AppendArrayString(elementType.DotNetName);
			}
		}
		
		public override IReturnType BaseType {
			get {
				return ReflectionReturnType.Array;
			}
		}
		
		/// <summary>
		/// Indexer used exclusively for array return types
		/// </summary>
		public class ArrayIndexer : DefaultProperty
		{
			public ArrayIndexer(IReturnType elementType)
				: base("Indexer", elementType, ModifierEnum.Public, DomRegion.Empty, DomRegion.Empty, ReflectionReturnType.Array.GetUnderlyingClass())
			{
				IsIndexer = true;
			}
		}
		
		public override List<IProperty> GetProperties()
		{
			List<IProperty> l = base.GetProperties();
			ArrayIndexer property = new ArrayIndexer(elementType);
			for (int i = 0; i < dimensions; ++i) {
				property.Parameters.Add(new DefaultParameter("index", ReflectionReturnType.Int, DomRegion.Empty));
			}
			l.Add(property);
			return l;
		}
		
		public override int ArrayDimensions {
			get {
				return dimensions;
			}
		}
		
		public override bool IsDefaultReturnType {
			get {
				return false;
			}
		}
		
		/// <summary>
		/// Appends the array characters ([,,,]) to the string <paramref name="a"/>.
		/// </summary>
		string AppendArrayString(string a)
		{
			StringBuilder b = new StringBuilder(a, a.Length + 1 + dimensions);
			b.Append('[');
			for (int i = 1; i < dimensions; ++i) {
				b.Append(',');
			}
			b.Append(']');
			return b.ToString();
		}
		
		public override string ToString()
		{
			return String.Format("[ArrayReturnType: {0}, dimensions={1}]", elementType, AppendArrayString(""));
		}
	}
}
