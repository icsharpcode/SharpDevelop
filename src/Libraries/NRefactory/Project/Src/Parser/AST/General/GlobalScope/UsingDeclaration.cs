// UsingDeclaration.cs
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
	public class Using : AbstractNode
	{
		string name;
		string alias;
		
		public string Name {
			get {
				return name;
			}
			set {
				name = value == null ? String.Empty : value;
			}
		}
		
		public string Alias {
			get {
				return alias;
			}
			set {
				alias = value == null ? String.Empty : value;
			}
		}
		
		public bool IsAlias {
			get {
				Debug.Assert(alias != null);
				return alias.Length > 0;
			}
		}
		
		public Using(string name, string alias)
		{
			this.Name = name;
			this.Alias = alias;
		}
		
		public Using(string name) : this(name, null)
		{
			
		}
		
		public override object AcceptVisitor(IASTVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}
		
		public override string ToString() {
			return String.Format("[Using: name = {0}, alias = {1}]",
			                     name,
			                     alias);
		}
	}
	
	public class UsingDeclaration : AbstractNode
	{
//		List<Using> namespaces;
		ArrayList usings;
		
		public ArrayList Usings {
			get {
				return usings;
			}
			set {
				usings = value == null ? new ArrayList(1) : value;
			}
		}
			
		public UsingDeclaration(string nameSpace) : this(nameSpace, null)
		{
		}
		
		public UsingDeclaration(string nameSpace, string alias)
		{
			Debug.Assert(nameSpace != null);
			usings = new ArrayList(1);
			usings.Add(new Using(nameSpace, alias));
		}
		
		public UsingDeclaration(ArrayList usings)
		{
			this.Usings = usings;
		}
		
		public override object AcceptVisitor(IASTVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}
		
		public override string ToString()
		{
			return String.Format("[UsingDeclaration: Namespace={0}]",
			                     GetCollectionString(usings));
		}
	}
}
