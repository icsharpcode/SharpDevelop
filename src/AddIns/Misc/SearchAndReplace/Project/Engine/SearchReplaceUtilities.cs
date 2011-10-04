// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom.Refactoring;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Gui;

namespace SearchAndReplace
{
	public sealed class SearchReplaceUtilities
	{
		public static bool IsTextAreaSelected {
			get {
				return WorkbenchSingleton.Workbench.ActiveViewContent is ITextEditorProvider;
			}
		}
		
		public static ITextEditor GetActiveTextEditor()
		{
			ITextEditorProvider provider = WorkbenchSingleton.Workbench.ActiveViewContent as ITextEditorProvider;
			if (provider != null) {
				return provider.TextEditor;
			} else {
				return null;
			}
		}
		
		static bool IsWordPart(char c)
		{
			return char.IsLetterOrDigit(c) || c == '_';
		}
		
		public static bool IsWholeWordAt(IDocument document, int offset, int length)
		{
			return (offset - 1 < 0 || !IsWordPart(document.GetCharAt(offset - 1))) &&
				(offset + length + 1 >= document.TextLength || !IsWordPart(document.GetCharAt(offset + length)));
		}
		
		public static ISearchStrategy CreateSearchStrategy(SearchStrategyType type)
		{
			switch (type) {
				case SearchStrategyType.Normal:
					return new BruteForceSearchStrategy(); // new KMPSearchStrategy();
				case SearchStrategyType.RegEx:
					return new RegExSearchStrategy();
				case SearchStrategyType.Wildcard:
					return new WildcardSearchStrategy();
				default:
					throw new System.NotImplementedException("CreateSearchStrategy for type " + type);
			}
		}
		
		public static IDocumentIterator CreateDocumentIterator(DocumentIteratorType type, IProgressMonitor monitor)
		{
			switch (type) {
				case DocumentIteratorType.CurrentDocument:
				case DocumentIteratorType.CurrentSelection:
					return new CurrentDocumentIterator();
				case DocumentIteratorType.Directory:
					try {
						if (!Directory.Exists(SearchOptions.LookIn)) {
							if (monitor != null) monitor.ShowingDialog = true;
							MessageService.ShowMessageFormatted("${res:Dialog.NewProject.SearchReplace.SearchStringNotFound.Title}", "${res:Dialog.NewProject.SearchReplace.LookIn.DirectoryNotFound}", FileUtility.NormalizePath(SearchOptions.LookIn));
							if (monitor != null) monitor.ShowingDialog = false;
							return new DummyDocumentIterator();
						}
					} catch (Exception ex) {
						if (monitor != null) monitor.ShowingDialog = true;
						MessageService.ShowMessage(ex.Message);
						if (monitor != null) monitor.ShowingDialog = false;
						return new DummyDocumentIterator();
					}
					return new DirectoryDocumentIterator(SearchOptions.LookIn,
					                                     SearchOptions.LookInFiletypes,
					                                     SearchOptions.IncludeSubdirectories);
				case DocumentIteratorType.AllOpenFiles:
					return new AllOpenDocumentIterator();
				case DocumentIteratorType.WholeProject:
					return new WholeProjectDocumentIterator();
				case DocumentIteratorType.WholeSolution:
					return new WholeSolutionDocumentIterator();
				default:
					throw new System.NotImplementedException("CreateDocumentIterator for type " + type);
			}
		}
		
		public static bool IsSearchable(string fileName)
		{
			const int BUFFER_LENGTH = 4 * 1024;
			
			if (!File.Exists(fileName))
				return false;
			
			using (var stream = File.OpenRead(fileName)) {
				string mime = "text/plain";
				if (stream.Length > 0) {
					stream.Position = 0;
					mime = MimeTypeDetection.FindMimeType(new BinaryReader(stream).ReadBytes(BUFFER_LENGTH));
				}
				
				return mime.StartsWith("text/", StringComparison.OrdinalIgnoreCase);
			}
		}
	}
}
