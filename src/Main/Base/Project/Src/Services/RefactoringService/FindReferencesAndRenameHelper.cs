// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.Core;
using ICSharpCode.NRefactory;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.AvalonEdit;
using ICSharpCode.SharpDevelop.Editor.Search;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Refactoring
{
	public static class FindReferencesAndRenameHelper
	{
		#region Extract Interface
		public static void ExtractInterface(IClass c)
		{
			ExtractInterfaceOptions extractInterface = new ExtractInterfaceOptions(c);
			using (ExtractInterfaceDialog eid = new ExtractInterfaceDialog()) {
				extractInterface = eid.ShowDialog(extractInterface);
				if (extractInterface.IsCancelled) {
					return;
				}
				
				// do rename
				/*
				 MessageService.ShowMessageFormatted("Extracting interface",
				                                    @"Extracting {0} [{1}] from {2} into {3}",
				                                    extractInterface.NewInterfaceName,
				                                    extractInterface.FullyQualifiedName,
				                                    extractInterface.ClassEntity.Name,
				                                    extractInterface.NewFileName
				                                   );
`				*/
			}
			
			string newInterfaceFileName = Path.Combine(Path.GetDirectoryName(c.CompilationUnit.FileName),
			                                           extractInterface.NewFileName);
			if (File.Exists(newInterfaceFileName)) {
				int confirmReplace = MessageService.ShowCustomDialog("Extract Interface",
				                                                     newInterfaceFileName+" already exists!",
				                                                     0,
				                                                     1,
				                                                     "${res:Global.ReplaceButtonText}",
				                                                     "${res:Global.AbortButtonText}");
				if (confirmReplace != 0) {
					return;
				}
			}
			
			LanguageProperties language = c.ProjectContent.Language;
			string classFileName = c.CompilationUnit.FileName;
			string existingClassCode = ParserService.GetParseableFileContent(classFileName).Text;
			
			// build the new interface...
			string newInterfaceCode = language.RefactoringProvider.GenerateInterfaceForClass(extractInterface.NewInterfaceName,
			                                                                                 existingClassCode,
			                                                                                 extractInterface.ChosenMembers,
			                                                                                 c, extractInterface.IncludeComments);
			if (newInterfaceCode == null)
				return;
			
			// ...dump it to a file...
			IViewContent viewContent = FileService.GetOpenFile(newInterfaceFileName);
			ITextEditorProvider editable = viewContent as ITextEditorProvider;
			
			if (viewContent != null && editable != null) {
				// simply update it
				editable.TextEditor.Document.Text = newInterfaceCode;
				viewContent.PrimaryFile.SaveToDisk();
			} else {
				// create it
				viewContent = FileService.NewFile(newInterfaceFileName, newInterfaceCode);
				viewContent.PrimaryFile.SaveToDisk(newInterfaceFileName);

				// ... and add it to the project
				IProject project = (IProject)c.ProjectContent.Project;
				if (project != null) {
					FileProjectItem projectItem = new FileProjectItem(project, ItemType.Compile);
					projectItem.FileName = newInterfaceFileName;
					ProjectService.AddProjectItem(project, projectItem);
					FileService.FireFileCreated(newInterfaceFileName, false);
					project.Save();
					ProjectBrowserPad.RefreshViewAsync();
				}
			}
			
			ICompilationUnit newCompilationUnit = ParserService.ParseFile(newInterfaceFileName).CompilationUnit;
			IClass newInterfaceDef = newCompilationUnit.Classes[0];
			
			// finally, add the interface to the base types of the class that we're extracting from
			if (extractInterface.AddInterfaceToClass) {
				string modifiedClassCode = language.RefactoringProvider.AddBaseTypeToClass(existingClassCode, c, newInterfaceDef);
				if (modifiedClassCode == null) {
					return;
				}
				
				// TODO: replacing the whole text is not an option, we would loose all breakpoints/bookmarks.
				viewContent = FileService.OpenFile(classFileName);
				editable = viewContent as ITextEditorProvider;
				if (editable == null) {
					return;
				}
				editable.TextEditor.Document.Text = modifiedClassCode;
			}
		}
		#endregion
		
		#region Rename Class
		public static void RenameClass(IClass c)
		{
			string newName = MessageService.ShowInputBox("${res:SharpDevelop.Refactoring.Rename}", "${res:SharpDevelop.Refactoring.RenameClassText}", c.Name);
			if (!FindReferencesAndRenameHelper.CheckName(newName, c.Name)) return;
			
			using (AsynchronousWaitDialog monitor = AsynchronousWaitDialog.ShowWaitDialog("${res:SharpDevelop.Refactoring.Rename}"))
			{
				RenameClass(c, newName);
			}
		}
		
		public static void RenameClass(IClass c, string newName)
		{
			if (c == null)
				throw new ArgumentNullException("c");
			if (newName == null)
				throw new ArgumentNullException("newName");
			c = c.GetCompoundClass(); // get compound class if class is partial
			
			List<Reference> list = RefactoringService.FindReferences(c, null);
			if (list == null) return;
			
			// Add the class declaration(s)
			foreach (IClass part in GetClassParts(c)) {
				AddDeclarationAsReference(list, part.CompilationUnit.FileName, part.Region, part.Name);
			}
			
			// Add the constructors
			foreach (IMethod m in c.Methods) {
				if (m.IsConstructor || (m.Name == "#dtor")) {
					AddDeclarationAsReference(list, m.DeclaringType.CompilationUnit.FileName, m.Region, c.Name);
				}
			}
			
			FindReferencesAndRenameHelper.RenameReferences(list, newName);
		}
		
		static IList<IClass> GetClassParts(IClass c)
		{
			CompoundClass cc = c as CompoundClass;
			if (cc != null) {
				return cc.Parts;
			} else {
				return new IClass[] {c};
			}
		}
		
		static void AddDeclarationAsReference(List<Reference> list, string fileName, DomRegion region, string name)
		{
			if (fileName == null)
				return;
			ProvidedDocumentInformation documentInformation = FindReferencesAndRenameHelper.GetDocumentInformation(fileName);
			int offset = documentInformation.Document.PositionToOffset(region.BeginLine, region.BeginColumn);
			string text = documentInformation.Document.GetText(offset, Math.Min(name.Length + 30, documentInformation.Document.TextLength - offset - 1));
			int offsetChange = -1;
			do {
				offsetChange = text.IndexOf(name, offsetChange + 1);
				if (offsetChange < 0 || offsetChange >= text.Length)
					return;
			} while (offsetChange + name.Length < text.Length
			         && char.IsLetterOrDigit(text[offsetChange + name.Length]));
			offset += offsetChange;
			foreach (Reference r in list) {
				if (r.Offset == offset)
					return;
			}
			list.Add(new Reference(fileName, offset, name.Length, name, null));
		}
		#endregion
		
		#region Rename member
		public static void RenameMember(IMember member)
		{
			string newName = MessageService.ShowInputBox("${res:SharpDevelop.Refactoring.Rename}", "${res:SharpDevelop.Refactoring.RenameMemberText}", member.Name);
			if (!FindReferencesAndRenameHelper.CheckName(newName, member.Name)) return;
			
			RenameMember(member, newName);
		}
		
		public static bool RenameMember(IMember member, string newName)
		{
			List<Reference> list;
			using (AsynchronousWaitDialog monitor = AsynchronousWaitDialog.ShowWaitDialog("${res:SharpDevelop.Refactoring.Rename}"))
			{
				list = RefactoringService.FindReferences(member, monitor);
				if (list == null) return false;
				FindReferencesAndRenameHelper.RenameReferences(list, newName);
			}
			
			if (member is IField) {
				IProperty property = FindProperty((IField)member);
				if (property != null) {
					string newPropertyName = member.DeclaringType.ProjectContent.Language.CodeGenerator.GetPropertyName(newName);
					if (newPropertyName != newName && newPropertyName != property.Name) {
						if (MessageService.AskQuestionFormatted("${res:SharpDevelop.Refactoring.Rename}", "${res:SharpDevelop.Refactoring.RenameFieldAndProperty}", property.FullyQualifiedName, newPropertyName)) {
							using (AsynchronousWaitDialog monitor = AsynchronousWaitDialog.ShowWaitDialog("${res:SharpDevelop.Refactoring.Rename}"))
							{
								list = RefactoringService.FindReferences(property, monitor);
								if (list != null) {
									FindReferencesAndRenameHelper.RenameReferences(list, newPropertyName);
								}
							}
						}
					}
				}
			}
			return true;
		}
		
		internal static IProperty FindProperty(IField field)
		{
			LanguageProperties language = field.DeclaringType.ProjectContent.Language;
			if (language.CodeGenerator == null) return null;
			string propertyName = language.CodeGenerator.GetPropertyName(field.Name);
			IProperty foundProperty = null;
			foreach (IProperty prop in field.DeclaringType.Properties) {
				if (language.NameComparer.Equals(propertyName, prop.Name)) {
					foundProperty = prop;
					break;
				}
			}
			return foundProperty;
		}
		#endregion
		
		#region Common helper functions
		public static ProvidedDocumentInformation GetDocumentInformation(string fileName)
		{
			OpenedFile file = FileService.GetOpenedFile(fileName);
			if (file != null) {
				IFileDocumentProvider documentProvider = file.CurrentView as IFileDocumentProvider;
				if (documentProvider != null) {
					IDocument document = documentProvider.GetDocumentForFile(file);
					if (document != null) {
						return new ProvidedDocumentInformation(document, fileName, 0);
					}
				}
			}
			ITextBuffer fileContent = ParserService.GetParseableFileContent(fileName);
			return new ProvidedDocumentInformation(fileContent, fileName, 0);
		}
		
		public static bool IsReadOnly(IClass c)
		{
			return c.CompilationUnit.FileName == null || c.GetCompoundClass().IsSynthetic;
		}
		
		public static ITextEditor JumpToDefinition(IMember member)
		{
			IViewContent viewContent = null;
			ICompilationUnit cu = member.DeclaringType.CompilationUnit;
			if (cu != null) {
				string fileName = cu.FileName;
				if (fileName != null) {
					if (!member.Region.IsEmpty) {
						viewContent = FileService.JumpToFilePosition(fileName, member.Region.BeginLine, member.Region.BeginColumn);
					} else {
						FileService.OpenFile(fileName);
					}
				}
			}
			ITextEditorProvider tecp = viewContent as ITextEditorProvider;
			return (tecp == null) ? null : tecp.TextEditor;
		}
		
		public static ITextEditor OpenDefinitionFile(IMember member, bool switchTo)
		{
			IViewContent viewContent = null;
			ICompilationUnit cu = member.DeclaringType.CompilationUnit;
			if (cu != null) {
				string fileName = cu.FileName;
				if (fileName != null) {
					viewContent = FileService.OpenFile(fileName, switchTo);
				}
			}
			ITextEditorProvider tecp = viewContent as ITextEditorProvider;
			return (tecp == null) ? null : tecp.TextEditor;
		}
		
		public static ITextEditor JumpBehindDefinition(IMember member)
		{
			IViewContent viewContent = null;
			ICompilationUnit cu = member.DeclaringType.CompilationUnit;
			if (cu != null) {
				string fileName = cu.FileName;
				if (fileName != null) {
					if (!member.Region.IsEmpty) {
						viewContent = FileService.JumpToFilePosition(fileName, member.Region.EndLine + 1, 1);
					} else {
						FileService.OpenFile(fileName);
					}
				}
			}
			ITextEditorProvider tecp = viewContent as ITextEditorProvider;
			return (tecp == null) ? null : tecp.TextEditor;
		}
		
		public static bool CheckName(string name, string oldName)
		{
			if (name == null || name.Length == 0 || name == oldName)
				return false;
			if (!char.IsLetter(name, 0) && name[0] != '_') {
				MessageService.ShowError("${res:SharpDevelop.Refactoring.InvalidNameStart}");
				return false;
			}
			for (int i = 1; i < name.Length; i++) {
				if (!char.IsLetterOrDigit(name, i) && name[i] != '_') {
					MessageService.ShowError("${res:SharpDevelop.Refactoring.InvalidName}");
					return false;
				}
			}
			return true;
		}
		
		public struct Modification
		{
			public IDocument Document;
			public int Offset;
			public int LengthDifference;
			
			public Modification(IDocument document, int offset, int lengthDifference)
			{
				this.Document = document;
				this.Offset = offset;
				this.LengthDifference = lengthDifference;
			}
		}
		
		public static void ModifyDocument(List<Modification> modifications, IDocument doc, int offset, int length, string newName)
		{
			using (doc.OpenUndoGroup()) {
				foreach (Modification m in modifications) {
					if (m.Document == doc) {
						if (m.Offset < offset)
							offset += m.LengthDifference;
					}
				}
				int lengthDifference = newName.Length - length;
				doc.Replace(offset, length, newName);
				if (lengthDifference != 0) {
					for (int i = 0; i < modifications.Count; ++i) {
						Modification m = modifications[i];
						if (m.Document == doc) {
							if (m.Offset > offset) {
								m.Offset += lengthDifference;
								modifications[i] = m; // Modification is a value type
							}
						}
					}
					modifications.Add(new Modification(doc, offset, lengthDifference));
				}
			}
		}
		
		public static void ShowAsSearchResults(string title, List<Reference> list)
		{
			if (list == null) return;
			List<SearchResultMatch> results = new List<SearchResultMatch>(list.Count);
			TextDocument document = null;
			ITextBuffer buffer = null;
			FileName fileName = null;
			ISyntaxHighlighter highlighter = null;
			foreach (Reference r in list) {
				var f = new FileName(r.FileName);
				if (document == null || !f.Equals(fileName)) {
					buffer = ParserService.GetParseableFileContent(r.FileName);
					document = new TextDocument(DocumentUtilitites.GetTextSource(buffer));
					fileName = new FileName(r.FileName);
					highlighter = EditorControlService.Instance.CreateHighlighter(new AvalonEditDocumentAdapter(document, null), fileName);
				}
				var start = document.GetLocation(r.Offset).ToLocation();
				var end = document.GetLocation(r.Offset + r.Length).ToLocation();
				var builder = SearchResultsPad.CreateInlineBuilder(start, end, document, highlighter);
				var defaultTextColor = highlighter != null ? highlighter.DefaultTextColor : null;
				SearchResultMatch res = new SearchResultMatch(fileName, start, end, r.Offset, r.Length, builder, defaultTextColor);
				results.Add(res);
			}
			SearchResultsPad.Instance.ShowSearchResults(title, results);
			SearchResultsPad.Instance.BringToFront();
		}
		
		sealed class FileView {
			public IViewContent ViewContent;
			public OpenedFile OpenedFile;
		}
		
		public static void RenameReferences(List<Reference> list, string newName)
		{
			Dictionary<IDocument, FileView> modifiedDocuments = new Dictionary<IDocument, FileView>();
			List<Modification> modifications = new List<Modification>();
			foreach (Reference r in list) {
				IDocument document = null;
				IViewContent viewContent = null;
				
				OpenedFile file = FileService.GetOpenedFile(r.FileName);
				if (file != null) {
					viewContent = file.CurrentView;
					IFileDocumentProvider p = viewContent as IFileDocumentProvider;
					if (p != null) {
						document = p.GetDocumentForFile(file);
					}
				}
				
				if (document == null) {
					viewContent = FileService.OpenFile(r.FileName, false);
					IFileDocumentProvider p = viewContent as IFileDocumentProvider;
					if (p != null) {
						file = FileService.GetOpenedFile(r.FileName);
						System.Diagnostics.Debug.Assert(file != null, "OpenedFile not found after opening the file.");
						document = p.GetDocumentForFile(file);
					}
				}
				
				if (document == null) {
					LoggingService.Warn("RenameReferences: Could not get document for file '" + r.FileName + "'");
					continue;
				}
				
				if (!modifiedDocuments.ContainsKey(document)) {
					modifiedDocuments.Add(document, new FileView() {ViewContent = viewContent, OpenedFile = file});
					document.StartUndoableAction();
				}
				
				ModifyDocument(modifications, document, r.Offset, r.Length, newName);
			}
			foreach (KeyValuePair<IDocument, FileView> entry in modifiedDocuments) {
				entry.Key.EndUndoableAction();
				entry.Value.OpenedFile.MakeDirty();
				if (entry.Value.ViewContent is IEditable) {
					ParserService.ParseViewContent(entry.Value.ViewContent);
				} else {
					ParserService.ParseFile(entry.Value.OpenedFile.FileName, entry.Key);
				}
			}
		}

		public static void MoveClassToFile(IClass c, string newFileName)
		{
			LanguageProperties language = c.ProjectContent.Language;
			string existingCode = ParserService.GetParseableFileContent(c.CompilationUnit.FileName).Text;
			DomRegion fullRegion = language.RefactoringProvider.GetFullCodeRangeForType(existingCode, c);
			if (fullRegion.IsEmpty) return;
			
			Action removeExtractedCodeAction;
			string newCode = ExtractCode(c, fullRegion, c.BodyRegion.BeginLine, out removeExtractedCodeAction);
			
			newCode = language.RefactoringProvider.CreateNewFileLikeExisting(existingCode, newCode);
			if (newCode == null)
				return;
			IViewContent viewContent = FileService.NewFile(newFileName, newCode);
			viewContent.PrimaryFile.SaveToDisk(newFileName);
			// now that the code is saved in the other file, remove it from the original document
			removeExtractedCodeAction();
			
			IProject project = (IProject)c.ProjectContent.Project;
			if (project != null) {
				FileProjectItem projectItem = new FileProjectItem(project, ItemType.Compile);
				projectItem.FileName = newFileName;
				ProjectService.AddProjectItem(project, projectItem);
				FileService.FireFileCreated(newFileName, false);
				project.Save();
				ProjectBrowserPad.RefreshViewAsync();
			}
		}
		
		public static string ExtractCode(IClass c, DomRegion codeRegion, int indentationLine, out Action removeExtractedCodeAction)
		{
			IDocument doc = GetDocument(c);
			if (indentationLine < 1) indentationLine = 1;
			if (indentationLine >= doc.TotalNumberOfLines) indentationLine = doc.TotalNumberOfLines;
			
			IDocumentLine segment = doc.GetLine(indentationLine);
			string mainLine = doc.GetText(segment.Offset, segment.Length);
			string indentation = mainLine.Substring(0, mainLine.Length - mainLine.TrimStart().Length);
			
			segment = doc.GetLine(codeRegion.BeginLine);
			int startOffset = segment.Offset;
			segment = doc.GetLine(codeRegion.EndLine);
			int endOffset = segment.Offset + segment.Length;
			
			StringReader reader = new StringReader(doc.GetText(startOffset, endOffset - startOffset));
			removeExtractedCodeAction = delegate {
				doc.Remove(startOffset, endOffset - startOffset);
			};
			
			// now remove indentation from extracted source code
			string line;
			StringBuilder b = new StringBuilder();
			int endOfLastFilledLine = 0;
			while ((line = reader.ReadLine()) != null) {
				int startpos;
				for (startpos = 0; startpos < line.Length && startpos < indentation.Length; startpos++) {
					if (line[startpos] != indentation[startpos])
						break;
				}
				if (startpos == line.Length) {
					// empty line
					if (b.Length > 0) {
						b.AppendLine();
					}
				} else {
					b.Append(line, startpos, line.Length - startpos);
					b.AppendLine();
					endOfLastFilledLine = b.Length;
				}
			}
			b.Length = endOfLastFilledLine;
			return b.ToString();
		}

		public static IDocument GetDocument(IClass c)
		{
			if (c is CompoundClass)
				throw new ArgumentException("Cannot get document from compound class - must pass a specific class part");
			
			ITextEditorProvider tecp = FileService.OpenFile(c.CompilationUnit.FileName) as ITextEditorProvider;
			if (tecp == null) return null;
			return tecp.TextEditor.Document;
		}
		
		#endregion
		
		
		#region Find references
		public static void RunFindReferences(IMember member)
		{
			string memberName = member.DeclaringType.Name + "." + member.Name;
			using (AsynchronousWaitDialog monitor = AsynchronousWaitDialog.ShowWaitDialog("${res:SharpDevelop.Refactoring.FindReferences}", true)) {
				FindReferencesAndRenameHelper.ShowAsSearchResults(
					StringParser.Parse("${res:SharpDevelop.Refactoring.ReferencesTo}",
					                   new StringTagPair("Name", memberName)),
					RefactoringService.FindReferences(member, monitor));
			}
		}
		
		public static void RunFindReferences(IClass c)
		{
			using (AsynchronousWaitDialog monitor = AsynchronousWaitDialog.ShowWaitDialog("${res:SharpDevelop.Refactoring.FindReferences}", true)) {
				FindReferencesAndRenameHelper.ShowAsSearchResults(
					StringParser.Parse("${res:SharpDevelop.Refactoring.ReferencesTo}",
					                   new StringTagPair("Name", c.Name)),
					RefactoringService.FindReferences(c, monitor)
				);
			}
		}
		
		public static void RunFindReferences(LocalResolveResult local)
		{
			FindReferencesAndRenameHelper.ShowAsSearchResults(
				StringParser.Parse("${res:SharpDevelop.Refactoring.ReferencesTo}",
				                   new StringTagPair("Name", local.VariableName)),
				RefactoringService.FindReferences(local, null)
			);
		}
		
		public static ICSharpCode.Core.WinForms.MenuCommand MakeFindReferencesMenuCommand(EventHandler handler)
		{
			return new ICSharpCode.Core.WinForms.MenuCommand("${res:SharpDevelop.Refactoring.FindReferencesCommand}", handler) {
				ShortcutKeys = System.Windows.Forms.Keys.F12
			};
		}
		#endregion
	}
}
