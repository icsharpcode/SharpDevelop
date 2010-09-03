// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.Refactoring;
using ICSharpCode.SharpDevelop.Refactoring;

namespace SharpRefactoring.ContextActions
{
	/// <summary>
	/// Caches common data for CheckAssignmentNull and CheckAssignmentNotNull.
	/// </summary>
	public class CheckAssignmentCache : IContextActionCache
	{
		public void Initialize(EditorContext context)
		{
			this.VariableName = GetVariableName(context);
			this.CodeGenerator = GetCodeGenerator(context);
			this.ElementRegion = GetStatementRegion(context);
		}
		
		public bool IsActionAvailable
		{
			get {
				return !string.IsNullOrEmpty(this.VariableName) && (this.CodeGenerator != null);
			}
		}
		
		public string VariableName { get; private set; }
		
		public CodeGenerator CodeGenerator { get; private set; }
		
		public DomRegion ElementRegion { get; private set; }
		
		protected string GetVariableName(EditorContext context)
		{
			// a = Foo()      : AssignmentExpression.Left == IdentifierExpression(*identifier*)
			// var a = Foo()  : VariableDeclaration(*name*).Initializer != empty
			
			var variableName = GetVariableNameFromAssignment(context.GetContainingElement<AssignmentExpression>());
			if (variableName != null)
				return variableName;
			variableName = GetVariableNameFromVariableDeclaration(context.GetContainingElement<LocalVariableDeclaration>());
			if (variableName != null)
				return variableName;
			
			return null;
		}
		
		protected DomRegion GetStatementRegion(EditorContext context)
		{
			// a = Foo()      : AssignmentExpression.Left == IdentifierExpression(*identifier*)
			// var a = Foo()  : VariableDeclaration(*name*).Initializer != empty
			
			var assignment = context.GetContainingElement<AssignmentExpression>();
			if (assignment != null)
				return DomRegion.FromLocation(assignment.StartLocation, assignment.EndLocation);
			var declaration = context.GetContainingElement<LocalVariableDeclaration>();
			if (declaration != null)
				return DomRegion.FromLocation(declaration.StartLocation, declaration.EndLocation);
			
			return DomRegion.Empty;
		}
		
		string GetVariableNameFromAssignment(AssignmentExpression assignment)
		{
			if (assignment == null)
				return null;
			var identifier = assignment.Left as IdentifierExpression;
			if (identifier == null)
				return null;
			if (assignment.Right is ObjectCreateExpression)
				// // don't offer action for "a = new Foo()"
				return null;
			return identifier.Identifier;
		}
		
		string GetVariableNameFromVariableDeclaration(LocalVariableDeclaration declaration)
		{
			if (declaration == null)
				return null;
			if (declaration.Variables.Count != 1)
				return null;
			VariableDeclaration varDecl = declaration.Variables[0];
			if (!varDecl.Initializer.IsNull &&
			    // don't offer action for "var a = new Foo()"
			    !(varDecl.Initializer is ObjectCreateExpression))
				return varDecl.Name;
			return null;
		}
		
		CodeGenerator GetCodeGenerator(EditorContext context)
		{
			var parseInfo = ParserService.GetParseInformation(context.Editor.FileName);
			if (parseInfo == null)
				return null;
			return parseInfo.CompilationUnit.Language.CodeGenerator;
		}
		
		public IReturnType GetResolvedType(ResolveResult symbol)
		{
			if (symbol != null)
				return symbol.ResolvedType;
			return null;
		}
	}
}
