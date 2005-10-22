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
using ICSharpCode.SharpDevelop.Refactoring;

namespace ICSharpCode.SharpDevelop.DefaultEditor.Commands
{
	/// <summary>
	/// Build context menu for class members in the text editor.
	/// </summary>
	public class ClassMemberMenuBuilder : ISubmenuBuilder
	{
		public ToolStripItem[] BuildSubmenu(Codon codon, object owner)
		{
			MenuCommand cmd;
			IMember member;
			MemberNode memberNode = owner as MemberNode;
			if (memberNode != null) {
				member = memberNode.Member;
			} else {
				ClassMemberBookmark bookmark = (ClassMemberBookmark)owner;
				member = bookmark.Member;
			}
			IMethod method = member as IMethod;
			List<ToolStripItem> list = new List<ToolStripItem>();
			
			if (method == null || !method.IsConstructor) {
				if (!FindReferencesAndRenameHelper.IsReadOnly(member.DeclaringType)) {
					cmd = new MenuCommand("${res:SharpDevelop.Refactoring.RenameCommand}", Rename);
					cmd.Tag = member;
					list.Add(cmd);
				}
			}
			if (member.IsOverride) {
				cmd = new MenuCommand("${res:SharpDevelop.Refactoring.GoToBaseClassCommand}", GoToBase);
				cmd.Tag = member;
				list.Add(cmd);
			}
			if (member.IsVirtual || member.IsAbstract || (member.IsOverride && !member.DeclaringType.IsSealed)) {
				cmd = new MenuCommand("${res:SharpDevelop.Refactoring.FindOverridesCommand}", FindOverrides);
				cmd.Tag = member;
				list.Add(cmd);
			}
			
			cmd = new MenuCommand("${res:SharpDevelop.Refactoring.FindReferencesCommand}", FindReferences);
			cmd.Tag = member;
			list.Add(cmd);
			
			if (member is IField) {
				IProperty foundProperty = FindProperty(member as IField);
				if (foundProperty != null) {
					cmd = new MenuCommand("${res:SharpDevelop.Refactoring.GoToProperty}", GotoTagMember);
					cmd.Tag = foundProperty;
					list.Add(cmd);
				} else {
					if (!FindReferencesAndRenameHelper.IsReadOnly(member.DeclaringType)) {
						cmd = new MenuCommand("${res:SharpDevelop.Refactoring.CreateGetter}", CreateGetter);
						cmd.Tag = member;
						list.Add(cmd);
						cmd = new MenuCommand("${res:SharpDevelop.Refactoring.CreateProperty}", CreateProperty);
						cmd.Tag = member;
						list.Add(cmd);
					}
				}
			}
			
			return list.ToArray();
		}
		
		IProperty FindProperty(IField field)
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
		
		void CreateProperty(object sender, EventArgs e)
		{
			CreateProperty(sender, e, true);
		}
		
		void CreateGetter(object sender, EventArgs e)
		{
			CreateProperty(sender, e, false);
		}
		
		void CreateProperty(object sender, EventArgs e, bool includeSetter)
		{
			MenuCommand item = (MenuCommand)sender;
			IField member = (IField)item.Tag;
			TextEditorControl textEditor = FindReferencesAndRenameHelper.JumpBehindDefinition(member);
			
			member.DeclaringType.ProjectContent.Language.CodeGenerator.CreateProperty(member, textEditor.Document, true, includeSetter);
			textEditor.Refresh();
		}
		
		void GotoTagMember(object sender, EventArgs e)
		{
			FindReferencesAndRenameHelper.JumpToDefinition((IMember)(sender as MenuCommand).Tag);
		}
		
		void GoToBase(object sender, EventArgs e)
		{
			MenuCommand item = (MenuCommand)sender;
			IMember member = (IMember)item.Tag;
			IMember baseMember = RefactoringService.FindBaseMember(member);
			if (baseMember != null) {
				FindReferencesAndRenameHelper.JumpToDefinition(baseMember);
			}
		}
		
		void Rename(object sender, EventArgs e)
		{
			MenuCommand item = (MenuCommand)sender;
			IMember member = (IMember)item.Tag;
			string newName = MessageService.ShowInputBox("${res:SharpDevelop.Refactoring.Rename}", "${res:SharpDevelop.Refactoring.RenameMemberText}", member.Name);
			if (!FindReferencesAndRenameHelper.CheckName(newName)) return;
			
			List<Reference> list = RefactoringService.FindReferences(member, null);
			if (list == null) return;
			FindReferencesAndRenameHelper.RenameReferences(list, newName);
			
			if (member is IField) {
				IProperty property = FindProperty((IField)member);
				if (property != null) {
					string newPropertyName = AbstractPropertyCodeGenerator.GetPropertyName(newName);
					if (newPropertyName != newName && newPropertyName != property.Name) {
						if (MessageService.AskQuestionFormatted("${res:SharpDevelop.Refactoring.Rename}", "${res:SharpDevelop.Refactoring.RenameFieldAndProperty}", property.FullyQualifiedName, newPropertyName)) {
							list = RefactoringService.FindReferences(property, null);
							if (list != null) {
								FindReferencesAndRenameHelper.RenameReferences(list, newPropertyName);
							}
						}
					}
				}
			}
		}
		
		void FindOverrides(object sender, EventArgs e)
		{
			MenuCommand item = (MenuCommand)sender;
			IMember member = (IMember)item.Tag;
			List<IClass> derivedClasses = RefactoringService.FindDerivedClasses(member.DeclaringType, ParserService.AllProjectContents, false);
			List<SearchResult> results = new List<SearchResult>();
			foreach (IClass derivedClass in derivedClasses) {
				if (derivedClass.CompilationUnit == null) continue;
				if (derivedClass.CompilationUnit.FileName == null) continue;
				IMember m = RefactoringService.FindSimilarMember(derivedClass, member);
				if (m != null && !m.Region.IsEmpty) {
					SearchResult res = new SimpleSearchResult(m.FullyQualifiedName, new Point(m.Region.BeginColumn - 1, m.Region.BeginLine - 1));
					res.ProvidedDocumentInformation = FindReferencesAndRenameHelper.GetDocumentInformation(derivedClass.CompilationUnit.FileName);
					results.Add(res);
				}
			}
			SearchReplaceInFilesManager.ShowSearchResults("Overrides of " + member.Name, results);
		}
		
		void FindReferences(object sender, EventArgs e)
		{
			MenuCommand item = (MenuCommand)sender;
			IMember member = (IMember)item.Tag;
			FindReferencesAndRenameHelper.ShowAsSearchResults("References to " + member.Name, RefactoringService.FindReferences(member, null));
		}
	}
}
