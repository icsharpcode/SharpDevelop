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
		IReadOnlyList<ICondition> conditions;
		
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
		
		public IReadOnlyList<ICondition> Conditions {
			get {
				return conditions;
			}
		}
		
		public Codon(AddIn addIn, string name, Properties properties, IReadOnlyList<ICondition> conditions)
		{
			if (name == null)
				throw new ArgumentNullException("name");
			if (properties == null)
				throw new ArgumentNullException("properties");
			this.addIn      = addIn;
			this.name       = name;
			this.properties = properties;
			this.conditions = conditions;
		}
		
		internal object BuildItem(BuildItemArgs args)
		{
			IDoozer doozer;
			if (!addIn.AddInTree.Doozers.TryGetValue(Name, out doozer))
				throw new CoreException("Doozer " + Name + " not found! " + ToString());
			
			if (!doozer.HandleConditions) {
				ConditionFailedAction action = Condition.GetFailedAction(args.Conditions, args.Parameter);
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
