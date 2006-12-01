// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Bookmarks;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.Refactoring;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Gui.ClassBrowser;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Refactoring;
using ICSharpCode.TextEditor.Document;
using SearchAndReplace;

namespace ICSharpCode.SharpDevelop.DefaultEditor.Commands
{
	/// <summary>
	/// Build context menu for class members in the text editor.
	/// </summary>
	public class ClassBookmarkMenuBuilder : ISubmenuBuilder
	{
		public ToolStripItem[] BuildSubmenu(Codon codon, object owner)
		{
			MenuCommand cmd;
			IClass c;
			ClassNode classNode = owner as ClassNode;
			if (classNode != null) {
				c = classNode.Class;
			} else {
				ClassBookmark bookmark = (ClassBookmark)owner;
				c = bookmark.Class;
			}
			
			LanguageProperties language = c.ProjectContent.Language;
			
			List<ToolStripItem> list = new List<ToolStripItem>();
			
			if (!FindReferencesAndRenameHelper.IsReadOnly(c)) {
				if (c.DeclaringType == null &&
				    !c.BodyRegion.IsEmpty &&
				    !c.Name.Equals(Path.GetFileNameWithoutExtension(c.CompilationUnit.FileName),
				                   StringComparison.InvariantCultureIgnoreCase))
				{
					// File name does not match class name
					string correctFileName = Path.Combine(Path.GetDirectoryName(c.CompilationUnit.FileName),
					                                      c.Name + Path.GetExtension(c.CompilationUnit.FileName));
					if (FileUtility.IsValidFileName(correctFileName)
					    && Path.IsPathRooted(correctFileName)
					    && !File.Exists(correctFileName))
					{
						if (c.CompilationUnit.Classes.Count == 1) {
							// Rename file to ##
							cmd = new MenuCommand(StringParser.Parse("${res:SharpDevelop.Refactoring.RenameFileTo}", new string[,] {{ "FileName", Path.GetFileName(correctFileName) }}),
							                      delegate {
							                      	FileService.RenameFile(c.CompilationUnit.FileName, correctFileName, false);
							                      	if (c.ProjectContent.Project != null) {
							                      		((IProject)c.ProjectContent.Project).Save();
							                      	}
							                      });
							list.Add(cmd);
						} else if (language.RefactoringProvider.SupportsCreateNewFileLikeExisting && language.RefactoringProvider.SupportsGetFullCodeRangeForType) {
							// Move class to file ##
							cmd = new MenuCommand(StringParser.Parse("${res:SharpDevelop.Refactoring.MoveClassToFile}", new string[,] {{ "FileName", Path.GetFileName(correctFileName) }}),
							                      delegate {
							                      	MoveClassToFile(c, correctFileName);
							                      });
							list.Add(cmd);
						}
					}
				}
				
				cmd = new MenuCommand("${res:SharpDevelop.Refactoring.RenameCommand}", Rename);
				cmd.Tag = c;
				list.Add(cmd);
			}
			
			if (c.BaseTypes.Count > 0) {
				cmd = new MenuCommand("${res:SharpDevelop.Refactoring.GoToBaseCommand}", GoToBase);
				cmd.Tag = c;
				list.Add(cmd);
				if (c.ClassType != ClassType.Interface && !FindReferencesAndRenameHelper.IsReadOnly(c)) {
					AddImplementInterfaceCommands(c, list);
				}
			}
			if (!c.IsSealed && !c.IsStatic) {
				cmd = new MenuCommand("${res:SharpDevelop.Refactoring.FindDerivedClassesCommand}", FindDerivedClasses);
				cmd.Tag = c;
				list.Add(cmd);
			}
			
			cmd = new MenuCommand("${res:SharpDevelop.Refactoring.FindReferencesCommand}", FindReferences);
			cmd.Tag = c;
			list.Add(cmd);
			
			return list.ToArray();
		}
		
		static void MoveClassToFile(IClass c, string newFileName)
		{
			LanguageProperties language = c.ProjectContent.Language;
			string existingCode = ParserService.GetParseableFileContent(c.CompilationUnit.FileName);
			DomRegion fullRegion = language.RefactoringProvider.GetFullCodeRangeForType(existingCode, c);
			if (fullRegion.IsEmpty) return;
			
			string newCode = ExtractCode(c, fullRegion, c.BodyRegion.BeginLine);
			
			newCode = language.RefactoringProvider.CreateNewFileLikeExisting(existingCode, newCode);
			IWorkbenchWindow window = FileService.NewFile(newFileName, "Text", newCode);
			window.ViewContent.Save(newFileName);
			
			IProject project = (IProject)c.ProjectContent.Project;
			if (project != null) {
				FileProjectItem projectItem = new FileProjectItem(project, ItemType.Compile);
				projectItem.FileName = newFileName;
				ProjectService.AddProjectItem(project, projectItem);
				project.Save();
				ProjectBrowserPad.Instance.ProjectBrowserControl.RefreshView();
			}
		}
		
		static string ExtractCode(IClass c, DomRegion codeRegion, int indentationLine)
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
			doc.Remove(startOffset, endOffset - startOffset);
			doc.RequestUpdate(new ICSharpCode.TextEditor.TextAreaUpdate(ICSharpCode.TextEditor.TextAreaUpdateType.WholeTextArea));
			doc.CommitUpdate();
			
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
		
		void AddImplementInterfaceCommandItems(List<ToolStripItem> subItems, IClass c, bool explicitImpl)
		{
			CodeGenerator codeGen = c.ProjectContent.Language.CodeGenerator;
			IAmbience ambience = AmbienceService.CurrentAmbience;
			ambience.ConversionFlags = ConversionFlags.None;
			foreach (IReturnType rt in c.BaseTypes) {
				IClass interf = rt.GetUnderlyingClass();
				if (interf != null && interf.ClassType == ClassType.Interface) {
					IReturnType rtCopy = rt; // copy for access by anonymous method
					EventHandler eh = delegate {
						TextEditorDocument d = new TextEditorDocument(GetDocument(c));
						if (d != null)
							codeGen.ImplementInterface(rtCopy, d, explicitImpl, c);
						ParserService.ParseCurrentViewContent();
					};
					subItems.Add(new MenuCommand(ambience.Convert(interf), eh));
				}
			}
		}
		
		void AddImplementInterfaceCommands(IClass c, List<ToolStripItem> list)
		{
			CodeGenerator codeGen = c.ProjectContent.Language.CodeGenerator;
			if (codeGen == null) return;
			List<ToolStripItem> subItems = new List<ToolStripItem>();
			if (c.ProjectContent.Language.SupportsImplicitInterfaceImplementation) {
				AddImplementInterfaceCommandItems(subItems, c, false);
				if (subItems.Count > 0) {
					list.Add(new ICSharpCode.Core.Menu("${res:SharpDevelop.Refactoring.ImplementInterfaceImplicit}", subItems.ToArray()));
					subItems = new List<ToolStripItem>();
				}
			}
			AddImplementInterfaceCommandItems(subItems, c, true);
			
			if (subItems.Count > 0) {
				if (c.ProjectContent.Language.SupportsImplicitInterfaceImplementation) {
					list.Add(new ICSharpCode.Core.Menu("${res:SharpDevelop.Refactoring.ImplementInterfaceExplicit}", subItems.ToArray()));
				} else {
					list.Add(new ICSharpCode.Core.Menu("${res:SharpDevelop.Refactoring.ImplementInterface}", subItems.ToArray()));
				}
			}
		}
		
		static ICSharpCode.TextEditor.Document.IDocument GetDocument(IClass c)
		{
			IWorkbenchWindow win = FileService.OpenFile(c.CompilationUnit.FileName);
			if (win == null) return null;
			ITextEditorControlProvider tecp = win.ViewContent as ITextEditorControlProvider;
			if (tecp == null) return null;
			return tecp.TextEditorControl.Document;
		}
		
		void GoToBase(object sender, EventArgs e)
		{
			MenuCommand item = (MenuCommand)sender;
			IClass c = (IClass)item.Tag;
			IClass baseClass = c.BaseClass;
			if (baseClass != null) {
				string fileName = baseClass.CompilationUnit.FileName;
				if (fileName != null) {
					FileService.JumpToFilePosition(fileName, baseClass.Region.BeginLine - 1, baseClass.Region.BeginColumn - 1);
				}
			}
		}
		
		void Rename(object sender, EventArgs e)
		{
			MenuCommand item = (MenuCommand)sender;
			FindReferencesAndRenameHelper.RenameClass((IClass)item.Tag);
		}
		
		void FindDerivedClasses(object sender, EventArgs e)
		{
			MenuCommand item = (MenuCommand)sender;
			IClass c = (IClass)item.Tag;
			List<IClass> derivedClasses = RefactoringService.FindDerivedClasses(c, ParserService.AllProjectContents, false);
			
			List<SearchResult> results = new List<SearchResult>();
			foreach (IClass derivedClass in derivedClasses) {
				if (derivedClass.CompilationUnit == null) continue;
				if (derivedClass.CompilationUnit.FileName == null) continue;
				
				SearchResult res = new SimpleSearchResult(derivedClass.FullyQualifiedName, new Point(derivedClass.Region.BeginColumn - 1, derivedClass.Region.BeginLine - 1));
				res.ProvidedDocumentInformation = FindReferencesAndRenameHelper.GetDocumentInformation(derivedClass.CompilationUnit.FileName);
				results.Add(res);
			}
			SearchInFilesManager.ShowSearchResults(StringParser.Parse("${res:SharpDevelop.Refactoring.ClassesDerivingFrom}", new string[,] {{ "Name", c.Name }}),
			                                       results);
		}
		
		void FindReferences(object sender, EventArgs e)
		{
			MenuCommand item = (MenuCommand)sender;
			IClass c = (IClass)item.Tag;
			using (AsynchronousWaitDialog monitor = AsynchronousWaitDialog.ShowWaitDialog("${res:SharpDevelop.Refactoring.FindReferencesCommand}"))
			{
				FindReferencesAndRenameHelper.ShowAsSearchResults(
					StringParser.Parse("${res:SharpDevelop.Refactoring.ReferencesTo}",
					                   new string[,] {{ "Name", c.Name }}),
					RefactoringService.FindReferences(c, monitor)
				);
			}
		}
	}
}
