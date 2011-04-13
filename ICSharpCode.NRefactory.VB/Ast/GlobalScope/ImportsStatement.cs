// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)
using System;
using System.Collections.Generic;

namespace ICSharpCode.NRefactory.VB.Ast
{
	public class ImportsStatement : AstNode
	{
		List<ImportsClause> importsClauses;
		
		public List<ImportsClause> ImportsClauses {
			get { return importsClauses; }
			set { importsClauses = value ?? new List<ImportsClause>(); }
		}
		
		public ImportsStatement(List<ImportsClause> importsClauses) {
			ImportsClauses = importsClauses;
		}
		
		public ImportsStatement(string xmlNamespace, string prefix)
		{
			importsClauses = new List<ImportsClause>(1);
			importsClauses.Add(new ImportsClause(xmlNamespace, prefix));
		}
		
		public ImportsStatement(string @namespace) : this(@namespace, TypeReference.Null) {}
		
		public ImportsStatement(string @namespace, TypeReference alias)
		{
			importsClauses = new List<ImportsClause>(1);
			importsClauses.Add(new ImportsClause(@namespace, alias));
		}
		
		public override string ToString() {
			return string.Format("[ImportsStatement ImportsClauses={0}]", GetCollectionString(ImportsClauses));
		}
		
		public override NodeType NodeType {
			get {
				 return NodeType.Unknown;
			}
		}
		
		protected internal override bool DoMatch(AstNode other, Match match)
		{
			ImportsStatement stmt = other as ImportsStatement;
			//return stmt != null && stmt.ImportsClauses
		}
		
		public override S AcceptVisitor<T, S>(IAstVisitor<T, S> visitor, T data)
		{
			return visitor.VisitImportsStatement(this, data);
		}
	}
	

	

	
}
