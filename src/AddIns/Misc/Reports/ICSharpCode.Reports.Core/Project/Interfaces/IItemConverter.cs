// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Drawing;
using ICSharpCode.Reports.Core.BaseClasses;
using ICSharpCode.Reports.Core.Events;
using ICSharpCode.Reports.Core.Exporter;

namespace ICSharpCode.Reports.Core.Interfaces
{
	/// <summary>
	/// Description of IItemConverter.
	/// </summary>
	public interface IBaseConverter
	{		
		ExporterCollection Convert (BaseReportItem parent,BaseReportItem item);	
		event EventHandler <NewPageEventArgs> PageFull;
		event EventHandler<SectionRenderEventArgs> SectionRendering;
		SectionBounds SectionBounds {get;}
		IDataNavigator DataNavigator {get;}
		Rectangle ParentRectangle {get;}
		ExporterPage SinglePage {get;}
		ILayouter Layouter {get;}
		Graphics Graphics {get;set;}
	}
}
