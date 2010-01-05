// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Peter Forstmeier" email="peter.forstmeier@t-online.de"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace ICSharpCode.Reports.Core {
	/// <summary>
	/// Description of SectionRenderEventArgs.
	/// </summary>
	
	public class SectionRenderEventArgs:SectionEventArgs{
		private int pageNumber;
		private int rowNumber;
		private GlobalEnums.ReportSection currentSection;
		
		public SectionRenderEventArgs(BaseSection section,
		                              int pageNumber,int rowNumber,
		                              GlobalEnums.ReportSection currentSection):base(section){
		                             
			this.pageNumber = pageNumber;
			this.currentSection = currentSection;
			this.rowNumber = rowNumber;
		}
		
		public int PageNumber {
			get {
				return pageNumber;
			}
		}
		
		
		public int RowNumber {
			get {
				return rowNumber;
			}
		}
		
		
		public GlobalEnums.ReportSection CurrentSection {
			get {
				return currentSection;
			}
		}
	}
}
