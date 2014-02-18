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
using ICSharpCode.Reports.Addin.Commands;
using ICSharpCode.Reports.Core;
using ICSharpCode.Reports.Core.Exporter.ExportRenderer;
using ICSharpCode.Reports.Core.Globals;
using ICSharpCode.Reports.Core.WpfReportViewer;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Workbench;

namespace ICSharpCode.Reports.Addin.SecondaryViews
{
	/// <summary>
	/// Description of WPFReportPreview.
	/// </summary>
	public class WPFReportPreview: AbstractSecondaryViewContent
	{
		ReportDesignerLoader designerLoader;
		
		IWpfReportViewer viewer = new WpfReportViewer();
		IExportRunner exportRunner = new ExportRunner();
		
		public WPFReportPreview(ReportDesignerLoader loader,IViewContent content):base(content)
		{
			this.designerLoader = loader;
			base.TabPageText = "Wpf View";
			Pages = new PagesCollection();
		}
		
		
		public PagesCollection Pages{get;private set;}
	
		
		protected override void LoadFromPrimary()
		{
			Pages.Clear();
			ReportModel model = designerLoader.CreateRenderableModel();
	//TODO change to designerLoader.CreateXmlModel();
//			var xmDoc = designerLoader.CreateXmlModel();
//			var modulLoader = new ModelLoader();
//			ReportModel model = (ReportModel)modulLoader.Load(xmDoc.DocumentElement);
			
			var collectCmd = new CollectParametersCommand(model.ReportSettings);
			collectCmd.Run();
			switch (model.DataModel)
			{
					case GlobalEnums.PushPullModel.FormSheet :
					{
						exportRunner.RunReport(model,(ReportParameters)null);
						break;
					}
					case GlobalEnums.PushPullModel.PullData:
					{
						exportRunner.RunReport(model,(ReportParameters)null);
						break;
					}
					case GlobalEnums.PushPullModel.PushData:
					{
						var cmd = new DataSetFromXsdCommand();
						cmd.Run();
						System.Data.DataSet ds = cmd.DataSet;
						exportRunner.RunReport(model,ds.Tables[0],(ReportParameters)null);
						break;
					}
				default:
					throw new InvalidReportModelException();
			}
			PreviewViewModel previewViewModel = new PreviewViewModel (model.ReportSettings,exportRunner.Pages);
			viewer.SetBinding(previewViewModel);
		}
		
		
		protected override void SaveToPrimary()
		{
//			throw new NotImplementedException();
		}
		
		
		public override object Control {
			get {
				return viewer;
			}
		}
	}
}
