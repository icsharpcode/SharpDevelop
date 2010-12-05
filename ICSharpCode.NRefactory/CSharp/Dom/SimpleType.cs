// 
// FullTypeName.cs
//
// Author:
//       Mike Krüger <mkrueger@novell.com>
// 
// Copyright (c) 2010 Novell, Inc (http://www.novell.com)
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Linq;

namespace ICSharpCode.NRefactory.CSharp
{
	public class SimpleType : DomNode
	{
		public const int AliasRole = 100;
		
		public override NodeType NodeType {
			get {
				return NodeType.Type;
			}
		}
		
		/// <summary>
		/// Gets whether this simple type is qualified with an alias
		/// </summary>
		public bool IsQualifiedWithAlias {
			get {
				return GetChildByRole (AliasRole) != null;
			}
		}
		
		public Identifier AliasIdentifier {
			get {
				return (Identifier)GetChildByRole (AliasRole) ?? CSharp.Identifier.Null;
			}
		}
		
		public Identifier IdentifierToken {
			get {
				return (Identifier)GetChildByRole (Roles.Identifier) ?? CSharp.Identifier.Null;
			}
		}
		
		public string Identifier {
			get { return IdentifierToken.Name; }
		}
		
		public IEnumerable<DomNode> TypeArguments {
			get {
				return GetChildrenByRole (Roles.TypeArgument);
			}
		}
		
		public override S AcceptVisitor<T, S> (DomVisitor<T, S> visitor, T data)
		{
			return visitor.VisitSimpleType (this, data);
		}
		
		public override string ToString()
		{
			return Identifier ?? base.ToString();
		}
	}
}

