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
