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
		string[] lexerTags = new string[0];
		DefaultXmlFileExtensions extensions;
		XmlTextReader reader;
		List<FoldingRegion> folds;
		Stack<XmlElementFold> elementFoldStack;
		XmlEditorOptions options;
		
		public XmlFoldParser(DefaultXmlFileExtensions extensions, XmlEditorOptions options)
		{
			this.extensions = extensions;
			this.options = options;
		}
		
		public XmlFoldParser()
			: this(new DefaultXmlFileExtensions(), XmlEditorService.XmlEditorOptions)
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
			DefaultCompilationUnit unit = new DefaultCompilationUnit(projectContent);
			unit.FileName = fileName;
			GetFolds(fileContent.CreateReader());
			unit.FoldingRegions.AddRange(folds);
			return unit;
		}
		
		void GetFolds(TextReader textReader)
		{
			folds = new List<FoldingRegion>();
			elementFoldStack = new Stack<XmlElementFold>();
			
			try {
				reader = new XmlTextReader(textReader);
				reader.Namespaces = false;
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
			} catch (Exception ex) {
//				// If the xml is not well formed keep the foldings 
//				// that already exist in the document.
//				//return new List<FoldMarker>(document.FoldingManager.FoldMarker);
//				return new List<FoldingRegion>();
				Console.WriteLine(ex.ToString());
			}

			folds.Sort(CompareFoldingRegion);
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
