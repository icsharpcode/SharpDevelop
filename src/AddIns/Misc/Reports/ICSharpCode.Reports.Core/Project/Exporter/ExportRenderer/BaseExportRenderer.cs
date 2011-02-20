// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Drawing;

namespace ICSharpCode.Reports.Core.Exporter.ExportRenderer{

	/// <summary>
	/// Description of BaseExportRenderer.
	/// </summary>
	/// 
	public class BaseExportRenderer{
		
		PagesCollection pages;
		
		public BaseExportRenderer ()
		{
		}
		
		
		public BaseExportRenderer(PagesCollection pages)
		{
			this.pages = pages;
		}
		
		
		protected static void DrawItems (Graphics graphics,ExporterCollection items)
		{
			foreach (ICSharpCode.Reports.Core.Exporter.BaseExportColumn baseExportColumn in items) {
				baseExportColumn.DrawItem(graphics);
			}
		}
		
		
		public virtual void Start()
		{
		}
		
		
		public virtual void RenderOutput ()
		{
		}
		
		public virtual void End ()
		{
		}
		
		
		public PagesCollection Pages 
		{
			get {if (this.pages == null) {
					this.pages = new PagesCollection();
				}
				return pages;
			}
		}
	}
}
