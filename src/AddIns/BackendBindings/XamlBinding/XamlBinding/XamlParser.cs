// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.Core;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem.Implementation;
using ICSharpCode.NRefactory.Utils;
using ICSharpCode.NRefactory.Xml;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Parser;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Refactoring;

namespace ICSharpCode.XamlBinding
{
	/// <summary>
	/// Parses xaml files to partial classes for the Dom.
	/// </summary>
	public class XamlParser : IParser
	{
		string[] lexerTags;
		
		public string[] LexerTags
		{
			get { return lexerTags; }
			set { lexerTags = value; }
		}

//		public LanguageProperties Language
//		{
//			get { return LanguageProperties.CSharp; }
//		}
		
		public XamlParser()
		{
		}

		public bool CanParse(string fileName)
		{
			return Path.GetExtension(fileName).Equals(".xaml", StringComparison.OrdinalIgnoreCase);
		}

		public bool CanParse(ICSharpCode.SharpDevelop.Project.IProject project)
		{
			return false;
		}
		
//		volatile ITextSourceVersion lastParsedVersion;
		
		/// <summary>
		/// Parse the given text and enter read lock.
		/// No parsing is done if the text is older than seen before.
		/// </summary>
//		public IDisposable ParseAndLock(ITextSource fileContent)
//		{
//			// Copy to ensure thread-safety
//			var lastVer = this.lastParsedVersion;
//			if (lastVer == null ||                                       // First parse
//			    fileContent.Version == null ||                           // Versioning not supported
//			    !fileContent.Version.BelongsToSameDocumentAs(lastVer) || // Different document instance? Can happen after closing and reopening of file.
//			    fileContent.Version.CompareAge(lastVer) > 0)             // Is fileContent newer?
//			{
//				parser.Lock.EnterWriteLock();
//				// Double check, now that we are thread-safe
//				lastVer = this.lastParsedVersion;
//				if (lastVer == null || fileContent.Version == null || !fileContent.Version.BelongsToSameDocumentAs(lastVer)) {
//					// First parse or versioning not supported
//					using (DebugTimer.Time("normal parse"))
//						parser.Parse(fileContent.Text, null);
//					this.lastParsedVersion = fileContent.Version;
//				} else if (fileContent.Version.CompareAge(lastParsedVersion) > 0) {
//					// Incremental parse
//					var changes = lastParsedVersion.GetChangesTo(fileContent.Version).
//						Select(c => new DocumentChangeEventArgs(c.Offset, c.RemovedText, c.InsertedText));
//					using (DebugTimer.Time("incremental parse"))
//						parser.Parse(fileContent.Text, changes);
//					this.lastParsedVersion = fileContent.Version;
//				} else {
//					// fileContent is older - no need to parse
//				}
//				parser.Lock.EnterReadLock();
//				parser.Lock.ExitWriteLock();
//			} else {
//				// fileContent is older - no need to parse
//				parser.Lock.EnterReadLock();
//			}
//			return new CallbackOnDispose(() => parser.Lock.ExitReadLock());
//		}
//
//		public ICompilationUnit Parse(IProjectContent projectContent, string fileName, ITextSource fileContent)
//		{
//			ICompilationUnit compilationUnit;
//
//			using (ParseAndLock(fileContent)) {
//				var document = parser.LastDocument;
//
//				CompilationUnitCreatorVisitor visitor =
//					new CompilationUnitCreatorVisitor(projectContent, fileContent.Text, fileName, lexerTags);
//
//				document.AcceptVisitor(visitor);
//
//				compilationUnit = visitor.CompilationUnit;
//			}
//
//			// During project load all XAML files are parsed
//			// most of them are not opened, thus fileContent.Version is null.
//			// We can clear the parser data, because the file will be reparsed
//			// as soon as it is opened by the user.
//
//			// This will save us some memory, because we only use the
//			// compilation unit created by the visitor above for code completion.
//			if (fileContent.Version == null) {
//				parser.Lock.EnterWriteLock();
//				// double-checked locking (other threads might parse the document in the meantime)
//				if (lastParsedVersion == null) {
//					parser.Clear();
//				}
//				parser.Lock.ExitWriteLock();
//			}
//
//			return compilationUnit;
//		}
		
		volatile IncrementalParserState parserState;
		
		public ParseInformation Parse(FileName fileName, ITextSource fileContent, bool fullParseInformationRequested)
		{
			AXmlParser parser = new AXmlParser();
			AXmlDocument document;
			IncrementalParserState newParserState;
			document = parser.ParseIncremental(parserState, fileContent, out newParserState);
			parserState = newParserState;
			XamlParsedFile parsedFile = XamlParsedFile.Create(fileName, fileContent, document);
			ParseInformation parseInfo;
			if (fullParseInformationRequested)
				parseInfo = new XamlFullParseInformation(parsedFile, document, fileContent);
			else
				parseInfo = new ParseInformation(parsedFile, false);
			AddTagComments(document, parseInfo, fileContent);
			return parseInfo;
		}
		
		void AddTagComments(AXmlDocument xmlDocument, ParseInformation parseInfo, ITextSource fileContent)
		{
			IDocument document = null;
			foreach (var tag in TreeTraversal.PreOrder<AXmlObject>(xmlDocument, node => node.Children).OfType<AXmlTag>().Where(t => t.IsComment)) {
				int matchLength;
				AXmlText comment = tag.Children.OfType<AXmlText>().First();
				int index = comment.Value.IndexOfAny(lexerTags, 0, out matchLength);
				if (index > -1) {
					if (document == null)
						document = fileContent as IDocument ?? new ReadOnlyDocument(fileContent);
					do {
						TextLocation startLocation = document.GetLocation(comment.StartOffset + index);
						int startOffset = index + comment.StartOffset;
						int endOffset = Math.Min(document.GetLineByOffset(startOffset).EndOffset, comment.EndOffset);
						string content = document.GetText(startOffset, endOffset - startOffset);
						parseInfo.TagComments.Add(new TagComment(content.Substring(0, matchLength), new DomRegion(parseInfo.FileName, startLocation.Line, startLocation.Column), content.Substring(matchLength)));
						index = comment.Value.IndexOfAny(lexerTags, endOffset - comment.StartOffset, out matchLength);
					} while (index > -1);
				}
			}
		}
		
		public ResolveResult Resolve(ParseInformation parseInfo, TextLocation location, ICompilation compilation, CancellationToken cancellationToken)
		{
			return new XamlResolver().Resolve((XamlFullParseInformation)parseInfo, location, compilation, cancellationToken);
		}
		
		public void FindLocalReferences(ParseInformation parseInfo, ITextSource fileContent, IVariable variable, ICompilation compilation, Action<Reference> callback, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}
		
		public ICompilation CreateCompilationForSingleFile(FileName fileName, IParsedFile parsedFile)
		{
			// TODO: create a simple compilation with WPF references?
			return null;
		}
	}
}
