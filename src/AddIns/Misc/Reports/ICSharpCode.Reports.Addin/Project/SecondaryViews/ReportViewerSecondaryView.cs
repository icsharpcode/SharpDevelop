// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;
using ICSharpCode.Reports.Addin.Commands;
using ICSharpCode.Reports.Core;
using ICSharpCode.Reports.Core.Globals;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.Reports.Addin.SecondaryViews
{
	/// <summary>
	/// Description of ReportViewerSecondaryView.
	/// </summary>
	public class ReportViewerSecondaryView: AbstractSecondaryViewContent
	{
		ReportDesignerLoader designerLoader;
		ICSharpCode.Reports.Core.ReportViewer.PreviewControl control;

		public ReportViewerSecondaryView(ReportDesignerLoader designerLoader,IViewContent content):base(content)
		{
			if (designerLoader == null) {
				throw new ArgumentNullException("designerLoader");
			}
			if (content == null) {
				throw new ArgumentNullException("content");
			}
			this.designerLoader = designerLoader;
			this.control = new ICSharpCode.Reports.Core.ReportViewer.PreviewControl();
			this.control.Messages = new ReportViewerMessages();
			this.control.PreviewLayoutChanged += OnPreviewLayoutChanged;
			base.TabPageText = ResourceService.GetString("SharpReport.ReportViewer");
		}
		
		
		private void OnPreviewLayoutChanged (object sender, EventArgs e)
		{
			LoadFromPrimary();
		}
		
		#region overrides
		
		protected override void LoadFromPrimary()
		{
			ReportModel model = designerLoader.CreateRenderableModel();
			AbstractPreviewCommand cmd = null;
		
			switch (model.DataModel) {
					case GlobalEnums.PushPullModel.FormSheet : {
						cmd = new FormSheetToReportViewerCommand (model,control);
						break;
					}
					case GlobalEnums.PushPullModel.PullData:{
						cmd = new PullModelToReportViewerCommand(model,control);					
						break;
					}
					case GlobalEnums.PushPullModel.PushData:{
						cmd = new PushModelToReportViewerCommand(model,control);						
						break;
					}
				default:
					throw new InvalidReportModelException();
			}
			cmd.Run();
		}
		
		
		protected override void SaveToPrimary()
		{
//			throw new NotImplementedException();
		}
		
		#endregion
		
		
		
		public override object Control {
			get {
				return this.control;
			}
		}
		
	}
}
