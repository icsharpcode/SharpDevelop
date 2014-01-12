// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)
using System;
using System.Drawing.Drawing2D;

namespace ICSharpCode.Reporting.Items
{
	/// <summary>
	/// Description of BaseGraphics.
	/// </summary>
	public class BaseGraphics:PrintableItem
	{
		public BaseGraphics()
		{
			this.Thickness = 1;
			DashStyle = DashStyle.Solid;
		}
		
		
		public virtual int Thickness {get;set;}
		
		public virtual DashStyle DashStyle {get;set;}	
	}
}
