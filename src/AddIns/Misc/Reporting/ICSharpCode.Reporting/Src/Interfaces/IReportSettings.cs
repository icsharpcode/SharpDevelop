/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 27.05.2015
 * Time: 20:15
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.ComponentModel;
using System.Drawing;
using ICSharpCode.Reporting.Globals;

namespace ICSharpCode.Reporting.Interfaces
{
	/// <summary>
	/// Description of IReportSettings.
	/// </summary>
	public interface IReportSettings:IComponent{
	
		string ReportName {get;set;}
		string FileName {get;set;}
		int BottomMargin {get;set;}
		int TopMargin  {get;set;}
		int LeftMargin {get;set;}
		int RightMargin {get;set;}
		Size PageSize {get;}
		bool Landscape {get;set;}
		PushPullModel DataModel {get;set;}
		ParameterCollection ParameterCollection {get;}
		
		SortColumnCollection SortColumnsCollection {get;}
		
		GroupColumnCollection GroupColumnsCollection {get;}
	}
}
