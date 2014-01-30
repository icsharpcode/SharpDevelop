// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using ICSharpCode.Core;
using ICSharpCode.Reports.Core;
using ICSharpCode.Reports.Core.Globals;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Workbench;

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
