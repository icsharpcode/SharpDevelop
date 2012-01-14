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
using System.Linq;

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
		
		private List <Namespace> NameSpaceQuery()
		{
			IEnumerable<Namespace> query  = new List<Namespace>();
			query  = from ns in MainModule.Namespaces
				select ns;
			return query.ToList();
		}
		
		public override List<ItemWithAction> GetQueryList()
		{
			List<ItemWithAction> items = new List<ItemWithAction>();
			items.Add(new ItemWithAction()
			                     {
			                     	Description = "# of IL Instructions",
			                     	Metrics =  "Instructions.Count",
			                     	Action = ExecuteILInstructions
			                     });
			items.Add(new ItemWithAction()
			                     {
			                     	Description = "# of Methods",
			                     	Metrics = Metrics.CyclomaticComplexity.ToString(),
			                     	Action = MethodsCount
			                     });
			items.Add(new ItemWithAction()
			                     {
			                     	Description = "# of Fields",
			                     	Metrics = Metrics.Variables.ToString(),
			                     	Action = FieldsCount
			                     });
			
			items.Add(new ItemWithAction()
			                     {
			                     	Description = "# of Types",
			                     	Metrics = Metrics.Variables.ToString(),
			                     	Action = TypesCount
			                     });
			return items;
		}
		
		
		private List<TreeMapViewModel> ExecuteILInstructions ()
		{
			var intermediate = this.NameSpaceQuery();
			var i = 0;
			var list = intermediate.Select(m =>  new TreeMapViewModel()
			                               {
			                               	Name = m.Name,
			                               	NumericValue = m.GetAllMethods().Aggregate(i, (current, x) => current + x.Instructions.Count)
			                               });
			var filtered = base.EliminateZeroValues(list);
			return filtered.ToList();
		}
		
		
		private List<TreeMapViewModel> MethodsCount()
		{
			var intermediate = this.NameSpaceQuery();
			
			var list = intermediate.Select(m =>  new TreeMapViewModel()
			                               {
			                               	Name = m.Name,
			                               	NumericValue = m.GetAllMethods().ToList().Count
			                               });
			var filtered = base.EliminateZeroValues(list);
			return filtered.ToList();
		}
		
		
		private List<TreeMapViewModel> FieldsCount()
		{
			var intermediate = this.NameSpaceQuery();
			var list = intermediate.Select(m =>  new TreeMapViewModel()
			                               {
			                               	Name = m.Name,
			                               	NumericValue = m.GetAllFields().ToList().Count
			                               });
			var filtered = base.EliminateZeroValues(list);
			return filtered.ToList();
		}
		
		
		private List<TreeMapViewModel> TypesCount()
		{
			var intermediate = this.NameSpaceQuery();
			
			var list = intermediate.Select(m =>  new TreeMapViewModel()
			                               {
			                               	Name = m.Name,
			                               	NumericValue = m.GetAllTypes().ToList().Count
			                               });
			var filtered = base.EliminateZeroValues(list);
			return filtered.ToList();
		}
	}
}
