// created on 08.09.2003 at 16:17

using ICSharpCode.SharpDevelop.Dom;
using System.Collections;
using System.Collections.Generic;

namespace ICSharpCode.SharpDevelop.Dom.NRefactoryResolver
{
	public class AttributeSection : AbstractAttributeSection
	{
		public AttributeSection(AttributeTarget attributeTarget, List<IAttribute> attributes) {
			this.attributeTarget = attributeTarget;
			this.Attributes = attributes;
		}
	}
	public class ASTAttribute : AbstractAttribute
	{
		public ASTAttribute(string name, ArrayList positionalArguments, SortedList namedArguments)
		{
			this.name = name;
			this.positionalArguments = positionalArguments;
			this.namedArguments = namedArguments;
		}
	}
}
