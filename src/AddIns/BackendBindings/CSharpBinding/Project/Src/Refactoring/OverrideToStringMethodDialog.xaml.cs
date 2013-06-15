// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Input;

using ICSharpCode.AvalonEdit.Snippets;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.CSharp.Refactoring;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Refactoring;

namespace CSharpBinding.Refactoring
{
	/// <summary>
	/// Interaction logic for OverrideToStringMethodDialog.xaml
	/// </summary>
	public partial class OverrideToStringMethodDialog : AbstractInlineRefactorDialog
	{
		AstNode baseCallNode;
		
		public OverrideToStringMethodDialog(InsertionContext context, ITextEditor editor, ITextAnchor anchor, IList<PropertyOrFieldWrapper> fields, AstNode baseCallNode)
			: base(context, editor, anchor)
		{
			InitializeComponent();
			
			this.baseCallNode = baseCallNode;
			this.listBox.ItemsSource = fields;
			
			listBox.SelectAll();
		}
		
		protected override string GenerateCode(ITypeDefinition currentClass)
		{
			string[] fields = listBox.SelectedItems.OfType<PropertyOrFieldWrapper>().Select(f2 => f2.MemberName).ToArray();
			PrimitiveExpression formatString = new PrimitiveExpression(GenerateFormatString(currentClass, editor.Language.CodeGenerator, fields));
			List<Expression> param = new List<Expression>() { formatString };
			ReturnStatement ret = new ReturnStatement(new InvocationExpression(
				new MemberReferenceExpression(new TypeReferenceExpression(ConvertType(KnownTypeCode.String)), "Format"),
				param.Concat(fields.Select(f => new IdentifierExpression(f))).ToList()
			));
			
			if (baseCallNode != null) {
				MethodDeclaration insertedOverrideMethod = refactoringContext.GetNode().PrevSibling as MethodDeclaration;
				if (insertedOverrideMethod == null)
				{
					// We are not inside of a method declaration
					return null;
				}
				
				using (Script script = refactoringContext.StartScript()) {
					NewLineNode nextNewLineNode = insertedOverrideMethod.NextSibling as NewLineNode;
					
					// Find base method call and replace it by return statement
					script.AddTo(insertedOverrideMethod.Body, ret);
					AppendNewLine(script, insertedOverrideMethod, nextNewLineNode);
				}
			}
			
			return null;
		}
		
		string GenerateFormatString(ITypeDefinition currentClass, ICodeGenerator generator, string[] fields)
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
		
		void SelectAllChecked(object sender, System.Windows.RoutedEventArgs e)
		{
			listBox.SelectAll();
		}
		
		void SelectAllUnchecked(object sender, System.Windows.RoutedEventArgs e)
		{
			listBox.UnselectAll();
		}
		
		bool AllSelected {
			get { return listBox.SelectedItems.Count == listBox.Items.Count; }
		}
		
		protected override void OnKeyDown(KeyEventArgs e)
		{
			Key? allAccessKey = GetAccessKeyFromButton(selectAll);
			
			if ((e.KeyboardDevice.Modifiers & ModifierKeys.Alt) == ModifierKeys.Alt && allAccessKey == e.SystemKey) {
				if (AllSelected)
					listBox.UnselectAll();
				else
					listBox.SelectAll();
				e.Handled = true;
			}
			
			base.OnKeyDown(e);
		}
		
		protected override void CancelButtonClick(object sender, System.Windows.RoutedEventArgs e)
		{
			base.CancelButtonClick(sender, e);
			
			if (baseCallNode != null) {
				// Insert at least the base call
				MethodDeclaration insertedOverrideMethod = refactoringContext.GetNode().PrevSibling as MethodDeclaration;
				if (insertedOverrideMethod == null)
				{
					// We are not inside of a method declaration
					return;
				}
				using (Script script = refactoringContext.StartScript()) {
					script.AddTo(insertedOverrideMethod.Body, baseCallNode);
				}
			}
		}
		
		protected override void OKButtonClick(object sender, System.Windows.RoutedEventArgs e)
		{
			base.OKButtonClick(sender, e);
		}
	}
}
