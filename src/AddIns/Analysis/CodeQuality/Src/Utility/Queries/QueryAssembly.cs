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
		
		public override System.Collections.Generic.List<ItemWithAction> GetQueryList()
		{
			List<ItemWithAction> items = new List<ItemWithAction>();
			items.Add(new ItemWithAction()
			                     {
			                     	Description = "# of NameSpaces",
			                     	Metrics =  "Instructions.Count",
			                     	Action = ExecuteNotImplemented
			                     });
			return items;
		}
		
		private List<INode> ExecuteNotImplemented()
		{
			MessageService.ShowMessage("Not Implemented yet","CodeQualityAnalysis");
			return null;
		}
	}
}
