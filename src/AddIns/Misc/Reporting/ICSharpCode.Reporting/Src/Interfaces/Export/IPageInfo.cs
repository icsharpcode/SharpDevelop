/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 11.04.2013
 * Time: 19:59
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace ICSharpCode.Reporting.Interfaces.Export
{
	/// <summary>
	/// Description of IPageInfo.
	/// </summary>
	public interface IPageInfo
	{
		int PageNumber {get;set;}
		int TotalPages {get;set;}
		string ReportName {get;set;}
		string ReportFileName {get;set;}
		string ReportFolder {get;}
	}
}
