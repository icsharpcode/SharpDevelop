// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;

namespace ICSharpCode.SharpDevelop.Dom
{
	public class DefaultAttribute : IAttribute
	{
		public static readonly IList<IAttribute> EmptyAttributeList = new List<IAttribute>().AsReadOnly();
		
		IReturnType attributeType;
		IList<object> positionalArguments;
		IDictionary<string, object> namedArguments;
		AttributeTarget attributeTarget;
		
		public DefaultAttribute(IReturnType attributeType) : this(attributeType, AttributeTarget.None) {}
		
		public DefaultAttribute(IReturnType attributeType, AttributeTarget attributeTarget)
			: this(attributeType, attributeTarget, null, null)
		{
		}
		
		public DefaultAttribute(IReturnType attributeType, AttributeTarget attributeTarget, IList<object> positionalArguments, IDictionary<string, object> namedArguments)
		{
			if (attributeType == null)
				throw new ArgumentNullException("attributeType");
			this.attributeType = attributeType;
			this.attributeTarget = attributeTarget;
			this.positionalArguments = positionalArguments ?? new List<object>();
			this.namedArguments = namedArguments ?? new SortedList<string, object>();
		}
		
		
		public IReturnType AttributeType {
			get { return attributeType; }
			set { attributeType = value; }
		}
		
		public AttributeTarget AttributeTarget {
			get { return attributeTarget; }
			set { attributeTarget = value; }
		}
		
		public IList<object> PositionalArguments {
			get { return positionalArguments; }
		}
		
		public IDictionary<string, object> NamedArguments {
			get { return namedArguments; }
		}
	}
}
