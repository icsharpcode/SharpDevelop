/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 24.05.2013
 * Time: 20:17
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.ObjectModel;
using ICSharpCode.Reporting.BaseClasses;
using ICSharpCode.Reporting.DataManager.Listhandling;

namespace ICSharpCode.Reporting.DataSource.Comparer
{
	/// <summary>
	/// Description of GroupComparer.
	/// </summary>
	public  class GroupComparer :SortComparer
	{
		
		public GroupComparer (Collection<AbstractColumn> owner, int listIndex, object[] values):base(owner,listIndex,values)
		{
			IndexList = new IndexList();
		}
		
		public IndexList IndexList {get;set;}
	}
}
