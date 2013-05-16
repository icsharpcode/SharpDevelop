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
	public class CSharpCodeGenerator : ICodeGenerator
	{
		IProject project;
		ProjectEntityModelContext model;
		
		public CSharpCodeGenerator(IProject project)
		{
			if (project == null)
				throw new ArgumentNullException("project");
			this.project = project;
			this.model = new ProjectEntityModelContext(project, ".cs");
		}
		
		public void AddAttribute(IEntity target, IAttribute attribute)
		{
			AddAttribute(target.Region, attribute);
		}
		
		public void AddAssemblyAttribute(IAttribute attribute)
		{
			// FIXME : will fail if there are no assembly attributes
			ICompilation compilation = SD.ParserService.GetCompilation(project);
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
				string fileName = part.UnresolvedFile.FileName;
				if (!".cs".Equals(Path.GetExtension(fileName), StringComparison.OrdinalIgnoreCase))
					continue;
				if (match == null || model.IsBetterPart(part, match))
					match = part;
			}
			
			if (match == null) return;
			
			var view = SD.FileService.OpenFile(new FileName(match.Region.FileName), jumpTo);
			var editor = view.GetRequiredService<ITextEditor>();
			var last = match.Members.LastOrDefault() ?? (IUnresolvedEntity)match;
			var context = SDRefactoringContext.Create(editor.FileName, editor.Document, last.BodyRegion.End, CancellationToken.None);
			
			var node = context.RootNode.GetNodeAt<EntityDeclaration>(last.Region.Begin);
			var resolver = context.GetResolverStateAfter(node);
			var builder = new TypeSystemAstBuilder(resolver);
			var delegateDecl = builder.ConvertEntity(eventDefinition.ReturnType.GetDefinition()) as DelegateDeclaration;
			if (delegateDecl == null) return;
			var decl = new MethodDeclaration() {
				ReturnType = delegateDecl.ReturnType.Clone(),
				Name = name,
				Body = new BlockStatement() {
					new ThrowStatement(new ObjectCreateExpression(context.CreateShortType("System", "NotImplementedException")))
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
					script.InsertAfter(node, decl);
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
		
		public string GetPropertyName(string fieldName)
		{
			if (string.IsNullOrEmpty(fieldName))
				return fieldName;
			if (fieldName.StartsWith("_") && fieldName.Length > 1)
				return Char.ToUpper(fieldName[1]) + fieldName.Substring(2);
			else if (fieldName.StartsWith("m_") && fieldName.Length > 2)
				return Char.ToUpper(fieldName[2]) + fieldName.Substring(3);
			else
				return Char.ToUpper(fieldName[0]) + fieldName.Substring(1);
		}
		
		public string GetParameterName(string fieldName)
		{
			if (string.IsNullOrEmpty(fieldName))
				return fieldName;
			if (fieldName.StartsWith("_") && fieldName.Length > 1)
				return Char.ToLower(fieldName[1]) + fieldName.Substring(2);
			else if (fieldName.StartsWith("m_") && fieldName.Length > 2)
				return Char.ToLower(fieldName[2]) + fieldName.Substring(3);
			else
				return Char.ToLower(fieldName[0]) + fieldName.Substring(1);
		}
		
		public string GetFieldName(string propertyName)
		{
			if (string.IsNullOrEmpty(propertyName))
				return propertyName;
			string newName = Char.ToLower(propertyName[0]) + propertyName.Substring(1);
			if (newName == propertyName)
				return "_" + newName;
			else
				return newName;
		}
	}
}
