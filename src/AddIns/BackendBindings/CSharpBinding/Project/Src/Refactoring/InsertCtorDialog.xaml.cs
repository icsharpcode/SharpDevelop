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
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;

using ICSharpCode.AvalonEdit.Snippets;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.CSharp.Refactoring;
using ICSharpCode.NRefactory.CSharp.Resolver;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Parser;
using CSharpBinding.FormattingStrategy;

namespace CSharpBinding.Refactoring
{
	/// <summary>
	/// Interaction logic for InsertCtorDialog.xaml
	/// </summary>
	public partial class InsertCtorDialog : AbstractInlineRefactorDialog
	{
		IList<PropertyOrFieldWrapper> parameterList;
		
		public InsertCtorDialog(InsertionContext context, ITextEditor editor, ITextAnchor anchor)
			: base(context, editor, anchor)
		{
			InitializeComponent();
			
			Visibility = System.Windows.Visibility.Collapsed;
		}
		
		protected override void Initialize()
		{
			base.Initialize();
			
			var typeResolveContext = refactoringContext.GetTypeResolveContext();
			if (typeResolveContext == null) {
				return;
			}
			var resolvedCurrent = typeResolveContext.CurrentTypeDefinition;
			
			parameterList = CreateCtorParams(resolvedCurrent).ToList();
			this.varList.ItemsSource = parameterList;
			
			if (parameterList.Any())
				Visibility = System.Windows.Visibility.Visible;
		}
		
		IEnumerable<PropertyOrFieldWrapper> CreateCtorParams(IType sourceType)
		{
			int i = 0;
			
			foreach (var f in sourceType.GetFields().Where(field => !field.IsConst
			                                               && field.IsStatic == sourceType.GetDefinition().IsStatic
			                                               && field.DeclaringType.FullName == sourceType.FullName
			                                               && field.ReturnType != null)) {
				yield return new PropertyOrFieldWrapper(f) { Index = i };
				i++;
			}
			
			foreach (var p in sourceType.GetProperties().Where(prop => prop.CanSet && !prop.IsIndexer
			                                                   && prop.IsAutoImplemented()
			                                                   && prop.IsStatic == sourceType.GetDefinition().IsStatic
			                                                   && prop.DeclaringType.FullName == sourceType.FullName
			                                                   && prop.ReturnType != null)) {
				yield return new PropertyOrFieldWrapper(p) { Index = i };
				i++;
			}
		}
		
		protected override string GenerateCode(ITypeDefinition currentClass)
		{
			List<PropertyOrFieldWrapper> filtered = parameterList
				.Where(p => p.IsIncluded)
				.OrderBy(p => p.Index)
				.ToList();

			var test = refactoringContext.GetNode();
			var insertedConstructor = refactoringContext.GetNode().PrevSibling as ConstructorDeclaration;
			if (insertedConstructor == null)
			{
				// We are not inside of a constructor declaration
				return null;
			}
			
			using (Script script = refactoringContext.StartScript()) {
				BlockStatement originalCtorBody = insertedConstructor.Body;
				
				foreach (PropertyOrFieldWrapper w in filtered) {
					if (w.AddCheckForNull) {
						// true = reference, null = generic or unknown
						if (w.Type.IsReferenceType != false)
							script.AddTo(originalCtorBody,
							             new IfElseStatement(
							             	new BinaryOperatorExpression(new IdentifierExpression(w.ParameterName), BinaryOperatorType.Equality, new PrimitiveExpression(null)),
							             	new ThrowStatement(new ObjectCreateExpression(new SimpleType("ArgumentNullException"), new List<Expression>() { new PrimitiveExpression(w.ParameterName, '"' + w.ParameterName + '"') }))
							             )
							            );
						else
							script.AddTo(originalCtorBody,
							             new IfElseStatement(
							             	new UnaryOperatorExpression(UnaryOperatorType.Not, new MemberReferenceExpression(new IdentifierExpression(w.MemberName), "HasValue")),
							             	new ThrowStatement(new ObjectCreateExpression(new SimpleType("ArgumentNullException"), new List<Expression>() { new PrimitiveExpression(w.ParameterName, '"' + w.ParameterName + '"') }))
							             )
							            );
					}
					if (w.AddRangeCheck) {
						script.AddTo(originalCtorBody,
						             new IfElseStatement(
						             	new BinaryOperatorExpression(
						             		new BinaryOperatorExpression(new IdentifierExpression(w.ParameterName), BinaryOperatorType.LessThan, new IdentifierExpression("lower")),
						             		BinaryOperatorType.ConditionalOr,
						             		new BinaryOperatorExpression(new IdentifierExpression(w.ParameterName), BinaryOperatorType.GreaterThan, new IdentifierExpression("upper"))
						             	),
						             	new ThrowStatement(
						             		new ObjectCreateExpression(
						             			new SimpleType("ArgumentOutOfRangeException"),
						             			new List<Expression>() { new PrimitiveExpression(w.ParameterName, '"' + w.ParameterName + '"'), new IdentifierExpression(w.ParameterName), new BinaryOperatorExpression(new PrimitiveExpression("Value must be between "), BinaryOperatorType.Add, new BinaryOperatorExpression(new IdentifierExpression("lower"), BinaryOperatorType.Add, new BinaryOperatorExpression(new PrimitiveExpression(" and "), BinaryOperatorType.Add, new IdentifierExpression("upper")))) }
						             		)
						             	)
						             )
						            );
					}
				}
				
				foreach (PropertyOrFieldWrapper w in filtered) {
					script.AddTo(originalCtorBody,
					             new ExpressionStatement(new AssignmentExpression(new MemberReferenceExpression(new ThisReferenceExpression(), w.MemberName), AssignmentOperatorType.Assign, new IdentifierExpression(w.ParameterName)))
					            );
				}
			}
			
			AnchorElement parameterListElement = insertionContext.ActiveElements
				.OfType<AnchorElement>()
				.FirstOrDefault(item => item.Name.Equals("parameterList", StringComparison.OrdinalIgnoreCase));

			if (parameterListElement != null) {
				StringBuilder pList = new StringBuilder();

				var parameters = filtered
					.Select(p => new ParameterDeclaration(refactoringContext.CreateShortType(p.Type), p.ParameterName))
					.ToList();

				using (StringWriter textWriter = new StringWriter(pList)) {
					// Output parameter list as string
					var formattingOptions = CSharpFormattingPolicies.Instance.GetProjectOptions(refactoringContext.Compilation.GetProject());
					CSharpOutputVisitor outputVisitor = new CSharpOutputVisitor(textWriter, formattingOptions.OptionsContainer.GetEffectiveOptions());
					for (int i = 0; i < parameters.Count; i++) {
						if (i > 0)
							textWriter.Write(",");
						outputVisitor.VisitParameterDeclaration(parameters[i]);
					}
				}

				parameterListElement.Text = pList.ToString();
			}
			
			return null;
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
					SelectAllUnchecked();
				else
					SelectAllChecked();
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
							insertionContext.Deactivate(null);
						else {
							insertionEndAnchor = editor.Document.CreateAnchor(anchor.Offset);
							insertionEndAnchor.MovementType = AnchorMovementType.AfterInsertion;
						}
					}
				)
			);
		}
		
		void SelectAllChecked()
		{
			foreach (var param in parameterList) {
				param.IsIncluded = true;
			}
		}
		
		void SelectAllChecked(object sender, System.Windows.RoutedEventArgs e)
		{
			SelectAllChecked();
		}
		
		void SelectAllUnchecked()
		{
			foreach (var param in parameterList) {
				param.IsIncluded = false;
			}
		}
		
		void SelectAllUnchecked(object sender, System.Windows.RoutedEventArgs e)
		{
			SelectAllUnchecked();
		}
		
		bool AllSelected {
			get { return parameterList.Count(p => p.IsIncluded) == parameterList.Count; }
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
