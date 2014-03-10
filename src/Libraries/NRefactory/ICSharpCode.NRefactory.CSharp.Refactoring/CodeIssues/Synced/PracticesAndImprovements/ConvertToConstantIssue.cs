//
// ConvertToConstantIssue.cs
//
// Author:
//       Mike Krüger <mkrueger@xamarin.com>
//
// Copyright (c) 2013 Xamarin Inc. (http://xamarin.com)
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using System.Collections.Generic;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.Refactoring;
using ICSharpCode.NRefactory.CSharp.Analysis;
using System.Linq;
using ICSharpCode.NRefactory.CSharp.Resolver;
using System.Threading;
using ICSharpCode.NRefactory.TypeSystem.Implementation;
using System;
using System.Diagnostics;
using ICSharpCode.NRefactory.Utils;
using ICSharpCode.NRefactory.CSharp.Refactoring.ExtractMethod;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	[IssueDescription("Convert local variable or field to constant",
	                  Description = "Convert local variable or field to constant",
	                  Category = IssueCategories.PracticesAndImprovements,
	                  Severity = Severity.Suggestion,
	                  AnalysisDisableKeyword = "ConvertToConstant.Local")]
	public class ConvertToConstantIssue : GatherVisitorCodeIssueProvider
	{
		protected override IGatherVisitor CreateVisitor(BaseRefactoringContext context)
		{
			return new GatherVisitor(context);
		}

		internal static IEnumerable<FieldDeclaration> CollectFields<T>(GatherVisitorBase<T> provider, TypeDeclaration typeDeclaration) where T : CodeIssueProvider
		{
			var fieldVisitor = new ConvertToConstantIssue.FieldCollectVisitor<T>(provider.Ctx, typeDeclaration);
			typeDeclaration.AcceptVisitor(fieldVisitor);
			return fieldVisitor.CollectedFields;
		}

		class FieldCollectVisitor<T> : GatherVisitorBase<T>  where T : CodeIssueProvider
		{
			readonly TypeDeclaration typeDeclaration;
			public readonly List<FieldDeclaration> CollectedFields = new List<FieldDeclaration>();

			public FieldCollectVisitor(BaseRefactoringContext context, TypeDeclaration typeDeclaration) : base (context)
			{
				this.typeDeclaration = typeDeclaration;
			}

			public override void VisitFieldDeclaration(FieldDeclaration fieldDeclaration)
			{
				if (IsSuppressed(fieldDeclaration.StartLocation))
					return;
				CollectedFields.Add(fieldDeclaration); 
			}

			public override void VisitTypeDeclaration(TypeDeclaration typeDeclaration)
			{
				if (typeDeclaration != this.typeDeclaration)
					return;
				base.VisitTypeDeclaration(typeDeclaration);
			}

			public override void VisitBlockStatement(BlockStatement blockStatement)
			{
				// SKIP
			}
		}

		class GatherVisitor : GatherVisitorBase<ConvertToConstantIssue>
		{
			readonly Stack<List<Tuple<VariableInitializer, IVariable>>> fieldStack = new Stack<List<Tuple<VariableInitializer, IVariable>>>();

			public GatherVisitor(BaseRefactoringContext context) : base (context)
			{
			}

			void Collect()
			{
				foreach (var varDecl in fieldStack.Peek()) {
					AddIssue(new CodeIssue(
						varDecl.Item1.NameToken,
						ctx.TranslateString("Convert to constant"),
						ctx.TranslateString("To const"),
						script => {
							var constVarDecl = (FieldDeclaration)varDecl.Item1.Parent;
							script.ChangeModifier(constVarDecl, (constVarDecl.Modifiers & ~Modifiers.Static) | Modifiers.Const);
						}
					));
				}
			}


			public override void VisitBlockStatement(BlockStatement blockStatement)
			{
				base.VisitBlockStatement(blockStatement);
				if (blockStatement.Parent is EntityDeclaration || blockStatement.Parent is Accessor) {
					var assignmentAnalysis = new VariableUsageAnalyzation (ctx);
					var newVars = new List<Tuple<VariableInitializer, IVariable>>();
					blockStatement.AcceptVisitor(assignmentAnalysis); 
					foreach (var variable in fieldStack.Pop()) {
						if (assignmentAnalysis.GetStatus(variable.Item2) == VariableState.Changed)
							continue;
						newVars.Add(variable);
					}
					fieldStack.Push(newVars);
				}
			}

			static bool IsValidConstType(IType type)
			{
				var def = type.GetDefinition();
				if (def == null)
					return false;
				return KnownTypeCode.Boolean <= def.KnownTypeCode && def.KnownTypeCode <= KnownTypeCode.Decimal ||
					def.KnownTypeCode == KnownTypeCode.String;
			}

			public override void VisitTypeDeclaration(TypeDeclaration typeDeclaration)
			{
				var list = new List<Tuple<VariableInitializer, IVariable>>();
				fieldStack.Push(list);
				foreach (var fieldDeclaration in ConvertToConstantIssue.CollectFields(this, typeDeclaration)) {
					if (IsSuppressed(fieldDeclaration.StartLocation))
						continue;
					if (fieldDeclaration.Modifiers.HasFlag (Modifiers.Const) || fieldDeclaration.Modifiers.HasFlag (Modifiers.Readonly))
						continue;
					if (fieldDeclaration.HasModifier(Modifiers.Public) || fieldDeclaration.HasModifier(Modifiers.Protected) || fieldDeclaration.HasModifier(Modifiers.Internal))
						continue;
					if (fieldDeclaration.Variables.Any (v => !ctx.Resolve (v.Initializer).IsCompileTimeConstant))
						continue;
					var rr = ctx.Resolve(fieldDeclaration.ReturnType);
					if (!IsValidConstType(rr.Type))
						continue;
					if (fieldDeclaration.Variables.Count() > 1)
						continue;
					var variable = fieldDeclaration.Variables.First();
					var mr = ctx.Resolve(variable) as MemberResolveResult;
					if (mr == null || !(mr.Member is IVariable))
						continue;
					list.Add(Tuple.Create(variable, (IVariable)mr.Member)); 
				}
				base.VisitTypeDeclaration(typeDeclaration);
				Collect();
				fieldStack.Pop();
			}

			public override void VisitVariableDeclarationStatement (VariableDeclarationStatement varDecl)
			{
				base.VisitVariableDeclarationStatement(varDecl);
				if (varDecl.Modifiers.HasFlag (Modifiers.Const) || varDecl.Role == ForStatement.InitializerRole)
					return;
				if (varDecl.Variables.Count () > 1)
					return;
				if (varDecl.Variables.Any (v => !ctx.Resolve (v.Initializer).IsCompileTimeConstant))
					return;
				var containingBlock = varDecl.GetParent<BlockStatement> ();
				if (containingBlock == null)
					return;

				var returnTypeRR = ctx.Resolve(varDecl.Type);
				if (returnTypeRR.Type.IsReferenceType.HasValue && returnTypeRR.Type.IsReferenceType.Value)
					return;

				var variable = varDecl.Variables.First();
				var vr = ctx.Resolve(variable) as LocalResolveResult;
				if (vr == null)
					return;

				if (ctx.Resolve(variable.Initializer).ConstantValue == null)
					return;

				var assignmentAnalysis = new VariableUsageAnalyzation (ctx);

				containingBlock.AcceptVisitor(assignmentAnalysis);

				if (assignmentAnalysis.GetStatus(vr.Variable) == VariableState.Changed)
					return;
				AddIssue (new CodeIssue(
					variable.NameToken,
					ctx.TranslateString ("Convert to constant"),
					ctx.TranslateString ("To const"),
					script => {
						var constVarDecl = (VariableDeclarationStatement)varDecl.Clone ();
						constVarDecl.Modifiers |= Modifiers.Const;
						if (varDecl.Type.IsVar()) {
							var builder = ctx.CreateTypeSystemAstBuilder(varDecl);
							constVarDecl.Type = builder.ConvertType (ctx.Resolve(varDecl.Type).Type);
						}
						script.Replace (varDecl, constVarDecl);
					}
				));
			}
		}
	

		public class VariableUsageAnalyzation : DepthFirstAstVisitor
		{
			readonly BaseRefactoringContext context;

			readonly Dictionary<IVariable, VariableState> states = new Dictionary<IVariable, VariableState> ();

			TextLocation startLocation = TextLocation.Empty;
			TextLocation endLocation = TextLocation.Empty;

			public VariableUsageAnalyzation (BaseRefactoringContext context)
			{
				this.context = context;
			}

			public bool Has(IVariable variable)
			{
				return states.ContainsKey (variable);
			}

			public void SetAnalyzedRange(AstNode start, AstNode end, bool startInclusive = true, bool endInclusive = true)
			{
				if (start == null)
					throw new ArgumentNullException("start");
				if (end == null)
					throw new ArgumentNullException("end");
				startLocation = startInclusive ? start.StartLocation : start.EndLocation;
				endLocation = endInclusive ? end.EndLocation : end.StartLocation;
				states.Clear ();
			}

			public VariableState GetStatus (IVariable variable)
			{
				VariableState state;
				if (!states.TryGetValue (variable, out state))
					return VariableState.None;
				return state;
			}

			void SetState (ResolveResult rr, VariableState state)
			{
				IVariable variable = null;
				var lr = rr as LocalResolveResult;
				if (lr != null)
					variable = lr.Variable;
				var mr = rr as MemberResolveResult;
				if (mr != null)
					variable = mr.Member as IVariable;
				if (variable == null)
					return;
				var sv = variable as SpecializedField;
				if (sv != null) {
					variable = sv.MemberDefinition as IVariable;
				}
				VariableState oldState;
				if (states.TryGetValue (variable, out oldState)) {
					if (oldState < state)
						states [variable] = state;
				} else {
					states [variable] = state;
				}
			}

			public override void VisitIdentifierExpression(IdentifierExpression identifierExpression)
			{
				if (startLocation.IsEmpty || startLocation <= identifierExpression.StartLocation && identifierExpression.EndLocation <= endLocation) {
					SetState (context.Resolve(identifierExpression), VariableState.Used);
				}
			}

			public override void VisitAssignmentExpression(AssignmentExpression assignmentExpression)
			{
				if (startLocation.IsEmpty || startLocation <= assignmentExpression.StartLocation && assignmentExpression.EndLocation <= endLocation) {
					SetState(context.Resolve(assignmentExpression.Left), VariableState.Changed);
				}
				base.VisitAssignmentExpression (assignmentExpression);
			}

			public override void VisitDirectionExpression(DirectionExpression directionExpression)
			{
				if (startLocation.IsEmpty || startLocation <= directionExpression.StartLocation && directionExpression.EndLocation <= endLocation) {
					SetState(context.Resolve(directionExpression.Expression), VariableState.Changed);
				}
				base.VisitDirectionExpression (directionExpression);
			}

			public override void VisitUnaryOperatorExpression(UnaryOperatorExpression unaryOperatorExpression)
			{
				if (startLocation.IsEmpty || startLocation <= unaryOperatorExpression.StartLocation && unaryOperatorExpression.EndLocation <= endLocation) {
					if (unaryOperatorExpression.Operator == UnaryOperatorType.Increment || unaryOperatorExpression.Operator == UnaryOperatorType.Decrement ||
					    unaryOperatorExpression.Operator == UnaryOperatorType.PostIncrement || unaryOperatorExpression.Operator == UnaryOperatorType.PostDecrement) {
						SetState(context.Resolve(unaryOperatorExpression.Expression), VariableState.Changed);
					}
				}
				base.VisitUnaryOperatorExpression (unaryOperatorExpression);
			}

			public override void VisitNamedExpression(NamedExpression namedExpression)
			{
				SetState(context.Resolve(namedExpression), VariableState.Changed);
				base.VisitNamedExpression(namedExpression);
			}

		}
	}
}

