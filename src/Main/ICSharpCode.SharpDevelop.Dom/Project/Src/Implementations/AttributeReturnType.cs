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
	/// Description of AttributeReturnType.
	/// </summary>
	public class AttributeReturnType : ProxyReturnType
	{
		string name;
		
		public AttributeReturnType(string name)
		{
			this.name = name;
		}
		
		public override IReturnType BaseType {
			get {
				
				return null;
			}
		}
	}
}

