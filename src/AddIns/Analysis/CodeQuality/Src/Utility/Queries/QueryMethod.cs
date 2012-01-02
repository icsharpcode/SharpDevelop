/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 25.12.2011
 * Time: 19:07
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using ICSharpCode.Core;

namespace ICSharpCode.CodeQualityAnalysis.Utility.Queries
{
	/// <summary>
	/// Description of testclass.
	/// </summary>
	public class QueryMethod:BaseQuery
	{
		public QueryMethod(Module mainModule):base(mainModule)
		{	
		}
		
		
		private List <Method> QueryForMethod()
		{
			IEnumerable<Method> query  = new List<Method>();
			query  = from ns in MainModule.Namespaces
				from type in ns.Types
				from method in type.Methods
				select method;
			return query.ToList();
		}
		
		/*
		private List <INode> QueryForMethod()
		{
			IEnumerable<INode> query  = new List<INode>();
			query  = from ns in MainModule.Namespaces
				from type in ns.Types
				from method in type.Methods
				select method;
			return query.ToList();
		}
		*/
		
		public override List<ItemWithAction> GetQueryList()
		{
			List<ItemWithAction> items = new List<ItemWithAction>();
			items.Add(new ItemWithAction()
			                     {
			                     	Description = "# of IL Instructions",
			                     	Metrics =  "Instructions.Count",
			                     	Action = ExecuteMethodILInstructions
			                     });
			items.Add(new ItemWithAction()
			                     {
			                     	Description = "Cyclomatic Complexity",
			                     	Metrics = Metrics.CyclomaticComplexity.ToString(),
			                     	Action = ExecuteMethodComplexity
			                     });
			items.Add(new ItemWithAction()
			                     {
			                     	Description = "Variables",
			                     	Metrics = Metrics.Variables.ToString(),
			                     	Action = ExecuteMethodVariables
			                     });
			return items;
		}
		
		
		private List<INode> ExecuteMethodILInstructions()
		{
			var intermediate = QueryForMethod();
			var filtered = from method in intermediate
				where method.Instructions.Count > 0
				select method;
			return filtered.Cast<INode>().ToList();
		}
		
		
		private List<INode> ExecuteMethodComplexity ()
		{
			var intermediate = QueryForMethod();
			var filtered = from method in intermediate
				where method.CyclomaticComplexity > 0
				select method;
			return filtered.Cast<INode>().ToList();
		}
	
		
		private List<INode> ExecuteMethodVariables ()
		{
			var intermediate = QueryForMethod();
// eliminate 0-values reduce time for my test assembly from 6 to 1 sek

			var filtered = from method in intermediate
				where method.Variables > 0
				select method;
			
			return filtered.Cast<INode>().ToList();
		}
	}
}
