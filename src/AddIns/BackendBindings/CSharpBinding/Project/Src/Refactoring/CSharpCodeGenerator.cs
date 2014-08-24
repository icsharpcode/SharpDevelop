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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Media.Animation;
using CSharpBinding.Parser;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.Core;
using ICSharpCode.Core.Presentation;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.CSharp.Refactoring;
using ICSharpCode.NRefactory.CSharp.Resolver;
using ICSharpCode.NRefactory.CSharp.TypeSystem;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Refactoring;

namespace CSharpBinding.Refactoring
{
	/// <summary>
	/// Description of CSharpCodeGenerator.
	/// </summary>
	public class CSharpCodeGenerator : CodeGenerator
	{
		public override void AddAttribute(IEntity target, IAttribute attribute)
		{
			AddAttribute(target.Region, attribute);
		}
		
		public override void AddAssemblyAttribute(IProject targetProject, IAttribute attribute)
		{
			// FIXME : will fail if there are no assembly attributes
			ICompilation compilation = SD.ParserService.GetCompilation(targetProject);
			IAttribute target = compilation.MainAssembly.AssemblyAttributes.LastOrDefault();
			if (target == null)
				throw new InvalidOperationException("no assembly attributes found, cannot continue!");
			AddAttribute(target.Region, attribute, "assembly");
		}

		public override void AddReturnTypeAttribute(IMethod target, IAttribute attribute)
		{
			AddAttribute(target.Region, attribute, "return");
		}
		
		public override void InsertEventHandler(ITypeDefinition target, string name, IEvent eventDefinition, bool jumpTo, InsertEventHandlerBodyKind bodyKind = InsertEventHandlerBodyKind.ThrowNotImplementedException)
		{
			IUnresolvedTypeDefinition match = null;
			
			foreach (var part in target.Parts) {
				if (match == null || EntityModelContextUtils.IsBetterPart(part, match, ".cs"))
					match = part;
			}
			
			if (match == null) return;
			
			var view = SD.FileService.OpenFile(new FileName(match.Region.FileName), jumpTo);
			var editor = view.GetRequiredService<ITextEditor>();
			var last = match.Members.LastOrDefault() ?? (IUnresolvedEntity)match;
			editor.Caret.Location = last.BodyRegion.End;
			var context = SDRefactoringContext.Create(editor, CancellationToken.None);
			
			var node = context.RootNode.GetNodeAt<EntityDeclaration>(last.Region.Begin);
			var resolver = context.GetResolverStateAfter(node);
			var builder = new TypeSystemAstBuilder(resolver);
			var invokeMethod = eventDefinition.ReturnType.GetDelegateInvokeMethod();
			if (invokeMethod == null) return;
			var importedMethod = resolver.Compilation.Import(invokeMethod);
			var delegateDecl = builder.ConvertEntity(importedMethod) as MethodDeclaration;
			if (delegateDecl == null) return;
			var throwStmt = new ThrowStatement(new ObjectCreateExpression(context.CreateShortType("System", "NotImplementedException")));
			var decl = new MethodDeclaration() {
				ReturnType = delegateDecl.ReturnType.Clone(),
				Name = name,
				Body = new BlockStatement() {
					throwStmt
				}
			};
			var param = delegateDecl.Parameters.Select(p => p.Clone()).ToArray();
			decl.Parameters.AddRange(param);
			
			using (Script script = context.StartScript()) {
				int eolLen = 0;
				if (last == match) {
					eolLen = 2;
					script.AddTo((TypeDeclaration)node, decl);
				} else {
					script.InsertAfter(node, decl);
				}
				switch (bodyKind) {
					case InsertEventHandlerBodyKind.TodoComment:
						Comment comment = new Comment(" TODO: Implement " + name);
						script.Replace(throwStmt, comment);
						script.Select(comment);
						break;
					case InsertEventHandlerBodyKind.Nothing:
						var segment = script.GetSegment(throwStmt);
						if (script is DocumentScript && eolLen > 0) {
							eolLen = ((DocumentScript)script).CurrentDocument.GetLineByOffset(segment.Offset).DelimiterLength;
						}
						script.RemoveText(segment.Offset, segment.Length - eolLen);
						script.Select(segment.Offset, segment.Offset);
						break;
					case InsertEventHandlerBodyKind.ThrowNotImplementedException:
						script.Select(throwStmt);
						break;
				}
			}
		}
		
		void AddAttribute(DomRegion region, IAttribute attribute, string target = "")
		{
			var view = SD.FileService.OpenFile(new FileName(region.FileName), false);
			var editor = view.GetRequiredService<ITextEditor>();
			var context = SDRefactoringContext.Create(editor.FileName, editor.Document, region.Begin, CancellationToken.None);
			var node = context.RootNode.GetNodeAt<EntityDeclaration>(region.Begin);
			var resolver = context.GetResolverStateBefore(node);
			var builder = new TypeSystemAstBuilder(resolver);
			
			using (Script script = context.StartScript()) {
				var attr = new AttributeSection();
				attr.AttributeTarget = target;
				attr.Attributes.Add(builder.ConvertAttribute(attribute));
				script.AddAttribute(node, attr);
			}
		}
		
		public override void AddField(ITypeDefinition declaringType, Accessibility accessibility, IType fieldType, string name)
		{
			SDRefactoringContext context = declaringType.CreateRefactoringContext();
			var typeDecl = context.GetNode<TypeDeclaration>();
			using (var script = context.StartScript()) {
				var astBuilder = context.CreateTypeSystemAstBuilder(typeDecl.FirstChild);
				var fieldDecl = new FieldDeclaration();
				fieldDecl.Modifiers = TypeSystemAstBuilder.ModifierFromAccessibility(accessibility);
				fieldDecl.ReturnType = astBuilder.ConvertType(context.Compilation.Import(fieldType));
				fieldDecl.Variables.Add(new VariableInitializer(name));
				script.InsertWithCursor("Add field: " + name, Script.InsertPosition.End, fieldDecl);
			}
		}
		
		public override void AddFieldAtStart(ITypeDefinition declaringType, Accessibility accessibility, IType fieldType, string name)
		{
			SDRefactoringContext context = declaringType.CreateRefactoringContext();
			var typeDecl = context.GetNode<TypeDeclaration>();
			using (var script = context.StartScript()) {
				var astBuilder = context.CreateTypeSystemAstBuilder(typeDecl.FirstChild);
				var fieldDecl = new FieldDeclaration();
				fieldDecl.Modifiers = TypeSystemAstBuilder.ModifierFromAccessibility(accessibility);
				fieldDecl.ReturnType = astBuilder.ConvertType(context.Compilation.Import(fieldType));
				fieldDecl.Variables.Add(new VariableInitializer(name));
				
				script.AddTo(typeDecl, fieldDecl);
			}
		}
		
		public override void AddMethodAtStart(ITypeDefinition declaringType, Accessibility accessibility, IType returnType, string name)
		{
			SDRefactoringContext context = declaringType.CreateRefactoringContext();
			var typeDecl = context.GetNode<TypeDeclaration>();
			using (var script = context.StartScript()) {
				var astBuilder = context.CreateTypeSystemAstBuilder(typeDecl.FirstChild);
				var methodDecl = new MethodDeclaration();
				methodDecl.Name = name;
				methodDecl.ReturnType = astBuilder.ConvertType(context.Compilation.Import(returnType));
				
				script.AddTo(typeDecl, methodDecl);
			}
		}
		
		public override void ChangeAccessibility(IEntity entity, Accessibility newAccessiblity)
		{
			// TODO script.ChangeModifiers(...)
			throw new NotImplementedException();
		}
		
		public override void AddImport(FileName fileName, string namespaceName)
		{
			var context = RefactoringExtensions.CreateRefactoringContext(new DomRegion(fileName, 0, 0));
			var astBuilder = context.CreateTypeSystemAstBuilder();
			using (var script = context.StartScript()) {
				AstType ns = astBuilder.ConvertNamespace(namespaceName);
				UsingHelper.InsertUsing(context, script, ns);
			}
		}
		
		public override void MakePartial(ITypeDefinition td)
		{
			SDRefactoringContext refactoringContext = td.CreateRefactoringContext();
			var typeDeclaration = refactoringContext.GetNode<TypeDeclaration>();
			
			using (Script script = refactoringContext.StartScript()) {
				script.ChangeModifier(typeDeclaration, typeDeclaration.Modifiers | Modifiers.Partial);
			}
		}
		
		public override void MakeVirtual(IMember member)
		{
			SDRefactoringContext refactoringContext = member.CreateRefactoringContext();
			var entityDeclaration = refactoringContext.GetNode<EntityDeclaration>();
			
			using (Script script = refactoringContext.StartScript()) {
				script.ChangeModifier(entityDeclaration, entityDeclaration.Modifiers | Modifiers.Virtual);
			}
		}
		
		public override string EscapeIdentifier(string identifier)
		{
			return "@" + identifier;
		}
	}
}
