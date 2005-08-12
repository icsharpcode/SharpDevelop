// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
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

namespace ICSharpCode.SharpDevelop.DefaultEditor.Commands
{
	/// <summary>
	/// Build context menu for class members in the text editor.
	/// </summary>
	public class ClassBookmarkMenuBuilder : ParserBookmarkMenuBuilderBase, ISubmenuBuilder
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
			
			if (!IsReadOnly(c)) {
				cmd = new MenuCommand("${res:SharpDevelop.Refactoring.RenameCommand}", Rename);
				cmd.Tag = c;
				list.Add(cmd);
			}
			
			if (c.BaseTypes.Count > 0) {
				cmd = new MenuCommand("${res:SharpDevelop.Refactoring.GoToBaseCommand}", GoToBase);
				cmd.Tag = c;
				list.Add(cmd);
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
			IClass c = (IClass)item.Tag;
			c = c.DefaultReturnType.GetUnderlyingClass(); // get compound class if class is partial
			string newName = MessageService.ShowInputBox("${res:SharpDevelop.Refactoring.Rename}", "${res:SharpDevelop.Refactoring.RenameClassText}", c.Name);
			if (!CheckName(newName)) return;
			
			List<Reference> list = RefactoringService.FindReferences(c, null);
			if (list == null) return;
			
			// Add the class declaration(s)
			foreach (IClass part in GetClassParts(c)) {
				AddDeclarationAsReference(list, part.CompilationUnit.FileName, part.Region, part.Name);
			}
			
			// Add the constructors
			foreach (IMethod m in c.Methods) {
				if (m.IsConstructor) {
					AddDeclarationAsReference(list, m.DeclaringType.CompilationUnit.FileName, m.Region, c.Name);
				}
			}
			
			RenameReferences(list, newName);
		}
		
		void AddDeclarationAsReference(List<Reference> list, string fileName, IRegion region, string name)
		{
			if (fileName == null)
				return;
			ProvidedDocumentInformation documentInformation = GetDocumentInformation(fileName);
			int offset = documentInformation.Document.PositionToOffset(new Point(region.BeginColumn - 1, region.BeginLine - 1));
			string text = documentInformation.TextBuffer.GetText(offset, Math.Min(name.Length + 30, documentInformation.TextBuffer.Length - offset - 1));
			int offsetChange = text.IndexOf(name);
			if (offsetChange < 0)
				return;
			offset += offsetChange;
			foreach (Reference r in list) {
				if (r.Offset == offset)
					return;
			}
			list.Add(new Reference(fileName, offset, name.Length, name, null));
		}
		
		List<IClass> GetClassParts(IClass c)
		{
			List<IClass> list;
			CompoundClass cc = c as CompoundClass;
			if (cc != null) {
				list = cc.Parts;
			} else {
				list = new List<IClass>(1);
				list.Add(c);
			}
			return list;
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
				res.ProvidedDocumentInformation = GetDocumentInformation(derivedClass.CompilationUnit.FileName);
				results.Add(res);
			}
			SearchReplaceInFilesManager.ShowSearchResults("Classes deriving from " + c.Name, results);
		}
		
		void FindReferences(object sender, EventArgs e)
		{
			MenuCommand item = (MenuCommand)sender;
			IClass c = (IClass)item.Tag;
			ShowAsSearchResults("References to " + c.Name, RefactoringService.FindReferences(c, null));
		}
	}
}
