// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version value="$version"/>
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
		
		public IReturnType ElementType {
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
		
		public override List<IIndexer> GetIndexers()
		{
			List<IParameter> p = new List<IParameter>();
			for (int i = 0; i < dimensions; ++i) {
				p.Add(new DefaultParameter("index", ReflectionReturnType.Int, null));
			}
			List<IIndexer> l = new List<IIndexer>();
			l.Add(new DefaultIndexer(elementType, p, ModifierEnum.Public, null, null, BaseType.GetUnderlyingClass()));
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
