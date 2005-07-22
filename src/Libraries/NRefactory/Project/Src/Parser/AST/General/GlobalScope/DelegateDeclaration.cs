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
	public class DelegateDeclaration : AttributedNode
	{
		string          name = "";
		TypeReference   returnType = TypeReference.Null;
		List<ParameterDeclarationExpression> parameters = new List<ParameterDeclarationExpression>(1);
		List<TemplateDefinition> templates = new List<TemplateDefinition>();
		
		public string Name {
			get {
				return name;
			}
			set {
				name = (value != null) ? value : "?";
			}
		}
		
		public TypeReference ReturnType {
			get {
				return returnType;
			}
			set {
				Debug.Assert(value != null);
				returnType = value;
			}
		}
		
		public List<ParameterDeclarationExpression> Parameters {
			get {
				return parameters;
			}
			set {
				Debug.Assert(value != null);
				parameters = value;
			}
		}
		
		public List<TemplateDefinition> Templates {
			get {
				return templates;
			}
			set {
				Debug.Assert(value != null);
				templates = value;
			}
		}
		
		public DelegateDeclaration(Modifier modifier, List<AttributeSection> attributes) : base(modifier, attributes)
		{
		}
		
		public override object AcceptVisitor(IASTVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}

		public override string ToString()
		{
			return String.Format("[DelegateDeclaration: Name={0}, Modifier={1}, ReturnType={2}, parameters={3}, attributes={4}]",
			                     name,
			                     modifier,
			                     returnType,
			                     GetCollectionString(parameters),
			                     GetCollectionString(attributes));
		}
	}
}
