/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 02.01.2012
 * Time: 20:11
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using ICSharpCode.Core;

namespace ICSharpCode.CodeQualityAnalysis.Utility.Queries
{
	/// <summary>
	/// Description of QueryNameSpace.
	/// </summary>
	public class QueryNameSpace:BaseQuery
	{
		public QueryNameSpace(Module mainModule):base (mainModule)
		{
		}
		
		
		public override List<ItemWithAction> GetQueryList()
		{
			List<ItemWithAction> items = new List<ItemWithAction>();
			items.Add(new ItemWithAction()
			                     {
			                     	Description = "# of Methods",
			                     	Metrics = Metrics.CyclomaticComplexity.ToString(),
			                     	Action = ExecuteNotImplemented
			                     });
			items.Add(new ItemWithAction()
			                     {
			                     	Description = "# of Fields",
			                     	Metrics = Metrics.Variables.ToString(),
			                     	Action = ExecuteNotImplemented
			                     });
			items.Add(new ItemWithAction()
			                     {
			                     	Description = "# of Types",
			                     	Metrics = Metrics.Variables.ToString(),
			                     	Action = ExecuteNotImplemented
			                     });
			
			items.Add(new ItemWithAction()
			                     {
			                     	Description = "# of Namespaces",
			                     	Metrics = Metrics.Variables.ToString(),
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
