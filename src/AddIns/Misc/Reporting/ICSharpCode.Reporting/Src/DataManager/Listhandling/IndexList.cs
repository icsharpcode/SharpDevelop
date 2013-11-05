/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 21.05.2013
 * Time: 20:54
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using ICSharpCode.Reporting.DataSource.Comparer;

namespace ICSharpCode.Reporting.DataManager.Listhandling
{
	/// <summary>
	/// Description of IndexList.
	/// </summary>
	public class IndexList :List<BaseComparer> 
	{
		
		public IndexList()
		{
		}
		
		public int CurrentPosition {get;set;}
	}
}
