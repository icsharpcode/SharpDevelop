// DelegateDeclaration.cs
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

namespace ICSharpCode.NRefactory.Parser.AST
{
	public class DelegateDeclaration : AttributedNode
	{
		string          name = "";
		TypeReference   returnType = TypeReference.Null;
//		List<ParameterDeclarationExpression> parameters = new List<ParameterDeclarationExpression>(1);
		ArrayList parameters = new ArrayList(1);
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
		
		public ArrayList Parameters {
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
		
		public DelegateDeclaration(Modifier modifier, ArrayList attributes) : base(modifier, attributes)
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
