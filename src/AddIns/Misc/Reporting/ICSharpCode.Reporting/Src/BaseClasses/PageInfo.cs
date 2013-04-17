/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 05.04.2013
 * Time: 19:50
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using ICSharpCode.Reporting.Interfaces.Export;

namespace ICSharpCode.Reporting.BaseClasses
{
	/// <summary>
	/// Description of PageInfo.
	/// </summary>
	/// 

	
	
	public class PageInfo:IPageInfo
	{
		public PageInfo()
		{
		}
		
		public int PageNumber {get;set;}
		
		
		
		public int TotalPages {get;set;}
		
		
		
		public string ReportName {get;set;}
		
		
		public string ReportFileName {get;set;}
		
		
		public string ReportFolder {
			get{
				return System.IO.Path.GetDirectoryName(this.ReportFileName);
			}
		}
		
		public DateTime ExecutionTime {get;set;}
		
	}
}
