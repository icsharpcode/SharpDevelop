// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Threading;
using CSharpBinding.Parser;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.CSharp.Refactoring;
using ICSharpCode.NRefactory.CSharp.Resolver;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Parser;
using ICSharpCode.SharpDevelop.Refactoring;

namespace CSharpBinding.Refactoring
{
	public class SDRefactoringContext : RefactoringContext
	{
		readonly ITextEditor editor;
		readonly ITextSource textSource;
		volatile IDocument document;
		readonly CSharpAstResolver resolver;
		int selectionStart, selectionLength;
		CancellationToken cancellationToken;
		
		public SDRefactoringContext(ITextSource textSource, CSharpAstResolver resolver, TextLocation location, int selectionStart, int selectionLength, CancellationToken cancellationToken)
		{
			this.textSource = textSource;
			this.document = textSource as IDocument;
			this.resolver = resolver;
			this.selectionStart = selectionStart;
			this.selectionLength = selectionLength;
			this.cancellationToken = cancellationToken;
			
			this.Unit = resolver.RootNode as CompilationUnit;
			this.Compilation = resolver.Compilation;
			this.Location = location;
		}
		
		public SDRefactoringContext(ITextEditor editor,  CSharpAstResolver resolver, TextLocation location)
		{
			this.editor = editor;
			this.textSource = editor.Document;
			this.document = editor.Document;
			this.resolver = resolver;
			this.selectionStart = editor.SelectionStart;
			this.selectionLength = editor.SelectionLength;
			
			this.Unit = resolver.RootNode as CompilationUnit;
			this.Compilation = resolver.Compilation;
			this.Location = location;
		}
		
		public override bool Supports(Version version)
		{
			CSharpProject project = resolver.TypeResolveContext.Compilation.GetProject() as CSharpProject;
			if (project == null)
				return false;
			return project.LanguageVersion >= version;
		}
		
		public override NodeOutputAction CreateNodeOutputAction(int offset, int removedChars, NodeOutput output)
		{
			return new SDScript.SDNodeOutputAction(document, offset, removedChars, output);
		}
		
		public override Script StartScript()
		{
			if (editor == null)
				throw new InvalidOperationException("Cannot start a script in IsAvailable().");
			return new SDScript(editor, this);
		}
		
		public override int SelectionStart {
			get { return selectionStart; }
		}
		
		public override int SelectionLength {
			get { return selectionLength; }
		}
		
		public override int SelectionEnd {
			get {
				return selectionStart + selectionLength;
			}
		}
		
		public override string SelectedText {
			get {
				return textSource.GetText(selectionStart, selectionLength);
			}
		}
		
		public override ResolveResult Resolve(AstNode expression)
		{
			lock (resolver)
				return resolver.Resolve(expression, cancellationToken);
		}
		
		public override void ReplaceReferences(IMember member, MemberDeclaration replaceWidth)
		{
			throw new NotImplementedException();
		}
		
		public override bool IsSomethingSelected {
			get {
				return selectionLength > 0;
			}
		}
		
		public override string GetText(int offset, int length)
		{
			return textSource.GetText(offset, length);
		}
		
		public override int GetOffset(TextLocation location)
		{
			if (document == null)
				document = new ReadOnlyDocument(textSource);
			return document.GetOffset(location);
		}
		
		public override TextLocation GetLocation(int offset)
		{
			if (document == null)
				document = new ReadOnlyDocument(textSource);
			return document.GetLocation(offset);
		}
		
		public override CSharpFormattingOptions FormattingOptions {
			get {
				return new CSharpFormattingOptions();
			}
		}
		
		public override string EolMarker {
			get {
				if (document == null)
					document = new ReadOnlyDocument(textSource);
				return DocumentUtilitites.GetLineTerminator(document, 1);
			}
		}
		
		public override AstType CreateShortType(IType fullType)
		{
			CSharpResolver csResolver;
			lock (resolver) {
				csResolver = resolver.GetResolverStateBefore(GetNode());
			}
			var builder = new TypeSystemAstBuilder(csResolver);
			return builder.ConvertType(fullType);
		}
	}
}
