/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 03.04.2013
 * Time: 20:35
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using ICSharpCode.Reporting.Interfaces;

namespace ICSharpCode.Reporting.BaseClasses
{
	/// <summary>
	/// Description of Page.
	/// </summary>
	/// 
	
	
	
	
	
	public interface IPage{
		bool IsFirstPage {get;set;}
		IPageInfo PageInfo {get;}
		List<IExportColumn> Items {get; set;}
	}
	
	
	
	
	public class Page:IPage
	{
		public Page(IPageInfo pageInfo)
		{
			if (pageInfo == null) {
				throw new ArgumentNullException("pageInfo");
			}
			PageInfo = pageInfo;
			Items = new List<IExportColumn>();
		}
		
		public bool IsFirstPage {get;set;}
		
		
		public IPageInfo PageInfo {get;private set;}
		
		public List<IExportColumn> Items {get; set;}
	}
}
