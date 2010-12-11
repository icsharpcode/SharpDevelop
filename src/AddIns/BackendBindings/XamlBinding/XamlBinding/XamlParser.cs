// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using System.Linq;

using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Xml;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;

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

		public LanguageProperties Language
		{
			get { return LanguageProperties.CSharp; }
		}
		
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
		
		AXmlParser parser = new AXmlParser();
		volatile ITextBufferVersion lastParsedVersion;
		
		/// <summary>
		/// Parse the given text and enter read lock.
		/// No parsing is done if the text is older than seen before.
		/// </summary>
		public IDisposable ParseAndLock(ITextBuffer fileContent)
		{
			// Copy to ensure thread-safety
			var lastVer = this.lastParsedVersion;
			if (lastVer == null ||                                       // First parse
			    fileContent.Version == null ||                           // Versioning not supported
			    !fileContent.Version.BelongsToSameDocumentAs(lastVer) || // Different document instance? Can happen after closing and reopening of file.
			    fileContent.Version.CompareAge(lastVer) > 0)             // Is fileContent newer?
			{
				parser.Lock.EnterWriteLock();
				// Double check, now that we are thread-safe
				lastVer = this.lastParsedVersion;
				if (lastVer == null || fileContent.Version == null || !fileContent.Version.BelongsToSameDocumentAs(lastVer)) {
					// First parse or versioning not supported
					using (DebugTimer.Time("normal parse"))
						parser.Parse(fileContent.Text, null);
					this.lastParsedVersion = fileContent.Version;
				} else if (fileContent.Version.CompareAge(lastParsedVersion) > 0) {
					// Incremental parse
					var changes = lastParsedVersion.GetChangesTo(fileContent.Version).
						Select(c => new DocumentChangeEventArgs(c.Offset, c.RemovedText, c.InsertedText));
					using (DebugTimer.Time("incremental parse"))
						parser.Parse(fileContent.Text, changes);
					this.lastParsedVersion = fileContent.Version;
				} else {
					// fileContent is older - no need to parse
				}
				parser.Lock.EnterReadLock();
				parser.Lock.ExitWriteLock();
			} else {
				// fileContent is older - no need to parse
				parser.Lock.EnterReadLock();
			}
			return new CallbackOnDispose(() => parser.Lock.ExitReadLock());
		}

		public ICompilationUnit Parse(IProjectContent projectContent, string fileName, ITextBuffer fileContent)
		{
			ICompilationUnit compilationUnit;
			
			using (ParseAndLock(fileContent)) {
				var document = parser.LastDocument;
				
				CompilationUnitCreatorVisitor visitor =
					new CompilationUnitCreatorVisitor(projectContent, fileContent.Text, fileName, lexerTags);
				
				document.AcceptVisitor(visitor);
				
				compilationUnit = visitor.CompilationUnit;
			}
			
			// During project load all XAML files are parsed
			// most of them are not opened, thus fileContent.Version is null.
			// We can clear the parser data, because the file will be reparsed
			// as soon as it is opened by the user.
			
			// This will save us some memory, because we only use the
			// compilation unit created by the visitor above for code completion.
			if (fileContent.Version == null) {
				parser.Lock.EnterWriteLock();
				// double-checked locking (other threads might parse the document in the meantime)
				if (lastParsedVersion == null) {
					parser.Clear();
				}
				parser.Lock.ExitWriteLock();
			}
			
			return compilationUnit;
		}
		
		/// <summary>
		/// Wraps AXmlParser.LastDocument. Returns the last cached version of the document.
		/// </summary>
		/// <exception cref="InvalidOperationException">No read lock is held by the current thread.</exception>
		public AXmlDocument LastDocument {
			get { return parser.LastDocument; }
		}

		public IExpressionFinder CreateExpressionFinder(string fileName)
		{
			return XamlExpressionFinder.Instance;
		}

		public IResolver CreateResolver()
		{
			return new XamlResolver();
		}
	}
}
