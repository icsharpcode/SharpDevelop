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

namespace ICSharpCode.Reports.Core
{
	/// <summary>
	/// Description of AbstractPage.
	/// </summary>
	public class SinglePage : ISinglePage
	{
		
		private SectionBounds sectionBounds;
		private Hashtable parameterHash;

		public SinglePage(SectionBounds sectionBounds, int pageNumber)
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
		
		
		public int StartRow {get;set;}
	
		
		public int EndRow {get;set;}
		
		
		public int PageNumber {get;set;}
			
		
		public int TotalPages {get;set;}
	
		
		public string ReportName {get;set;}
			
		
		public string ReportFileName {get;set;}
		
	
		public string ReportFolder {
			get{
				return System.IO.Path.GetDirectoryName(this.ReportFileName);
			}
		}
		
			
		
		public DateTime ExecutionTime {get;set;}
			
		
		public Hashtable ParameterHash{
		get{
				if (this.parameterHash == null) {
					this.parameterHash  = new Hashtable();
				}
				return parameterHash;
			}
			set {this.parameterHash = value;}
		}
		
		
		public IDataNavigator IDataNavigator {get;set;}
	}
}
