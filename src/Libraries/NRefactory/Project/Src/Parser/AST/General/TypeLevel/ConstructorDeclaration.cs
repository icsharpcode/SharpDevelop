// ConstructorDeclaration.cs
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

namespace ICSharpCode.NRefactory.Parser.AST
{
	public class ConstructorDeclaration : ParametrizedNode
	{
		ConstructorInitializer constructorInitializer = ConstructorInitializer.Null; // only for C#, can be null if no initializer is present
		BlockStatement         blockStatement         = BlockStatement.Null;
		
		public ConstructorInitializer ConstructorInitializer {
			get {
				return constructorInitializer;
			}
			set {
				constructorInitializer = value == null ? ConstructorInitializer.Null : value;
			}
		}
		
		public BlockStatement Body {
			get {
				return blockStatement;
			}
			set {
				blockStatement = BlockStatement.CheckNull(value);;
			}
		}
		
		public ConstructorDeclaration(string name, Modifier modifier, ArrayList parameters, ArrayList attributes) : base(modifier, attributes, name, parameters)
		{
		}
		
		public ConstructorDeclaration(string name, Modifier modifier, ArrayList parameters, ConstructorInitializer constructorInitializer, ArrayList attributes) : base(modifier, attributes, name, parameters)
		{
			this.ConstructorInitializer = constructorInitializer;
		}
		
		public override object AcceptVisitor(IASTVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}
		public override string ToString()
		{
			return String.Format("[ConstructorDeclaration: Name={0}, Modifier={1}, Parameters={2}, Attributes={3}, Body={4}]",
			                     Name,
			                     Modifier,
			                     GetCollectionString(Parameters),
			                     GetCollectionString(Attributes),
			                     Body);
		}
	}
}
