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
using System.ComponentModel.Design;
using System.Threading;

using ICSharpCode.NRefactory.Utils;
using CSharpBinding.FormattingStrategy;
using CSharpBinding.Parser;
using ICSharpCode.Core;
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
		readonly CSharpAstResolver resolver;
		readonly ITextEditor editor;
		readonly ITextSource textSource;
		readonly TextLocation location;
		IDocument document;
		int selectionStart, selectionLength;
		
		public static SDRefactoringContext Create(ITextEditor editor, CancellationToken cancellationToken)
		{
			var parseInfo = SD.ParserService.Parse(editor.FileName, editor.Document, cancellationToken: cancellationToken) as CSharpFullParseInformation;
			var compilation = SD.ParserService.GetCompilationForFile(editor.FileName);
			CSharpAstResolver resolver;
			if (parseInfo != null) {
				resolver = parseInfo.GetResolver(compilation);
			} else {
				// create dummy refactoring context
				resolver = new CSharpAstResolver(compilation, new SyntaxTree());
			}
			var context = new SDRefactoringContext(editor, resolver, editor.Caret.Location, cancellationToken);
			return context;
		}
		
		public static SDRefactoringContext Create(FileName fileName, ITextSource textSource, TextLocation location = default(TextLocation), CancellationToken cancellationToken = default(CancellationToken))
		{
			var parseInfo = SD.ParserService.Parse(fileName, textSource, cancellationToken: cancellationToken) as CSharpFullParseInformation;
			var compilation = SD.ParserService.GetCompilationForFile(fileName);
			CSharpAstResolver resolver;
			if (parseInfo != null) {
				resolver = parseInfo.GetResolver(compilation);
			} else {
				// create dummy refactoring context
				resolver = new CSharpAstResolver(compilation, new SyntaxTree());
			}
			var context = new SDRefactoringContext(textSource, resolver, location, 0, 0, cancellationToken);
			return context;
		}
		
		public SDRefactoringContext(ITextSource textSource, CSharpAstResolver resolver, TextLocation location, int selectionStart, int selectionLength, CancellationToken cancellationToken)
			: base(resolver, cancellationToken)
		{
			this.resolver = resolver;
			this.textSource = textSource;
			this.document = textSource as IDocument;
			this.selectionStart = selectionStart;
			this.selectionLength = selectionLength;
			this.location = location;
			InitializeServices();
		}
		
		public SDRefactoringContext(ITextEditor editor, CSharpAstResolver resolver, TextLocation location, CancellationToken cancellationToken = default(CancellationToken))
			: base(resolver, cancellationToken)
		{
			this.resolver = resolver;
			this.editor = editor;
			this.textSource = editor.Document;
			this.document = editor.Document;
			this.selectionStart = editor.SelectionStart;
			this.selectionLength = editor.SelectionLength;
			this.location = location;
			InitializeServices();
		}
		
		void InitializeServices()
		{
			this.Services = new ServiceContainer(SD.Services);
			this.Services.AddService(typeof(NamingConventionService), new SDNamingConventionService());
			this.Services.AddService(typeof(CodeGenerationService), new SDCodeGenerationService());
		}
		
		public override bool Supports(Version version)
		{
			CSharpProject project = resolver.TypeResolveContext.Compilation.GetProject() as CSharpProject;
			if (project == null)
				return false;
			return project.LanguageVersion >= version;
		}
		
		public Script StartScript()
		{
			var formattingOptions = CSharpFormattingPolicies.Instance.GetProjectOptions(resolver.Compilation.GetProject());
			if (editor != null)
				return new EditorScript(editor, this, formattingOptions.OptionsContainer.GetEffectiveOptions());
			else if (document == null || document is ReadOnlyDocument)
				throw new InvalidOperationException("Cannot start a script in a read-only context");
			else {
				var textEditorOptions = this.TextEditorOptions;
				formattingOptions.OptionsContainer.CustomizeEditorOptions(textEditorOptions);
				return new DocumentScript(document, formattingOptions.OptionsContainer.GetEffectiveOptions(), textEditorOptions);
			}
		}
		
		public IDocument Document {
			get {
				IDocument result = LazyInit.VolatileRead(ref document);
				if (result != null)
					return result;
				return LazyInit.GetOrSet(ref document, new ReadOnlyDocument(textSource, resolver.UnresolvedFile.FileName));
			}
		}
		
		public override TextLocation Location {
			get { return location; }
		}
		
		public override TextLocation SelectionStart {
			get { return GetLocation(selectionStart); }
		}
		
		public override TextLocation SelectionEnd {
			get {
				return GetLocation(selectionStart + selectionLength);
			}
		}
		
		public override string SelectedText {
			get {
				return textSource.GetText(selectionStart, selectionLength);
			}
		}
		
		public override bool IsSomethingSelected {
			get {
				return selectionLength > 0;
			}
		}
		
		public override string DefaultNamespace {
			get {
				return string.Empty; // TODO: get namespace from current project
			}
		}
		
		public override string GetText(int offset, int length)
		{
			return textSource.GetText(offset, length);
		}
		
		public override string GetText(ISegment segment)
		{
			return textSource.GetText(segment);
		}
		
		public ITextSourceVersion Version {
			get { return textSource.Version; }
		}
		
		public override IDocumentLine GetLineByOffset(int offset)
		{
			return Document.GetLineByOffset(offset);
		}
		
		public override int GetOffset(TextLocation location)
		{
			return Document.GetOffset(location);
		}
		
		public override TextLocation GetLocation(int offset)
		{
			return Document.GetLocation(offset);
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
