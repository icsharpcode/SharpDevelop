// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version value="$version"/>
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

namespace ICSharpCode.SharpDevelop.Commands
{
	/// <summary>
	/// Build context menu for class members in the text editor.
	/// </summary>
	public class ClassMemberMenuBuilder : ISubmenuBuilder
	{
		public ToolStripItem[] BuildSubmenu(Codon codon, object owner)
		{
			MenuCommand cmd;
			ClassMemberBookmark bookmark = (ClassMemberBookmark)owner;
			IMember member = bookmark.Member;
			List<ToolStripItem> list = new List<ToolStripItem>();
			cmd = new MenuCommand("&Rename", Rename);
			cmd.Tag = member;
			list.Add(cmd);
			
			if (member.IsOverride) {
				cmd = new MenuCommand("Go to &base class", GoToBase);
				cmd.Tag = member;
				list.Add(cmd);
			}
			if (member.IsVirtual || member.IsAbstract || (member.IsOverride && !member.DeclaringType.IsSealed)) {
				cmd = new MenuCommand("Find &overrides", FindOverrides);
				cmd.Tag = member;
				list.Add(cmd);
			}
			
			cmd = new MenuCommand("&Find references", FindReferences);
			cmd.Tag = member;
			list.Add(cmd);
			
			return list.ToArray();
		}
		
		void GoToBase(object sender, EventArgs e)
		{
			MenuCommand item = (MenuCommand)sender;
			IMember member = (IMember)item.Tag;
			IMember baseMember = RefactoringService.FindBaseMember(member);
			if (baseMember != null) {
				ICompilationUnit cu = baseMember.DeclaringType.CompilationUnit;
				if (cu != null) {
					string fileName = cu.FileName;
					if (fileName != null) {
						if (baseMember.Region != null && baseMember.Region.BeginLine > 0) {
							FileService.JumpToFilePosition(fileName, baseMember.Region.BeginLine - 1, 0);
						} else {
							FileService.JumpToFilePosition(fileName, 0, 0);
						}
						return;
					}
				}
			}
		}
		
		void Rename(object sender, EventArgs e)
		{
			MenuCommand item = (MenuCommand)sender;
			IMember member = (IMember)item.Tag;
			MessageService.ShowMessage("Not implemented.");
		}
		
		void FindOverrides(object sender, EventArgs e)
		{
			MenuCommand item = (MenuCommand)sender;
			IMember member = (IMember)item.Tag;
			List<IClass> derivedClasses = RefactoringService.FindDerivedClasses(member.DeclaringType, ParserService.AllProjectContents);
			List<SearchResult> results = new List<SearchResult>();
			foreach (IClass derivedClass in derivedClasses) {
				if (derivedClass.CompilationUnit == null) continue;
				if (derivedClass.CompilationUnit.FileName == null) continue;
				IMember m = RefactoringService.FindSimilarMember(derivedClass, member);
				if (m != null && m.Region != null) {
					SearchResult res = new SimpleSearchResult(m.FullyQualifiedName, new Point(m.Region.BeginColumn - 1, m.Region.BeginLine - 1));
					res.ProvidedDocumentInformation = GetDocumentInformation(derivedClass.CompilationUnit.FileName);
					results.Add(res);
				}
			}
			SearchReplaceInFilesManager.ShowSearchResults(results);
		}
		
		void FindReferences(object sender, EventArgs e)
		{
			MenuCommand item = (MenuCommand)sender;
			IMember member = (IMember)item.Tag;
			List<Reference> list = RefactoringService.FindReferences(member, null);
			if (list == null) return;
			List<SearchResult> results = new List<SearchResult>();
			foreach (Reference r in list) {
				SearchResult res = new SearchResult(r.Offset, r.Length);
				res.ProvidedDocumentInformation = GetDocumentInformation(r.FileName);
				results.Add(res);
			}
			SearchReplaceInFilesManager.ShowSearchResults(results);
		}
		
		ProvidedDocumentInformation GetDocumentInformation(string fileName)
		{
			foreach (IViewContent content in WorkbenchSingleton.Workbench.ViewContentCollection) {
				if (content is ITextEditorControlProvider &&
				    content.FileName != null &&
				    FileUtility.IsEqualFile(content.FileName, fileName))
				{
					return new ProvidedDocumentInformation(((ITextEditorControlProvider)content).TextEditorControl.Document, fileName, 0);
				}
			}
			ITextBufferStrategy strategy = StringTextBufferStrategy.CreateTextBufferFromFile(fileName);
			return new ProvidedDocumentInformation(strategy, fileName, 0);
		}
	}
}
