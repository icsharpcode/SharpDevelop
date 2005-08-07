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
	public class Using : AbstractNode
	{
		string name;
		TypeReference alias;
		
		public string Name {
			get {
				return name;
			}
			set {
				name = value == null ? String.Empty : value;
			}
		}
		
		public TypeReference Alias {
			get {
				return alias;
			}
			set {
				alias = TypeReference.CheckNull(value);
			}
		}
		
		public bool IsAlias {
			get {
				return !alias.IsNull;
			}
		}
		
		public Using(string name, TypeReference alias)
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
		List<Using> usings;
		
		public List<Using> Usings {
			get {
				return usings;
			}
			set {
				usings = value == null ? new List<Using>(1) : value;
			}
		}
			
		public UsingDeclaration(string nameSpace) : this(nameSpace, null)
		{
		}
		
		public UsingDeclaration(string nameSpace, TypeReference alias)
		{
			Debug.Assert(nameSpace != null);
			usings = new List<Using>(1);
			usings.Add(new Using(nameSpace, alias));
		}
		
		public UsingDeclaration(List<Using> usings)
		{
			this.Usings = usings;
		}
		
		public override object AcceptVisitor(IASTVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}
		
		public override string ToString()
		{
			return String.Format("[UsingDeclaration: Usings={0}]",
			                     GetCollectionString(usings));
		}
	}
}
