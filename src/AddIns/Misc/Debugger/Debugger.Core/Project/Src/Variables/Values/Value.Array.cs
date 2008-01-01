// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using Debugger.Wrappers.CorDebug;

using Ast = ICSharpCode.NRefactory.Ast;

//TODO: Support for lower bound

namespace Debugger
{
	// This part of the class provides support for arrays
	public partial class Value
	{
		ICorDebugArrayValue CorArrayValue {
			get {
				if (IsArray) {
					return CorValue.CastTo<ICorDebugArrayValue>();
				} else {
					throw new DebuggerException("Value is not an array");
				}
			}
		}
		
		/// <summary> Returns true if the value is an array </summary>
		public bool IsArray {
			get {
				return !IsNull && this.Type.IsArray;
			}
		}
		
		/// <summary>
		/// Gets the number of elements the array can store.
		/// eg new object[4,5] returns 20
		/// </summary>
		public uint ArrayLenght {
			get {
				return CorArrayValue.Count;
			}
		}
		
		/// <summary>
		/// Gets the number of dimensions of the array.
		/// eg new object[4,5] returns 2
		/// </summary>
		public uint ArrayRank { 
			get {
				return CorArrayValue.Rank;
			}
		}
		
		/// <summary>
		/// Gets the lengths of individual dimensions.
		/// eg new object[4,5] returns {4,5};
		/// </summary>
		public uint[] ArrayDimensions {
			get {
				return CorArrayValue.Dimensions;
			}
		}
		
		/// <summary> Returns an element of a single-dimensional array </summary>
		public Value GetArrayElement(uint index)
		{
			return GetArrayElement(new uint[] {index});
		}
		
		/// <summary> Returns an element of an array </summary>
		public Value GetArrayElement(uint[] elementIndices)
		{
			uint[] indices = (uint[])elementIndices.Clone();
			
			return new Value(
				Process,
				GetNameFromIndices(indices),
				GetExpressionFromIndices(indices),
				new IExpirable[] {this},
				new IMutable[] {this},
				delegate { return GetCorValueOfArrayElement(indices); }
			);
		}
		
		static string GetNameFromIndices(uint[] indices)
		{
			string elementName = "[";
			for (int i = 0; i < indices.Length; i++) {
				elementName += indices[i].ToString() + ",";
			}
			elementName = elementName.TrimEnd(new char[] {','}) + "]";
			return elementName;
		}
		
		Expression GetExpressionFromIndices(uint[] indices)
		{
			List<Ast.Expression> indicesAst = new List<Ast.Expression>();
			foreach(uint indice in indices) {
				indicesAst.Add(new Ast.PrimitiveExpression((int)indice, ((int)indice).ToString()));
			}
			return new Ast.IndexerExpression(
				this.Expression,
				indicesAst
			);
		}
		
		// May be called later
		ICorDebugValue GetCorValueOfArrayElement(uint[] indices)
		{
			if (!IsArray) {
				throw new CannotGetValueException("The value is not an array");
			}
			if (indices.Length != ArrayRank) {
				throw new CannotGetValueException("Given indicies do not have the same dimension as array.");
			}
			for (int i = 0; i < indices.Length; i++) {
				if (indices[i] > ArrayDimensions[i]) {
					throw new CannotGetValueException("Given indices are out of range of the array");
				}
			}
			
			return CorArrayValue.GetElement(indices);
		}
		
		/// <summary> Returns all elements in the array </summary>
		public ValueCollection GetArrayElements()
		{
			return new ValueCollection(GetArrayElementsEnum());
		}
		
		IEnumerable<Value> GetArrayElementsEnum()
		{
			uint[] indices = new uint[ArrayRank];
			uint rank = ArrayRank;
			uint[] dimensions = ArrayDimensions;
			
			while(true) { // Go thought all combinations
				for (uint i = rank - 1; i >= 1; i--) {
					if (indices[i] >= dimensions[i]) {
						indices[i] = 0;
						indices[i-1]++;
					}
				}
				if (indices[0] >= dimensions[0]) break; // We are done
				
				yield return GetArrayElement(indices);
				
				indices[rank - 1]++;
			}
		}
	}
}
