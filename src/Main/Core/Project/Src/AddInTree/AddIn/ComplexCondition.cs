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
using System.Diagnostics;
using System.Text;
using System.Xml;

namespace ICSharpCode.Core
{
	/// <summary>
	/// Negates a condition
	/// </summary>
	public class NegatedCondition : ICondition
	{
		ICondition condition;
		
		public string Name {
			get {
				return "Not " + condition.Name;
			}
		}
		
		ConditionFailedAction action = ConditionFailedAction.Exclude;
		public ConditionFailedAction Action {
			get {
				return action;
			}
			set {
				action = value;
			}
		}
		
		public NegatedCondition(ICondition condition)
		{
			Debug.Assert(condition != null);
			this.condition = condition;
		}
		
		public bool IsValid(object parameter)
		{
			return !condition.IsValid(parameter);
		}
		
		public static ICondition Read(XmlReader reader, AddIn addIn)
		{
			return new NegatedCondition(Condition.ReadConditionList(reader, "Not", addIn)[0]);
		}
	}

	/// <summary>
	/// Gives back the and result of two conditions.
	/// </summary>
	public class AndCondition : ICondition
	{
		ICondition[] conditions;
		
		public string Name {
			get {
				StringBuilder sb = new StringBuilder();
				for (int i = 0; i < conditions.Length; ++i) {
					sb.Append(conditions[i].Name);
					if (i + 1 < conditions.Length) {
						sb.Append(" And ");
					}
				}
				return sb.ToString();
			}
		}
		
		ConditionFailedAction action = ConditionFailedAction.Exclude;
		public ConditionFailedAction Action {
			get {
				return action;
			}
			set {
				action = value;
			}
		}
		
		public AndCondition(ICondition[] conditions)
		{
			Debug.Assert(conditions.Length >= 1);
			this.conditions = conditions;
		}
		
		public bool IsValid(object parameter)
		{
			foreach (ICondition condition in conditions) {
				if (!condition.IsValid(parameter)) {
					return false;
				}
			}
			return true;
		}
		
		public static ICondition Read(XmlReader reader, AddIn addIn)
		{
			return new AndCondition(Condition.ReadConditionList(reader, "And", addIn));
		}
	}
	
	/// <summary>
	/// Gives back the or result of two conditions.
	/// </summary>
	public class OrCondition : ICondition
	{
		ICondition[] conditions;
		
		
		public string Name {
			get {
				StringBuilder sb = new StringBuilder();
				for (int i = 0; i < conditions.Length; ++i) {
					sb.Append(conditions[i].Name);
					if (i + 1 < conditions.Length) {
						sb.Append(" Or ");
					}
				}
				return sb.ToString();
			}
		}
		
		ConditionFailedAction action = ConditionFailedAction.Exclude;
		public ConditionFailedAction Action {
			get {
				return action;
			}
			set {
				action = value;
			}
		}
		
		public OrCondition(ICondition[] conditions)
		{
			Debug.Assert(conditions.Length >= 1);
			this.conditions = conditions;
		}
		
		public bool IsValid(object parameter)
		{
			foreach (ICondition condition in conditions) {
				if (condition.IsValid(parameter)) {
					return true;
				}
			}
			return false;
		}
		
		public static ICondition Read(XmlReader reader, AddIn addIn)
		{
			return new OrCondition(Condition.ReadConditionList(reader, "Or", addIn));
		}
	}
}
