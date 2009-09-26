// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;
using ICSharpCode.SharpDevelop.Bookmarks;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.Refactoring;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Gui.ClassBrowser;
using ICSharpCode.SharpDevelop.Refactoring;
using ICSharpCode.TextEditor;
using SearchAndReplace;

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
			
			bool canGenerateCode =
				member.DeclaringType.ProjectContent.Language.CodeGenerator != null
				&& !FindReferencesAndRenameHelper.IsReadOnly(member.DeclaringType);
			
			if (method == null || !method.IsConstructor && !method.GetIsOperator()) {
				if (!FindReferencesAndRenameHelper.IsReadOnly(member.DeclaringType) &&
				    !(member is IProperty && ((IProperty)member).IsIndexer)) {
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
				if (((IProperty)member).CanSet && canGenerateCode) {
					cmd = new MenuCommand("${res:SharpDevelop.Refactoring.CreateChangedEvent}", CreateChangedEvent);
					cmd.Tag = member;
					list.Add(cmd);
				}
			}
			if (member is IEvent) {
				if (canGenerateCode) {
					cmd = new MenuCommand("${res:SharpDevelop.Refactoring.CreateOnEventMethod}", CreateOnEventMethod);
					cmd.Tag = member;
					list.Add(cmd);
				}
			}
			
			return list.ToArray();
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
			
			CodeGenerator codeGen = member.DeclaringType.ProjectContent.Language.CodeGenerator;
			codeGen.InsertCodeAfter(member, new TextEditorDocument(textEditor.Document),
			                        codeGen.CreateProperty(member, true, includeSetter));
			ParserService.ParseCurrentViewContent();
		}
		
		void CreateChangedEvent(object sender, EventArgs e)
		{
			MenuCommand item = (MenuCommand)sender;
			IProperty member = (IProperty)item.Tag;
			TextEditorControl textEditor = FindReferencesAndRenameHelper.JumpBehindDefinition(member);
			member.DeclaringType.ProjectContent.Language.CodeGenerator.CreateChangedEvent(member, new TextEditorDocument(textEditor.Document));
			ParserService.ParseCurrentViewContent();
		}
		
		void CreateOnEventMethod(object sender, EventArgs e)
		{
			MenuCommand item = (MenuCommand)sender;
			IEvent member = (IEvent)item.Tag;
			TextEditorControl textEditor = FindReferencesAndRenameHelper.JumpBehindDefinition(member);
			CodeGenerator codeGen = member.DeclaringType.ProjectContent.Language.CodeGenerator;
			codeGen.InsertCodeAfter(member, new TextEditorDocument(textEditor.Document),
			                        codeGen.CreateOnEventMethod(member));
			ParserService.ParseCurrentViewContent();
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
		
		void FindOverrides(object sender, EventArgs e)
		{
			MenuCommand item = (MenuCommand)sender;
			IMember member = (IMember)item.Tag;
			IEnumerable<IClass> derivedClasses = RefactoringService.FindDerivedClasses(member.DeclaringType, ParserService.AllProjectContents, false);
			List<SearchResultMatch> results = new List<SearchResultMatch>();
			IAmbience ambience = AmbienceService.GetCurrentAmbience();
			ambience.ConversionFlags = ConversionFlags.UseFullyQualifiedMemberNames | ConversionFlags.ShowTypeParameterList;
			foreach (IClass derivedClass in derivedClasses) {
				if (derivedClass.CompilationUnit == null) continue;
				if (derivedClass.CompilationUnit.FileName == null) continue;
				IMember m = MemberLookupHelper.FindSimilarMember(derivedClass, member);
				if (m != null && !m.Region.IsEmpty) {
					string matchText = ambience.Convert(m);
					SearchResultMatch res = new SimpleSearchResultMatch(matchText, new TextLocation(m.Region.BeginColumn - 1, m.Region.BeginLine - 1));
					res.ProvidedDocumentInformation = FindReferencesAndRenameHelper.GetDocumentInformation(m.DeclaringType.CompilationUnit.FileName);
					results.Add(res);
				}
			}
			SearchResultPanel.Instance.ShowSearchResults(new SearchResult(
				StringParser.Parse("${res:SharpDevelop.Refactoring.OverridesOf}", new string[,] {{ "Name", member.Name }}),
				results
			));
		}
		
		void FindReferences(object sender, EventArgs e)
		{
			MenuCommand item = (MenuCommand)sender;
			IMember member = (IMember)item.Tag;
			string memberName = member.DeclaringType.Name + "." + member.Name;
			using (AsynchronousWaitDialog monitor = AsynchronousWaitDialog.ShowWaitDialog("${res:SharpDevelop.Refactoring.FindReferences}"))
			{
				FindReferencesAndRenameHelper.ShowAsSearchResults(StringParser.Parse("${res:SharpDevelop.Refactoring.ReferencesTo}",
				                                                                     new string[,] {{ "Name", memberName }}),
				                                                  RefactoringService.FindReferences(member, monitor));
			}
		}
	}
}
