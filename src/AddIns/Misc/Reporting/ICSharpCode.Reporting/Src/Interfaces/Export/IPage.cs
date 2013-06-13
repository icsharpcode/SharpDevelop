/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 11.04.2013
 * Time: 19:58
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;

namespace ICSharpCode.Reporting.Interfaces.Export
{
	/// <summary>
	/// Description of IPage.
	/// </summary>
	public interface IPage:IExportContainer
	{
		bool IsFirstPage {get;set;}
		IPageInfo PageInfo {get;}
	}
}
