// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Media.Animation;
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
	public class CSharpCodeGenerator : ICodeGenerator
	{
		CSharpProject project;
		ProjectEntityModelContext model;
		
		public CSharpCodeGenerator(CSharpProject project)
		{
			if (project == null)
				throw new ArgumentNullException("project");
			this.project = project;
			this.model = new ProjectEntityModelContext(project, ".cs");
		}
		
		public void AddAttribute(IEntity target, IAttribute attribute)
		{
			var view = SD.FileService.OpenFile(new FileName(target.Region.FileName), false);
			var editor = view.GetRequiredService<ITextEditor>();
			var context = SDRefactoringContext.Create(editor, CancellationToken.None);
			var node = context.RootNode.GetNodeAt<EntityDeclaration>(target.Region.Begin);
			var resolver = context.GetResolverStateBefore(node);
			var builder = new TypeSystemAstBuilder(resolver);
			
			editor.Document.Insert(editor.Document.GetOffset(target.BodyRegion.Begin),
			                       PrintAst(builder.ConvertAttribute(attribute)));
		}
		
		public void InsertEventHandler(ITypeDefinition target, string name, IEvent eventDefinition, bool jumpTo)
		{
			IUnresolvedTypeDefinition match = null;
			
			foreach (var part in target.Parts) {
				string fileName = part.UnresolvedFile.FileName;
				if (!".cs".Equals(Path.GetExtension(fileName), StringComparison.OrdinalIgnoreCase))
					continue;
				if (match == null || model.IsBetterPart(part, match))
					match = part;
			}
			
			if (match == null) return;
			
			var view = SD.FileService.OpenFile(new FileName(match.Region.FileName), jumpTo);
			var editor = view.GetRequiredService<ITextEditor>();
			var context = SDRefactoringContext.Create(editor, CancellationToken.None);
			
			IEntity last = (IEntity)target.GetMembers().LastOrDefault() ?? (IEntity)target;
			var node = context.RootNode.GetNodeAt<EntityDeclaration>(last.Region.Begin);
			var resolver = context.GetResolverStateAfter(node);
			var builder = new TypeSystemAstBuilder(resolver);
			
			#warning implement code generation!
			string eventHandler = "<generated code>";
			
			editor.Document.Insert(editor.Document.GetOffset(target.Region.End), eventHandler);
		}

		string PrintAst(AstNode node)
		{
			StringBuilder sb = new StringBuilder();
			CSharpOutputVisitor visitor = new CSharpOutputVisitor(new StringWriter(sb), FormattingOptionsFactory.CreateSharpDevelop());
			node.AcceptVisitor(visitor);
			return sb.ToString();
		}
	}
}
