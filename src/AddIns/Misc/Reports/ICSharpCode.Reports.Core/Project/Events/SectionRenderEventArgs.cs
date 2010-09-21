// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.Reports.Core {
	/// <summary>
	/// Description of SectionRenderEventArgs.
	/// </summary>
	
	public class SectionRenderEventArgs:SectionEventArgs{
		private int pageNumber;
		private int rowNumber;
		private BaseSection currentSection;
		
		public SectionRenderEventArgs(BaseSection section,
		                              int pageNumber,int rowNumber,
		                              BaseSection currentSection):base(section){
		                             
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
		
		
		public BaseSection CurrentSection {
			get {
				return currentSection;
			}
		}
	}
}
