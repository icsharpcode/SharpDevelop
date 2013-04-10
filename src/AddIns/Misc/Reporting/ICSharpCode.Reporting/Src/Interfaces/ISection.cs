/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 08.04.2013
 * Time: 19:50
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using ICSharpCode.Reporting.Items;

namespace ICSharpCode.Reporting.Interfaces
{
	/// <summary>
	/// Description of ISection.
	/// </summary>
	public interface ISection:IReportItem
	{
		List<ReportItem> Items {get;}
	}
}
