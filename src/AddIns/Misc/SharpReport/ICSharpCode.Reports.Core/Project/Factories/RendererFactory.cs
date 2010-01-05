// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Peter Forstmeier" email="peter.forstmeier@t-online.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.Reports.Core.Interfaces;

	/// <summary>
	/// This class is a Factory for all Renderer
	/// <see cref="RenderFormSheetReport"></see>
	/// <see cref="RenderDataReport"></see>
	/// </summary>
	/// <remarks>
	/// 	created by - Forstmeier Peter
	/// 	created on - 21.09.2005 14:02:01
	/// </remarks>
	
namespace ICSharpCode.Reports.Core
{
	
	internal static class RendererFactory
	{
		public static AbstractRenderer Create(IReportModel model,IDataManager container) {
			ReportDocument repDocumet = new ReportDocument();
			if (model != null) {
				Layouter layouter = new Layouter();
				switch (model.ReportSettings.ReportType) {
						case GlobalEnums.ReportType.FormSheet :{
							return new RenderFormSheetReport(model,repDocumet,layouter);
						}
						case GlobalEnums.ReportType.DataReport:{
							return new RenderDataReport(model,container,repDocumet,layouter);
						}
				}
			}
			throw  new MissingModelException();
		}
	}
}
