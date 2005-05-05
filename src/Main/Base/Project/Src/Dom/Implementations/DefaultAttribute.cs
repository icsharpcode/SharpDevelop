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
	[Serializable]
	public class DefaultAttributeSection : IAttributeSection
	{
		AttributeTarget attributeTarget;
		List<IAttribute> attributes = null;
		
		public DefaultAttributeSection(AttributeTarget attributeTarget, List<IAttribute> attributes)
		{
			this.attributeTarget = attributeTarget;
			this.attributes = attributes;
		}
		
		public AttributeTarget AttributeTarget {
			get {
				return attributeTarget;
			}
			set {
				attributeTarget = value;
			}
		}
		
		public List<IAttribute> Attributes {
			get {
				return attributes;
			}
		}
		
		public virtual int CompareTo(IAttributeSection value) {
			int cmp;
			
			if(0 != (cmp = (int)(AttributeTarget - value.AttributeTarget)))
				return cmp;
			
			return DiffUtility.Compare(Attributes, value.Attributes);
		}
		
		int IComparable.CompareTo(object value) {
			return CompareTo((IAttributeSection)value);
		}
	}
	
	public class DefaultAttribute : IAttribute
	{
		string name;
		ArrayList positionalArguments;
		SortedList namedArguments;
		
		public DefaultAttribute(string name)
		{
			this.name = name;
			this.positionalArguments = new ArrayList();
			this.namedArguments = new SortedList();
		}
		
		public DefaultAttribute(string name, ArrayList positionalArguments, SortedList namedArguments)
		{
			this.name = name;
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
		public ArrayList PositionalArguments { // [expression]
			get {
				return positionalArguments;
			}
		}
		public SortedList NamedArguments { // string/expression
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
			
			cmp = DiffUtility.Compare(PositionalArguments, value.PositionalArguments);
			if (cmp != 0) {
				return cmp;
			}
			
			return DiffUtility.Compare(NamedArguments, value.NamedArguments);
		}
		
		int IComparable.CompareTo(object value) {
			return CompareTo((IAttribute)value);
		}
	}
}
