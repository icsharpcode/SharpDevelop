// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="siegfriedpammer@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

using ICSharpCode.Core;
using ICSharpCode.NRefactory.Visitors;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.NRefactoryResolver;
using ICSharpCode.SharpDevelop.Dom.Refactoring;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Refactoring;
using SharpRefactoring.Gui;
using Ast = ICSharpCode.NRefactory.Ast;

namespace SharpRefactoring
{
	/// <summary>
	/// Description of IntroduceMethod.
	/// </summary>
	public class IntroduceMethod : IRefactoringMenuItemFactory
	{
		public MenuItem Create(RefactoringMenuContext context)
		{
			if (context.ExpressionResult.Context == ExpressionContext.Attribute)
				return null;
			if (!(context.ResolveResult is UnknownMethodResolveResult))
				return null;
			
			UnknownMethodResolveResult rr = context.ResolveResult as UnknownMethodResolveResult;
			ITextEditor editor = context.Editor;
			
			Ast.Expression ex = GetExpressionInContext(rr, editor);
			
			if (ex == null)
				return null;
			
			MenuItem item = new MenuItem() {
				Header = string.Format(StringParser.Parse("${res:AddIns.SharpRefactoring.IntroduceMethod}"), rr.CallName, rr.Target.FullyQualifiedName),
				Icon = ClassBrowserIconService.GotoArrow.CreateImage()
			};
			
			item.Click += delegate {
				IClass targetClass = rr.Target.GetUnderlyingClass();
				bool isNew = false;
				object result = null;
				
				if (targetClass.BodyRegion.IsEmpty) {
					IntroduceMethodDialog dialog = new IntroduceMethodDialog(rr.CallingClass);
					dialog.Owner = WorkbenchSingleton.MainWindow;
					
					if (dialog.ShowDialog() != true)
						return;
					
					isNew = dialog.IsNew;
					result = dialog.Result;
				}
				
				ExecuteIntroduceMethod(rr, ex, editor, isNew, result);
			};
			
			return item;
		}
		
		internal static NRefactoryResolver CreateResolverForContext(LanguageProperties language, ITextEditor context)
		{
			NRefactoryResolver resolver = new NRefactoryResolver(language);
			resolver.Initialize(ParserService.GetParseInformation(context.FileName), context.Caret.Line, context.Caret.Column);
			return resolver;
		}

		internal static Ast.Expression GetExpressionInContext(UnknownMethodResolveResult rr, ITextEditor editor)
		{
			if (rr.Target == null || rr.Target.GetUnderlyingClass() == null)
				return null;
			
			NRefactoryResolver resolver = CreateResolverForContext(rr.CallingClass.ProjectContent.Language, editor);
			Ast.INode node = resolver.ParseCurrentMember(editor.Document.Text);
			resolver.RunLookupTableVisitor(node);

			InvocationExpressionLookupVisitor visitor = new InvocationExpressionLookupVisitor(editor);
			node.AcceptVisitor(visitor, null);
			return visitor.Expression;
		}
		
		internal void ExecuteIntroduceMethod(UnknownMethodResolveResult rr, Ast.Expression expression, ITextEditor editor, bool isNew, object result)
		{
			IClass targetClass = rr.Target.GetUnderlyingClass();
			
			CodeGenerator gen = targetClass.ProjectContent.Language.CodeGenerator;
			IAmbience ambience = targetClass.ProjectContent.Language.GetAmbience();
			
			ClassFinder finder = new ClassFinder(rr.CallingMember);
			
			ModifierEnum modifiers = ModifierEnum.None;
			
			bool isExtension = targetClass.BodyRegion.IsEmpty;
			
			if (rr.CallingClass == targetClass) {
				if (rr.CallingMember != null)
					modifiers |= (rr.CallingMember.Modifiers & ModifierEnum.Static);
			} else {
				if (isExtension) {
					if (isNew)
						targetClass = rr.CallingClass;
					else
						targetClass = result as IClass;
				}
				// exclude in Unit Test mode
				if (WorkbenchSingleton.Workbench != null)
					editor = (FileService.OpenFile(targetClass.CompilationUnit.FileName) as ITextEditorProvider).TextEditor;
				if (targetClass.ClassType != ClassType.Interface)
					modifiers |= ModifierEnum.Public;
				if (rr.IsStaticContext)
					modifiers |= ModifierEnum.Static;
			}
			
			NRefactoryResolver resolver = CreateResolverForContext(targetClass.ProjectContent.Language, editor);
			
			IReturnType type = resolver.GetExpectedTypeFromContext(expression);
			Ast.TypeReference typeRef = CodeGenerator.ConvertType(type, finder);
			
			if (typeRef.IsNull) {
				if (expression.Parent is Ast.ExpressionStatement)
					typeRef = new Ast.TypeReference("void", true);
				else
					typeRef = new Ast.TypeReference("object", true);
			}
			
			Ast.MethodDeclaration method = new Ast.MethodDeclaration {
				Name = rr.CallName,
				Modifier = CodeGenerator.ConvertModifier(modifiers, finder),
				TypeReference = typeRef,
				Parameters = CreateParameters(rr, finder, expression as Ast.InvocationExpression).ToList(),
			};
			
			if (targetClass.ClassType != ClassType.Interface)
				method.Body = CodeGenerator.CreateNotImplementedBlock();
			
			RefactoringDocumentAdapter documentWrapper = new RefactoringDocumentAdapter(editor.Document);
			
			if (isExtension) {
				method.Parameters.Insert(0, new Ast.ParameterDeclarationExpression(CodeGenerator.ConvertType(rr.Target, finder), "thisInstance"));
				method.IsExtensionMethod = true;
				method.Modifier |= Ast.Modifiers.Static;
			}
			
			if (isNew) {
				Ast.TypeDeclaration newType = new Ast.TypeDeclaration(isExtension ? Ast.Modifiers.Static : Ast.Modifiers.None, null);
				newType.Name = result as string;
				newType.AddChild(method);
				gen.InsertCodeAfter(targetClass, documentWrapper, newType);
			} else {
				if (rr.CallingClass == targetClass)
					gen.InsertCodeAfter(rr.CallingMember, documentWrapper, method);
				else
					gen.InsertCodeAtEnd(targetClass.BodyRegion, documentWrapper, method);
			}
			
			if (targetClass.ClassType == ClassType.Interface)
				return;
			
			ParseInformation info = ParserService.ParseFile(targetClass.CompilationUnit.FileName);
			if (info != null) {
				IMember newMember;
				
				if (isNew)
					targetClass = info.CompilationUnit.Classes.FirstOrDefault(c => c.DotNetName == c.Namespace + "." + (result as string));
				else
					targetClass = info.CompilationUnit.Classes.FirstOrDefault(c => c.DotNetName == targetClass.DotNetName);
				
				if (rr.CallingClass.DotNetName == targetClass.DotNetName) {
					newMember = targetClass.GetInnermostMember(editor.Caret.Line, editor.Caret.Column);
					newMember = targetClass.AllMembers
						.OrderBy(m => m.BodyRegion.BeginLine)
						.ThenBy(m2 => m2.BodyRegion.BeginColumn)
						.First(m3 => m3.BodyRegion.BeginLine > newMember.BodyRegion.BeginLine);
				} else {
					newMember = targetClass.Methods.Last();
				}
				
				IDocumentLine line = editor.Document.GetLine(newMember.BodyRegion.BeginLine + 2);
				int indentLength = DocumentUtilitites.GetWhitespaceAfter(editor.Document, line.Offset).Length;
				editor.Select(line.Offset + indentLength, "throw new NotImplementedException();".Length);
			}
		}
		
		IEnumerable<Ast.ParameterDeclarationExpression> CreateParameters(UnknownMethodResolveResult rr, ClassFinder context, Ast.InvocationExpression invocation)
		{
			List<string> usedNames = new List<string>();
			
			for (int i = 0; i < rr.Arguments.Count; i++) {
				IReturnType type = rr.Arguments[i];
				
				if (type is LambdaReturnType)
					type = (type as LambdaReturnType).ToDefaultDelegate();
				
				Ast.TypeReference typeRef = CodeGenerator.ConvertType(type, context);
				typeRef = typeRef.IsNull ? new Ast.TypeReference("object", true) : typeRef;
				
				Ast.Expression ex = invocation.Arguments[i];
				string paramName = IsNumericType(type) ? "num" + i : type.Name + i.ToString();
				
				if (ex is Ast.IdentifierExpression) {
					paramName = (ex as Ast.IdentifierExpression).Identifier;
				}
				
				if (ex is Ast.MemberReferenceExpression) {
					paramName = (ex as Ast.MemberReferenceExpression).MemberName;
				}
				
				Ast.ParameterModifiers mod = Ast.ParameterModifiers.None;
				
				if (ex is Ast.DirectionExpression) {
					var dex = ex as Ast.DirectionExpression;
					
					if (dex.Expression is Ast.IdentifierExpression) {
						paramName = (dex.Expression as Ast.IdentifierExpression).Identifier;
					}
					
					if (dex.Expression is Ast.MemberReferenceExpression) {
						paramName = (dex.Expression as Ast.MemberReferenceExpression).MemberName;
					}
					
					mod = dex.FieldDirection == Ast.FieldDirection.Out ? Ast.ParameterModifiers.Out : (dex.FieldDirection == Ast.FieldDirection.Ref ? Ast.ParameterModifiers.Ref : Ast.ParameterModifiers.None);
				}
				
				paramName = rr.CallingClass.ProjectContent.Language.CodeGenerator.GetParameterName(paramName);
				
				if (usedNames.Contains(paramName))
					paramName += i.ToString();
				
				usedNames.Add(paramName);
				
				yield return new Ast.ParameterDeclarationExpression(typeRef, paramName) {
					ParamModifier = mod
				};
			}
		}
		
		bool IsNumericType(IReturnType type)
		{
			return type.FullyQualifiedName == "System.Int32" ||
				type.FullyQualifiedName == "System.Int16" ||
				type.FullyQualifiedName == "System.Int64" ||
				type.FullyQualifiedName == "System.Single" ||
				type.FullyQualifiedName == "System.Double" ||
				type.FullyQualifiedName == "System.UInt16" ||
				type.FullyQualifiedName == "System.UInt32" ||
				type.FullyQualifiedName == "System.UInt64";
		}
		
		class InvocationExpressionLookupVisitor : AbstractAstVisitor
		{
			ITextEditor editor;
			Ast.InvocationExpression expression;
			
			public Ast.InvocationExpression Expression {
				get { return expression; }
			}
			
			public InvocationExpressionLookupVisitor(ITextEditor editor)
			{
				this.editor = editor;
				this.expression = null;
			}
			
			public override object VisitInvocationExpression(Ast.InvocationExpression invocationExpression, object data)
			{
				int startOffset = editor.Document.PositionToOffset(invocationExpression.TargetObject.StartLocation.Line, invocationExpression.TargetObject.StartLocation.Column);
				int endOffset = editor.Document.PositionToOffset(invocationExpression.EndLocation.Line, invocationExpression.EndLocation.Column);
				
				int offset = editor.Caret.Offset;
				
				if (offset >= startOffset && offset <= endOffset)
					expression = invocationExpression;
				
				return base.VisitInvocationExpression(invocationExpression, data);
			}
		}
	}
}
