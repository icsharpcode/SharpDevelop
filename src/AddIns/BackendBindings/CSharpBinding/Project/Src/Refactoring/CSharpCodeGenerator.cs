// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Media.Animation;
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
	public class CSharpCodeGenerator : DefaultCodeGenerator
	{
		public void AddAttribute(IEntity target, IAttribute attribute)
		{
			AddAttribute(target.Region, attribute);
		}
		
		public void AddAssemblyAttribute(IProject targetProject, IAttribute attribute)
		{
			// FIXME : will fail if there are no assembly attributes
			ICompilation compilation = SD.ParserService.GetCompilation(targetProject);
			IAttribute target = compilation.MainAssembly.AssemblyAttributes.LastOrDefault();
			if (target == null)
				throw new InvalidOperationException("no assembly attributes found, cannot continue!");
			AddAttribute(target.Region, attribute, "assembly");
		}

		public void AddReturnTypeAttribute(IMethod target, IAttribute attribute)
		{
			AddAttribute(target.Region, attribute, "return");
		}
		
		public void InsertEventHandler(ITypeDefinition target, string name, IEvent eventDefinition, bool jumpTo)
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
			var delegateDecl = builder.ConvertEntity(eventDefinition.ReturnType.GetDefinition()) as DelegateDeclaration;
			if (delegateDecl == null) return;
			var throwStmt = new ThrowStatement(new ObjectCreateExpression(context.CreateShortType("System", "NotImplementedException")));
			var decl = new MethodDeclaration() {
				ReturnType = delegateDecl.ReturnType.Clone(),
				Name = name,
				Body = new BlockStatement() {
					throwStmt
				}
			};
			var param = delegateDecl.Parameters.Select(p => p.Clone()).OfType<ParameterDeclaration>().ToArray();
			decl.Parameters.AddRange(param);
			
			using (Script script = context.StartScript()) {
				// FIXME : will not work properly if there are no members.
				if (last == match) {
					throw new NotImplementedException();
					// TODO InsertWithCursor not implemented!
					//script.InsertWithCursor("Insert event handler", Script.InsertPosition.End, decl).RunSynchronously();
				} else {
					// TODO does not jump correctly...
					script.InsertAfter(node, decl);
					editor.JumpTo(throwStmt.StartLocation.Line, throwStmt.StartLocation.Column);
				}
			}
		}
		
		void AddAttribute(DomRegion region, IAttribute attribute, string target = "")
		{
			var view = SD.FileService.OpenFile(new FileName(region.FileName), false);
			var editor = view.GetRequiredService<ITextEditor>();
			var context = SDRefactoringContext.Create(editor.FileName, editor.Document, region.Begin, CancellationToken.None);
			var node = context.RootNode.GetNodeAt<AstNode>(region.Begin);
			if (node is ICSharpCode.NRefactory.CSharp.Attribute) node = node.Parent;
			var resolver = context.GetResolverStateBefore(node);
			var builder = new TypeSystemAstBuilder(resolver);
			
			using (Script script = context.StartScript()) {
				var attr = new AttributeSection();
				attr.AttributeTarget = target;
				attr.Attributes.Add(builder.ConvertAttribute(attribute));
				script.InsertBefore(node, attr);
			}
		}
	}
}
