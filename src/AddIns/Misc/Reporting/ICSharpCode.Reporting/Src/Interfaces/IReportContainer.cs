/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 16.04.2013
 * Time: 19:53
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using ICSharpCode.Reporting.Interfaces.Export;
using ICSharpCode.Reporting.Items;

namespace ICSharpCode.Reporting.Interfaces
{
	/// <summary>
	/// Description of IReportContainer.
	/// </summary>
	public interface IReportContainer :IPrintableObject
	{
		 List<IPrintableObject> Items {get;}
	}
}
