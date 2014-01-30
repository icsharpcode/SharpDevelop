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
using ICSharpCode.Core;

namespace ICSharpCode.CodeCoverage
{
	public class CodeCoverageOptions
	{
		public static readonly string OptionsProperty = "CodeCoverage.Options";

		#region Property names
		public static readonly string VisitedColorProperty            = "VisitedColor";
		public static readonly string VisitedForeColorProperty        = "VisitedForeColor";
		public static readonly string PartVisitedColorProperty        = "PartVisitedColor";
		public static readonly string PartVisitedForeColorProperty    = "PartVisitedForeColor";
		public static readonly string NotVisitedColorProperty         = "NotVisitedColor";
		public static readonly string NotVisitedForeColorProperty     = "NotVisitedForeColor";
		
		// This name is also referenced by CodeCoverage.addin
		public static readonly string CodeCoverageHighlightedProperty = "CodeCoverageHighlighted";
		public static readonly string ShowSourceCodePanelProperty     = "ShowSourceCodePanel";
		public static readonly string ShowVisitCountPanelProperty     = "ShowVisitCountPanel";
		#endregion
		
		static Properties properties;

		static CodeCoverageOptions()
		{
			properties = PropertyService.NestedProperties(OptionsProperty);
		}

		public static Properties Properties {
			get { return properties; }
		}
				
		/// <summary>
		/// Enables/disables code coverage highlighting.
		/// </summary>
		public static bool CodeCoverageHighlighted {
			get { return Properties.Get<bool>(CodeCoverageHighlightedProperty, false); }
			set { Properties.Set<bool>(CodeCoverageHighlightedProperty, value); }
		}
		
		/// <summary>
		/// Shows/hides the source code panel in the code coverage pad.
		/// </summary>
		public static bool ShowSourceCodePanel {
			get { return Properties.Get<bool>(ShowSourceCodePanelProperty, false); }
			set { Properties.Set<bool>(ShowSourceCodePanelProperty, value); }
		}

		/// <summary>
		/// Shows/hides the visit count panel in the code coverage pad.
		/// </summary>
		public static bool ShowVisitCountPanel {
			get { return Properties.Get<bool>(ShowVisitCountPanelProperty, true); }
			set { Properties.Set<bool>(ShowVisitCountPanelProperty, value);	}
		}

		/// <summary>
		/// Gets the colour that will be used when highlighting visited code.
		/// </summary>
		public static Color VisitedColor {
			get { return Properties.Get<Color>(VisitedColorProperty, Color.Lime); }
			set { Properties.Set<Color>(VisitedColorProperty, value); }
		}
		
		/// <summary>
		/// Gets the foreground colour that will be used when highlighting
		/// visited code.
		/// </summary>
		public static Color VisitedForeColor {
			get { return Properties.Get<Color>(VisitedForeColorProperty, Color.Black); }
			set { Properties.Set<Color>(VisitedForeColorProperty, value); }
		}
		
		/// <summary>
		/// Gets the colour that will be used when highlighting visited code.
		/// </summary>
		public static Color PartVisitedColor {
			get { return Properties.Get<Color>(PartVisitedColorProperty, Color.Yellow); }
			set { Properties.Set<Color>(PartVisitedColorProperty, value); }
		}
		
		/// <summary>
		/// Gets the foreground colour that will be used when highlighting
		/// visited code.
		/// </summary>
		public static Color PartVisitedForeColor {
			get { return Properties.Get<Color>(PartVisitedForeColorProperty, Color.Black); }
			set { Properties.Set<Color>(PartVisitedForeColorProperty, value); }
		}
		
		/// <summary>
		/// Gets the colour that will be used when highlighting code that has not
		/// been visited.
		/// </summary>
		public static Color NotVisitedColor {
			get { return Properties.Get<Color>(NotVisitedColorProperty, Color.Red); }
			set { Properties.Set<Color>(NotVisitedColorProperty, value); }
		}
		
		/// <summary>
		/// Gets the foreground colour that will be used when highlighting
		/// code that has not been visited.
		/// </summary>
		public static Color NotVisitedForeColor {
			get { return Properties.Get<Color>(NotVisitedForeColorProperty, Color.White); }
			set { Properties.Set<Color>(NotVisitedForeColorProperty, value); }
		}
	}
}
