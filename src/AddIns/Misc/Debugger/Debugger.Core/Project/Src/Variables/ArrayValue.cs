// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Specialized;
using System.Runtime.InteropServices;

using Debugger.Interop.CorDebug;

//TODO: Support for lower bound

namespace Debugger
{
	public class ArrayValue: Value
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

		public override string AsString { 
			get {
				string txt = "{" + ElementsType + "[";
				for (int i = 0; i < rank; i++)
					txt += dimensions[i].ToString() + ",";
				txt = txt.TrimEnd(new char[] {','}) + "]}";
				return txt;
			} 
		}


		internal unsafe ArrayValue(NDebugger debugger, ICorDebugValue corValue, string name):base(debugger, corValue, name)
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


		public Value this[uint index] {
			get {
				return this[new uint[] {index}];
			}
		}

		public Value this[uint index1, uint index2] {
			get {
				return this[new uint[] {index1, index2}];
			}
		}

		public Value this[uint index1, uint index2, uint index3] {
			get {
				return this[new uint[] {index1, index2, index3}];
			}
		}

		public unsafe Value this[uint[] indices] {
			get {
				if (indices.Length != rank) throw new DebuggerException("Given indicies does not match array size.");

				string elementName = "[";
				for (int i = 0; i < indices.Length; i++)
					elementName += indices[i].ToString() + ",";
				elementName = elementName.TrimEnd(new char[] {','}) + "]";

				ICorDebugValue element;
				fixed (void* pIndices = indices)
					corArrayValue.GetElement(rank, new IntPtr(pIndices), out element);

				return ValueFactory.CreateValue(debugger, element, elementName);
			}
		}

		public override bool MayHaveSubVariables {
			get {
				return true;
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
