// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Drawing;

namespace ICSharpCode.CodeCoverage
{
	/// <summary>
	/// Represents a code coverage display item that can have its colour customised 
	/// (e.g. Visited code and Not visited code.
	/// </summary>
	public class CodeCoverageDisplayItem
	{
		string item = String.Empty;
		string itemBackColorPropertyName = String.Empty;
		string itemForeColorPropertyName = String.Empty;
		Color backColor;
		Color foreColor;
		bool changed;
		
		public CodeCoverageDisplayItem(string item, string itemBackColorPropertyName, Color backColor, string itemForeColorPropertyName, Color foreColor)
		{
			this.item = item;
			this.backColor = backColor;
			this.foreColor = foreColor;
			this.itemBackColorPropertyName = itemBackColorPropertyName;
			this.itemForeColorPropertyName = itemForeColorPropertyName;
		}
		
		/// <summary>
		/// Gets whether any of the colours has changed from their origina values.
		/// </summary>
		public bool HasChanged {
			get {
				return changed;
			}
		}
		
		public override string ToString()
		{
			return item;
		}
		
		public string BackColorPropertyName {
			get {
				return itemBackColorPropertyName;
			}
		}
		
		public Color BackColor {
			get {
				return backColor;
			}
			set {
				if (backColor != value) {
					backColor = value;
					changed = true;
				}
			}
		}
		
		public string ForeColorPropertyName {
			get {
				return itemForeColorPropertyName;
			}
		}
		
		public Color ForeColor {
			get {
				return foreColor;
			}
			set {
				if (foreColor != null) {
					foreColor = value;
					changed = true;
				}
			}
		}
	}
}
