// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Peter Forstmeier" email="peter.forstmeier@t-online.de"/>
//     <version>$Revision$</version>
// </file>

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
				
				if (baseExportColumn != null) {
					ExportContainer container = baseExportColumn as ExportContainer;
					if (container == null) {
						baseExportColumn.DrawItem(graphics);
					} else {
						container.DrawItem(graphics);
						DrawItems(graphics,container.Items);
					}
				}
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
