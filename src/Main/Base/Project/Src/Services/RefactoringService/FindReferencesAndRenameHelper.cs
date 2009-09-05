// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.NRefactory.Ast;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;
using SearchAndReplace;

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
			string existingClassCode = ParserService.GetParseableFileContent(classFileName);
			
			// build the new interface...
			string newInterfaceCode = language.RefactoringProvider.GenerateInterfaceForClass(extractInterface.NewInterfaceName,
			                                                                                 existingClassCode,
			                                                                                 extractInterface.ChosenMembers,
			                                                                                 c, extractInterface.IncludeComments);
			if (newInterfaceCode == null)
				return;
			
			// ...dump it to a file...
			IViewContent viewContent = FileService.GetOpenFile(newInterfaceFileName);
			IEditable editable = viewContent as IEditable;
			
			if (viewContent != null && editable != null) {
				// simply update it
				editable.Text = newInterfaceCode;
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
					ProjectBrowserPad.Instance.ProjectBrowserControl.RefreshView();
				}
			}
			
			ICompilationUnit newCompilationUnit = ParserService.ParseFile(newInterfaceFileName).MostRecentCompilationUnit;
			IClass newInterfaceDef = newCompilationUnit.Classes[0];
			
			// finally, add the interface to the base types of the class that we're extracting from
			if (extractInterface.AddInterfaceToClass) {
				string modifiedClassCode = language.RefactoringProvider.AddBaseTypeToClass(existingClassCode, c, newInterfaceDef);
				if (modifiedClassCode == null) {
					return;
				}
				
				viewContent = FileService.OpenFile(classFileName);
				editable = viewContent as IEditable;
				if (editable == null) {
					return;
				}
				editable.Text = modifiedClassCode;
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
			int offset = documentInformation.CreateDocument().PositionToOffset(new TextLocation(region.BeginColumn - 1, region.BeginLine - 1));
			string text = documentInformation.TextBuffer.GetText(offset, Math.Min(name.Length + 30, documentInformation.TextBuffer.Length - offset - 1));
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
			ITextBufferStrategy strategy = StringTextBufferStrategy.CreateTextBufferFromFile(fileName);
			return new ProvidedDocumentInformation(strategy, fileName, 0);
		}
		
		public static bool IsReadOnly(IClass c)
		{
			return c.CompilationUnit.FileName == null || c.GetCompoundClass().IsSynthetic;
		}
		
		public static TextEditorControl JumpToDefinition(IMember member)
		{
			IViewContent viewContent = null;
			ICompilationUnit cu = member.DeclaringType.CompilationUnit;
			if (cu != null) {
				string fileName = cu.FileName;
				if (fileName != null) {
					if (!member.Region.IsEmpty) {
						viewContent = FileService.JumpToFilePosition(fileName, member.Region.BeginLine - 1, member.Region.BeginColumn - 1);
					} else {
						FileService.OpenFile(fileName);
					}
				}
			}
			ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor.ITextEditorControlProvider tecp = viewContent as ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor.ITextEditorControlProvider;
			return (tecp == null) ? null : tecp.TextEditorControl;
		}
		
		public static TextEditorControl JumpBehindDefinition(IMember member)
		{
			IViewContent viewContent = null;
			ICompilationUnit cu = member.DeclaringType.CompilationUnit;
			if (cu != null) {
				string fileName = cu.FileName;
				if (fileName != null) {
					if (!member.Region.IsEmpty) {
						viewContent = FileService.JumpToFilePosition(fileName, member.Region.EndLine, 0);
					} else {
						FileService.OpenFile(fileName);
					}
				}
			}
			ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor.ITextEditorControlProvider tecp = viewContent as ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor.ITextEditorControlProvider;
			return (tecp == null) ? null : tecp.TextEditorControl;
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
			public ICSharpCode.TextEditor.Document.IDocument Document;
			public int Offset;
			public int LengthDifference;
			
			public Modification(ICSharpCode.TextEditor.Document.IDocument document, int offset, int lengthDifference)
			{
				this.Document = document;
				this.Offset = offset;
				this.LengthDifference = lengthDifference;
			}
		}
		
		public static void ModifyDocument(List<Modification> modifications, ICSharpCode.TextEditor.Document.IDocument doc, int offset, int length, string newName)
		{
			doc.UndoStack.StartUndoGroup();
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
			doc.UndoStack.EndUndoGroup();
		}
		
		public static void ShowAsSearchResults(string pattern, List<Reference> list)
		{
			if (list == null) return;
			List<SearchResultMatch> results = new List<SearchResultMatch>(list.Count);
			foreach (Reference r in list) {
				SearchResultMatch res = new SearchResultMatch(r.Offset, r.Length);
				res.ProvidedDocumentInformation = GetDocumentInformation(r.FileName);
				results.Add(res);
			}
			SearchResultPanel.Instance.ShowSearchResults(new SearchResult(pattern, results));
		}
		
		sealed class FileView {
			public IViewContent ViewContent { get; set; }
			public OpenedFile OpenedFile { get; set; }
		}
		
		public static void RenameReferences(List<Reference> list, string newName)
		{
			Dictionary<ICSharpCode.TextEditor.Document.IDocument, FileView> modifiedDocuments = new Dictionary<ICSharpCode.TextEditor.Document.IDocument, FileView>();
			List<Modification> modifications = new List<Modification>();
			foreach (Reference r in list) {
				ICSharpCode.TextEditor.Document.IDocument document = null;
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
					document.UndoStack.StartUndoGroup();
				}
				
				ModifyDocument(modifications, document, r.Offset, r.Length, newName);
			}
			foreach (KeyValuePair<ICSharpCode.TextEditor.Document.IDocument, FileView> entry in modifiedDocuments) {
				entry.Key.UndoStack.EndUndoGroup();
				entry.Value.OpenedFile.MakeDirty();
				if (entry.Value.ViewContent is IEditable) {
					ParserService.ParseViewContent(entry.Value.ViewContent);
				} else {
					ParserService.ParseFile(entry.Value.OpenedFile.FileName, entry.Key.TextContent, !entry.Value.OpenedFile.IsUntitled);
				}
			}
		}

		public static void MoveClassToFile(IClass c, string newFileName)
		{
			LanguageProperties language = c.ProjectContent.Language;
			string existingCode = ParserService.GetParseableFileContent(c.CompilationUnit.FileName);
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
				ProjectBrowserPad.Instance.ProjectBrowserControl.RefreshView();
			}
		}
		
		public static string ExtractCode(IClass c, DomRegion codeRegion, int indentationLine, out Action removeExtractedCodeAction)
		{
			ICSharpCode.TextEditor.Document.IDocument doc = GetDocument(c);
			if (indentationLine < 1) indentationLine = 1;
			if (indentationLine >= doc.TotalNumberOfLines) indentationLine = doc.TotalNumberOfLines;
			
			LineSegment segment = doc.GetLineSegment(indentationLine - 1);
			string mainLine = doc.GetText(segment);
			string indentation = mainLine.Substring(0, mainLine.Length - mainLine.TrimStart().Length);
			
			segment = doc.GetLineSegment(codeRegion.BeginLine - 1);
			int startOffset = segment.Offset;
			segment = doc.GetLineSegment(codeRegion.EndLine - 1);
			int endOffset = segment.Offset + segment.Length;
			
			StringReader reader = new StringReader(doc.GetText(startOffset, endOffset - startOffset));
			removeExtractedCodeAction = delegate {
				doc.Remove(startOffset, endOffset - startOffset);
				doc.RequestUpdate(new ICSharpCode.TextEditor.TextAreaUpdate(ICSharpCode.TextEditor.TextAreaUpdateType.WholeTextArea));
				doc.CommitUpdate();
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

		public static ICSharpCode.TextEditor.Document.IDocument GetDocument(IClass c)
		{
			ITextEditorControlProvider tecp = FileService.OpenFile(c.CompilationUnit.FileName) as ITextEditorControlProvider;
			if (tecp == null) return null;
			return tecp.TextEditorControl.Document;
		}
		
		#endregion
	}
}
