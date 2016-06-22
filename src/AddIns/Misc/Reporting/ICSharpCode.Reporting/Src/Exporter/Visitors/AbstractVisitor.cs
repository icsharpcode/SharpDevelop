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
using System.Collections.ObjectModel;
using System.Drawing;

using ICSharpCode.Reporting.Interfaces.Export;
using ICSharpCode.Reporting.PageBuilder.ExportColumns;

namespace ICSharpCode.Reporting.Exporter.Visitors
{
	/// <summary>
	/// Description of AbstractVisitor.
	/// </summary>
	/// 
	class AbstractVisitor : IVisitor{

		public virtual void Run (Collection<ExportPage> pages) {
			if (pages == null)
				throw new ArgumentNullException("pages");
			Pages = pages;
			foreach (var page in pages) {
				Visit(page);
			}
		}
		
		
		public virtual void Visit (ExportPage page) {
			
			foreach (var element in page.ExportedItems) {
				var ac = element as IAcceptor;
				ac.Accept(this);
			}
		}
		
		
		public virtual void Visit (ExportContainer exportContainer) {
			foreach (var element in exportContainer.ExportedItems) {
				
				var ac = element as IAcceptor;
				ac.Accept(this);
			}
		}
		
		
		public virtual void Visit(ExportText exportColumn){
		}
		
		public virtual void Visit (ExportRow exportRow) {
			
		}
		
		public virtual void Visit (ExportImage exportImage) {
			
		}
		
		public virtual void Visit(ExportLine exportGraphics){
		}
		
		public virtual void Visit (ExportRectangle exportRectangle) {	
		}
		
		
		public virtual void Visit (ExportCircle exportCircle) {	
		}
		
		
		protected static bool ShouldSetBackcolor (ExportColumn exportColumn) {
			return exportColumn.BackColor != Color.White;
		}
		
		protected static bool HasFrame (ExportColumn exportColummn) {
			return exportColummn.DrawBorder;
		}
		
		protected Collection<ExportPage> Pages {get; private set;}
		
		
		static protected bool IsContainer (IExportColumn column) {
			return (column is ExportContainer)|| (column is GraphicsContainer);
		}
		
		
		static protected bool IsGraphicsContainer (IExportColumn column) {
			return column is GraphicsContainer;
		}
		
		static protected IAcceptor AsAcceptor (IExportColumn element) {
			return element as IAcceptor;
		}
	}
}
