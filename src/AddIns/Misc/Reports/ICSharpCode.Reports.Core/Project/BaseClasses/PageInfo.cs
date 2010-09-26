/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 25.09.2010
 * Time: 19:42
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections;
using ICSharpCode.Reports.Core.Interfaces;

namespace ICSharpCode.Reports.Core.BaseClasses
{
	/// <summary>
	/// Description of PageInfo.
	/// </summary>
	public class PageInfo:IPageInfo
	{
		
		private Hashtable parameterHash;
		
		
		public PageInfo(int pageNumber)
		{
			this.PageNumber = pageNumber;
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
			
		
		public Hashtable ParameterHash{
		get{
				if (this.parameterHash == null) {
					this.parameterHash  = new Hashtable();
				}
				return parameterHash;
			}
			set {this.parameterHash = value;}
		}
		
		
		public IDataNavigator IDataNavigator {get;set;}
	}
}
