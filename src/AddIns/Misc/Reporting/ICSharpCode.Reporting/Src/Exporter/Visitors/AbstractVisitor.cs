/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 28.04.2013
 * Time: 19:49
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.ObjectModel;
using System.Drawing;

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
		
		
		public virtual void Visit(ExportText exportColumn)
		{
		}
		
		
		public virtual void Visit(ExportLine exportGraphics)
		{
		}
		
		public virtual void Visit (ExportRectangle exportRectangle) {
			
		}
		
		
		public virtual void Visit (ExportCircle exportCircle) {
			
		}
		
		
		protected bool ShouldSetBackcolor (ExportColumn exportColumn) {
			return exportColumn.BackColor != Color.White;
		}
		
		
		protected Collection<ExportPage> Pages {get; private set;}
		
		
	}
}
