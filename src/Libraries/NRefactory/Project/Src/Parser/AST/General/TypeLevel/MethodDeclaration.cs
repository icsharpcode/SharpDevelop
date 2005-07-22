// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace ICSharpCode.NRefactory.Parser.AST
{
	public class MethodDeclaration : ParametrizedNode
	{
		TypeReference    typeReference    = TypeReference.Null;
		BlockStatement   body             = BlockStatement.Null;
		ArrayList     handlesClause    = new ArrayList();    // VB only
		ArrayList     implementsClause = new ArrayList(); // VB only
		AttributeSection returnTypeAttributeSection = AttributeSection.Null;
		List<TemplateDefinition> templates = new List<TemplateDefinition>();
		
		public List<TemplateDefinition> Templates {
			get {
				return templates;
			}
			set {
				templates = value;
			}
		}
		
		public AttributeSection ReturnTypeAttributeSection {
			get {
				return returnTypeAttributeSection;
			}
			set {
				returnTypeAttributeSection = AttributeSection.CheckNull(value);
			}
		}
		
		public BlockStatement Body {
			get {
				return body;
			}
			set {
				body = BlockStatement.CheckNull(value);
			}
		}
		
		public TypeReference TypeReference {
			get {
				return typeReference;
			}
			set {
				typeReference = TypeReference.CheckNull(value);
			}
		}
		
		public ArrayList HandlesClause {
			get {
				return handlesClause;
			}
			set {
				handlesClause = value == null ? new ArrayList() : value;
			}
		}
		
		public ArrayList ImplementsClause {
			get {
				return implementsClause;
			}
			set {
				implementsClause = value == null ? new ArrayList() : value;
			}
		}
		
		public MethodDeclaration(string name, Modifier modifier, TypeReference typeReference, List<ParameterDeclarationExpression> parameters, List<AttributeSection> attributes) : base(modifier, attributes, name, parameters)
		{
			this.TypeReference = typeReference;
		}
		
		public override object AcceptVisitor(IASTVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}
		public override string ToString()
		{
			return String.Format("[MethodDeclaration: Name = {0}, Body = {1}, Modifier = {2}, TypeReference = {3}, Parameters = {4}, Attributes = {5}]",
			                     Name,
			                     Body,
			                     Modifier,
			                     TypeReference,
			                     GetCollectionString(Parameters),
			                     GetCollectionString(Attributes));
		}
	}
}
