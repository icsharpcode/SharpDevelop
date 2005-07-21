// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>
using System;
using System.Collections;
using System.Collections.Generic;

namespace ICSharpCode.SharpDevelop.Dom
{
	public class DefaultAttribute : IAttribute
	{
		string name;
		List<AttributeArgument> positionalArguments;
		SortedList<string, AttributeArgument> namedArguments;
		AttributeTarget attributeTarget;
		
		public DefaultAttribute(string name) : this(name, AttributeTarget.None) {}
		
		public DefaultAttribute(string name, AttributeTarget attributeTarget)
		{
			this.name = name;
			this.attributeTarget = attributeTarget;
			this.positionalArguments = new List<AttributeArgument>();
			this.namedArguments = new SortedList<string, AttributeArgument>();
		}
		
		public DefaultAttribute(string name, AttributeTarget attributeTarget, List<AttributeArgument> positionalArguments, SortedList<string, AttributeArgument> namedArguments)
		{
			this.name = name;
			this.attributeTarget = attributeTarget;
			this.positionalArguments = positionalArguments;
			this.namedArguments = namedArguments;
		}
		
		public string Name {
			get {
				return name;
			}
			set {
				name = value;
			}
		}
		
		public AttributeTarget AttributeTarget {
			get {
				return attributeTarget;
			}
			set {
				attributeTarget = value;
			}
		}
		
		public List<AttributeArgument> PositionalArguments {
			get {
				return positionalArguments;
			}
		}
		
		public SortedList<string, AttributeArgument> NamedArguments {
			get {
				return namedArguments;
			}
		}
		
		public virtual int CompareTo(IAttribute value) {
			int cmp;
			
			cmp = Name.CompareTo(value.Name);
			if (cmp != 0) {
				return cmp;
			}
			
			return DiffUtility.Compare(PositionalArguments, value.PositionalArguments);
		}
		
		int IComparable.CompareTo(object value) {
			return CompareTo((IAttribute)value);
		}
	}
}
