/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 03.01.2012
 * Time: 19:51
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using ICSharpCode.Core;

namespace ICSharpCode.CodeQualityAnalysis.Utility.Queries
{
	/// <summary>
	/// Description of QueryType.
	/// </summary>
	public class QueryType:BaseQuery
	{
		public QueryType(Module mainModule):base(mainModule)
		{
		}
		
		public override System.Collections.Generic.List<ItemWithAction> GetQueryList()
		{
			List<ItemWithAction> items = new List<ItemWithAction>();
			items.Add(new ItemWithAction()
			                     {
			                     	Description = "# of IL Instructions",
			                     	Metrics =  "Instructions.Count",
			                     	Action = ExecuteNotImplemented
			                     });
			
			items.Add(new ItemWithAction()
			                     {
			                     	Description = "IL Cyclomatic Complexity",
			                     	Metrics = Metrics.CyclomaticComplexity.ToString(),
			                     	Action = ExecuteNotImplemented
			                     });
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
			
			return items;
		}
		
		private List<INode> ExecuteNotImplemented()
		{
			MessageService.ShowMessage("Not Implemented yet","CodeQualityAnalysis");
			return null;
		}
	}
}
