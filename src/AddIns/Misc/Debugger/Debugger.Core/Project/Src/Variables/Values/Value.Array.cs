// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;

using Debugger.Expressions;
using Debugger.Wrappers.CorDebug;

// TODO: Test non-zero LowerBound
// TODO: Test very large arrays (Length > Int32.MaxValue)

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
		/// Gets the number of elements in the array.
		/// eg new object[4,5] returns 20
		/// </summary>
		public int ArrayLenght {
			get {
				return (int)CorArrayValue.Count;
			}
		}
		
		/// <summary>
		/// Gets the number of dimensions of the array.
		/// eg new object[4,5] returns 2
		/// </summary>
		public int ArrayRank { 
			get {
				return (int)CorArrayValue.Rank;
			}
		}
		
		/// <summary> Gets the dimensions of the array  </summary>
		[Debugger.Tests.ToStringOnly]
		public ArrayDimensions ArrayDimensions {
			get {
				int rank = this.ArrayRank;
				uint[] baseIndicies;
				if (CorArrayValue.HasBaseIndicies() == 1) {
					baseIndicies = CorArrayValue.BaseIndicies;
				} else {
					baseIndicies = new uint[this.ArrayRank];
				}
				uint[] dimensionCounts = CorArrayValue.Dimensions;
				
				List<ArrayDimension> dimensions = new List<ArrayDimension>();
				for(int i = 0; i < rank; i++) {
					dimensions.Add(new ArrayDimension((int)baseIndicies[i], (int)baseIndicies[i] + (int)dimensionCounts[i] - 1));
				}
				
				return new ArrayDimensions(dimensions);
			}
		}
		
		/// <summary> Returns an element of a single-dimensional array </summary>
		public Value GetArrayElement(int index)
		{
			return GetArrayElement(new int[] {index});
		}
		
		/// <summary> Returns an element of an array </summary>
		public Value GetArrayElement(int[] elementIndices)
		{
			int[] indices = (int[])elementIndices.Clone();
			
			return new Value(Process, new ArrayIndexerExpression(this.Expression, indices), GetCorValueOfArrayElement(indices));
		}
		
		// May be called later
		ICorDebugValue GetCorValueOfArrayElement(int[] indices)
		{
			if (IsNull) {
				throw new GetValueException("Null reference");
			}
			if (!IsArray) {
				throw new GetValueException("The value is not an array");
			}
			if (indices.Length != ArrayRank) {
				throw new GetValueException("Given indicies do not have the same dimension as array.");
			}
			if (!this.ArrayDimensions.IsIndexValid(indices)) {
				throw new GetValueException("Given indices are out of range of the array");
			}
			
			return CorArrayValue.GetElement(indices);
		}
		
		/// <summary> Returns all elements in the array </summary>
		public ValueCollection GetArrayElements()
		{
			return new ValueCollection(this.ArrayElements);
		}
		
		/// <summary> Enumerate over all array elements </summary>
		[Debugger.Tests.Ignore]
		public IEnumerable<Value> ArrayElements {
			get {
				foreach(int[] indices in this.ArrayDimensions.Indices) {
					yield return GetArrayElement(indices);
				}
			}
		}
	}
}
