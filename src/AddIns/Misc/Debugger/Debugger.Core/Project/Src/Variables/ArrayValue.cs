// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Runtime.InteropServices;

using Debugger.Wrappers.CorDebug;

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
		
		
		internal unsafe ArrayValue(NDebugger debugger, ICorDebugValue corValue):base(debugger, corValue)
		{
			corArrayValue = this.corValue.CastTo<ICorDebugArrayValue>();
			corElementType = (CorElementType)corArrayValue.ElementType;
			
			rank = corArrayValue.Rank;
			lenght = corArrayValue.Count;
			
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
		
		public Variable this[uint[] indices] {
			get {
				return GetItem(indices, delegate {return this;});
			}
		}
		
		Variable GetItem(uint[] indices, ValueGetter getter)
		{
			if (indices.Length != rank) throw new DebuggerException("Given indicies does not match array size.");
			
			string elementName = "[";
			for (int i = 0; i < indices.Length; i++)
				elementName += indices[i].ToString() + ",";
			elementName = elementName.TrimEnd(new char[] {','}) + "]";
			
			return new Variable(debugger,
			                    elementName,
			                    delegate {
			                    	ArrayValue updatedVal = getter() as ArrayValue;
			                    	if (this.IsEquivalentValue(updatedVal)) {
			                    		ICorDebugValue element;
			                    		unsafe {
			                    			fixed (void* pIndices = indices) {
			                    				element = updatedVal.corArrayValue.GetElement(rank, new IntPtr(pIndices));
			                    			}
			                    		}
			                    		return Value.CreateValue(debugger, element);
			                    	} else {
			                    		return new UnavailableValue(debugger, "Value is not array");
			                    	}
			                    });
		}
		
		public override bool MayHaveSubVariables {
			get {
				return true;
			}
		}
		
		public override IEnumerable<Variable> GetSubVariables(ValueGetter getter)
		{
			uint[] indices = new uint[rank];
			
			while(true) { // Go thought all combinations
				for (uint i = rank - 1; i >= 1; i--)
					if (indices[i] >= dimensions[i])
					{
						indices[i] = 0;
						indices[i-1]++;
					}
				if (indices[0] >= dimensions[0]) break; // We are done
				
				yield return GetItem(indices, getter);
				
				indices[rank - 1]++;
			}
		}
		
		public override bool IsEquivalentValue(Value val)
		{
			ArrayValue arrayVal = val as ArrayValue;
			return arrayVal != null &&
			       arrayVal.ElementsType == this.ElementsType &&
			       arrayVal.Lenght == this.Lenght &&
			       arrayVal.Rank == this.Rank;
		}
	}
}
