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

namespace ICSharpCode.CodeQualityAnalysis.Utility
{
	/// <summary>
	/// Description of testclass.
	/// </summary>
	public class QueryMainModule
	{
		public QueryMainModule(Module mainModule)
		{
			MainModule = mainModule;
		}
		
		public Module MainModule {get; private set;}
		
		private List <INode> QueryForMethod()
		{
			IEnumerable<INode> query  = new List<INode>();
			query  = from ns in MainModule.Namespaces
				from type in ns.Types
				from method in type.Methods
				select method;
			return query.ToList();
		}
		
		
		public List<ItemWithAction> NameSpaceList()
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
		
		
		
		
		public List<ItemWithAction> MethodList()
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
		
		private List<INode> ExecuteNotImplemented()
		{
			MessageService.ShowMessage("Not Implemented yet","CodeQualityAnalysis");
			return null;
		}
		
		
		private List<INode> ExecuteMethodILInstructions()
		{
			var intermediate = QueryForMethod().Cast<Method>().ToList();
			var filtered = from method in intermediate
				where method.Instructions.Count > 0
				select method;
			return filtered.Cast<INode>().ToList();
		}
		
		
		private List<INode> ExecuteMethodComplexity ()
		{
			var intermediate = QueryForMethod().Cast<Method>().ToList();
			var filtered = from method in intermediate
				where method.CyclomaticComplexity > 0
				select method;
			return filtered.Cast<INode>().ToList();
		}
	
		
		private List<INode> ExecuteMethodVariables ()
		{
			var intermediate = QueryForMethod().Cast<Method>().ToList();
// eliminate 0-values
			var filtered = from method in intermediate
				where method.Variables > 0
				select method;
			
			return filtered.Cast<INode>().ToList();
		}
	}
}
