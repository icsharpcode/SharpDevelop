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
		
		private List <Method> MethodQuery()
		{
			IEnumerable<Method> query  = new List<Method>();
			query  = from ns in MainModule.Namespaces
				from type in ns.Types
				from method in type.Methods
				select method;
			return query.ToList();
		}
		
		
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
			                     	Description = "IL Cyclomatic Complexity",
			                     	Metrics = Metrics.CyclomaticComplexity.ToString(),
			                     	Action = ExecuteMethodComplexity
			                     });
			                
			items.Add(new ItemWithAction()
			                     {
			                     	Description = "# of Variables",
			                     	Metrics = Metrics.Variables.ToString(),
			                     	Action = ExecuteMethodVariables
			                     });
			                   
			return items;
		}
		
		
		private List<TreeMapViewModel> ExecuteMethodILInstructions()
		{
			var intermediate = MethodQuery();
			var filtered = from method in intermediate
				where method.Instructions.Count > 0
				select method;
			
			int i = 0;
			var list = filtered.Select(m =>  new TreeMapViewModel()
			                               {
			                               	Name = m.Name,
			                               	Numval = m.GetAllMethods().Aggregate(i, (current, x) => current + x.Instructions.Count)
			                               });

			return list.ToList();
		}
		
		
		private List<TreeMapViewModel> ExecuteMethodComplexity ()
		{
			var intermediate = MethodQuery();
			var filtered = from method in intermediate
				where method.CyclomaticComplexity > 0
				select method;
			
			var list = filtered.Select(m =>  new TreeMapViewModel()
			                               {
			                               	Name = m.Name,
			                               	Numval = m.CyclomaticComplexity
			                               });
			return list.ToList();
		}
	
		
		private List<TreeMapViewModel> ExecuteMethodVariables ()
		{
			var intermediate = MethodQuery();
			var filtered = from method in intermediate
				where method.Variables > 0
				select method;
			var list = filtered.Select(m =>  new TreeMapViewModel()
			                               {
			                               	Name = m.Name,
			                               	Numval =  m.Variables
			                               });
			return list.ToList();
		}
	}
}
