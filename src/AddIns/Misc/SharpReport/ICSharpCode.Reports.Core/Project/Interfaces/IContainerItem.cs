// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Peter Forstmeier" email="peter.forstmeier@t-online.de"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.Reports.Expressions.ReportingLanguage;
using System;
using System.Windows.Forms;
using ICSharpCode.Reports.Core.Exporter;

namespace ICSharpCode.Reports.Core.Interfaces
{
	/// <summary>
	/// Description of IContainerControl.
	/// </summary>
	public interface IContainerItem{

		ReportItemCollection Items {get;}
		BaseReportItem Parent {set;get;}	
	}
	
	
	public interface ITableContainer:IContainerItem
	{
		IDataNavigator DataNavigator {set;}
		IExpressionEvaluatorFacade ExpressionEvaluatorFacade {set;}
		void RenderTable (BaseReportItem parent, SectionBounds sectionBounds, ReportPageEventArgs rpea,ILayouter layouter); 
	}
}
