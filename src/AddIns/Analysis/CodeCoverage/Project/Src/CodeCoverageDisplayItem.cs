// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
