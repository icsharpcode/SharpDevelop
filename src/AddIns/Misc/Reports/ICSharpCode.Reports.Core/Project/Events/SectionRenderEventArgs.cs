// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.Reports.Core {
	/// <summary>
	/// Description of SectionRenderEventArgs.
	/// </summary>
	
	public class GroupFooterEventArgs: EventArgs
	{
		public GroupFooterEventArgs (GroupFooter groupFooter)
		{
			GroupFooter = groupFooter;
		}
		
		public GroupFooter GroupFooter {get; private set;}
		
	}
	
	
	
	
	public class GroupHeaderEventArgs: EventArgs
	{
		public GroupHeaderEventArgs (GroupHeader groupHeader)
		{
			GroupHeader = groupHeader;
		}
		
		public GroupHeader GroupHeader {get; private set;}
		
	}
	
	
	public class RowRenderEventArgs : EventArgs
	{
		public RowRenderEventArgs (BaseRowItem row,object rowData)
		{
			Row = row;	
			RowData = rowData;
		}
		
		public BaseRowItem Row {get; private set;}
		public object RowData {get; private set;}
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
