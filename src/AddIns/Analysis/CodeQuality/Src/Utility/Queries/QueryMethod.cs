// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
		public QueryMethod(AssemblyNode mainModule):base(mainModule)
		{
		}
		
		private List<MethodNode> MethodQuery()
		{
			IEnumerable<MethodNode> query =
				from ns in MainModule.Namespaces
				from type in ns.Types
				from method in type.Methods
				select method;
			return query.ToList();
		}
		
		
		public override List<ItemWithFunc> GetQueryList()
		{
			List<ItemWithFunc> items = new List<ItemWithFunc>();
			items.Add(new ItemWithFunc()
			          {
			          	Description = "# of IL Instructions",
			          	Metrics =  "Instructions.Count",
			          	Action = ExecuteMethodILInstructions
			          });
		
			items.Add(new ItemWithFunc()
			                     {
			                     	Description = "IL Cyclomatic Complexity",
			                     	Action = ExecuteMethodComplexity
			                     });
			                
			items.Add(new ItemWithFunc()
			                     {
			                     	Description = "# of Variables",
			                     	Action = ExecuteMethodVariables
			                     });
			 */
			return items;
		}
		
		
		private List<TreeMapViewModel> ExecuteMethodILInstructions()
		{
			var intermediate = MethodQuery();
<<<<<<< .mine=======			var filtered = from method in intermediate
				where method.Instructions.Count() > 0
				select method;
			
>>>>>>> .theirs			int i = 0;
			var list = intermediate.Select(m =>  new TreeMapViewModel()
<<<<<<< .mine			                               {
			                               	Name = m.Name,
			                               	NumericValue = m.GetAllMethods().Aggregate(i, (current, x) => current + x.Instructions.Count)
			                               });
=======			                           {
			                           	Name = m.Name,
//			                           	Numval = m.GetAllMethods().Aggregate(i, (current, x) => current + x.Instructions.Count)
			                           });
>>>>>>> .theirs<<<<<<< .mine
			var filtered = base.EliminateZeroValues(list);
			base.SetToolstripText(filtered,base.InstructionsCount);
			return filtered.ToList();
=======//			var list = intermediate.Select(m =>  new TreeMapViewModel()
//			                               {
//			                               	Name = m.Name,
//			                               	Numval = m.GetAllMethods().Aggregate(i, (current, x) => current + x.Instructions.Count)
//			                               });
			
			return list.ToList();
			//return filtered.Cast<TreeMapViewModel>().ToList();
>>>>>>> .theirs		}
		
		
		private List<TreeMapViewModel> ExecuteMethodComplexity ()
		{
			var intermediate = MethodQuery();

			var list = intermediate.Select(m =>  new TreeMapViewModel()
			                               {
			                               	Name = m.Name,
			                               	NumericValue = m.CyclomaticComplexity
			                               });
			var filtered = base.EliminateZeroValues(list);
			base.SetToolstripText(filtered,base.Complexity);
			return filtered.ToList();
		}
		
		
		private List<TreeMapViewModel> ExecuteMethodVariables ()
		{
			var intermediate = MethodQuery();
<<<<<<< .mine=======			// eliminate 0-values reduce time for my test assembly from 6 to 1 sek

			var filtered = from method in intermediate
				where method.Variables > 0
				select method;
>>>>>>> .theirs			
			var list = intermediate.Select(m =>  new TreeMapViewModel()
			                               {
			                               	Name = m.Name,
			                               	NumericValue =  m.Variables
			                               });
			var filtered = base.EliminateZeroValues(list);
			base.SetToolstripText(filtered,base.VariablesCount);
			return filtered.ToList();
		}
	}
}
