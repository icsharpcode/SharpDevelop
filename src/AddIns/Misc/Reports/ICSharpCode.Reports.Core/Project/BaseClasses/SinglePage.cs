/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Peter Forstmeier
 * Datum: 02.02.2009
 * Zeit: 13:27
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */
using System;
using System.Collections;
using ICSharpCode.Reports.Core.Interfaces;

namespace ICSharpCode.Reports.Core.BaseClasses
{
	/// <summary>
	/// Description of AbstractPage.
	/// </summary>
	public class SinglePage :PageInfo, ISinglePage
	{
		
		private SectionBounds sectionBounds;
		

		public SinglePage(SectionBounds sectionBounds, int pageNumber):base(pageNumber)
			
		{
			if (sectionBounds == null) {
				throw new ArgumentNullException("sectionBounds");
			}
			if (pageNumber < 0) {
				throw new ArgumentNullException("pageNumber");
			}
			this.sectionBounds = sectionBounds;
			this.PageNumber = pageNumber;
		}


		public void CalculatePageBounds(IReportModel reportModel)
		{
			if (reportModel == null) {
				throw new ArgumentNullException("reportModel");
			}

			sectionBounds.MeasureReportHeader(reportModel.ReportHeader);

			//PageHeader
			this.sectionBounds.MeasurePageHeader(reportModel.PageHeader);

			//PageFooter
			this.sectionBounds.MeasurePageFooter(reportModel.PageFooter);

			//ReportFooter

			this.sectionBounds.MeasureReportFooter(reportModel.ReportFooter);

			this.sectionBounds.MeasureDetailArea();
			
			this.sectionBounds.DetailSectionRectangle = new System.Drawing.Rectangle(reportModel.DetailSection.Location.X,sectionBounds.DetailStart.Y,
			                                                                         reportModel.DetailSection.Size.Width,
			                                                                         reportModel.DetailSection.Size.Height);

		}

		
		public SectionBounds SectionBounds {
			get { return sectionBounds; }
			set { this.sectionBounds = value; }
		}
		
	}
}
