// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ICSharpCode.AvalonEdit.Snippets;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.Refactoring;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;
using Ast = ICSharpCode.NRefactory.Ast;

namespace SharpRefactoring.Gui
{
	/// <summary>
	/// Interaction logic for OverrideToStringMethodDialog.xaml
	/// </summary>
	public partial class OverrideToStringMethodDialog : AbstractInlineRefactorDialog
	{
		List<Wrapper<IField>> fields;
		
		public OverrideToStringMethodDialog(InsertionContext context, ITextEditor editor, ITextAnchor startAnchor, ITextAnchor anchor, IList<IField> fields)
			: base(context, editor, anchor)
		{
			InitializeComponent();
			
			this.fields = fields.Select(f => new Wrapper<IField>() { Entity = f }).ToList();
			this.listBox.ItemsSource = this.fields.Select(i => i.Create(null));
		}
		
		protected override string GenerateCode(LanguageProperties language, IClass currentClass)
		{
			var fields = this.fields
				.Where(f => f.IsChecked)
				.Select(f2 => f2.Entity.Name)
				.ToArray();
			
			ClassFinder context = new ClassFinder(currentClass, editor.Caret.Line, editor.Caret.Column);
			IMember member =  OverrideCompletionItemProvider.GetOverridableMembers(currentClass).First(m => m.Name == "ToString");
			var node = language.CodeGenerator.GetOverridingMethod(member, context) as Ast.MethodDeclaration;
			var formatString = new Ast.PrimitiveExpression(GenerateFormatString(currentClass, language.CodeGenerator, fields));
			var param = new List<Ast.Expression>() { formatString };
			
			Ast.ReturnStatement ret = new Ast.ReturnStatement(new Ast.InvocationExpression(
				new Ast.MemberReferenceExpression(new Ast.TypeReferenceExpression(new Ast.TypeReference("System.String", true)), "Format"),
				param.Concat(fields.Select(f => new Ast.IdentifierExpression(f))).ToList()
			));
			
			node.Body.Children.Clear();
			node.Body.AddChild(ret);
			
			int offset = editor.Document.GetLineForOffset(editor.Caret.Offset).Offset;
			
			return language.CodeGenerator.GenerateCode(node, DocumentUtilitites.GetWhitespaceAfter(editor.Document, offset)).TrimStart();
		}
		
		string GenerateFormatString(IClass currentClass, CodeGenerator generator, string[] fields)
		{
			string fieldsString = "";
			
			if (fields.Any()) {
				StringBuilder formatString = new StringBuilder();
				
				for (int i = 0; i < fields.Length; i++) {
					if (i != 0)
						formatString.Append(", ");
					formatString.AppendFormat("{0}={{{1}}}", generator.GetPropertyName(fields[i]), i);
				}
				
				fieldsString = " " + formatString.ToString();
			}
			
			return "[" + currentClass.Name + fieldsString + "]";
		}
	}
}
