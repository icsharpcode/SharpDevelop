// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
			ICSharpCode.Reports.Addin.Commands.CollectParametersCommand cmd = new ICSharpCode.Reports.Addin.Commands.CollectParametersCommand(model);
			cmd.Run();
			return  base.SetupStandardRenderer (model,null);
		}
		
		#endregion
	}
}
