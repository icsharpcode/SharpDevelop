// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Threading;
using CSharpBinding.Parser;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.CSharp.Refactoring;
using ICSharpCode.NRefactory.CSharp.Resolver;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Parser;

namespace CSharpBinding
{
	public class SDRefactoringContext : RefactoringContext
	{
		ITextEditor editor;
		ICompilation compilation;
		CancellationToken cancellationToken;
		
		public SDRefactoringContext(ITextEditor editor, CancellationToken cancellationToken)
		{
			this.editor = editor;
			this.cancellationToken = cancellationToken;
			this.compilation = ParserService.GetCompilationForFile(editor.FileName);
		}
		
		public override bool Supports(Version version)
		{
			CSharpProject project = compilation.GetProject() as CSharpProject;
			if (project == null)
				return false;
			return project.LanguageVersion >= version;
		}
		
		public override Script StartScript()
		{
			throw new NotImplementedException();
		}
		
		public override int SelectionStart {
			get {
				return editor.SelectionStart;
			}
		}
		
		public override int SelectionLength {
			get {
				return editor.SelectionLength;
			}
		}
		
		public override int SelectionEnd {
			get {
				return editor.SelectionStart + editor.SelectionLength;
			}
		}
		
		public override string SelectedText {
			get {
				return editor.SelectedText;
			}
		}
		
		public override ResolveResult Resolve(AstNode expression)
		{
			var parseInfo = ParserService.Parse(editor.FileName, editor.Document) as CSharpFullParseInformation;
			var resolver = new CSharpAstResolver(compilation, parseInfo.CompilationUnit, parseInfo.ParsedFile);
			return resolver.Resolve(expression, cancellationToken);
		}
		
		public override void ReplaceReferences(IMember member, MemberDeclaration replaceWidth)
		{
			throw new NotImplementedException();
		}
		
		public override bool IsSomethingSelected {
			get {
				return editor.SelectionLength > 0;
			}
		}
		
		public override string GetText(int offset, int length)
		{
			return editor.Document.GetText(offset, length);
		}
		
		public override int GetOffset(TextLocation location)
		{
			return editor.Document.GetOffset(location);
		}
		
		public override TextLocation GetLocation(int offset)
		{
			return editor.Document.GetLocation(offset);
		}
		
		public override CSharpFormattingOptions FormattingOptions {
			get {
				return new CSharpFormattingOptions();
			}
		}
		
		public override string EolMarker {
			get {
				return DocumentUtilitites.GetLineTerminator(editor.Document, 1);
			}
		}
		
		public override AstType CreateShortType(IType fullType)
		{
			var parseInfo = ParserService.Parse(editor.FileName, editor.Document) as CSharpFullParseInformation;
			var parsedFile = parseInfo.ParsedFile;
			var csResolver = parsedFile.GetResolver(compilation, editor.Caret.Location);
			var builder = new TypeSystemAstBuilder(csResolver);
			return builder.ConvertType(fullType);
		}
	}
}
