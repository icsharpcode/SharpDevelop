// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.Reports.Core {
	/// <summary>
	/// Description of SectionRenderEventArgs.
	/// </summary>
	
	
	public class GroupHeaderEventArgs: EventArgs
	{
		public GroupHeaderEventArgs (BaseGroupedRow groupHeader)
		{
			GroupHeader = groupHeader;
		}
		
		public BaseGroupedRow GroupHeader {get; private set;}
		
	}
	
	
	public class RowRenderEventArgs : EventArgs
	{
		public RowRenderEventArgs (BaseRowItem row)
		{
			Row = row;	
		}
		
		BaseRowItem Row {get;set;}
	}
	
	
	
	public class SectionRenderEventArgs:SectionEventArgs{
		
		
		public SectionRenderEventArgs(BaseSection section,
		                              int pageNumber,int rowNumber,
		                              BaseSection currentSection):base(section){
		                             
			this.PageNumber = pageNumber;
			this.CurrentSection = currentSection;
			this.RowNumber = rowNumber;
		}
		
		public int PageNumber {get; private set;}
		
		
		public int RowNumber {get; private set;}
		
		
		public BaseSection CurrentSection {get; private set;}
		
	}
}
