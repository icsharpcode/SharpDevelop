// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Xml;

namespace ICSharpCode.Core
{
	
	public class Condition : ICondition
	{
		string                name;
		Properties            properties;
		ConditionFailedAction action;
		/// <summary>
		/// Returns the action which occurs, when this condition fails.
		/// </summary>
		public ConditionFailedAction Action {
			get {
				return action;
			}
			set {
				action = value;
			}
		}
		public string Name {
			get {
				return name;
			}
		}
		
		public string this[string key] {
			get {
				return properties[key];
			}
		}
		
		public Properties Properties {
			get {
				return properties;
			}
		}
		
		public Condition(string name, Properties properties)
		{
			this.name = name;
			this.properties = properties;
			action = properties.Get("action", ConditionFailedAction.Exclude);
		}
		
		public bool IsValid(object caller)
		{
			try {
				return AddInTree.ConditionEvaluators[name].IsValid(caller, this);
			} catch (KeyNotFoundException) {
				throw new CoreException("Condition evaluator " + name + " not found!");
			}

		}
		
		public static ICondition Read(XmlReader reader)
		{
			Properties properties = Properties.ReadFromAttributes(reader);
			string conditionName = properties["name"];
			return new Condition(conditionName, properties);
		}
		
		public static ICondition ReadComplexCondition(XmlReader reader)
		{
			Properties properties = Properties.ReadFromAttributes(reader);
			reader.Read();
			ICondition condition = null;
			while (reader.Read()) {
				switch (reader.NodeType) {
					case XmlNodeType.Element:
						switch (reader.LocalName) {
							case "And":
								condition = AndCondition.Read(reader);
								goto exit;
							case "Or":
								condition = OrCondition.Read(reader);
								goto exit;
							case "Not":
								condition = NegatedCondition.Read(reader);
								goto exit;
							default:
								throw new AddInLoadException("Invalid element name '" + reader.LocalName
								                             + "', the first entry in a ComplexCondition " +
								                             "must be <And>, <Or> or <Not>");
						}
				}
			}
		exit:
			if (condition != null) {
				ConditionFailedAction action = properties.Get("action", ConditionFailedAction.Exclude);
				condition.Action = action;
			}
			return condition;
		}
		
		public static ICondition[] ReadConditionList(XmlReader reader, string endElement)
		{
			List<ICondition> conditions = new List<ICondition>();
			while (reader.Read()) {
				switch (reader.NodeType) {
					case XmlNodeType.EndElement:
						if (reader.LocalName == endElement) {
							return conditions.ToArray();
						}
						break;
					case XmlNodeType.Element:
						switch (reader.LocalName) {
							case "And":
								conditions.Add(AndCondition.Read(reader));
								break;
							case "Or":
								conditions.Add(OrCondition.Read(reader));
								break;
							case "Not":
								conditions.Add(NegatedCondition.Read(reader));
								break;
							case "Condition":
								conditions.Add(Condition.Read(reader));
								break;
							default:
								throw new AddInLoadException("Invalid element name '" + reader.LocalName
								                             + "', entries in a <" + endElement + "> " +
								                             "must be <And>, <Or>, <Not> or <Condition>");
						}
						break;
				}
			}
			return conditions.ToArray();
		}
		
		public static ConditionFailedAction GetFailedAction(IEnumerable<ICondition> conditionList, object caller)
		{
			ConditionFailedAction action = ConditionFailedAction.Nothing;
			foreach (ICondition condition in conditionList) {
				if (!condition.IsValid(caller)) {
					if (condition.Action == ConditionFailedAction.Disable) {
						action = ConditionFailedAction.Disable;
					} else {
						return ConditionFailedAction.Exclude;
					}
				}
			}
			return action;
		}
	}
}
