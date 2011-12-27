/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 25.12.2011
 * Time: 19:07
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ICSharpCode.CodeQualityAnalysis.Utility
{
	/// <summary>
	/// Description of testclass.
	/// </summary>
	public class testclass
	{
		public testclass(Module mainModule)
		{
			MainModule = mainModule;
		}
		
		public Module MainModule {get; private set;}
		
		public ObservableCollection <INode> QueryMethod()
		{
			IEnumerable<INode> list  = new List<INode>();
			list  = from ns in MainModule.Namespaces
				from type in ns.Types
				from method in type.Methods
				select method;
			return new  ObservableCollection <INode>(list);
		}
		
		public List<ItemWithAction> MethodList()
		{
			List<ItemWithAction> itemsWithCommand = new List<ItemWithAction>();
			itemsWithCommand.Add(new ItemWithAction()
				                     {
				                     	Description = "IL Instructions",
				                     	Action = ExecuteMerhodIlInstructions
				                     });
				itemsWithCommand.Add(new ItemWithAction()
				                     {
				                     	Description = "Cyclomatic Complexity",
				                     	Action = ExecuteMethodComplexity
				                     });
				itemsWithCommand.Add(new ItemWithAction()
				                     {
				                     	Description = "Variables",
				                     	Action = ExecuteMethodVariables
				                     });
			return itemsWithCommand;
		}
		
		
		private void ExecuteMerhodIlInstructions()
		{
//			var t = new testclass(MainModule);
//			TreeValueProperty = "Instructions.Count";
//			Nodes = t.QueryMethod();
		}
		
		private void ExecuteMethodComplexity ()
		{
//			var t = new testclass(MainModule);
//			TreeValueProperty = Metrics.CyclomaticComplexity.ToString();
//			var tt = t.QueryMethod();
//			foreach (var element in tt) {
//				var m = element as Method;
//				Console.WriteLine("{0} - {1}",m.Name,m.CyclomaticComplexity);
//			}
//			Nodes = t.QueryMethod();
		}
	
		
		private void ExecuteMethodVariables ()
		{
//			var t = new testclass(MainModule);
//			TreeValueProperty = Metrics.Variables.ToString();
//			Nodes = t.QueryMethod();
		}
	}
}
