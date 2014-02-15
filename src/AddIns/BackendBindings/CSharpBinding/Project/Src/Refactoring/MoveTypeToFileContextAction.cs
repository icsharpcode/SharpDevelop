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
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.Core;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.CSharp.Refactoring;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem.Implementation;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Parser;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Refactoring;
using CSharpBinding.Parser;

namespace CSharpBinding.Refactoring
{
	[ContextAction("Move type to file", Description = "Moves a selected type into a new file matching the type name.")]
	public class MoveTypeToFileContextAction : ContextAction
	{
		public override async Task<bool> IsAvailableAsync(EditorRefactoringContext context, CancellationToken cancellationToken)
		{
			SyntaxTree st = await context.GetSyntaxTreeAsync().ConfigureAwait(false);
			Identifier identifier = (Identifier)st.GetNodeAt(context.CaretLocation, node => node.Role == Roles.Identifier);
			if (identifier == null) return false;
			if (MakeValidFileName(identifier.Name).Equals(Path.GetFileName(context.FileName), StringComparison.OrdinalIgnoreCase))
				return false;
			if (identifier.Parent.Parent is TypeDeclaration)
				return false;
			ParseInformation info = await context.GetParseInformationAsync().ConfigureAwait(false);
			if (info.UnresolvedFile.TopLevelTypeDefinitions.Count == 1)
				return false;
			return identifier.Parent is TypeDeclaration || identifier.Parent is DelegateDeclaration;
		}
		
		public override string DisplayName
		{
			get {
				return "Move type to file";
			}
		}
		
		public override string GetDisplayName(EditorRefactoringContext context)
		{
			CSharpFullParseInformation parseInformation = context.GetParseInformation() as CSharpFullParseInformation;
			if (parseInformation != null) {
				SyntaxTree st = parseInformation.SyntaxTree;
				Identifier identifier = (Identifier) st.GetNodeAt(context.CaretLocation, node => node.Role == Roles.Identifier);
				if (identifier == null)
					return DisplayName;
				
				return StringParser.Parse("${res:SharpDevelop.Refactoring.MoveClassToFile}",
				                          new StringTagPair("FileName", MakeValidFileName(identifier.Name)));
			}
			
			return DisplayName;
		}
		
		public override async void Execute(EditorRefactoringContext context)
		{
			SyntaxTree st = await context.GetSyntaxTreeAsync().ConfigureAwait(false);
			ICompilation compilation = await context.GetCompilationAsync().ConfigureAwait(false);
			CSharpFullParseInformation info = await context.GetParseInformationAsync().ConfigureAwait(false) as CSharpFullParseInformation;
			EntityDeclaration node = (EntityDeclaration)st.GetNodeAt(context.CaretLocation, n => n is TypeDeclaration || n is DelegateDeclaration);
			IDocument document = context.Editor.Document;
			
			FileName newFileName = FileName.Create(Path.Combine(Path.GetDirectoryName(context.FileName), MakeValidFileName(node.Name)));
			string header = CopyFileHeader(document, info);
			string footer = CopyFileEnd(document, info);
			
			AstNode newNode = node.Clone();
			
			foreach (var ns in node.Ancestors.OfType<NamespaceDeclaration>()) {
				var newNS = new NamespaceDeclaration(ns.Name);
				newNS.Members.AddRange(ns.Children.Where(ch => ch is UsingDeclaration
				                                         || ch is UsingAliasDeclaration
				                                         || ch is ExternAliasDeclaration).Select(usingDecl => usingDecl.Clone()));
				newNS.AddMember(newNode);
				newNode = newNS;
			}
			
			var topLevelUsings = st.Children.Where(ch => ch is UsingDeclaration
			                                       || ch is UsingAliasDeclaration
			                                       || ch is ExternAliasDeclaration);
			StringBuilder newCode = new StringBuilder(header);
			CSharpOutputVisitor visitor = new CSharpOutputVisitor(new StringWriter(newCode), FormattingOptionsFactory.CreateSharpDevelop());
			
			foreach (var topLevelUsing in topLevelUsings)
				topLevelUsing.AcceptVisitor(visitor);
			
			newNode.AcceptVisitor(visitor);
			
			newCode.AppendLine(footer);
			
			IViewContent viewContent = FileService.NewFile(newFileName, newCode.ToString());
			viewContent.PrimaryFile.SaveToDisk(newFileName);
			// now that the code is saved in the other file, remove it from the original document
			RemoveExtractedNode(context, node);
			
			IProject project = (IProject)compilation.GetProject();
			if (project != null) {
				FileProjectItem projectItem = new FileProjectItem(project, ItemType.Compile);
				projectItem.FileName = newFileName;
				ProjectService.AddProjectItem(project, projectItem);
				FileService.FireFileCreated(newFileName, false);
				project.Save();
				ProjectBrowserPad.RefreshViewAsync();
			}
		}
		
		void RemoveExtractedNode(EditorRefactoringContext context, EntityDeclaration node)
		{
			IDocument document = context.Editor.Document;
			int start = document.GetOffset(node.StartLocation);
			int end   = document.GetOffset(node.EndLocation);
			document.Remove(start, end - start);
		}
		
		string CopyFileHeader(IDocument document, CSharpFullParseInformation info)
		{
			var lastHeadNode = info.SyntaxTree.Children
				.TakeWhile(node => node.NodeType == NodeType.Whitespace && (!(node is Comment) || !((Comment)node).IsDocumentation))
				.LastOrDefault();
			if (lastHeadNode == null)
				return "";
			return document.GetText(0, document.GetOffset(lastHeadNode.EndLocation));
		}
		
		string CopyFileEnd(IDocument document, CSharpFullParseInformation info)
		{
			var firstFootNode = info.SyntaxTree.Children.Reverse()
				.TakeWhile(node => node.NodeType == NodeType.Whitespace && (!(node is Comment) || !((Comment)node).IsDocumentation))
				.LastOrDefault();
			if (firstFootNode == null)
				return "";
			int offset = document.GetOffset(firstFootNode.StartLocation);
			return document.GetText(offset, document.TextLength - offset);
		}
		
		string MakeValidFileName(string name)
		{
			if (name == null)
				throw new ArgumentNullException("name");
			if (name.IndexOfAny(Path.GetInvalidFileNameChars()) > -1)
				return name.RemoveAny(Path.GetInvalidFileNameChars()) + ".cs";
			return name + ".cs";
		}
	}
}
