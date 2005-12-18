// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace ICSharpCode.NRefactory.Parser.AST
{
	public abstract class ParametrizedNode : AttributedNode
	{
		protected string                               name = String.Empty;
		protected List<ParameterDeclarationExpression> parameters;
		
		public string Name {
			get {
				return name;
			}
			set {
				name = value == null ? String.Empty : value;
			}
		}
		
		public List<ParameterDeclarationExpression> Parameters {
			get {
				return parameters;
			}
			set {
				parameters = value == null ? new List<ParameterDeclarationExpression>(1) : value;
			}
		}
		
		public ParametrizedNode(Modifier modifier, List<AttributeSection> attributes) : this(modifier, attributes, null)
		{
		}
		
		public ParametrizedNode(Modifier modifier, List<AttributeSection> attributes, string name) : this(modifier, attributes, name, null)
		{
		}
		
		public ParametrizedNode(Modifier modifier, List<AttributeSection> attributes, string name, List<ParameterDeclarationExpression> parameters) : base(modifier, attributes)
		{
			// use properties because of the null check.
			this.Name       = name;
			this.Parameters = parameters;
		}
	}
}
