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
using ICSharpCode.SharpDevelop.Gui.ClassBrowser;
using SearchAndReplace;
using ICSharpCode.SharpDevelop.DefaultEditor.Commands;
using ICSharpCode.SharpDevelop.Refactoring;

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
			
			List<ToolStripItem> list = new List<ToolStripItem>();
			
			if (!FindReferencesAndRenameHelper.IsReadOnly(c)) {
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
		
		void AddImplementInterfaceCommandItems(List<ToolStripItem> subItems, IClass c, bool explicitImpl, ModifierEnum modifier)
		{
			CodeGenerator codeGen = c.ProjectContent.Language.CodeGenerator;
			foreach (IReturnType rt in c.BaseTypes) {
				IClass interf = rt.GetUnderlyingClass();
				if (interf != null && interf.ClassType == ClassType.Interface) {
					EventHandler eh = delegate {
						IDocument d = GetDocument(c);
						if (d != null)
							codeGen.ImplementInterface(rt, d, explicitImpl, modifier, c);
						ParserService.ParseCurrentViewContent();
					};
					subItems.Add(new MenuCommand(interf.Name, eh));
				}
			}
		}
		
		void AddImplementInterfaceCommands(IClass c, List<ToolStripItem> list)
		{
			CodeGenerator codeGen = c.ProjectContent.Language.CodeGenerator;
			if (codeGen == null) return;
			List<ToolStripItem> subItems = new List<ToolStripItem>();
			if (c.ProjectContent.Language.SupportsImplicitInterfaceImplementation) {
				AddImplementInterfaceCommandItems(subItems, c, false, ModifierEnum.Public);
				if (subItems.Count > 0) {
					list.Add(new ICSharpCode.Core.Menu("${res:SharpDevelop.Refactoring.ImplementInterfaceImplicit}", subItems.ToArray()));
					subItems = new List<ToolStripItem>();
				}
				
				AddImplementInterfaceCommandItems(subItems, c, true, ModifierEnum.None);
			} else {
				AddImplementInterfaceCommandItems(subItems, c, true, ModifierEnum.Public);
			}
			if (subItems.Count > 0) {
				if (c.ProjectContent.Language.SupportsImplicitInterfaceImplementation) {
					list.Add(new ICSharpCode.Core.Menu("${res:SharpDevelop.Refactoring.ImplementInterfaceExplicit}", subItems.ToArray()));
				} else {
					list.Add(new ICSharpCode.Core.Menu("${res:SharpDevelop.Refactoring.ImplementInterface}", subItems.ToArray()));
				}
			}
		}
		
		static IDocument GetDocument(IClass c)
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
			SearchInFilesManager.ShowSearchResults("Classes deriving from " + c.Name, results);
		}
		
		void FindReferences(object sender, EventArgs e)
		{
			MenuCommand item = (MenuCommand)sender;
			IClass c = (IClass)item.Tag;
			FindReferencesAndRenameHelper.ShowAsSearchResults("References to " + c.Name, RefactoringService.FindReferences(c, null));
		}
	}
}
