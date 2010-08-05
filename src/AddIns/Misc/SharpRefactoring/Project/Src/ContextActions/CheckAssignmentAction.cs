// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Konicek" email="martin.konicek@gmail.com"/>
//     <version>$Revision: $</version>
// </file>
using System;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.Refactoring;
using ICSharpCode.SharpDevelop.Refactoring;

namespace SharpRefactoring.ContextActions
{
	/// <summary>
	/// Description of CheckAssignmentAction.
	/// </summary>
	public abstract class CheckAssignmentAction : ContextAction
	{
		protected string VariableName { get; private set; }
		
		protected CodeGenerator CodeGenerator { get; private set; }
		
		protected DomRegion ElementRegion { get; private set; }
		
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
		
		public override bool IsEnabled(EditorContext context)
		{
			this.VariableName = GetVariableName(context);
			this.CodeGenerator = GetCodeGenerator(context);
			this.ElementRegion = GetStatementRegion(context);
			return !string.IsNullOrEmpty(this.VariableName) && (this.CodeGenerator != null);
		}
	}
}
