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
using ICSharpCode.SharpDevelop.DefaultEditor.Commands;

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
			IMethod method = member as IMethod;
			List<ToolStripItem> list = new List<ToolStripItem>();
			
			if (method == null || !method.IsConstructor) {
				cmd = new MenuCommand("&Rename", Rename);
				cmd.Tag = member;
				list.Add(cmd);
			}
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
			
			if (member is IField) {
				string propertyName = AbstractPropertyCodeGenerator.GetPropertyName(member.Name);
				LanguageProperties language = member.DeclaringType.ProjectContent.Language;
				IProperty foundProperty = null;
				foreach (IProperty prop in member.DeclaringType.Properties) {
					if (language.NameComparer.Equals(propertyName, prop.Name)) {
						foundProperty = prop;
						break;
					}
				}
				if (foundProperty != null) {
					cmd = new MenuCommand("Go to &property", GotoProperty);
					cmd.Tag = member;
					list.Add(cmd);
				} else {
					cmd = new MenuCommand("Create &getter", CreateGetter);
					cmd.Tag = member;
					list.Add(cmd);
					cmd = new MenuCommand("Create &property", CreateProperty);
					cmd.Tag = member;
					list.Add(cmd);
				}
			}
			
			return list.ToArray();
		}
		
		TextEditorControl JumpToDefinition(IMember member)
		{
			IViewContent viewContent = null;
			ICompilationUnit cu = member.DeclaringType.CompilationUnit;
			if (cu != null) {
				string fileName = cu.FileName;
				if (fileName != null) {
					if (member.Region != null && member.Region.BeginLine > 0) {
						viewContent = FileService.JumpToFilePosition(fileName, member.Region.BeginLine - 1, member.Region.BeginColumn - 1);
					} else {
						FileService.OpenFile(fileName);
					}
				}
			}
			ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor.ITextEditorControlProvider tecp = viewContent as ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor.ITextEditorControlProvider;
			return (tecp == null) ? null : tecp.TextEditorControl;
		}
		
		TextEditorControl JumpBehindDefinition(IMember member)
		{
			IViewContent viewContent = null;
			ICompilationUnit cu = member.DeclaringType.CompilationUnit;
			if (cu != null) {
				string fileName = cu.FileName;
				if (fileName != null) {
					if (member.Region != null && member.Region.EndLine > 0) {
						viewContent = FileService.JumpToFilePosition(fileName, member.Region.EndLine, 0);
					} else {
						FileService.OpenFile(fileName);
					}
				}
			}
			ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor.ITextEditorControlProvider tecp = viewContent as ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor.ITextEditorControlProvider;
			return (tecp == null) ? null : tecp.TextEditorControl;
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
			IMember member = (IMember)item.Tag;
			TextEditorControl textEditor = JumpBehindDefinition(member);
			
			AbstractPropertyCodeGenerator generator;
			if (includeSetter)
				generator = new GetterAndSetterCodeGenerator(member.DeclaringType);
			else
				generator = new GetterCodeGenerator(member.DeclaringType);
			List<AbstractFieldCodeGenerator.FieldWrapper> list = new List<AbstractFieldCodeGenerator.FieldWrapper>();
			foreach (AbstractFieldCodeGenerator.FieldWrapper fw in generator.Content) {
				if (fw.Field == member) {
					list.Add(fw);
				}
			}
			
			generator.BeginWithNewLine = true;
			generator.GenerateCode(textEditor.ActiveTextAreaControl.TextArea, list);
		}
		
		void GotoProperty(object sender, EventArgs e)
		{
			MenuCommand item = (MenuCommand)sender;
			IMember member = (IMember)item.Tag;
			string propertyName = AbstractPropertyCodeGenerator.GetPropertyName(member.Name);
			LanguageProperties language = member.DeclaringType.ProjectContent.Language;
			foreach (IProperty prop in member.DeclaringType.Properties) {
				if (language.NameComparer.Equals(propertyName, prop.Name)) {
					JumpToDefinition(prop);
					break;
				}
			}
		}
		
		void GoToBase(object sender, EventArgs e)
		{
			MenuCommand item = (MenuCommand)sender;
			IMember member = (IMember)item.Tag;
			IMember baseMember = RefactoringService.FindBaseMember(member);
			if (baseMember != null) {
				JumpToDefinition(baseMember);
			}
		}
		
		private struct Modification {
			public IDocument Document;
			public int Offset;
			public int LengthDifference;
			
			public Modification(IDocument Document, int Offset, int LengthDifference)
			{
				this.Document = Document;
				this.Offset = Offset;
				this.LengthDifference = LengthDifference;
			}
		}
		
		void Rename(object sender, EventArgs e)
		{
			MenuCommand item = (MenuCommand)sender;
			IMember member = (IMember)item.Tag;
			string newName = MessageService.ShowInputBox("Rename", "Enter the new name of the member", member.Name);
			if (newName == null || newName.Length == 0) return;
			
			List<Reference> list = RefactoringService.FindReferences(member, null);
			if (list == null) return;
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
					IDocument doc = p.TextEditorControl.Document;
					int offset = r.Offset;
					foreach (Modification m in modifications) {
						if (m.Document != doc) continue;
						if (m.Offset < offset) offset += m.LengthDifference;
					}
					int lengthDifference = newName.Length - r.Length;
					doc.Replace(offset, r.Length, newName);
					if (lengthDifference != 0) {
						for (int i = 0; i < modifications.Count; ++i) {
							Modification m = modifications[i];
							if (m.Document != doc) continue;
							if (m.Offset > offset) {
								m.Offset += lengthDifference;
								modifications[i] = m; // Modification is a value type
							}
						}
						modifications.Add(new Modification(doc, offset, lengthDifference));
					}
				}
			}
			foreach (IViewContent viewContent in modifiedContents) {
				ParserService.ParseViewContent(viewContent);
			}
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
				    FileUtility.IsEqualFileName(content.FileName, fileName))
				{
					return new ProvidedDocumentInformation(((ITextEditorControlProvider)content).TextEditorControl.Document, fileName, 0);
				}
			}
			ITextBufferStrategy strategy = StringTextBufferStrategy.CreateTextBufferFromFile(fileName);
			return new ProvidedDocumentInformation(strategy, fileName, 0);
		}
	}
}
