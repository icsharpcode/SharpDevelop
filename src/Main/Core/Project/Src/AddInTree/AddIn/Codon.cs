// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ICSharpCode.Core
{
	/// <summary>
	/// Represents a node in the add in tree that can produce an item.
	/// </summary>
	public class Codon
	{
		AddIn       addIn;
		string      name;
		Properties  properties;
		ICondition[] conditions;
		
		public string Name {
			get {
				return name;
			}
		}
		
		public AddIn AddIn {
			get {
				return addIn;
			}
		}
		
		public string Id {
			get {
				return properties["id"];
			}
		}
		
		public string InsertAfter {
			get {
				return properties["insertafter"];
			}
		}
		
		public string InsertBefore {
			get {
				return properties["insertbefore"];
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
		
		public ICondition[] Conditions {
			get {
				return conditions;
			}
		}
		
		public Codon(AddIn addIn, string name, Properties properties, ICondition[] conditions)
		{
			this.addIn      = addIn;
			this.name       = name;
			this.properties = properties;
			this.conditions = conditions;
		}
		
		[Obsolete("Use BuildItemArgs.Conditions instead")]
		public ConditionFailedAction GetFailedAction(object caller)
		{
			return Condition.GetFailedAction(conditions, caller);
		}
		
//
//		public void BinarySerialize(BinaryWriter writer)
//		{
//			writer.Write(AddInTree.GetNameOffset(name));
//			writer.Write(AddInTree.GetAddInOffset(addIn));
//			properties.BinarySerialize(writer);
//		}
//
		
		internal object BuildItem(BuildItemArgs args)
		{
			IDoozer doozer;
			if (!AddInTree.Doozers.TryGetValue(Name, out doozer))
				throw new CoreException("Doozer " + Name + " not found! " + ToString());
			
			if (!doozer.HandleConditions) {
				ConditionFailedAction action = Condition.GetFailedAction(args.Conditions, args.Caller);
				if (action != ConditionFailedAction.Nothing) {
					return null;
				}
			}
			return doozer.BuildItem(args);
		}
		
		public override string ToString()
		{
			return String.Format("[Codon: name = {0}, id = {1}, addIn={2}]",
			                     name,
			                     Id,
			                     addIn.FileName);
		}
	}
}
