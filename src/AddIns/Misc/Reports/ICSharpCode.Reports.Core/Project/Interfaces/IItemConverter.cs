// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Drawing;
using ICSharpCode.Reports.Core.BaseClasses;
using ICSharpCode.Reports.Core.Events;
using ICSharpCode.Reports.Core.Exporter;
using ICSharpCode.Reports.Expressions.ReportingLanguage;

namespace ICSharpCode.Reports.Core.Interfaces
{
	/// <summary>
	/// Description of IItemConverter.
	/// </summary>
	/// 
	public interface IRenderer
	{
		event EventHandler <NewPageEventArgs> PageFull;
		event EventHandler<SectionRenderEventArgs> SectionRendering;
		
		SectionBounds SectionBounds {get;}
		IDataNavigator DataNavigator {get;}
		Rectangle ParentRectangle {get;}
		ISinglePage SinglePage {get;}
		ILayouter Layouter {get;}
		Graphics Graphics {get;set;}
	}
		
	public interface IBaseRenderer
	{
		void Render(ISimpleContainer container,ReportPageEventArgs rpea,IExpressionEvaluatorFacade evaluator);
	}
		
	
	public interface IBaseConverter:IRenderer
	{		
		ExporterCollection Convert (BaseReportItem parent,BaseReportItem item);
		Point CurrentPosition {get;set;}
		event EventHandler<GroupHeaderEventArgs> GroupHeaderRendering;
		event EventHandler<RowRenderEventArgs> RowRendering;
		event EventHandler<GroupFooterEventArgs> GroupFooterRendering;
	}
}
