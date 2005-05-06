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
	public sealed class ArrayReturnType : ProxyReturnType
	{
		IReturnType elementType;
		int dimensions;
		
		public ArrayReturnType(IReturnType elementType, int dimensions)
		{
			this.elementType = elementType;
			this.dimensions = dimensions;
		}
		
		public override string FullyQualifiedName {
			get {
				return AppendArrayString(elementType.FullyQualifiedName);
			}
		}
		
		public override string Name {
			get {
				return AppendArrayString(elementType.Name);
			}
		}
		
		public override string DotNetName {
			get {
				return AppendArrayString(elementType.DotNetName);
			}
		}
		
		public override IReturnType BaseType {
			get {
				return ProjectContentRegistry.GetMscorlibContent().GetClass("System.Array").DefaultReturnType;
			}
		}
		
		public override List<IIndexer> GetIndexers()
		{
			IClass arr = ProjectContentRegistry.GetMscorlibContent().GetClass("System.Array");
			IReturnType intRT = ProjectContentRegistry.GetMscorlibContent().GetClass("System.Int32").DefaultReturnType;
			List<IParameter> p = new List<IParameter>();
			for (int i = 0; i < dimensions; ++i) {
				p.Add(new DefaultParameter("index", intRT, null));
			}
			List<IIndexer> l = new List<IIndexer>();
			l.Add(new DefaultIndexer(elementType, p, ModifierEnum.Public, null, null, arr));
			return l;
		}
		
		public override int ArrayDimensions {
			get {
				return dimensions;
			}
		}
		
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
