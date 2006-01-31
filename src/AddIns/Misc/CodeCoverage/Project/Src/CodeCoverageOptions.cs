// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.Core;
using System;
using System.Drawing;

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
		public static readonly string NCoverFileNameProperty          = "NCoverFileName";
		public static readonly string CodeCoverageHighlightedProperty = "CodeCoverageHighlighted";
		#endregion
		
		static Properties properties;

		static CodeCoverageOptions()
 		{
			properties = PropertyService.Get(OptionsProperty, new Properties());
		}

 		public static Properties Properties {
			get {
				return properties;
 			}
		}
		
		/// <summary>
		/// The full path to the NCover console executable.
		/// </summary>
		public static string NCoverFileName {
			get {
				return Properties.Get<String>(NCoverFileNameProperty, String.Empty);
			}
			set {
				Properties.Set<String>(NCoverFileNameProperty, value);
			}
		}
		
		/// <summary>
		/// Enables/disables code coverage highlighting.
		/// </summary>
		public static bool CodeCoverageHighlighted {
			get {
				return Properties.Get<bool>(CodeCoverageHighlightedProperty, false);
			}
			set {
				Properties.Set<bool>(CodeCoverageHighlightedProperty, value);
			}
		}

		/// <summary>
		/// Gets the colour that will be used when highlighting visited code.
		/// </summary>
		public static Color VisitedColor
		{
			get {
				return Properties.Get<Color>(VisitedColorProperty, Color.Lime);
			}
			
			set {
				Properties.Set<Color>(VisitedColorProperty, value);
			}
		}	
		
		/// <summary>
		/// Gets the foreground colour that will be used when highlighting 
		/// visited code.
		/// </summary>
		public static Color VisitedForeColor
		{
			get {
				return Properties.Get<Color>(VisitedForeColorProperty, Color.Black);
			}
			
			set {
				Properties.Set<Color>(VisitedForeColorProperty, value);
			}
		}	
		
		/// <summary>
		/// Gets the colour that will be used when highlighting code that has not
		/// been visited.
		/// </summary>
		public static Color NotVisitedColor
		{
			get {
				return Properties.Get<Color>(NotVisitedColorProperty, Color.Red);
			}
			
			set {
				Properties.Set<Color>(NotVisitedColorProperty, value);
			}
		}	
		
		/// <summary>
		/// Gets the foreground colour that will be used when highlighting 
		/// code that has not been visited.
		/// </summary>
		public static Color NotVisitedForeColor
		{
			get {
				return Properties.Get<Color>(NotVisitedForeColorProperty, Color.White);
			}
			
			set {
				Properties.Set<Color>(NotVisitedForeColorProperty, value);
			}
		}	
	}
}
