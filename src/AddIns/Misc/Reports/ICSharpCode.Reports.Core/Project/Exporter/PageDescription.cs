/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 16.05.2010
 * Time: 19:06
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace ICSharpCode.Reports.Core.Exporter
{
	/// <summary>
	/// Description of PageDescription.
	/// </summary>
	
	
	public class PageDescription:SinglePage
	{
		
		private ReportItemCollection items;

		
		public  PageDescription (SectionBounds sectionBounds,int pageNumber):base(sectionBounds,pageNumber)
		{	
			if (sectionBounds == null) {
				throw new ArgumentNullException("sectionBounds");
			}
			this.SectionBounds = sectionBounds; 
		}
		
		public ReportItemCollection Items
		{
			get {
				if (this.items == null) {
					this.items = new ReportItemCollection();
				}
				return this.items;
			}
		}
		
//		public ExporterCollection Items
//		{
//			get {
//				if (this.items == null) {
//					items = new ExporterCollection();
//				}
//				return items;
//			}
//		}
	}
}
