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
		uint[] dimensions;
		
		uint lenght;
		CorElementType corElementType;
		readonly uint rank;
		
		protected ICorDebugArrayValue CorArrayValue {
			get {
				return this.CorValue.CastTo<ICorDebugArrayValue>();
			}
		}
		
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
		
		
		internal unsafe ArrayValue(NDebugger debugger, PersistentValue pValue):base(debugger, pValue)
		{
			corElementType = (CorElementType)CorArrayValue.ElementType;
			
			rank = CorArrayValue.Rank;
			lenght = CorArrayValue.Count;
			
			dimensions = new uint[rank];
			fixed (void* pDimensions = dimensions)
				CorArrayValue.GetDimensions(rank, new IntPtr(pDimensions));
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
				return GetItem(indices, new PersistentValue(delegate {return this;}));
			}
		}
		
		Variable GetItem(uint[] itemIndices, PersistentValue pValue)
		{
			uint[] indices = (uint[])itemIndices.Clone();
			
			if (indices.Length != rank) throw new DebuggerException("Given indicies does not match array size.");
			
			string elementName = "[";
			for (int i = 0; i < indices.Length; i++)
				elementName += indices[i].ToString() + ",";
			elementName = elementName.TrimEnd(new char[] {','}) + "]";
			
			return new Variable(debugger,
			                    elementName,
			                    new PersistentValue(debugger, delegate { return GetCorValueOfItem(indices, pValue); }));
		}
		
		ICorDebugValue GetCorValueOfItem(uint[] indices, PersistentValue pValue)
		{
			ArrayValue updatedVal = pValue.Value as ArrayValue;
			if (this.IsEquivalentValue(updatedVal)) {
				unsafe {
					fixed (void* pIndices = indices) {
						return updatedVal.CorArrayValue.GetElement(rank, new IntPtr(pIndices));
					}
				}
			} else {
				throw new CannotGetValueException("Value is not array");
			}
		}
		
		public override bool MayHaveSubVariables {
			get {
				return true;
			}
		}
		
		public override IEnumerable<Variable> GetSubVariables(PersistentValue pValue)
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
				
				yield return GetItem(indices, pValue);
				
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
