// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under MIT X11 license (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory.VB.Ast;

namespace ICSharpCode.NRefactory.VB {
	public interface IAstVisitor<in T, out S> {
		// Global scope
		S VisitOptionStatement(OptionStatement optionStatement, T data);
		S VisitImportsStatement(ImportsStatement importsStatement, T data);
		S VisitAliasImportsClause(AliasImportsClause aliasImportsClause, T data);
		S VisitMembersImportsClause(MemberImportsClause membersImportsClause, T data);
		S VisitXmlNamespaceImportsClause(XmlNamespaceImportsClause xmlNamespaceImportsClause, T data);
		
		// Expression scope
		S VisitIdentifier(Identifier identifier, T data);
		S VisitIdentifierExpression(IdentifierExpression identifierExpression, T data);
		S VisitPrimitiveExpression(PrimitiveExpression primitiveExpression, T data);
	}
}
