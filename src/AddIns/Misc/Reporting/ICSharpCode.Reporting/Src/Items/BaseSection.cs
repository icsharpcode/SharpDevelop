/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 19.03.2013
 * Time: 20:19
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using ICSharpCode.Reporting.Interfaces;
using ICSharpCode.Reporting.Interfaces.Export;
using ICSharpCode.Reporting.PageBuilder.ExportColumns;

namespace ICSharpCode.Reporting.Items
{
	/// <summary>
	/// Description of BaseSection.
	/// </summary>
	
public class BaseSection:ReportContainer,IReportContainer
	{
		#region Constructors
		
		public BaseSection()
		{
		}
		
		public BaseSection (string name) {
			Name = name;
		}
	
		#endregion
	}
}
