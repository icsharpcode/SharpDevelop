// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using ICSharpCode.Core;
using ICSharpCode.Core.Presentation;
using ICSharpCode.Core.WinForms;
using ICSharpCode.NRefactory;
using ICSharpCode.SharpDevelop.Bookmarks;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.Refactoring;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.Search;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Gui.ClassBrowser;
using ICSharpCode.SharpDevelop.Refactoring;

namespace ICSharpCode.SharpDevelop.Editor.Commands
{
	/// <summary>
	/// Build context menu for class members in the text editor.
	/// </summary>
	public class ClassMemberMenuBuilder : ISubmenuBuilder, IMenuItemBuilder
	{
		public System.Collections.ICollection BuildItems(Codon codon, object owner)
		{
			return BuildSubmenu(codon, owner).TranslateToWpf();
		}
		
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
			
			bool canGenerateCode =
				member.DeclaringType.ProjectContent.Language.CodeGenerator != null
				&& !FindReferencesAndRenameHelper.IsReadOnly(member.DeclaringType);
			
			if (method == null || !method.IsConstructor && !method.IsOperator) {
				if (!FindReferencesAndRenameHelper.IsReadOnly(member.DeclaringType) &&
				    !(member is IProperty && ((IProperty)member).IsIndexer)) {
					cmd = new MenuCommand("${res:SharpDevelop.Refactoring.RenameCommand}", Rename);
					cmd.ShortcutKeys = Keys.Control | Keys.R;
					cmd.Tag = member;
					list.Add(cmd);
				}
			}
			if (member != null && member.IsOverride) {
				cmd = new MenuCommand("${res:SharpDevelop.Refactoring.GoToBaseClassCommand}", GoToBase);
				cmd.Tag = member;
				list.Add(cmd);
			}
			
			cmd = new MenuCommand("${res:SharpDevelop.Refactoring.FindReferencesCommand}", FindReferences);
			cmd.ShortcutKeys = Keys.F12;
			cmd.Tag = member;
			list.Add(cmd);
			
			list.AddIfNotNull(MakeFindOverridesItem(member));
			
			if (member is IField && member.DeclaringType.ClassType != ClassType.Enum) {
				IProperty foundProperty = FindReferencesAndRenameHelper.FindProperty(member as IField);
				if (foundProperty != null) {
					cmd = new MenuCommand("${res:SharpDevelop.Refactoring.GoToProperty}", GotoTagMember);
					cmd.Tag = foundProperty;
					list.Add(cmd);
				} else {
					if (canGenerateCode) {
						if (member.IsReadonly) {
							cmd = new MenuCommand("${res:SharpDevelop.Refactoring.CreateProperty}", CreateGetter);
							cmd.Tag = member;
							list.Add(cmd);
						} else {
							cmd = new MenuCommand("${res:SharpDevelop.Refactoring.CreateGetter}", CreateGetter);
							cmd.Tag = member;
							list.Add(cmd);
							cmd = new MenuCommand("${res:SharpDevelop.Refactoring.CreateProperty}", CreateProperty);
							cmd.Tag = member;
							list.Add(cmd);
						}
					}
				}
			}
			if (member is IProperty) {
				IProperty property = member as IProperty;
				if (property.CanSet && canGenerateCode && !property.IsAbstract && property.DeclaringType.ClassType != ClassType.Interface) {
					cmd = new MenuCommand("${res:SharpDevelop.Refactoring.CreateChangedEvent}", CreateChangedEvent);
					cmd.Tag = member;
					list.Add(cmd);
				}
			}
			if (member is IEvent) {
				if (canGenerateCode && !member.IsAbstract && member.DeclaringType.ClassType != ClassType.Interface) {
					cmd = new MenuCommand("${res:SharpDevelop.Refactoring.CreateOnEventMethod}", CreateOnEventMethod);
					cmd.Tag = member;
					list.Add(cmd);
				}
			}
			
			return list.ToArray();
		}
		
		MenuCommand MakeFindOverridesItem(IMember member)
		{
			if (member == null || !member.IsOverridable)
				return null;
			var item = new MenuCommand(StringParser.Parse("${res:SharpDevelop.Refactoring.FindOverridesCommand}"));
			//item.Image = ClassBrowserIconService.Method.Bitmap;
			item.ShortcutKeys = Keys.F6;
			item.Click += delegate {
				ContextActionsHelper.MakePopupWithOverrides(member).OpenAtCaretAndFocus();
			};
			return item;
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
			ITextEditor textEditor = FindReferencesAndRenameHelper.JumpBehindDefinition(member);
			
			if (textEditor != null) {
				CodeGenerator codeGen = member.DeclaringType.ProjectContent.Language.CodeGenerator;
				codeGen.InsertCodeAfter(member, new RefactoringDocumentAdapter(textEditor.Document),
				                        codeGen.CreateProperty(member, true, includeSetter));
				ParserService.ParseCurrentViewContent();
			}
		}
		
		void CreateChangedEvent(object sender, EventArgs e)
		{
			MenuCommand item = (MenuCommand)sender;
			IProperty member = (IProperty)item.Tag;
			ITextEditor textEditor = FindReferencesAndRenameHelper.JumpBehindDefinition(member);
			if (textEditor != null) {
				member.DeclaringType.ProjectContent.Language.CodeGenerator.CreateChangedEvent(member, new RefactoringDocumentAdapter(textEditor.Document));
				ParserService.ParseCurrentViewContent();
			}
		}
		
		void CreateOnEventMethod(object sender, EventArgs e)
		{
			MenuCommand item = (MenuCommand)sender;
			IEvent member = (IEvent)item.Tag;
			ITextEditor textEditor = FindReferencesAndRenameHelper.JumpBehindDefinition(member);
			if (textEditor != null) {
				CodeGenerator codeGen = member.DeclaringType.ProjectContent.Language.CodeGenerator;
				codeGen.InsertCodeAfter(member, new RefactoringDocumentAdapter(textEditor.Document),
				                        codeGen.CreateOnEventMethod(member));
				ParserService.ParseCurrentViewContent();
			}
		}
		
		void GotoTagMember(object sender, EventArgs e)
		{
			FindReferencesAndRenameHelper.JumpToDefinition((IMember)(sender as MenuCommand).Tag);
		}
		
		void GoToBase(object sender, EventArgs e)
		{
			MenuCommand item = (MenuCommand)sender;
			IMember member = (IMember)item.Tag;
			IMember baseMember = MemberLookupHelper.FindBaseMember(member);
			if (baseMember != null) {
				FindReferencesAndRenameHelper.JumpToDefinition(baseMember);
			}
		}
		
		void Rename(object sender, EventArgs e)
		{
			MenuCommand item = (MenuCommand)sender;
			FindReferencesAndRenameHelper.RenameMember((IMember)item.Tag);
		}
		
		void FindReferences(object sender, EventArgs e)
		{
			MenuCommand item = (MenuCommand)sender;
			IMember member = (IMember)item.Tag;
			FindReferencesAndRenameHelper.RunFindReferences(member);
		}
	}
}
