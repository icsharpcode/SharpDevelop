// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Peter Forstmeier" email="peter.forstmeier@t-online.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Drawing;
using System.Windows.Forms;

using ICSharpCode.Reports.Core.Exporter;
using ICSharpCode.Reports.Expressions.ReportingLanguage;

namespace ICSharpCode.Reports.Core.Interfaces
{
	/// <summary>
	/// Description of IContainerControl.
	/// </summary>
	public interface ISimpleContainer{

		ReportItemCollection Items {get;}
		BaseReportItem Parent {set;get;}
		Point Location {set;get;}
		Size Size {get;set;}
	}
	
	
	public interface ITableContainer:ISimpleContainer
	{
		IDataNavigator DataNavigator {set;}
		IExpressionEvaluatorFacade ExpressionEvaluatorFacade {set;}
		void StartLayoutAt (BaseSection section);         
		void RenderTable (BaseReportItem parent, SectionBounds sectionBounds, ReportPageEventArgs rpea,ILayouter layouter); 
	}
}
