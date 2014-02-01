// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Documents;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.Core.Presentation;
using ICSharpCode.Core.WinForms;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor.Bookmarks;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.Refactoring;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Refactoring;
using Ast = ICSharpCode.NRefactory.Ast;

namespace SharpRefactoring
{
	public class PropertyRefactoringMenuBuilder : ISubmenuBuilder, IMenuItemBuilder
	{
		public System.Collections.ICollection BuildItems(ICSharpCode.Core.Codon codon, object owner)
		{
			return BuildSubmenu(codon, owner).TranslateToWpf();
		}
		
		public System.Windows.Forms.ToolStripItem[] BuildSubmenu(ICSharpCode.Core.Codon codon, object owner)
		{
			List<System.Windows.Forms.ToolStripItem> items = new List<System.Windows.Forms.ToolStripItem>();
			MenuCommand cmd;
			
			IProperty property;
			if (owner is ICSharpCode.SharpDevelop.Gui.ClassBrowser.MemberNode)
				property = ((ICSharpCode.SharpDevelop.Gui.ClassBrowser.MemberNode)owner).Member as IProperty;
			else if (owner is ClassMemberBookmark)
				property = ((ClassMemberBookmark)owner).Member as IProperty;
			else
				property = null;
			
			if (property == null)
				return items.ToArray();
			
			ITextEditor editor = FindReferencesAndRenameHelper.OpenDefinitionFile(property, false);
			string field = null;
			PropertyDeclaration astProp = null;
			if (property.IsAutoImplemented()) {
				cmd = new MenuCommand("${res:SharpDevelop.Refactoring.ExpandAutomaticProperty}", ExpandAutomaticProperty);
				cmd.Tag = property;
				items.Add(cmd);
			} else if (IsSimpleProperty(editor, property, out astProp, out field)) {
				cmd = new MenuCommand("${res:SharpDevelop.Refactoring.ConvertToAutomaticProperty}",
				                      delegate {
				                      	IField fieldDef = property.DeclaringType.Fields.FirstOrDefault(f => f.Name == field);
				                      	if (fieldDef == null)
				                      		return;
				                      	ConvertToAutomaticProperty(editor, property, fieldDef, astProp);
				                      }
				                     );
				cmd.Tag = property;
				items.Add(cmd);
			}
			
			return items.ToArray();
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
				
				PropertyDeclaration oldProp = ParseMember<PropertyDeclaration>(Path.GetExtension(textEditor.FileName), GetMemberText(member, textEditor));
				
				if (member.CanGet)
					p.GetRegion.Modifier = CodeGenerator.ConvertModifier(member.GetterModifiers, finder);
				
				if (member.CanSet)
					p.SetRegion.Modifier = CodeGenerator.ConvertModifier(member.SetterModifiers, finder);
				
				int startOffset = textEditor.Document.PositionToOffset(member.Region.BeginLine, member.Region.BeginColumn);
				int endOffset = textEditor.Document.PositionToOffset(member.Region.EndLine, member.Region.EndColumn);
				
				if (!member.BodyRegion.IsEmpty)
					endOffset = textEditor.Document.PositionToOffset(member.BodyRegion.EndLine, member.BodyRegion.EndColumn);
				
				FieldDeclaration f = CodeGenerator.ConvertMember(field, finder);
				
				f.Fields[0].Initializer = oldProp.Initializer;
				
				using (textEditor.Document.OpenUndoGroup()) {
					textEditor.Document.Remove(startOffset, endOffset - startOffset);
					
					if (codeGen.Options.EmptyLinesBetweenMembers) {
						var line = textEditor.Document.GetLine(member.Region.BeginLine);
						textEditor.Document.Remove(line.Offset, line.TotalLength);
					}
					
					codeGen.InsertCodeInClass(member.DeclaringType, new RefactoringDocumentAdapter(textEditor.Document), member.Region.BeginLine - 1, f, p);
				}
				
				ParserService.ParseCurrentViewContent();
			}
		}
		
		#region ConvertToAutomaticProperty
		bool IsSimpleProperty(ITextEditor editor, IProperty property, out Ast.PropertyDeclaration astProperty, out string fieldName)
		{
			astProperty = null;
			fieldName = null;
			
			if (editor == null)
				return false;
			
			string ext = Path.GetExtension(editor.FileName);
			string code = GetMemberText(property, editor);
			
			Ast.PropertyDeclaration p = ParseMember<Ast.PropertyDeclaration>(ext, code);
			
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

		T ParseMember<T>(string ext, string code) where T : INode
		{
			IParser parser = null;
			
			try {
				if (ext.Equals(".cs", StringComparison.OrdinalIgnoreCase))
					parser = ParserFactory.CreateParser(SupportedLanguage.CSharp, new StringReader(code));
				if (ext.Equals(".vb", StringComparison.OrdinalIgnoreCase))
					parser = ParserFactory.CreateParser(SupportedLanguage.VBNet, new StringReader(code));
				
				if (parser == null)
					return default(T);
				
				var nodes = parser.ParseTypeMembers();
				
				if (parser.Errors.Count > 0)
					return default(T);
				
				return (T)nodes.FirstOrDefault();
			} finally {
				if (parser != null)
					parser.Dispose();
			}
		}
		
		string GetMemberText(IMember member, ITextEditor editor)
		{
			int startOffset = editor.Document.PositionToOffset(member.Region.BeginLine, member.Region.BeginColumn);
			int endOffset;
			
			if (member.BodyRegion.IsEmpty)
				endOffset = editor.Document.PositionToOffset(member.Region.EndLine, member.Region.EndColumn);
			else
				endOffset = editor.Document.PositionToOffset(member.BodyRegion.EndLine, member.BodyRegion.EndColumn);
			
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
			
			Ast.FieldDeclaration f = ParseMember<Ast.FieldDeclaration>(Path.GetExtension(editor.FileName),
			                                                           GetMemberText(fieldDef, editor));
			astProp.Initializer = f.Fields.First().Initializer;
			
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
		#endregion
	}
}
