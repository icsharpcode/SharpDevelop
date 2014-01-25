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
using ICSharpCode.Reports.Addin.Commands;
using ICSharpCode.Reports.Core;
using ICSharpCode.Reports.Core.Globals;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Workbench;

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
