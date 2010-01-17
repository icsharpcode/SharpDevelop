// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Input;

using ICSharpCode.Core;
using ICSharpCode.Core.Presentation;
using ICSharpCode.NRefactory.Visitors;
using ICSharpCode.SharpDevelop.Bookmarks;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.NRefactoryResolver;
using ICSharpCode.SharpDevelop.Dom.Refactoring;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Gui.ClassBrowser;
using ICSharpCode.SharpDevelop.Project;
using Ast = ICSharpCode.NRefactory.Ast;

namespace ICSharpCode.SharpDevelop.Refactoring
{
	public class RefactoringMenuContext
	{
		public ITextEditor Editor;
		public ExpressionResult ExpressionResult;
		public ResolveResult ResolveResult;
		public bool IsDefinition;
	}
	
	/// <summary>
	/// Build a menu with refactoring commands for the item that has been clicked on in the text editor.
	/// </summary>
	public class RefactoringMenuBuilder : IMenuItemBuilder
	{
		public ICollection BuildItems(Codon codon, object owner)
		{
			MenuItem item;
			
			ITextEditor textEditor = (ITextEditor)owner;
			if (string.IsNullOrEmpty(textEditor.FileName))
				return new object[0];
			List<object> resultItems = new List<object>();
			IDocument doc = textEditor.Document;
			int caretLine = textEditor.Caret.Line;
			IBookmarkMargin bookmarkMargin = textEditor.GetService(typeof(IBookmarkMargin)) as IBookmarkMargin;
			
			// list of dotnet names that have definition bookmarks in this line
			List<string> definitions = new List<string>();
			
			// Include definitions (use the bookmarks which should already be present)
			if (bookmarkMargin != null) {
				// we need to use .ToArray() because the bookmarks might change during enumeration:
				// building member/class submenus can cause reparsing the current file, which might change
				// the available bookmarks
				foreach (IBookmark mark in bookmarkMargin.Bookmarks.ToArray()) {
					if (mark != null && mark.LineNumber == caretLine) {
						ClassMemberBookmark cmb = mark as ClassMemberBookmark;
						ClassBookmark cb = mark as ClassBookmark;
						IClass type = null;
						if (cmb != null) {
							definitions.Add(cmb.Member.DotNetName);
							item = new MenuItem {
								Header = MemberNode.GetText(cmb.Member),
								Icon = cmb.Image.CreateImage(),
								ItemsSource = MenuService.CreateMenuItems(null, mark, ClassMemberBookmark.ContextMenuPath)
							};
							resultItems.Add(item);
							type = cmb.Member.DeclaringType;
						} else if (cb != null) {
							type = cb.Class;
						}
						if (type != null) {
							definitions.Add(type.DotNetName);
							item = new MenuItem {
								Header = ClassNode.GetText(type),
								Icon = ClassBrowserIconService.GetIcon(type).CreateImage(),
								ItemsSource = MenuService.CreateMenuItems(null,
								                                          cb ?? new ClassBookmark(type),
								                                          ClassBookmark.ContextMenuPath)
							};
							resultItems.Add(item);
						}
					}
				}
			}
			
			// Include menu for member that has been clicked on
			IExpressionFinder expressionFinder = ParserService.GetExpressionFinder(textEditor.FileName);
			ExpressionResult expressionResult;
			ResolveResult rr;
			int insertIndex = resultItems.Count;	// Insert items at this position to get the outermost expression first, followed by the inner expressions (if any).
			expressionResult = FindFullExpressionAtCaret(textEditor, expressionFinder);
		repeatResolve:
			rr = ResolveExpressionAtCaret(textEditor, expressionResult);
			RefactoringMenuContext context = new RefactoringMenuContext {
				Editor = textEditor,
				ResolveResult = rr,
				ExpressionResult = expressionResult
			};
			item = null;
			if (rr is MethodGroupResolveResult) {
				item = MakeItem(definitions, ((MethodGroupResolveResult)rr).GetMethodIfSingleOverload());
			} else if (rr is MemberResolveResult) {
				MemberResolveResult mrr = (MemberResolveResult)rr;
				item = MakeItem(definitions, mrr.ResolvedMember);
				if (RefactoringService.FixIndexerExpression(expressionFinder, ref expressionResult, mrr)) {
					if (item != null) {
						resultItems.Insert(insertIndex, item);
					}
					// Include menu for the underlying expression of the
					// indexer expression as well.
					goto repeatResolve;
				}
			} else if (rr is TypeResolveResult) {
				item = MakeItem(definitions, ((TypeResolveResult)rr).ResolvedClass);
			} else if (rr is LocalResolveResult) {
				context.IsDefinition = caretLine == ((LocalResolveResult)rr).VariableDefinitionRegion.BeginLine;
				item = MakeItem((LocalResolveResult)rr, context);
				insertIndex = 0;	// Insert local variable menu item at the topmost position.
			} else if (rr is UnknownIdentifierResolveResult) {
				item = MakeItemForResolveError((UnknownIdentifierResolveResult)rr, expressionResult.Context, textEditor);
				insertIndex = 0;	// Insert menu item at the topmost position.
			} else if (rr is UnknownConstructorCallResolveResult) {
				item = MakeItemForResolveError((UnknownConstructorCallResolveResult)rr, expressionResult.Context, textEditor);
				insertIndex = 0;	// Insert menu item at the topmost position.
			} else if (rr is UnknownMethodResolveResult) {
				item = MakeItemForResolveError((UnknownMethodResolveResult)rr, expressionResult.Context, textEditor);
				insertIndex = 0;	// Insert menu item at the topmost position.
			}
			if (item != null) {
				resultItems.Insert(insertIndex, item);
			}
			
			// Include menu for current class and method
			ICompilationUnit cu = null;
			IMember callingMember = null;
			if (rr != null && rr.CallingMember != null) {
				callingMember = rr.CallingMember;
			} else {
				ParseInformation parseInfo = ParserService.GetParseInformation(textEditor.FileName);
				if (parseInfo != null) {
					cu = parseInfo.CompilationUnit;
					if (cu != null) {
						IClass callingClass = cu.GetInnermostClass(caretLine, textEditor.Caret.Column);
						callingMember = GetCallingMember(callingClass, caretLine, textEditor.Caret.Column);
					}
				}
			}
			if (callingMember != null) {
				item = MakeItem(definitions, callingMember);
				if (item != null) {
					item.Header = StringParser.Parse("${res:SharpDevelop.Refactoring.CurrentMethod}: ") + callingMember.Name;
					resultItems.Add(item);
				}
			}
			
			if (resultItems.Count > 0) {
				resultItems.Add(new Separator());
			}
			return resultItems;
		}
		
		MenuItem MakeItemForResolveError(UnknownIdentifierResolveResult rr, ExpressionContext context, ITextEditor textArea)
		{
			return MakeItemForUnknownClass(rr.CallingClass, rr.Identifier, textArea);
		}
		
		MenuItem MakeItemForResolveError(UnknownConstructorCallResolveResult rr, ExpressionContext context, ITextEditor textArea)
		{
			return MakeItemForUnknownClass(rr.CallingClass, rr.TypeName, textArea);
		}
		
		MenuItem MakeItemForResolveError(UnknownMethodResolveResult rr, ExpressionContext context, ITextEditor editor)
		{
			// TODO : make easy testable (hide dialog in test mode)
			// TODO : add unit tests
			
			if (rr.Target == null)
				return null;
			
			MenuItem item = new MenuItem() {
				Header = "Introduce method " + rr.CallName + " in " + rr.Target.FullyQualifiedName,
				Icon = ClassBrowserIconService.GotoArrow.CreateImage()
			};
			
			IClass targetClass = rr.Target.GetUnderlyingClass();
			
			CodeGenerator gen = targetClass.ProjectContent.Language.CodeGenerator;
			IAmbience ambience = targetClass.ProjectContent.Language.GetAmbience();
			
			ClassFinder finder = new ClassFinder(rr.CallingMember);
			
			ModifierEnum modifiers = ModifierEnum.None;
			
			if (rr.CallingClass == targetClass) {
				if (rr.CallingMember != null)
					modifiers |= (rr.CallingMember.Modifiers & ModifierEnum.Static);
			} else {
				modifiers |= ModifierEnum.Public;
				if (rr.IsStaticContext)
					modifiers |= ModifierEnum.Static;
			}
			
			NRefactoryResolver resolver = new NRefactoryResolver(rr.CallingClass.ProjectContent.Language);
			resolver.Initialize(ParserService.GetParseInformation(editor.FileName), editor.Caret.Line, editor.Caret.Column);
			
			Ast.INode node = resolver.ParseCurrentMember(editor.Document.Text);
			resolver.RunLookupTableVisitor(node);
			
			InvocationExpressionLookupVisitor visitor = new InvocationExpressionLookupVisitor(editor);
			node.AcceptVisitor(visitor, null);
			
			if (visitor.Expression == null)
				return null;
			
			IReturnType type = resolver.GetExpectedTypeFromContext(visitor.Expression);
			Ast.TypeReference typeRef = CodeGenerator.ConvertType(type, finder);
			
			if (typeRef.IsNull) {
				if (visitor.Expression.Parent is Ast.ExpressionStatement)
					typeRef = new Ast.TypeReference("void", true);
				else
					typeRef = new Ast.TypeReference("object", true);
			}

			item.Click += delegate {
				Ast.MethodDeclaration method = new Ast.MethodDeclaration {
					Name = rr.CallName,
					Modifier = CodeGenerator.ConvertModifier(modifiers, finder),
					TypeReference = typeRef,
					Parameters = CreateParameters(rr, finder, visitor.Expression).ToList(),
					Body = CodeGenerator.CreateNotImplementedBlock()
				};
				
				if (targetClass.BodyRegion.IsEmpty) {
					method.Parameters.Insert(0, new Ast.ParameterDeclarationExpression(CodeGenerator.ConvertType(rr.Target, finder), "thisInstance"));
					method.IsExtensionMethod = true;
					method.Modifier |= Ast.Modifiers.Static;
					// TODO : combine code from IntroduceMethodDialog to remove code duplication
					IntroduceMethodDialog dialog = new IntroduceMethodDialog(rr.CallingClass, method, editor);
					dialog.Owner = WorkbenchSingleton.MainWindow;
					dialog.ShowDialog();
					return;
				}
				gen.InsertCodeAtEnd(targetClass.BodyRegion, new RefactoringDocumentAdapter(editor.Document), method);
				ParserService.ParseCurrentViewContent();
				// does not work yet, wrong method selected -> new method not yet present
				// TODO : need to retrieve updated IClass instance
				//IMethod newMember = targetClass.Methods.Last();
				//IDocumentLine line = editor.Document.GetLine(newMember.BodyRegion.BeginLine + 1);
				//int indentLength = DocumentUtilitites.GetWhitespaceAfter(editor.Document, line.Offset).Length;
				//editor.Select(line.Offset + indentLength, "new NotImplementedException();".Length);
			};
			
			return item;
		}
		
		IEnumerable<Ast.ParameterDeclarationExpression> CreateParameters(UnknownMethodResolveResult rr, ClassFinder context, Ast.InvocationExpression invocation)
		{
			List<string> usedNames = new List<string>();
			
			for (int i = 0; i < rr.Arguments.Count; i++) {
				IReturnType type = rr.Arguments[i];
				var typeRef = CodeGenerator.ConvertType(type, context);
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
		
		MenuItem MakeItemForUnknownClass(IClass callingClass, string unknownClassName, ITextEditor textArea)
		{
			if (callingClass == null)
				return null;
			IProjectContent pc = callingClass.ProjectContent;
			if (!pc.Language.RefactoringProvider.IsEnabledForFile(callingClass.CompilationUnit.FileName))
				return null;
			MenuItem item = MakeItemInternal(unknownClassName, ClassBrowserIconService.GotoArrow, callingClass.CompilationUnit, DomRegion.Empty);
			List<IClass> searchResults = new List<IClass>();
			SearchAllClassesWithName(searchResults, pc, unknownClassName, pc.Language);
			foreach (IProjectContent rpc in pc.ReferencedContents) {
				SearchAllClassesWithName(searchResults, rpc, unknownClassName, pc.Language);
			}
			if (searchResults.Count == 0)
				return null;
			foreach (IClass c in searchResults) {
				string newNamespace = c.Namespace;
				MenuItem subItem = new MenuItem();
				subItem.Header = "using " + newNamespace;
				subItem.Icon = ClassBrowserIconService.Namespace.CreateImage();
				item.Items.Add(subItem);
				subItem.Click += delegate {
					NamespaceRefactoringService.AddUsingDeclaration(callingClass.CompilationUnit, textArea.Document, newNamespace, true);
					ParserService.BeginParse(textArea.FileName, textArea.Document);
				};
			}
			return item;
		}
		
		void SearchAllClassesWithName(List<IClass> searchResults, IProjectContent pc, string name, LanguageProperties language)
		{
			foreach (string ns in pc.NamespaceNames) {
				IClass c = pc.GetClass(ns + "." + name, 0, language, GetClassOptions.None);
				if (c != null)
					searchResults.Add(c);
			}
		}
		
		IMember GetCallingMember(IClass callingClass, int caretLine, int caretColumn)
		{
			if (callingClass == null) {
				return null;
			}
			foreach (IMethod method in callingClass.Methods) {
				if (method.BodyRegion.IsInside(caretLine, caretColumn)) {
					return method;
				}
			}
			foreach (IProperty property in callingClass.Properties) {
				if (property.BodyRegion.IsInside(caretLine, caretColumn)) {
					return property;
				}
			}
			return null;
		}
		
		MenuItem MakeItem(LocalResolveResult local, RefactoringMenuContext context)
		{
			Debug.Assert(local == context.ResolveResult);
			MenuItem item = MakeItemInternal(local.VariableName,
			                                 local.IsParameter ? ClassBrowserIconService.Parameter : ClassBrowserIconService.LocalVariable,
			                                 local.CallingClass.CompilationUnit,
			                                 context.IsDefinition ? DomRegion.Empty : local.VariableDefinitionRegion);
			string treePath = "/SharpDevelop/ViewContent/DefaultTextEditor/Refactoring/";
			treePath += local.IsParameter ? "Parameter" : "LocalVariable";
			if (context.IsDefinition) treePath += "Definition";
			foreach (object obj in MenuService.CreateMenuItems(null, context, treePath))
				item.Items.Add(obj);
			return item;
		}
		
		MenuItem MakeItem(List<string> definitions, IMember member)
		{
			if (member == null) return null;
			if (definitions.Contains(member.DotNetName)) return null;
			definitions.Add(member.DotNetName);
			MenuItem item = MakeItem(MemberNode.Create(member), member.DeclaringType.CompilationUnit, member.Region);
			MenuItem declaringType = MakeItem(null, member.DeclaringType);
			if (declaringType != null) {
				item.Items.Add(new Separator());
				declaringType.Header = StringParser.Parse("${res:SharpDevelop.Refactoring.DeclaringType}: ") + declaringType.Header;
				item.Items.Add(declaringType);
			}
			return item;
		}
		
		MenuItem MakeItem(List<string> definitions, IClass c)
		{
			if (c == null) return null;
			if (definitions != null) {
				if (definitions.Contains(c.DotNetName)) return null;
				definitions.Add(c.DotNetName);
			}
			return MakeItem(new ClassNode((IProject)c.ProjectContent.Project, c), c.CompilationUnit, c.Region);
		}
		
		MenuItem MakeItemInternal(string title, IImage image, ICompilationUnit cu, DomRegion region)
		{
			MenuItem item = new MenuItem();
			item.Header = title;
			item.Icon = image.CreateImage();
			
			//ToolStripMenuItem titleItem = new ToolStripMenuItem(title);
			//titleItem.Enabled = false;
			//item.DropDown.Items.Add(titleItem);
			//item.DropDown.Items.Add(new ToolStripSeparator());
			
			if (cu != null && cu.FileName != null && !region.IsEmpty) {
				MenuItem gotoDefinitionItem = new MenuItem();
				gotoDefinitionItem.Header = MenuService.ConvertLabel(StringParser.Parse("${res:ICSharpCode.NAntAddIn.GotoDefinitionMenuLabel}"));
				gotoDefinitionItem.Icon = ClassBrowserIconService.GotoArrow.CreateImage();
				gotoDefinitionItem.InputGestureText = new KeyGesture(Key.Enter, ModifierKeys.Control).GetDisplayStringForCulture(Thread.CurrentThread.CurrentUICulture);
				gotoDefinitionItem.Click += delegate {
					FileService.JumpToFilePosition(cu.FileName, region.BeginLine, region.BeginColumn);
				};
				item.Items.Add(gotoDefinitionItem);
				item.Items.Add(new Separator());
			}
			return item;
		}
		
		MenuItem MakeItem(ExtTreeNode classBrowserTreeNode, ICompilationUnit cu, DomRegion region)
		{
			MenuItem item = MakeItemInternal(classBrowserTreeNode.Text, ClassBrowserIconService.GetImageByIndex(classBrowserTreeNode.ImageIndex), cu, region);
			foreach (object obj in MenuService.CreateMenuItems(null, classBrowserTreeNode, classBrowserTreeNode.ContextmenuAddinTreePath))
				item.Items.Add(obj);
			return item;
		}
		
		static ExpressionResult FindFullExpressionAtCaret(ITextEditor textArea, IExpressionFinder expressionFinder)
		{
			if (expressionFinder != null) {
				return expressionFinder.FindFullExpression(textArea.Document.Text, textArea.Caret.Offset);
			} else {
				return new ExpressionResult(null);
			}
		}
		
		static ResolveResult ResolveExpressionAtCaret(ITextEditor textArea, ExpressionResult expressionResult)
		{
			if (expressionResult.Expression != null) {
				return ParserService.Resolve(expressionResult, textArea.Caret.Line, textArea.Caret.Column, textArea.FileName, textArea.Document.Text);
			}
			return null;
		}
	}
}





