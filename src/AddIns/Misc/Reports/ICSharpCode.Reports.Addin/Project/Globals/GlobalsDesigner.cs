/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Peter
 * Datum: 18.01.2009
 * Zeit: 11:41
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */
using System;
using System.Drawing;
using ICSharpCode.Reports.Core;

namespace ICSharpCode.Reports.Addin
{
	/// <summary>
	/// Description of GlobalsDesigner.
	/// </summary>
	public sealed class GlobalsDesigner
	{
		private GlobalsDesigner()
		{
		}
		
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
