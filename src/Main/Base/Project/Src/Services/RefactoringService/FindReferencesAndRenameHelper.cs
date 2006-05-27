// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Drawing;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;
using SearchAndReplace;

namespace ICSharpCode.SharpDevelop.Refactoring
{
	public static class FindReferencesAndRenameHelper
	{
		#region Rename Class
		public static void RenameClass(IClass c)
		{
			string newName = MessageService.ShowInputBox("${res:SharpDevelop.Refactoring.Rename}", "${res:SharpDevelop.Refactoring.RenameClassText}", c.Name);
			if (!FindReferencesAndRenameHelper.CheckName(newName, c.Name)) return;
			
			RenameClass(c, newName);
		}
		
		public static void RenameClass(IClass c, string newName)
		{
			c = c.GetCompoundClass(); // get compound class if class is partial
			
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
			int offset = documentInformation.CreateDocument().PositionToOffset(new Point(region.BeginColumn - 1, region.BeginLine - 1));
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
		#endregion
		
		#region Rename member
		public static void RenameMember(IMember member)
		{
			string newName = MessageService.ShowInputBox("${res:SharpDevelop.Refactoring.Rename}", "${res:SharpDevelop.Refactoring.RenameMemberText}", member.Name);
			if (!FindReferencesAndRenameHelper.CheckName(newName, member.Name)) return;
			
			RenameMember(member, newName);
		}
		
		public static void RenameMember(IMember member, string newName)
		{
			List<Reference> list = RefactoringService.FindReferences(member, null);
			if (list == null) return;
			FindReferencesAndRenameHelper.RenameReferences(list, newName);
			
			if (member is IField) {
				IProperty property = FindProperty((IField)member);
				if (property != null) {
					string newPropertyName = member.DeclaringType.ProjectContent.Language.CodeGenerator.GetPropertyName(newName);
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
			return c.CompilationUnit.FileName == null;
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
		#endregion
	}
}
