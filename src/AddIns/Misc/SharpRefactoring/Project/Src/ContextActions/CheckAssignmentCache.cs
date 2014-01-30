// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
			this.context = context;
			this.VariableName = GetVariableName(context);
			this.CodeGenerator = GetCodeGenerator(context);
			this.Element = GetElement(context);
			this.ElementRegion = (Element == null) ? DomRegion.Empty : DomRegion.FromLocation(Element.StartLocation, Element.EndLocation);
		}
		
		EditorContext context;
		
		public bool IsActionAvailable
		{
			get {
				return !string.IsNullOrEmpty(this.VariableName) && (this.CodeGenerator != null) && (this.context.CurrentLine.Text.Contains(";"));
			}
		}
		
		public string VariableName { get; private set; }
		
		public CodeGenerator CodeGenerator { get; private set; }
		
		/// <summary>
		/// Either AssignmentExpression or LocalVariableDeclaration.
		/// </summary>
		public AbstractNode Element { get; private set; }
		
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
		
		protected AbstractNode GetElement(EditorContext context)
		{
			// a = Foo()      : AssignmentExpression.Left == IdentifierExpression(*identifier*)
			// var a = Foo()  : VariableDeclaration(*name*).Initializer != empty
			var assignment = context.GetContainingElement<AssignmentExpression>();
			if (assignment != null)
				return assignment;
			var declaration = context.GetContainingElement<LocalVariableDeclaration>();
			if (declaration != null)
				return declaration;
			
			return null;
		}
		
		string GetVariableNameFromAssignment(AssignmentExpression assignment)
		{
			if (assignment == null)
				return null;
			var identifier = assignment.Left as IdentifierExpression;
			if (identifier == null)
				return null;
			if ((!ExpressionCanBeNull(assignment.Right)) || ExpressionIsValueType(assignment.Right))
				// don't offer action where it makes no sense
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
			if (!ExpressionCanBeNull(varDecl.Initializer) || ExpressionIsValueType(varDecl.Initializer))
				// don't offer action where it makes no sense
				return null;
			return varDecl.Name;
		}
		
		bool ExpressionIsValueType(Expression expr)
		{
			ResolveResult rr = this.context.ResolveExpression(expr);
			if (rr == null) return false;	// when cannot resolve the assignment right, still offer the action
			return (rr.ResolvedType != null && rr.ResolvedType.IsReferenceType == false);
		}
		
		bool ExpressionCanBeNull(Expression expr)
		{
			if (expr == null) return false;
			if (expr.IsNull) return false;
			if (expr is PrimitiveExpression) return false;
			if (expr is IdentifierExpression) return true;
			if (expr is MemberReferenceExpression) return true;
			if (expr is InvocationExpression) return true;
			if (expr is CastExpression && ((CastExpression)expr).CastType == CastType.TryCast) return true;
			return false;
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
