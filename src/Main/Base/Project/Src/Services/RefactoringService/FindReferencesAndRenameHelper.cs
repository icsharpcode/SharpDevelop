// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ICSharpCode.Core;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Bookmarks;
using ICSharpCode.SharpDevelop.Gui;
using SearchAndReplace;
using ICSharpCode.SharpDevelop.DefaultEditor.Commands;

namespace ICSharpCode.SharpDevelop.Refactoring
{
	public static class FindReferencesAndRenameHelper
	{
		public static ProvidedDocumentInformation GetDocumentInformation(string fileName)
		{
			foreach (IViewContent content in WorkbenchSingleton.Workbench.ViewContentCollection) {
				if (content is ITextEditorControlProvider &&
				    FileUtility.IsEqualFileName(content.IsUntitled ? content.UntitledName : content.FileName, fileName))
				{
					return new ProvidedDocumentInformation(((ITextEditorControlProvider)content).TextEditorControl.Document, fileName, 0);
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
		
		public static void ShowAsSearchResults(string pattern, List<Reference> list)
		{
			if (list == null) return;
			List<SearchResult> results = new List<SearchResult>(list.Count);
			foreach (Reference r in list) {
				SearchResult res = new SearchResult(r.Offset, r.Length);
				res.ProvidedDocumentInformation = GetDocumentInformation(r.FileName);
				results.Add(res);
			}
			SearchInFilesManager.ShowSearchResults(pattern, results);
		}
		
		public static void RenameReferences(List<Reference> list, string newName)
		{
			List<IViewContent> modifiedContents = new List<IViewContent>();
			List<Modification> modifications = new List<Modification>();
			foreach (Reference r in list) {
				FileService.OpenFile(r.FileName);
				IViewContent viewContent = FileService.GetOpenFile(r.FileName).ViewContent;
				if (!modifiedContents.Contains(viewContent)) {
					modifiedContents.Add(viewContent);
				}
				ITextEditorControlProvider p = viewContent as ITextEditorControlProvider;
				if (p != null) {
					ModifyDocument(modifications, p.TextEditorControl.Document, r.Offset, r.Length, newName);
				}
			}
			foreach (IViewContent viewContent in modifiedContents) {
				ParserService.ParseViewContent(viewContent);
			}
		}
	}
}
