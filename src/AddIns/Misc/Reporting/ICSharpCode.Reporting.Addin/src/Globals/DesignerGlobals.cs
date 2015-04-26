/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 24.02.2014
 * Time: 19:55
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Drawing;

namespace ICSharpCode.Reporting.Addin.Globals
{
	/// <summary>
	/// Description of DesignerGlobals.
	/// </summary>
	static class DesignerGlobals{
		
		public static Font DesignerFont
		{
			get {
				return new Font("Microsoft Sans Serif",
				               8,
				               FontStyle.Regular,
				               GraphicsUnit.Point);
			}
		}
		
		public static int GabBetweenSection{
			get {return 15;}
		}
	}
}
