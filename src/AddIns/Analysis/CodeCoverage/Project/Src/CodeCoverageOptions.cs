// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
			properties = PropertyService.Get(OptionsProperty, new Properties());
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
