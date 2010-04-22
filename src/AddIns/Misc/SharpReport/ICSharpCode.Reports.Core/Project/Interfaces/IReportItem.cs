// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Peter Forstmeier" email="peter.forstmeier@t-online.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Drawing;
	

namespace ICSharpCode.Reports.Core {
	
	/// <summary>
	/// All ReportItems must implement this Interface
	/// </summary>
	public interface IReportItem   {
		
		string Name{get;set;}
		
		Size Size {get;set;}
		Point Location {get;set;}
		Font Font {get;set;}
		bool Visible {get;set;}
		Color BackColor {get;set;}
		Color FrameColor {get;set;}
		int SectionOffset {get;set;}
		bool CanGrow {get;set;}
		bool CanShrink {get;set;} 
	}
	
	public interface IDataRenderer:IReportItem 
	{
		string ColumnName{
			get;set;
		}
		string BaseTableName{
			get;set;
		}
		
		string MappingName {
			get;
		}
			
	}
}
