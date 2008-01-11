// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace Debugger
{
	/// <summary>
	/// Specifies the range of valid indicies for an array dimension
	/// </summary>
	public class ArrayDimension
	{
		int lowerBound;
		int upperBound;
		
		/// <summary> The smallest valid index in this dimension </summary>
		public int LowerBound {
			get { return lowerBound; }
		}
		
		/// <summary> The largest valid index in this dimension.  
		/// Returns LowerBound - 1 if the array is empty. </summary>
		public int UpperBound {
			get { return upperBound; }
		}
		
		/// <summary> The number of valid indicies of this dimension </summary>
		public int Count {
			get { return upperBound - lowerBound + 1; }
		}
		
		/// <summary> Determines whether the given index is a valid index for this dimension </summary>
		public bool IsIndexValid(int index)
		{
			return (this.LowerBound <= index && index <= this.UpperBound);
		}
		
		public ArrayDimension(int lowerBound, int upperBound)
		{
			this.lowerBound = lowerBound;
			this.upperBound = upperBound;
		}
		
		public override string ToString()
		{
			if (this.LowerBound == 0) {
				return this.Count.ToString();
			} else {
				return this.LowerBound + ".." + this.UpperBound;
			}
		}
	}
}
