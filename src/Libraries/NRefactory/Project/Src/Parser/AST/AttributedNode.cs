// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace ICSharpCode.NRefactory.Parser.AST
{
	public abstract class AttributedNode : AbstractNode
	{
		protected List<AttributeSection> attributes;
		protected Modifier               modifier;
		
		public List<AttributeSection> Attributes {
			get {
				return attributes;
			}
			set {
				attributes = value == null ? new List<AttributeSection>(1) : value;
			}
		}
		
		public Modifier Modifier {
			get {
				return modifier;
			}
			set {
				modifier = value;
			}
		}
		
		public AttributedNode(List<AttributeSection> attributes) : this(Modifier.None, attributes)
		{
		}
		
		public AttributedNode(Modifier modifier, List<AttributeSection> attributes)
		{
			this.modifier   = modifier;
			
			// use property because of the null check.
			this.Attributes = attributes;
		}
		
	}
}
