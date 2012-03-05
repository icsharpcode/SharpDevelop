// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the BSD license (for details please see \src\AddIns\Debugger\Debugger.AddIn\license.txt)

using System;
using System.Collections.Generic;
using System.Text;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.SharpDevelop.Debugging;

namespace Debugger.AddIn.TreeModel
{
	public partial class Utils
	{
		const int MaxElementCount = 100;
		
		public static TreeNode GetArrayRangeNode(ExpressionNode expr, ArrayDimensions bounds, ArrayDimensions originalBounds)
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
					name.Append("-");
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
			
			return new TreeNode(name.ToString(), () => GetChildNodesOfArray(expr, bounds, originalBounds));
		}
		
		public static IEnumerable<TreeNode> GetChildNodesOfArray(ExpressionNode arrayTarget, ArrayDimensions bounds, ArrayDimensions originalBounds)
		{
			if (bounds.TotalElementCount == 0)
			{
				yield return new TreeNode("(empty)", null);
				yield break;
			}
			
			// The whole array is small - just add all elements as childs
			if (bounds.TotalElementCount <= MaxElementCount) {
				foreach(int[] indices in bounds.Indices) {
					StringBuilder sb = new StringBuilder(indices.Length * 4);
					sb.Append("[");
					bool isFirst = true;
					foreach(int index in indices) {
						if (!isFirst) sb.Append(", ");
						sb.Append(index.ToString());
						isFirst = false;
					}
					sb.Append("]");
					int[] indicesCopy = indices;
					yield return new ExpressionNode("Icons.16x16.Field", sb.ToString(), () => arrayTarget.Evaluate().GetArrayElement(indicesCopy));
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
				yield return GetArrayRangeNode(arrayTarget, new ArrayDimensions(newDims), originalBounds);
			}
			yield break;
		}
	}
}
