// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Drawing;

namespace ICSharpCode.Reports.Core.Exporter.ExportRenderer
{
	/// <summary>
	/// Description of StandartRenderer.
	/// </summary>
	
	
	internal class PreviewRenderer:BaseExportRenderer
	{
		
		#region Constructor
		
		public static PreviewRenderer CreateInstance () {
			return new PreviewRenderer();
		}
		
		private PreviewRenderer() :base(){
		}
		
		#endregion
	
		public override void RenderOutput(){
			base.RenderOutput();
			if (this.Graphics != null) {
				BaseExportRenderer.DrawItems(this.Graphics,this.Page.Items);
			} else {
				return;
			}
		}
		
	
		public Graphics Graphics {get;set;}
			
		
		public ExporterPage Page {get;set;}
		
	}
}
