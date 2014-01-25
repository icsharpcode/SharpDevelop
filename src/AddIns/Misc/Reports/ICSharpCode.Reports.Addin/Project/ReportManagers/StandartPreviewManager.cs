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
using System.Data;
using ICSharpCode.Reports.Core;
using ICSharpCode.Reports.Core.Globals;

namespace ICSharpCode.Reports.Addin
{
	internal class StandartPreviewManager:BaseManager
	{
		
		#region Constructor
		
		public StandartPreviewManager () :base()
		{
		}
		
		#endregion
		
		
		public  AbstractRenderer CreateRenderer (ReportModel model)
		{
			if (model == null) {
				throw new ArgumentNullException("model");
			}
			AbstractRenderer abstractRenderer = null;
			switch (model.DataModel) {
					case GlobalEnums.PushPullModel.FormSheet : {
						abstractRenderer = StandartReportRenderer(model);
						break;
					}
					case GlobalEnums.PushPullModel.PullData:{
						abstractRenderer = StandartReportRenderer(model);
						break;
					}
					case GlobalEnums.PushPullModel.PushData:{
						abstractRenderer = PushDataRenderer(model);
						break;
					}
			}
			return abstractRenderer;
		}
		
		
		#region PushDataModel
		
		private AbstractRenderer PushDataRenderer(ReportModel model)
		{
			ICSharpCode.Reports.Addin.Commands.DataSetFromXsdCommand cmd =
				new ICSharpCode.Reports.Addin.Commands.DataSetFromXsdCommand();
			cmd.Run();
			DataSet ds = cmd.DataSet;
			if ( ds != null) {
				return  base.SetupPushDataRenderer(model,ds.Tables[0]);
			}
			return null;
		}
		
		#endregion
		
		#region PullDataModel	
		
		private AbstractRenderer StandartReportRenderer(ReportModel model)
		{		
			ICSharpCode.Reports.Addin.Commands.CollectParametersCommand cmd = new ICSharpCode.Reports.Addin.Commands.CollectParametersCommand(model.ReportSettings);
			cmd.Run();
			return  base.SetupStandardRenderer (model,null);
		}
		
		#endregion
	}
}
