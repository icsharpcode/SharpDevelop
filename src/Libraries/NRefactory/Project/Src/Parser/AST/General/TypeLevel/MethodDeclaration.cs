// MethodDeclaration.cs
// Copyright (C) 2003 Mike Krueger (mike@icsharpcode.net)
// 
// This program is free software; you can redistribute it and/or
// modify it under the terms of the GNU General Public License
// as published by the Free Software Foundation; either version 2
// of the License, or (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.

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
		
		public MethodDeclaration(string name, Modifier modifier, TypeReference typeReference, ArrayList parameters, ArrayList attributes) : base(modifier, attributes, name, parameters)
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
