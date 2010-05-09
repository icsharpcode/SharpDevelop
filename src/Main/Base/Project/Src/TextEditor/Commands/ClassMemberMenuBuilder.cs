// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

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
using Ast = ICSharpCode.NRefactory.Ast;

namespace ICSharpCode.SharpDevelop.DefaultEditor.Commands
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
				IProperty property = member as IProperty;
				if (property.CanSet && canGenerateCode) {
					cmd = new MenuCommand("${res:SharpDevelop.Refactoring.CreateChangedEvent}", CreateChangedEvent);
					cmd.Tag = member;
					list.Add(cmd);
				}
				ITextEditor editor = FindReferencesAndRenameHelper.OpenDefinitionFile(property, false);
				string field = null;
				Ast.PropertyDeclaration astProp = null;
				if (IsAutomaticProperty(editor, property)) {
					cmd = new MenuCommand("${res:SharpDevelop.Refactoring.ExpandAutomaticProperty}", ExpandAutomaticProperty);
					cmd.Tag = member;
					list.Add(cmd);
				} else if (IsSimpleProperty(editor, property, out astProp, out field)) {
					cmd = new MenuCommand("${res:SharpDevelop.Refactoring.ConvertToAutomaticProperty}",
					                      delegate {
					                      	IField fieldDef = property.DeclaringType.Fields.FirstOrDefault(f => f.Name == field);
					                      	if (fieldDef == null)
					                      		return;
					                      	ConvertToAutomaticProperty(editor, property, fieldDef, astProp);
					                      }
					                     );
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
					ProvidedDocumentInformation documentInfo = FindReferencesAndRenameHelper.GetDocumentInformation(m.DeclaringType.CompilationUnit.FileName);
					SearchResultMatch res = new SimpleSearchResultMatch(documentInfo, matchText, new Location(m.Region.BeginColumn, m.Region.BeginLine));
					results.Add(res);
				}
			}
			SearchResultsPad.Instance.ShowSearchResults(
				StringParser.Parse("${res:SharpDevelop.Refactoring.OverridesOf}", new string[,] {{ "Name", member.Name }}),
				results
			);
			SearchResultsPad.Instance.BringToFront();
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
		
		bool IsAutomaticProperty(ITextEditor editor, IProperty property)
		{
			if (editor == null)
				return false;
			
			bool isAutomatic = false;
			
			if (property.CanGet) {
				if (property.GetterRegion.IsEmpty)
					isAutomatic = true;
				else {
					int getterStartOffset = editor.Document.PositionToOffset(property.GetterRegion.BeginLine, property.GetterRegion.BeginColumn);
					int getterEndOffset = editor.Document.PositionToOffset(property.GetterRegion.EndLine, property.GetterRegion.EndColumn);
					
					string text = editor.Document.GetText(getterStartOffset, getterEndOffset - getterStartOffset)
						.Replace(" ", "").Replace("\t", "").Replace("\n", "").Replace("\r", "");
					
					isAutomatic = text == "get;";
				}
			}
			
			if (property.CanSet) {
				if (property.SetterRegion.IsEmpty)
					isAutomatic |= true;
				else {
					int setterStartOffset = editor.Document.PositionToOffset(property.SetterRegion.BeginLine, property.SetterRegion.BeginColumn);
					int setterEndOffset = editor.Document.PositionToOffset(property.SetterRegion.EndLine, property.SetterRegion.EndColumn);
					
					string text = editor.Document.GetText(setterStartOffset, setterEndOffset - setterStartOffset)
						.Replace(" ", "").Replace("\t", "").Replace("\n", "").Replace("\r", "");
					
					isAutomatic |= text == "set;";
				}
			}
			
			return isAutomatic;
		}
		
		void ExpandAutomaticProperty(object sender, EventArgs e)
		{
			MenuCommand item = (MenuCommand)sender;
			IProperty member = (IProperty)item.Tag;
			ITextEditor textEditor = FindReferencesAndRenameHelper.JumpToDefinition(member);
			
			if (textEditor != null) {
				CodeGenerator codeGen = member.DeclaringType.ProjectContent.Language.CodeGenerator;
				IField field = new DefaultField(member.ReturnType, codeGen.GetFieldName(member.Name),
				                                member.Modifiers & ModifierEnum.Static, DomRegion.Empty, member.DeclaringType);
				ClassFinder finder = new ClassFinder(member);
				
				Ast.PropertyDeclaration p = codeGen.CreateProperty(field, member.CanGet, member.CanSet);
				
				p.Modifier = CodeGenerator.ConvertModifier(member.Modifiers, finder);
				
				if (member.CanGet)
					p.GetRegion.Modifier = CodeGenerator.ConvertModifier(member.GetterModifiers, finder);
				
				if (member.CanSet)
					p.SetRegion.Modifier = CodeGenerator.ConvertModifier(member.SetterModifiers, finder);
				
				int startOffset = textEditor.Document.PositionToOffset(member.Region.BeginLine, member.Region.BeginColumn);
				int endOffset = textEditor.Document.PositionToOffset(member.Region.EndLine, member.Region.EndColumn);
				
				if (!member.BodyRegion.IsEmpty)
					endOffset = textEditor.Document.PositionToOffset(member.BodyRegion.EndLine, member.BodyRegion.EndColumn);
				
				using (textEditor.Document.OpenUndoGroup()) {
					textEditor.Document.Remove(startOffset, endOffset - startOffset);
					
					if (codeGen.Options.EmptyLinesBetweenMembers) {
						var line = textEditor.Document.GetLine(member.Region.BeginLine);
						textEditor.Document.Remove(line.Offset, line.TotalLength);
					}
					
					codeGen.InsertCodeInClass(member.DeclaringType, new RefactoringDocumentAdapter(textEditor.Document), member.Region.BeginLine - 1, CodeGenerator.ConvertMember(field, finder), p);
				}
				
				ParserService.ParseCurrentViewContent();
			}
		}
		
		bool IsSimpleProperty(ITextEditor editor, IProperty property, out Ast.PropertyDeclaration astProperty, out string fieldName)
		{
			astProperty = null;
			fieldName = null;
			
			if (editor == null)
				return false;
			
			string ext = Path.GetExtension(editor.FileName);
			string code = GetMemberText(property, editor);
			
			Ast.PropertyDeclaration p = ParseMember(ext, code);
			
			if (p == null)
				return false;
			
			bool getResult = true;
			bool setResult = true;
			
			string identifier = null;
			
			if (p.HasGetRegion) {
				getResult = false;
				var ret = p.GetRegion.Block.Children.FirstOrDefault() as Ast.ReturnStatement;
				
				if (ret != null) {
					getResult = ret.Expression is Ast.IdentifierExpression;
					identifier = getResult ? (ret.Expression as Ast.IdentifierExpression).Identifier : null;
				}
			}
			
			if (p.HasSetRegion) {
				setResult = false;
				var ret = p.SetRegion.Block.Children.FirstOrDefault() as Ast.ExpressionStatement;
				
				if (ret != null && p.SetRegion.Block.Children.Count == 1 && ret.Expression is Ast.AssignmentExpression) {
					var assign = ret.Expression as Ast.AssignmentExpression;
					
					setResult = assign.Op == Ast.AssignmentOperatorType.Assign &&
						IsValidMemberAssignment(assign.Left, ref identifier) &&
						(assign.Right is Ast.IdentifierExpression && (assign.Right as Ast.IdentifierExpression).Identifier == "value");
				}
			}
			
			astProperty = p;
			fieldName = identifier;
			
			return getResult && setResult;
		}

		Ast.PropertyDeclaration ParseMember(string ext, string code)
		{
			IParser parser = null;
			
			try {
				if (ext.Equals(".cs", StringComparison.OrdinalIgnoreCase))
					parser = ParserFactory.CreateParser(SupportedLanguage.CSharp, new StringReader(code));
				if (ext.Equals(".vb", StringComparison.OrdinalIgnoreCase))
					parser = ParserFactory.CreateParser(SupportedLanguage.VBNet, new StringReader(code));
				
				if (parser == null)
					return null;
				
				var nodes = parser.ParseTypeMembers();
				
				if (parser.Errors.Count > 0)
					return null;
				
				return nodes.FirstOrDefault() as Ast.PropertyDeclaration;
			} finally {
				if (parser != null)
					parser.Dispose();
			}
		}
		
		string GetMemberText(IMember member, ITextEditor editor)
		{
			int startOffset = editor.Document.PositionToOffset(member.Region.BeginLine, member.Region.BeginColumn);
			int endOffset = editor.Document.PositionToOffset(member.BodyRegion.EndLine, member.BodyRegion.EndColumn);
			
			return editor.Document.GetText(startOffset, endOffset - startOffset);
		}
		
		bool IsValidMemberAssignment(Ast.Expression left, ref string identifier)
		{
			if (left is Ast.MemberReferenceExpression) {
				var e = left as Ast.MemberReferenceExpression;
				if (identifier == null) {
					identifier = e.MemberName;
					return e.TargetObject is Ast.ThisReferenceExpression;
				} else
					return e.TargetObject is Ast.ThisReferenceExpression && e.MemberName == identifier;
			}
			if (identifier == null) {
				if (left is Ast.IdentifierExpression) {
					identifier = (left as Ast.IdentifierExpression).Identifier;
					return true;
				}
				return false;
			} else
				return (left is Ast.IdentifierExpression) && (left as Ast.IdentifierExpression).Identifier == identifier;
		}
		
		void ConvertToAutomaticProperty(ITextEditor editor, IProperty property, IField fieldDef, Ast.PropertyDeclaration astProp)
		{
			CodeGenerator codeGen = property.DeclaringType.ProjectContent.Language.CodeGenerator;
			
			int fieldStartOffset = editor.Document.PositionToOffset(fieldDef.Region.BeginLine, fieldDef.Region.BeginColumn);
			int fieldEndOffset = editor.Document.PositionToOffset(fieldDef.Region.EndLine, fieldDef.Region.EndColumn);
			
			int startOffset = editor.Document.PositionToOffset(property.Region.BeginLine, property.Region.BeginColumn);
			int endOffset = editor.Document.PositionToOffset(property.BodyRegion.EndLine, property.BodyRegion.EndColumn);
			
			ITextAnchor startAnchor = editor.Document.CreateAnchor(startOffset);
			ITextAnchor endAnchor = editor.Document.CreateAnchor(endOffset);
			
			if (astProp.HasGetRegion)
				astProp.GetRegion.Block = null;
			
			if (!astProp.HasSetRegion) {
				astProp.SetRegion = new Ast.PropertySetRegion(null, null);
				astProp.SetRegion.Modifier = CodeGenerator.ConvertModifier(fieldDef.Modifiers, new ClassFinder(fieldDef))
					& (Ast.Modifiers.Private | Ast.Modifiers.Internal | Ast.Modifiers.Protected | Ast.Modifiers.Public);
			}
			
			if (astProp.HasSetRegion)
				astProp.SetRegion.Block = null;
			using (AsynchronousWaitDialog monitor = AsynchronousWaitDialog.ShowWaitDialog("${res:SharpDevelop.Refactoring.ConvertToAutomaticProperty}")) {
				var refs = RefactoringService.FindReferences(fieldDef, monitor);
				using (editor.Document.OpenUndoGroup()) {
					FindReferencesAndRenameHelper.RenameReferences(refs, property.Name);
					editor.Document.Remove(fieldStartOffset, fieldEndOffset - fieldStartOffset);
					editor.Document.Replace(startAnchor.Offset, endAnchor.Offset - startAnchor.Offset, codeGen.GenerateCode(astProp, ""));
				}
			}
		}
	}
}
