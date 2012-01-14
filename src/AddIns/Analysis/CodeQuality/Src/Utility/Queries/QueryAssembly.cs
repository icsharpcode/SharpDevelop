/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 05.01.2012
 * Time: 20:13
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using ICSharpCode.Core;

namespace ICSharpCode.CodeQualityAnalysis.Utility.Queries
{
	/// <summary>
	/// Description of QueryAssembly.
	/// </summary>
	public class QueryAssembly:BaseQuery
	{
		public QueryAssembly(Module mainModule):base(mainModule)
		{
		}
		
		public override System.Collections.Generic.List<ItemWithFunc> GetQueryList()
		{
			List<ItemWithFunc> items = new List<ItemWithFunc>();
			items.Add(new ItemWithFunc()
			                     {
			                     	Description = "# of NameSpaces",
			                     	Action = ExecuteNotImplemented
			                     });
			return items;
		}
		
		private List<TreeMapViewModel> ExecuteNotImplemented()
		{
			MessageService.ShowMessage("Not Implemented yet","CodeQualityAnalysis");
			return null;
		}
	}
}
