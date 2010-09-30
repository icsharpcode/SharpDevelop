// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
