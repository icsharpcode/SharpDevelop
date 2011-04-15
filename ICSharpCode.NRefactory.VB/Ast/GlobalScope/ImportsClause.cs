// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)
using System;
using System.Collections.Generic;

namespace ICSharpCode.NRefactory.VB.Ast
{
	public abstract class ImportsClause : AstNode
	{
		public static readonly ImportsClause Null = new NullImportsClause();
		
		public static readonly Role<ImportsClause> ImportsClauseRole = new Role<ImportsClause>("ImportsClause", Null);
		
		class NullImportsClause : ImportsClause
		{
			public override bool IsNull {
				get { return true; }
			}
			
			protected internal override bool DoMatch(AstNode other, ICSharpCode.NRefactory.PatternMatching.Match match)
			{
				return other != null && other.IsNull;
			}
			
			public override S AcceptVisitor<T, S>(IAstVisitor<T, S> visitor, T data)
			{
				return default(S);
			}
		}
	}
	
	public class AliasImportsClause : ImportsClause
	{
		public Identifier Name { get; set; }
		
		AstType alias;
		
		public AstType Alias {
			get { return alias; }
			set { alias = value ?? AstType.Null; }
		}
		
		public AliasImportsClause(Identifier name) {
			Name = name;
			alias = AstType.Null;
		}
		
		public AliasImportsClause(Identifier name, AstType alias) {
			Name = name;
			Alias = alias;
		}
		
		protected internal override bool DoMatch(AstNode other, ICSharpCode.NRefactory.PatternMatching.Match match)
		{
			var clause = other as AliasImportsClause;
			return clause != null
				&& Name.DoMatch(clause.Name, match)
				&& alias.DoMatch(clause.Alias, match);
		}
		
		public override S AcceptVisitor<T, S>(IAstVisitor<T, S> visitor, T data)
		{
			return visitor.VisitAliasImportsClause(this, data);
		}
		
		public override string ToString() {
			return string.Format("[AliasImportsClause Name={0} Alias={1}]", Name, Alias);
		}
	}
	
	public class MemberImportsClause : ImportsClause
	{
		public AstType NamespaceOrType { get; set; }
		
		public MemberImportsClause(AstType namespaceOrType)
		{
			this.NamespaceOrType = namespaceOrType;
		}
		
		protected internal override bool DoMatch(AstNode other, ICSharpCode.NRefactory.PatternMatching.Match match)
		{
			var node = other as MemberImportsClause;
			return node != null
				&& NamespaceOrType.DoMatch(node.NamespaceOrType, match);
		}
		
		public override S AcceptVisitor<T, S>(IAstVisitor<T, S> visitor, T data)
		{
			return visitor.VisitMembersImportsClause(this, data);
		}
		
		public override string ToString()
		{
			return string.Format("[MemberImportsClause NamespaceOrType={0}]", NamespaceOrType);
		}

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
