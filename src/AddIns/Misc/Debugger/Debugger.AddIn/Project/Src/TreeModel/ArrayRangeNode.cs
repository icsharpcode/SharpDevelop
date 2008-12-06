// <file>
//     <copyright license="BSD-new" see="prj:///COPYING"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Text;
using Debugger.Expressions;

namespace Debugger.AddIn.TreeModel
{
	public partial class Utils
	{
		public static IEnumerable<AbstractNode> GetChildNodesOfArray(Expression expression, ArrayDimensions dimensions)
		{
			if (dimensions.TotalElementCount == 0) return new AbstractNode[0];
			
			return new ArrayRangeNode(expression, dimensions, dimensions).ChildNodes;
		}
	}
	
	/// <summary> This is a partent node for all elements within a given bounds </summary>
	public class ArrayRangeNode: AbstractNode
	{
		const int MaxElementCount = 100;
		
		Expression arrayTarget;
		ArrayDimensions bounds;
		ArrayDimensions originalBounds;
		
		public ArrayRangeNode(Expression arrayTarget, ArrayDimensions bounds, ArrayDimensions originalBounds)
		{
			this.arrayTarget = arrayTarget;
			this.bounds = bounds;
			this.originalBounds = originalBounds;
			
			this.Name = GetName();
			this.ChildNodes = GetChildren();
		}
		
		string GetName()
		{
			StringBuilder name = new StringBuilder();
			bool isFirst = true;
			name.Append("[");
			for(int i = 0; i < bounds.Count; i++) {
				if (!isFirst) name.Append(", ");
				isFirst = false;
				ArrayDimension dim = bounds[i];
				ArrayDimension originalDim = originalBounds[i];
				
				if (dim.Count == 0) {
					throw new DebuggerException("Empty dimension");
				} else if (dim.Count == 1) {
					name.Append(dim.LowerBound.ToString());
				} else if (dim.Equals(originalDim)) {
					name.Append("*");
				} else {
					name.Append(dim.LowerBound);
					name.Append("..");
					name.Append(dim.UpperBound);
				}
			}
			name.Append("]");
			return name.ToString();
		}
		
		IEnumerable<AbstractNode> GetChildren()
		{
			// The whole array is small - just add all elements as childs
			if (bounds.TotalElementCount <= MaxElementCount) {
				foreach(Expression childExpr in arrayTarget.AppendIndexers(bounds)) {
					yield return ValueNode.Create(childExpr);
				}
				yield break;
			}
			
			// Find a dimension of size at least 2
			int splitDimensionIndex = bounds.Count - 1;
			for(int i = 0; i < bounds.Count; i++) {
				if (bounds[i].Count > 1) {
					splitDimensionIndex = i;
					break;
				}
			}
			ArrayDimension splitDim = bounds[splitDimensionIndex];
			
			// Split the dimension
			int elementsPerSegment = 1;
			while (splitDim.Count > elementsPerSegment * MaxElementCount) {
				elementsPerSegment *= MaxElementCount;
			}
			for(int i = splitDim.LowerBound; i <= splitDim.UpperBound; i += elementsPerSegment) {
				List<ArrayDimension> newDims = new List<ArrayDimension>(bounds);
				newDims[splitDimensionIndex] = new ArrayDimension(i, Math.Min(i + elementsPerSegment - 1, splitDim.UpperBound));
				yield return new ArrayRangeNode(arrayTarget, new ArrayDimensions(newDims), originalBounds);
			}
			yield break;
		}
	}
}
