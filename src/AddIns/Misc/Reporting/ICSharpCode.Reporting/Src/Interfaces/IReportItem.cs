/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 06.04.2013
 * Time: 19:55
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Drawing;

namespace ICSharpCode.Reporting.Interfaces
{
	/// <summary>
	/// Description of IPrintObject.
	/// </summary>
	public interface IReportItem
	{
		string Name{get;set;}
		Size Size {get;set;}
		Point Location {get;set;}
//		Font Font {get;set;}
////		bool VisibleInReport {get;set;}
//		Color BackColor {get;set;}
//		Color FrameColor {get;set;}
//		int SectionOffset {get;set;}
//		bool CanGrow {get;set;}
//		bool CanShrink {get;set;} 
IExportColumn CreateExportColumn();
	}
}
