// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;

namespace ICSharpCode.Core
{
	/// <summary>
	/// Condition evaluator that lazy-loads another condition evaluator and executes it.
	/// </summary>
	sealed class LazyConditionEvaluator : IConditionEvaluator
	{
		AddIn addIn;
		string name;
		string className;
		
		public string Name {
			get {
				return name;
			}
		}
		
		public LazyConditionEvaluator(AddIn addIn, Properties properties)
		{
			if (addIn == null)
				throw new ArgumentNullException("addIn");
			this.addIn      = addIn;
			this.name       = properties["name"];
			this.className  = properties["class"];
		}
		
		public bool IsValid(object parameter, Condition condition)
		{
			IConditionEvaluator evaluator = (IConditionEvaluator)addIn.CreateObject(className);
			if (evaluator == null)
				return false;
			addIn.AddInTree.ConditionEvaluators[name] = evaluator;
			return evaluator.IsValid(parameter, condition);
		}
		
		public override string ToString()
		{
			return String.Format("[LazyLoadConditionEvaluator: className = {0}, name = {1}]",
			                     className,
			                     name);
		}
	}
}
