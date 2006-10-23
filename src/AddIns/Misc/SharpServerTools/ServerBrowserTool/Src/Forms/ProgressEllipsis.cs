/*
 * User: dickon
 * Date: 23/10/2006
 * Time: 09:01
 * 
 */

using System;
using System.Text;

namespace SharpServerTools.Forms
{
	/// <summary>
	/// Shows progress as a series of ellipses. Not threadsafe.
	/// </summary>
	public class ProgressEllipsis
	{
		int noOfDots;
		StringBuilder currentString;
		int currentValue;
		
		public ProgressEllipsis()
		{
		}
		
		public ProgressEllipsis(int noOfDots)
		{
			this.noOfDots = noOfDots;
			currentString = new StringBuilder();
			currentValue = 0;
		}
		
		public void performStep()
		{
			currentValue++;
			if ((currentValue % noOfDots) == 0) {
				currentValue = 0;
				currentString.Remove(0, currentString.Length-1);
			} else {
				currentString.Append(".");
			}
		}
		
		public string Text {
			get {
				return currentString.ToString();
			}
		}
		
		public int Value {
			set {
				this.currentValue = value;
			}
		}
	}
}
