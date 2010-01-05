// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Peter Forstmeier" email="peter.forstmeier@t-online.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Drawing;

namespace ICSharpCode.Reports.Core.Exporter.ExportRenderer
{
	/// <summary>
	/// Description of StandartRenderer.
	/// </summary>
	
	
	public class PreviewRenderer:BaseExportRenderer
	{
		Graphics graphics;
		ExporterPage page;
		
		#region Constructor
		
		public static PreviewRenderer CreateInstance () {
			return new PreviewRenderer();
		}
		
		private PreviewRenderer() :base(){
		}
		
		#endregion
	
		public override void RenderOutput(){
			base.RenderOutput();
			if (this.graphics != null) {
				BaseExportRenderer.DrawItems(this.graphics,this.page.Items);
			} else {
				return;
			}
		}
		
	
		public Graphics Graphics {
			get { return this.graphics;}
			set { this.graphics = value;}
		}
		
		public ExporterPage Page {
			get {return this.page;}
			set { page = value; }
		}
		
	}
}
