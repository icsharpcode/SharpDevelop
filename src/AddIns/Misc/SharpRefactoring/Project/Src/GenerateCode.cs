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
using System.Collections.Generic;
using System.Linq;

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
	/// Provides generate member / class functionality for Unknown ResolveResults.
	/// </summary>
	public class GenerateCode
	{
		/// <summary>
		/// If given symbol is Unknown ResolveResult, returns action that can generate code for this missing symbol.
		/// </summary>
		public static GenerateCodeContextAction GetContextAction(EditorContext context)
		{
			if (context.CurrentSymbol is UnknownMethodResolveResult) {
				UnknownMethodResolveResult unknownMethodCall = (UnknownMethodResolveResult)context.CurrentSymbol;
				Ast.Expression expression = context.GetContainingElement<Ast.InvocationExpression>();
				if (expression == null)
					return null;
				
				string title = "${res:AddIns.SharpRefactoring.IntroduceMethod}";
				try {
					title = string.Format(StringParser.Parse("${res:AddIns.SharpRefactoring.IntroduceMethod}"), unknownMethodCall.CallName, unknownMethodCall.Target.FullyQualifiedName);
				} catch (FormatException) {
				}
				
				if (unknownMethodCall.Target != null && unknownMethodCall.Target.IsUserCode()) {
					// Don't introduce method on non-modyfiable types
					return new IntroduceMethodContextAction(unknownMethodCall, expression, context.Editor) {
						Title = title
					};
				}
			}
			return null;
		}
	}
	
	public abstract class GenerateCodeContextAction : IContextAction
	{
		public string Title { get; set; }
		
		public abstract void Execute();
	}
	
	#region Introduce method
	public class IntroduceMethodContextAction : GenerateCodeContextAction
	{
		public UnknownMethodResolveResult UnknownMethodCall { get; private set; }
		public Ast.Expression InvocationExpr { get; private set; }
		public ITextEditor Editor { get; private set; }
		
		public IntroduceMethodContextAction(UnknownMethodResolveResult symbol, Ast.Expression invocationExpr, ITextEditor editor)
		{
			if (symbol == null)
				throw new ArgumentNullException("rr");
			if (invocationExpr == null)
				throw new ArgumentNullException("ex");
			if (editor == null)
				throw new ArgumentNullException("editor");
			this.UnknownMethodCall = symbol;
			this.InvocationExpr = invocationExpr;
			this.Editor = editor;
		}
		
		public override void Execute()
		{
			IClass targetClass = UnknownMethodCall.Target.GetUnderlyingClass();
			if (targetClass == null)
				return;
			bool isNew = false;
			object result = null;
			
			if (targetClass is CompoundClass) {
				var cc = targetClass as CompoundClass;
				if (cc.Parts.Any(c => UnknownMethodCall.CallingClass.DotNetName == c.DotNetName))
					targetClass = UnknownMethodCall.CallingClass;
				else
					targetClass = cc.Parts.FirstOrDefault(c => !c.BodyRegion.IsEmpty)
						?? targetClass;
			}
			
			if (!targetClass.IsUserCode()) {
				IntroduceMethodDialog dialog = new IntroduceMethodDialog(UnknownMethodCall.CallingClass);
				dialog.Owner = WorkbenchSingleton.MainWindow;
				
				if (dialog.ShowDialog() != true)
					return;
				
				isNew = dialog.IsNew;
				result = dialog.Result;
			}
			
			ExecuteIntroduceMethod(UnknownMethodCall, InvocationExpr, Editor, isNew, result);
		}
		
		internal void ExecuteIntroduceMethod(UnknownMethodResolveResult rr, Ast.Expression invocationExpr, ITextEditor editor, bool isNew, object result)
		{
			IClass targetClass = IsEqualClass(rr.CallingClass, rr.Target.GetUnderlyingClass()) ? rr.CallingClass
				: rr.Target.GetUnderlyingClass();
			
			CodeGenerator gen = targetClass.ProjectContent.Language.CodeGenerator;
			IAmbience ambience = targetClass.ProjectContent.Language.GetAmbience();
			
			ClassFinder finder = new ClassFinder(rr.CallingMember);
			
			ModifierEnum modifiers = ModifierEnum.None;
			
			bool isExtension = !targetClass.IsUserCode();
			
			if (IsEqualClass(rr.CallingClass, targetClass)) {
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
			
			NRefactoryResolver resolver = Extensions.CreateResolverForContext(targetClass.ProjectContent.Language, editor);
			
			IReturnType type = resolver.GetExpectedTypeFromContext(invocationExpr);
			Ast.TypeReference typeRef = CodeGenerator.ConvertType(type, finder);
			
			if (typeRef.IsNull) {
				if (invocationExpr.Parent is Ast.ExpressionStatement)
					typeRef = new Ast.TypeReference("void", true);
				else
					typeRef = new Ast.TypeReference("object", true);
			}
			
			Ast.MethodDeclaration method = new Ast.MethodDeclaration {
				Name = rr.CallName,
				Modifier = CodeGenerator.ConvertModifier(modifiers, finder),
				TypeReference = typeRef,
				Parameters = CreateParameters(rr, finder, invocationExpr as Ast.InvocationExpression).ToList(),
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
				if (IsEqualClass(rr.CallingClass, targetClass))
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
					targetClass = info.CompilationUnit.Classes.Flatten(c => c.InnerClasses).FirstOrDefault(c => c.DotNetName == targetClass.DotNetName);
				
				if (targetClass == null)
					return;
				
				if (IsEqualClass(rr.CallingClass, targetClass)) {
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
		
		bool IsEqualClass(IClass a, IClass b)
		{
			return a.DotNetName == b.DotNetName;
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
	}
	#endregion
}
