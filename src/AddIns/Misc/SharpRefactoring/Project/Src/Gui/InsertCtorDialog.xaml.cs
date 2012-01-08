// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;

using ICSharpCode.AvalonEdit.Snippets;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.Refactoring;
using ICSharpCode.SharpDevelop.Editor;

namespace SharpRefactoring.Gui
{
	/// <summary>
	/// Interaction logic for InsertCtorDialog.xaml
	/// </summary>
	public partial class InsertCtorDialog : AbstractInlineRefactorDialog
	{
		IList<PropertyOrFieldWrapper> parameterList;
		
		public InsertCtorDialog(InsertionContext context, ITextEditor editor, ITextAnchor anchor, IClass current, IList<PropertyOrFieldWrapper> possibleParameters)
			: base(context, editor, anchor)
		{
			InitializeComponent();
			
			this.varList.ItemsSource = parameterList = possibleParameters;
			
			if (!parameterList.Any())
				Visibility = System.Windows.Visibility.Collapsed;
		}
		
		protected override string GenerateCode(LanguageProperties language, IClass currentClass)
		{
			IDocumentLine line = editor.Document.GetLineForOffset(anchor.Offset);
			string indent = DocumentUtilitites.GetWhitespaceAfter(editor.Document, line.Offset);
			
			List<PropertyOrFieldWrapper> filtered = this.varList.SelectedItems.OfType<PropertyOrFieldWrapper>()
				.OrderBy(p => p.Index)
				.ToList();
			
			BlockStatement block = new BlockStatement();
			
			foreach (PropertyOrFieldWrapper w in filtered) {
				if (w.AddCheckForNull) {
					// true = reference, null = generic or unknown
					if (w.Type.IsReferenceType != false)
						block.AddChild(
							new IfElseStatement(
								new BinaryOperatorExpression(new IdentifierExpression(w.ParameterName), BinaryOperatorType.Equality, new PrimitiveExpression(null)),
								new ThrowStatement(new ObjectCreateExpression(new TypeReference("ArgumentNullException"), new List<Expression>() { new PrimitiveExpression(w.ParameterName, '"' + w.ParameterName + '"') }))
							)
						);
					else
						block.AddChild(
							new IfElseStatement(
								new UnaryOperatorExpression(new MemberReferenceExpression(new IdentifierExpression(w.MemberName), "HasValue"), UnaryOperatorType.Not),
								new ThrowStatement(new ObjectCreateExpression(new TypeReference("ArgumentNullException"), new List<Expression>() { new PrimitiveExpression(w.ParameterName, '"' + w.ParameterName + '"') }))
							)
						);
				}
				if (w.AddRangeCheck) {
					block.AddChild(
						new IfElseStatement(
							new BinaryOperatorExpression(
								new BinaryOperatorExpression(new IdentifierExpression(w.ParameterName), BinaryOperatorType.LessThan, new IdentifierExpression("lower")),
								BinaryOperatorType.LogicalOr,
								new BinaryOperatorExpression(new IdentifierExpression(w.ParameterName), BinaryOperatorType.GreaterThan, new IdentifierExpression("upper"))
							),
							new ThrowStatement(
								new ObjectCreateExpression(
									new TypeReference("ArgumentOutOfRangeException"),
									new List<Expression>() { new PrimitiveExpression(w.ParameterName, '"' + w.ParameterName + '"'), new IdentifierExpression(w.ParameterName), new BinaryOperatorExpression(new PrimitiveExpression("Value must be between "), BinaryOperatorType.Concat, new BinaryOperatorExpression(new IdentifierExpression("lower"), BinaryOperatorType.Concat, new BinaryOperatorExpression(new PrimitiveExpression(" and "), BinaryOperatorType.Concat, new IdentifierExpression("upper")))) }
								)
							)
						)
					);
				}
			}
			
			foreach (PropertyOrFieldWrapper w in filtered)
				block.AddChild(new ExpressionStatement(new AssignmentExpression(new MemberReferenceExpression(new ThisReferenceExpression(), w.MemberName), AssignmentOperatorType.Assign, new IdentifierExpression(w.ParameterName))));
			
			AnchorElement parameterListElement = context.ActiveElements
				.OfType<AnchorElement>()
				.FirstOrDefault(item => item.Name.Equals("parameterList", StringComparison.OrdinalIgnoreCase));
			
			if (parameterListElement != null) {
				StringBuilder pList = new StringBuilder();
				
				var parameters = filtered
					.Select(p => new ParameterDeclarationExpression(ConvertType(p.Type), p.ParameterName))
					.ToList();
				
				for (int i = 0; i < parameters.Count; i++) {
					if (i > 0)
						pList.Append(", ");
					pList.Append(language.CodeGenerator.GenerateCode(parameters[i], ""));
				}
				
				parameterListElement.Text = pList.ToString();
			}
			
			StringBuilder builder = new StringBuilder();
			
			foreach (var element in block.Children.OfType<AbstractNode>()) {
				builder.Append(language.CodeGenerator.GenerateCode(element, indent));
			}
			
			return builder.ToString().Trim();
		}
		
		void UpClick(object sender, System.Windows.RoutedEventArgs e)
		{
			int selection = varList.SelectedIndex;
			
			if (selection <= 0)
				return;
			
			var curItem = parameterList.First(p => p.Index == selection);
			var exchangeItem = parameterList.First(p => p.Index == selection - 1);
			
			curItem.Index = selection - 1;
			exchangeItem.Index = selection;
			
			varList.ItemsSource = parameterList.OrderBy(p => p.Index);
			varList.SelectedIndex = selection - 1;
		}
		
		void DownClick(object sender, System.Windows.RoutedEventArgs e)
		{
			int selection = varList.SelectedIndex;
			
			if (selection < 0 || selection >= parameterList.Count - 1)
				return;
			
			var curItem = parameterList.First(p => p.Index == selection);
			var exchangeItem = parameterList.First(p => p.Index == selection + 1);
			
			curItem.Index = selection + 1;
			exchangeItem.Index = selection;
			
			varList.ItemsSource = parameterList.OrderBy(p => p.Index);
			varList.SelectedIndex = selection + 1;
		}
		
		protected override void OnKeyDown(KeyEventArgs e)
		{
			Key? downAccessKey = GetAccessKeyFromButton(moveDown);
			Key? upAccessKey = GetAccessKeyFromButton(moveUp);
			Key? allAccessKey = GetAccessKeyFromButton(selectAll);
			
			if ((e.KeyboardDevice.Modifiers & ModifierKeys.Alt) == ModifierKeys.Alt && allAccessKey == e.SystemKey) {
				if (AllSelected)
					varList.UnselectAll();
				else
					varList.SelectAll();
				e.Handled = true;
			}
			if ((e.KeyboardDevice.Modifiers & ModifierKeys.Alt) == ModifierKeys.Alt && upAccessKey == e.SystemKey) {
				UpClick(this, null);
				e.Handled = true;
			}
			if ((e.KeyboardDevice.Modifiers & ModifierKeys.Alt) == ModifierKeys.Alt && downAccessKey == e.SystemKey) {
				DownClick(this, null);
				e.Handled = true;
			}
			
			base.OnKeyDown(e);
		}
		
		protected override void FocusFirstElement()
		{
			Dispatcher.BeginInvoke((Action)TryFocusAndSelectItem, DispatcherPriority.Background);
		}
		
		void TryFocusAndSelectItem()
		{
			if (!parameterList.Any())
				return;
			
			object ctorParamWrapper = varList.Items.GetItemAt(0);
			if (ctorParamWrapper != null) {
				ListBoxItem item = (ListBoxItem)varList.ItemContainerGenerator.ContainerFromItem(ctorParamWrapper);
				item.Focus();
				
				varList.ScrollIntoView(item);
				varList.SelectedItem = item;
				Keyboard.Focus(item);
			}
		}
		
		protected override void OnInsertionCompleted()
		{
			base.OnInsertionCompleted();
			
			Dispatcher.BeginInvoke(
				DispatcherPriority.Background,
				(Action)(
					() => {
						if (!parameterList.Any())
							context.Deactivate(null);
						else {
							insertionEndAnchor = editor.Document.CreateAnchor(anchor.Offset);
							insertionEndAnchor.MovementType = AnchorMovementType.AfterInsertion;
						}
					}
				)
			);
		}
		
		void SelectAllChecked(object sender, System.Windows.RoutedEventArgs e)
		{
			this.varList.SelectAll();
		}
		
		void SelectAllUnchecked(object sender, System.Windows.RoutedEventArgs e)
		{
			this.varList.UnselectAll();
		}
		
		bool AllSelected {
			get { return varList.SelectedItems.Count == varList.Items.Count; }
		}
		
		protected override void CancelButtonClick(object sender, System.Windows.RoutedEventArgs e)
		{
			base.CancelButtonClick(sender, e);
			
			editor.Caret.Offset = anchor.Offset;
		}
		
		protected override void OKButtonClick(object sender, System.Windows.RoutedEventArgs e)
		{
			base.OKButtonClick(sender, e);
			
			editor.Caret.Offset = insertionEndAnchor.Offset;
		}
	}
	
	[ValueConversion(typeof(int), typeof(bool))]
	public class IntToBoolConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return ((int)value) != -1;
		}
		
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return ((bool)value) ? 0 : -1;
		}
	}
}
