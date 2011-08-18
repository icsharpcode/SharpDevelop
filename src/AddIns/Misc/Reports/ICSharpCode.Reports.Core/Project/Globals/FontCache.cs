// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.Drawing;

namespace ICSharpCode.Reports.Core.Globals
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
