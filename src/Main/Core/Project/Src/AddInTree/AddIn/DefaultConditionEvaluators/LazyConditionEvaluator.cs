// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace ICSharpCode.Core
{
	/// <summary>
	/// Condition evaluator that lazy-loads another condition evaluator and executes it.
	/// </summary>
	public class LazyConditionEvaluator : IConditionEvaluator
	{
		AddIn addIn;
		string name;
		string className;
		
		public string Name {
			get {
				return name;
			}
		}
		
		public string ClassName {
			get {
				return className;
			}
		}
		
		public LazyConditionEvaluator(AddIn addIn, Properties properties)
		{
			this.addIn      = addIn;
			this.name       = properties["name"];
			this.className  = properties["class"];
		}
		
		public bool IsValid(object caller, Condition condition)
		{
			IConditionEvaluator evaluator = (IConditionEvaluator)addIn.CreateObject(className);
			if (evaluator == null) {
				return false;
			}
			AddInTree.ConditionEvaluators[name] = evaluator;
			return evaluator.IsValid(caller, condition);
		}
		
		public override string ToString()
		{
			return String.Format("[LazyLoadConditionEvaluator: className = {0}, name = {1}]",
			                     className,
			                     name);
		}
		
	}
}
