// <file>
//     <owner name="David Srbecký" email="dsrbecky@post.cz"/>
// </file>

using System;
using System.Collections.Specialized;
using System.Runtime.InteropServices;

using DebuggerInterop.Core;

//TODO: Support for lower bound

namespace DebuggerLibrary
{
	public class ArrayVariable: Variable
	{
		ICorDebugArrayValue corArrayValue;
		uint[] dimensions;


		uint lenght;
		CorElementType corElementType;
		readonly uint rank;

		public uint Lenght {
			get {
				return lenght;
			}
		}

		public string ElementsType { 
			get {
				return CorTypeToString(corElementType); 
			} 
		}

		public uint Rank { 
			get {
				return rank; 
			} 
		}

		public override object Value { 
			get {
				string txt = "{" + ElementsType + "[";
				for (int i = 0; i < rank; i++)
					txt += dimensions[i].ToString() + ",";
				txt = txt.TrimEnd(new char[] {','}) + "]}";
				return txt;
			} 
		}


		internal unsafe ArrayVariable(ICorDebugValue corValue, string name):base(corValue, name)
		{
            corArrayValue = (ICorDebugArrayValue)this.corValue;
			uint corElementTypeRaw;
			corArrayValue.GetElementType(out corElementTypeRaw);
			corElementType = (CorElementType)corElementTypeRaw;

			corArrayValue.GetRank(out rank);
			corArrayValue.GetCount(out lenght);

			dimensions = new uint[rank];
			fixed (void* pDimensions = dimensions)
				corArrayValue.GetDimensions(rank, new IntPtr(pDimensions));
		}


		public Variable this[uint index] {
			get {
				return this[new uint[] {index}];
			}
		}

		public Variable this[uint index1, uint index2] {
			get {
				return this[new uint[] {index1, index2}];
			}
		}

		public Variable this[uint index1, uint index2, uint index3] {
			get {
				return this[new uint[] {index1, index2, index3}];
			}
		}

		public unsafe Variable this[uint[] indices] {
			get {
				if (indices.Length != rank) throw new DebuggerException("Given indicies does not match array size.");

				string elementName = "[";
				for (int i = 0; i < indices.Length; i++)
					elementName += indices[i].ToString() + ",";
				elementName = elementName.TrimEnd(new char[] {','}) + "]";

				ICorDebugValue element;
				fixed (void* pIndices = indices)
					corArrayValue.GetElement(rank, new IntPtr(pIndices), out element);

				return VariableFactory.CreateVariable(element, elementName);
			}
		}


		protected override VariableCollection GetSubVariables()
		{
			VariableCollection subVariables = new VariableCollection();

			uint[] indices = new uint[rank];

			for(;;) // Go thought all combinations
			{
				for (uint i = rank - 1; i >= 1; i--)
					if (indices[i] >= dimensions[i])
					{
						indices[i] = 0;
						indices[i-1]++;
					}
				if (indices[0] >= dimensions[0]) break; // We are done

				subVariables.Add(this[indices]);

				indices[rank - 1]++;
			}

			return subVariables;
		}
	}
}
