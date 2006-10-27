// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

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
		
		public bool IsValid(object owner)
		{
			return !condition.IsValid(owner);
		}
		
		public static ICondition Read(XmlReader reader)
		{
			return new NegatedCondition(Condition.ReadConditionList(reader, "Not")[0]);
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
		
		public bool IsValid(object owner)
		{
			foreach (ICondition condition in conditions) {
				if (!condition.IsValid(owner)) {
					return false;
				}
			}
			return true;
		}
		
		public static ICondition Read(XmlReader reader)
		{
			return new AndCondition(Condition.ReadConditionList(reader, "And"));
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
		
		public bool IsValid(object owner)
		{
			foreach (ICondition condition in conditions) {
				if (condition.IsValid(owner)) {
					return true;
				}
			}
			return false;
		}
		
		public static ICondition Read(XmlReader reader)
		{
			return new OrCondition(Condition.ReadConditionList(reader, "Or"));
		}
	}
}
