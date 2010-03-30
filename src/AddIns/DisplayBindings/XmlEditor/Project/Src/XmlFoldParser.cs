// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.XmlEditor
{
	public class XmlFoldParser : IParser
	{
		DefaultXmlFileExtensions extensions;
		XmlEditorOptions options;
		IParserService parserService;

		string[] lexerTags = new string[0];
		XmlTextReader reader;
		List<FoldingRegion> folds;
		Stack<XmlElementFold> elementFoldStack;
		DefaultCompilationUnit unit;
		
		public XmlFoldParser(DefaultXmlFileExtensions extensions, 
			XmlEditorOptions options, 
			IParserService parserService)
		{
			this.extensions = extensions;
			this.options = options;
			this.parserService = parserService;
		}
		
		public XmlFoldParser()
			: this(new DefaultXmlFileExtensions(), 
				XmlEditorService.XmlEditorOptions, 
				new DefaultParserService())
		{
		}
		
		public string[] LexerTags {
			get { return lexerTags; }
			set { lexerTags = value; }
		}
		
		public LanguageProperties Language {
			get { return LanguageProperties.None; }
		}
		
		public IExpressionFinder CreateExpressionFinder(string fileName)
		{
			return null;
		}
		
		public bool CanParse(string fileName)
		{
			return extensions.Contains(Path.GetExtension(fileName).ToLowerInvariant());
		}
		
		public bool CanParse(IProject project)
		{
			return true;
		}
		
		public IResolver CreateResolver()
		{
			return null;
		}
		
		public ICompilationUnit Parse(IProjectContent projectContent, string fileName, ITextBuffer fileContent)
		{
			// SharpDevelop may call IParser.Parse in parallel. This will be done on the same IParser instance
			// if there are two parallel parse requests for the same file. Parser implementations must be thread-safe.
			
			// In XmlFoldParser, we do this by simply using a big lock per IParser instance.
			lock (this) {
				try {
					CreateCompilationUnit(projectContent, fileName);
					GetFolds(fileContent.CreateReader());
					AddFoldsToCompilationUnit(unit, folds);
				} catch (XmlException) {
					ICompilationUnit existingUnit = FindPreviouslyParsedCompilationUnit(projectContent, fileName);
					if (existingUnit != null) {
						return existingUnit;
					}
				}
				return unit;
			}
		}
		
		void CreateCompilationUnit(IProjectContent projectContent, string fileName)
		{
			unit = new DefaultCompilationUnit(projectContent);
			unit.FileName = fileName;
		}
		
		void GetFolds(TextReader textReader)
		{
			folds = new List<FoldingRegion>();
			elementFoldStack = new Stack<XmlElementFold>();
			
			CreateXmlTextReaderWithNoNamespaceSupport(textReader);
			
			while (reader.Read()) {
				switch (reader.NodeType) {
					case XmlNodeType.Element:
						AddElementFoldToStackIfNotEmptyElement();
						break;
					
					case XmlNodeType.EndElement:
						CreateElementFoldingRegionIfNotSingleLine();
						break;
						
					case XmlNodeType.Comment:
						CreateCommentFoldingRegionIfNotSingleLine();
						break;
				}
			}
			folds.Sort(CompareFoldingRegion);
		}
		
		ICompilationUnit FindPreviouslyParsedCompilationUnit(IProjectContent projectContent, string fileName)
		{
			ParseInformation parseInfo = parserService.GetExistingParseInformation(projectContent, fileName);
			if (parseInfo != null) {
				return parseInfo.CompilationUnit;
			}
			return null;
		}
		
		void CreateXmlTextReaderWithNoNamespaceSupport(TextReader textReader)
		{
			reader = new XmlTextReader(textReader);
			reader.XmlResolver = null; // prevent XmlTextReader from loading external DTDs
			reader.Namespaces = false;
		}

		void AddFoldsToCompilationUnit(DefaultCompilationUnit unit, List<FoldingRegion> folds)
		{
			unit.FoldingRegions.AddRange(folds);
		}
		
		void AddElementFoldToStackIfNotEmptyElement()
		{
			if (!reader.IsEmptyElement) {
				XmlElementFold fold = new XmlElementFold();
				fold.ReadStart(reader);
				elementFoldStack.Push(fold);
			}
		}
		
		void CreateElementFoldingRegionIfNotSingleLine()
		{
			XmlElementFold fold = elementFoldStack.Pop();
			fold.ReadEnd(reader);
			if (!fold.IsSingleLine) {
				FoldingRegion foldingRegion = CreateFoldingRegion(fold);
				folds.Add(foldingRegion);
			}
		}
		
		FoldingRegion CreateFoldingRegion(XmlElementFold fold)
		{
			if (options.ShowAttributesWhenFolded) {
				return fold.CreateFoldingRegionWithAttributes();
			}
			return fold.CreateFoldingRegion();
		}
		
		/// <summary>
		/// Creates a comment fold if the comment spans more than one line.
		/// </summary>
		/// <remarks>The text displayed when the comment is folded is the first 
		/// line of the comment.</remarks>
		void CreateCommentFoldingRegionIfNotSingleLine()
		{
			XmlCommentFold fold = new XmlCommentFold(reader);
			if (!fold.IsSingleLine) {
				folds.Add(fold.CreateFoldingRegion());
			}
		}
		
		int CompareFoldingRegion(FoldingRegion lhs, FoldingRegion rhs)
		{
			int compareBeginLine = lhs.Region.BeginLine.CompareTo(rhs.Region.BeginLine);
			if (compareBeginLine == 0) {
				return lhs.Region.BeginColumn.CompareTo(rhs.Region.BeginColumn);
			}
			return compareBeginLine;
		}
	}
}
