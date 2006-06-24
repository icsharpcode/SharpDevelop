/*
 * Created by SharpDevelop.
 * User: Forstmeier Helmut
 * Date: 20.06.2006
 * Time: 13:07
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;

namespace SharpReportCore {
	/// <summary>
	/// Description of SectionRenderEventArgs.
	/// </summary>
	
	public class SectionRenderEventArgs:SectionEventArgs{
		private int pageNumber;
		private int rowNumber;
		private GlobalEnums.enmSection currentSection;
		
		public SectionRenderEventArgs(BaseSection section,
		                              int pageNumber,int rowNumber,
		                              GlobalEnums.enmSection currentSection):base(section){
		                             
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
		
		
		public SharpReportCore.GlobalEnums.enmSection CurrentSection {
			get {
				return currentSection;
			}
		}
		
	}
}
