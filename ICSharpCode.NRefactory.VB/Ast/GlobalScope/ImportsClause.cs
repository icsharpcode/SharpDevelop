// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)
using System;
using System.Collections.Generic;

namespace ICSharpCode.NRefactory.VB.Ast
{
	public abstract class ImportsClause : AstNode
	{
		
	}
	
	public class AliasImportsClause : ImportsClause
	{
		string name;
		
		TypeReference alias;
		

		
		public string Name {
			get {
				return name;
			}
			set {
				name = string.IsNullOrEmpty(value) ? "?" : value;
			}
		}
		
		public TypeReference Alias {
			get {
				return alias;
			}
			set {
				alias = value ?? TypeReference.Null;
				if (!alias.IsNull) alias.Parent = this;
			}
		}
		

		
		public AliasImportsClause(string name) {
			Name = name;
			alias = TypeReference.Null;
		}
		
		public AliasImportsClause(string name, TypeReference alias) {
			Name = name;
			Alias = alias;
		}
		
		public bool IsAlias {
			get {
				return !alias.IsNull;
			}
		}
		
		public override object AcceptVisitor(IAstVisitor visitor, object data) {
			return visitor.VisitImportsClause(this, data);
		}
		
		public override string ToString() {
			return string.Format("[ImportsClause Name={0} Alias={1} XmlPrefix={2}]", Name, Alias, XmlPrefix);
		}
	}
	
	public class MemberImportsClause : ImportsClause
	{
		
	}
	
	public class XmlNamespaceImportsClause : ImportsClause
	{
		string xmlPrefix;
		
		public string XmlPrefix {
			get { return xmlPrefix; }
			set { xmlPrefix = value ?? ""; }
		}
		
		string xmlNamespace;
		
		public string XmlNamespace {
			get { return xmlNamespace; }
			set { xmlNamespace = value ?? ""; }
		}
		
		public XmlNamespaceImportsClause(string xmlPrefix, string xmlNamespace)
		{
			this.xmlPrefix = xmlPrefix;
			this.xmlNamespace = xmlNamespace;
		}
		
		public XmlNamespaceImportsClause(string xmlNamespace)
		{
			this.xmlNamespace = xmlNamespace;
		}
		
		protected internal override bool DoMatch(AstNode other, ICSharpCode.NRefactory.PatternMatching.Match match)
		{
			var clause = other as XmlNamespaceImportsClause;
			return clause != null && clause.xmlNamespace == xmlNamespace && clause.xmlPrefix == xmlPrefix;
		}
		
		public override S AcceptVisitor<T, S>(IAstVisitor<T, S> visitor, T data)
		{
			return visitor.VisitXmlNamespaceImportsClause(this, data);
		}
		
		public override string ToString()
		{
			return string.Format("[XmlNamespaceImportsClause XmlPrefix={0}, XmlNamespace={1}]", xmlPrefix, xmlNamespace);
		}
	}
}
