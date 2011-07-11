// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;
using ICSharpCode.Reports.Core;
using ICSharpCode.Reports.Core.Globals;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.Reports.Addin.SecondaryViews
{
	/// <summary>
	/// Description of the view content
	/// </summary>
	internal class ReportPreview : AbstractSecondaryViewContent
	{
		PreviewControl previewControl;
		ReportDesignerLoader designerLoader;
		
		StandartPreviewManager reportManager;
		/// <summary>
		/// Creates a new ReportPreview object
		/// </summary>

		public ReportPreview(ReportDesignerLoader loader,IViewContent content):base(content)
		{
			if (loader == null) {
				throw new ArgumentNullException("loader");
			}
			this.designerLoader = loader;
			base.TabPageText = ResourceService.GetString("SharpReport.Preview");
		}
		
		
		protected override void LoadFromPrimary()
		{
			reportManager = new StandartPreviewManager();
			ReportModel model = designerLoader.CreateRenderableModel();
			AbstractRenderer rc = reportManager.CreateRenderer (model);
			if (rc != null) {
				previewControl.ShowPreview(rc,GlobalValues.DefaultZoomFactor,true);
			}
		}
		
		
		protected override void SaveToPrimary()
		{
//			throw new NotImplementedException();
		}
		
	
		/// <summary>
		/// The <see cref="System.Windows.Forms.Control"/> representing the view
		/// </summary>
		public override object Control {
			get {
				if (this.previewControl == null) {
					previewControl = new PreviewControl();
				}
				return previewControl;
			}
		}
		
		
		/// <summary>
		/// Cleans up all used resources
		/// </summary>
		public sealed override void Dispose()
		{
			try {
				if (this.reportManager != null) {
					this.reportManager.Dispose();
				}
				if (this.previewControl != null) {
					previewControl.Dispose();
				}
				
			} finally {
				base.Dispose();
			}
		}
	}
}
