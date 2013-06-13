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
using ICSharpCode.Reporting.Arrange;
using ICSharpCode.Reporting.Interfaces.Export;

namespace ICSharpCode.Reporting.Interfaces
{
	/// <summary>
	/// Description of IPrintObject.
	/// </summary>
	public interface IReportObject {
		string Name{get;set;}
		Size Size {get;set;}
		Point Location {get;set;}
		Color ForeColor {get;set;}
		Color BackColor {get;set;}
		Color FrameColor{get;set;}
		bool CanGrow {get;set;}
	}
	
	
	public interface IPrintableObject:IReportObject {
		IExportColumn CreateExportColumn();
		IMeasurementStrategy MeasurementStrategy ();
		
	}
	
}
