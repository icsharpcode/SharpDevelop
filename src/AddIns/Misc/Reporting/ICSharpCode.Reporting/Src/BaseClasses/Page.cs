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

using ICSharpCode.Reporting.Interfaces;
using ICSharpCode.Reporting.Interfaces.Export;

namespace ICSharpCode.Reporting.BaseClasses
{
	/// <summary>
	/// Description of Page.
	/// </summary>
	///
	
	public class Page:IExportContainer,IPage
	{
		public Page(IPageInfo pageInfo,Size pageSize)
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
		

		public string Name {get;set;}
		
		
		public System.Drawing.Size Size {get;set;}

		
		public System.Drawing.Point Location {get;set;}
			
		
		public List<IExportColumn> exportedItems;
		
		public List<IExportColumn> ExportedItems {
			get { return exportedItems; }
		}
		
		
		public IExportContainer CreateExportColumn()
		{
			throw new NotImplementedException();
		}
		
		public ICSharpCode.Reporting.Arrange.IArrangeStrategy GetArrangeStrategy()
		{
			throw new NotImplementedException();
		}
		
		public Size DesiredSize {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public Color ForeColor {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public Color BackColor {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		
		public Color FrameColor {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		
		public IExportColumn Parent {
			get {
				return null;
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public bool CanGrow {get;set;}
		
		public bool CanShrink {get;set;}
		
		public Rectangle DisplayRectangle {
			get {
				return new Rectangle(Location,Size);
			}
		}
	}
}
