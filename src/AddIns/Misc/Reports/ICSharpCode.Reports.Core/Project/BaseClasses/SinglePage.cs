// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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

			sectionBounds.CalculatePageBounds(reportModel);
			this.sectionBounds.DetailSectionRectangle = new System.Drawing.Rectangle(reportModel.DetailSection.Location.X,sectionBounds.DetailArea.Top,
			                                                                         reportModel.DetailSection.Size.Width,
			                                                                         reportModel.DetailSection.Size.Height);
		}

		
		public SectionBounds SectionBounds {
			get { return sectionBounds; }
			set { this.sectionBounds = value; }
		}
		
	}
}
