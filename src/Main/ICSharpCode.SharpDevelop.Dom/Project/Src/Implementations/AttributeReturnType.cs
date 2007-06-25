// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace ICSharpCode.SharpDevelop.Dom
{
	/// <summary>
	/// Like SearchClassReturnType, but tries both the specified name and name+"Attribute".
	/// </summary>
	public class AttributeReturnType : ProxyReturnType
	{
		string name;
		SearchClassReturnType scrt1, scrt2;
		
		public AttributeReturnType(ClassFinder context, string name)
		{
			if (context == null)
				throw new ArgumentNullException("context");
			if (name == null)
				throw new ArgumentNullException("name");
			this.name = name;
			scrt1 = new SearchClassReturnType(context.ProjectContent, context.CallingClass,
			                                  context.CaretLine, context.CaretColumn, name, 0);
			scrt2 = new SearchClassReturnType(context.ProjectContent, context.CallingClass,
			                                  context.CaretLine, context.CaretColumn, name + "Attribute", 0);
		}
		
		public override IReturnType BaseType {
			get {
				if (scrt1.GetUnderlyingClass() != null)
					return scrt1;
				else
					return scrt2;
			}
		}
	}
}


