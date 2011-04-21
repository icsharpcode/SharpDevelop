// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.IO;

using ICSharpCode.NRefactory.PatternMatching;

namespace ICSharpCode.NRefactory.VB
{
	/// <summary>
	/// Description of OutputVisitor.
	/// </summary>
	public class OutputVisitor : IAstVisitor<object, object>, IPatternAstVisitor<object, object>
	{
		readonly IOutputFormatter formatter;
		readonly VBFormattingOptions policy;
		
		readonly Stack<AstNode> containerStack = new Stack<AstNode>();
		readonly Stack<AstNode> positionStack = new Stack<AstNode>();
		
		public OutputVisitor(TextWriter textWriter, VBFormattingOptions formattingPolicy)
		{
			if (textWriter == null)
				throw new ArgumentNullException("textWriter");
			if (formattingPolicy == null)
				throw new ArgumentNullException("formattingPolicy");
			this.formatter = new TextWriterOutputFormatter(textWriter);
			this.policy = formattingPolicy;
		}
		
		public OutputVisitor(IOutputFormatter formatter, VBFormattingOptions formattingPolicy)
		{
			if (formatter == null)
				throw new ArgumentNullException("formatter");
			if (formattingPolicy == null)
				throw new ArgumentNullException("formattingPolicy");
			this.formatter = formatter;
			this.policy = formattingPolicy;
		}
		
		public object VisitPatternPlaceholder(AstNode placeholder, Pattern pattern, object data)
		{
			throw new NotImplementedException();
		}
		
		public object VisitVBTokenNode(ICSharpCode.NRefactory.VB.Ast.VBTokenNode vBTokenNode, object data)
		{
			throw new NotImplementedException();
		}
		
		public object VisitCompilationUnit(ICSharpCode.NRefactory.VB.Ast.CompilationUnit compilationUnit, object data)
		{
			// don't do node tracking as we visit all children directly
			foreach (AstNode node in compilationUnit.Children)
				node.AcceptVisitor(this, data);
			return null;
		}
		
		public object VisitBlockStatement(ICSharpCode.NRefactory.VB.Ast.BlockStatement blockStatement, object data)
		{
			throw new NotImplementedException();
		}
		
		public object VisitOptionStatement(ICSharpCode.NRefactory.VB.Ast.OptionStatement optionStatement, object data)
		{
			throw new NotImplementedException();
		}
		
		public object VisitImportsStatement(ICSharpCode.NRefactory.VB.Ast.ImportsStatement importsStatement, object data)
		{
			throw new NotImplementedException();
		}
		
		public object VisitAliasImportsClause(ICSharpCode.NRefactory.VB.Ast.AliasImportsClause aliasImportsClause, object data)
		{
			throw new NotImplementedException();
		}
		
		public object VisitMembersImportsClause(ICSharpCode.NRefactory.VB.Ast.MemberImportsClause membersImportsClause, object data)
		{
			throw new NotImplementedException();
		}
		
		public object VisitXmlNamespaceImportsClause(ICSharpCode.NRefactory.VB.Ast.XmlNamespaceImportsClause xmlNamespaceImportsClause, object data)
		{
			throw new NotImplementedException();
		}
		
		public object VisitAttribute(ICSharpCode.NRefactory.VB.Ast.Attribute attribute, object data)
		{
			throw new NotImplementedException();
		}
		
		public object VisitAttributeBlock(ICSharpCode.NRefactory.VB.Ast.AttributeBlock attributeBlock, object data)
		{
			throw new NotImplementedException();
		}
		
		public object VisitIdentifier(ICSharpCode.NRefactory.VB.Ast.Identifier identifier, object data)
		{
			throw new NotImplementedException();
		}
		
		public object VisitXmlIdentifier(ICSharpCode.NRefactory.VB.Ast.XmlIdentifier xmlIdentifier, object data)
		{
			throw new NotImplementedException();
		}
		
		public object VisitSimpleNameExpression(ICSharpCode.NRefactory.VB.Ast.SimpleNameExpression identifierExpression, object data)
		{
			throw new NotImplementedException();
		}
		
		public object VisitPrimitiveExpression(ICSharpCode.NRefactory.VB.Ast.PrimitiveExpression primitiveExpression, object data)
		{
			throw new NotImplementedException();
		}
		
		public object VisitPrimitiveType(ICSharpCode.NRefactory.VB.Ast.PrimitiveType primitiveType, object data)
		{
			throw new NotImplementedException();
		}
		
		public object VisitComposedType(ICSharpCode.NRefactory.VB.Ast.ComposedType composedType, object data)
		{
			throw new NotImplementedException();
		}
		
		public object VisitArraySpecifier(ICSharpCode.NRefactory.VB.Ast.ArraySpecifier arraySpecifier, object data)
		{
			throw new NotImplementedException();
		}
		
		public object VisitSimpleType(ICSharpCode.NRefactory.VB.Ast.SimpleType simpleType, object data)
		{
			throw new NotImplementedException();
		}
		
		public object VisitAnyNode(AnyNode anyNode, object data)
		{
			throw new NotImplementedException();
		}
		
		public object VisitBackreference(Backreference backreference, object data)
		{
			throw new NotImplementedException();
		}
		
		public object VisitChoice(Choice choice, object data)
		{
			throw new NotImplementedException();
		}
		
		public object VisitNamedNode(NamedNode namedNode, object data)
		{
			throw new NotImplementedException();
		}
		
		public object VisitRepeat(Repeat repeat, object data)
		{
			throw new NotImplementedException();
		}
		
		public object VisitOptionalNode(OptionalNode optionalNode, object data)
		{
			throw new NotImplementedException();
		}
		
		public object VisitIdentifierExpressionBackreference(IdentifierExpressionBackreference identifierExpressionBackreference, object data)
		{
			throw new NotImplementedException();
		}
		
		public object VisitXmlLiteralString(ICSharpCode.NRefactory.VB.Ast.XmlLiteralString xmlLiteralString, object data)
		{
			throw new NotImplementedException();
		}
	}
}
