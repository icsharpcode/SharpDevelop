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
using System.Collections.Generic;
using System.Drawing;

using ICSharpCode.Reporting.Exporter.Visitors;
using ICSharpCode.Reporting.Interfaces.Export;

namespace ICSharpCode.Reporting.PageBuilder.ExportColumns
{
	/// <summary>
	/// Description of Page.
	/// </summary>
	///
	
	public class ExportPage:ExportColumn,IPage,IAcceptor
	{
		public ExportPage(IPageInfo pageInfo,Size pageSize):base()
		{
			if (pageInfo == null) {
				throw new ArgumentNullException("pageInfo");
			}
			PageInfo = pageInfo;
			Name = "Page";
			Size = pageSize;
			exportedItems = new List<IExportColumn>();
		}
		
		
		public bool IsFirstPage {get;set;}
		
		
		public IPageInfo PageInfo {get;private set;}
	
		
		public bool CanShrink {get;set;}
		
		
		public void Accept(IVisitor visitor)
		{
			visitor.Visit(this as ExportPage);
		}
		
		
		List<IExportColumn> exportedItems;
		
		public List<IExportColumn> ExportedItems {
			get { return exportedItems; }
		}
	}
}
