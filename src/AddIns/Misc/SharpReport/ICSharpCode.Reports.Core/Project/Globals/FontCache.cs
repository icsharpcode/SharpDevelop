/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Peter Forstmeier
 * Datum: 07.09.2008
 * Zeit: 19:22
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */
using System;
using System.Collections;
using System.Drawing;

namespace ICSharpCode.Reports.Core
{
	/// <summary>
	/// Description of FontCache.
	/// </summary>
	public class FontCache:IDisposable
	{
		Hashtable Fonts=new Hashtable();
		
		public FontCache()
		{
		}

		
		protected virtual Font GetFont(string family, float size, FontStyle style)
		{
			string s=family+size.ToString(System.Globalization.CultureInfo.InvariantCulture)+((int)style).ToString(System.Globalization.CultureInfo.InvariantCulture);
			if(Fonts.Contains(s))

				return (Font)Fonts[s];
			Font f=new Font(family,size,style);
			Fonts.Add(s,f);
			return f;
		}

		
		public void Dispose ()
		{
			this.Dispose(true);
		}
		

		public void Dispose(bool disposing)
		{
			foreach(Font f in Fonts.Values)
				f.Dispose();
			Fonts.Clear();
		}
	}
}
