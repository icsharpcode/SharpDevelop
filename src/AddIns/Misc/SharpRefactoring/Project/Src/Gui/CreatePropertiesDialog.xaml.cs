// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

using ICSharpCode.AvalonEdit.Snippets;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Editor;

namespace SharpRefactoring.Gui
{
	/// <summary>
	/// Interaction logic for CreatePropertiesDialog.xaml
	/// </summary>
	public partial class CreatePropertiesDialog : AbstractInlineRefactorDialog
	{
		IList<FieldWrapper> fields;
		
		public CreatePropertiesDialog(InsertionContext context, ITextEditor editor, ITextAnchor anchor, IClass current, IList<FieldWrapper> availableFields)
			: base(context, editor, anchor)
		{
			InitializeComponent();
			
			this.listBox.ItemsSource = fields = availableFields;
			
			if (!fields.Any())
				Visibility = System.Windows.Visibility.Collapsed;
			
			implementInterface.IsChecked = !current.IsStatic && HasOnPropertyChanged(current);
			if (current.IsStatic)
				implementInterface.Visibility = System.Windows.Visibility.Collapsed;
			
			listBox.UnselectAll();
		}
		
		protected override string GenerateCode(LanguageProperties language, IClass currentClass)
		{
			StringBuilder builder = new StringBuilder();
			IDocumentLine line = editor.Document.GetLineForOffset(anchor.Offset);
			string indent = DocumentUtilitites.GetWhitespaceAfter(editor.Document, line.Offset);
			bool implementInterface = this.implementInterface.IsChecked == true;
			bool hasOnPropertyChanged = HasOnPropertyChanged(currentClass);
			bool useEventArgs = false;
			
			if (implementInterface && !currentClass.IsStatic) {
				if (!hasOnPropertyChanged) {
					var nodes = new List<AbstractNode>();
					var rt = new GetClassReturnType(currentClass.ProjectContent, "System.ComponentModel.INotifyPropertyChanged", 0);
					if (!currentClass.ClassInheritanceTree.Any(bt => bt.FullyQualifiedName == "System.ComponentModel.INotifyPropertyChanged")) {
						int insertion = editor.Document.PositionToOffset(currentClass.BodyRegion.BeginLine, currentClass.BodyRegion.BeginColumn);
						if (currentClass.BaseTypes.Count > 0)
							editor.Document.Insert(insertion, ", INotifyPropertyChanged");
						else
							editor.Document.Insert(insertion, " : INotifyPropertyChanged");
					}
					language.CodeGenerator.ImplementInterface(nodes, rt, false, currentClass);
					var ev = rt.GetEvents().First(e => e.Name == "PropertyChanged");
					MethodDeclaration onEvent = language.CodeGenerator.CreateOnEventMethod(new DefaultEvent(ev.Name, ev.ReturnType, ev.Modifiers, ev.Region, ev.BodyRegion, currentClass));
					nodes.Add(onEvent);
					onEvent.Parameters[0].TypeReference = new TypeReference("string", true);
					onEvent.Parameters[0].ParameterName = "propertyName";
					((RaiseEventStatement)onEvent.Body.Children[0]).Arguments[1] = new ObjectCreateExpression(new TypeReference("PropertyChangedEventArgs"), new List<Expression> { new IdentifierExpression("propertyName") });
					foreach (var node in nodes)
						builder.AppendLine(language.CodeGenerator.GenerateCode(node, indent));
					useEventArgs = false;
				} else {
					useEventArgs = currentClass.DefaultReturnType.GetMethods().First(m => m.Name == "OnPropertyChanged").Parameters[0].ReturnType.FullyQualifiedName != "System.String";
				}
			}
			
			foreach (FieldWrapper field in listBox.SelectedItems) {
				var prop = language.CodeGenerator.CreateProperty(field.Field, true, field.AddSetter);
				if (!field.Field.IsStatic && !currentClass.IsStatic && field.AddSetter && implementInterface) {
					var invocation = new ExpressionStatement(CreateInvocation(field.PropertyName, useEventArgs));
					var assignment = prop.SetRegion.Block.Children[0];
					prop.SetRegion.Block.Children.Clear();
					prop.SetRegion.Block.AddChild(
						new IfElseStatement(
							new BinaryOperatorExpression(new IdentifierExpression(field.MemberName), BinaryOperatorType.InEquality, new IdentifierExpression("value")),
							new BlockStatement { Children = { assignment, invocation } }
						)
					);
				}
				builder.AppendLine(language.CodeGenerator.GenerateCode(prop, indent));
			}
			
			return builder.ToString().Trim();
		}
		
		string GetCodeFromRegion(DomRegion region)
		{
			int startOffset = editor.Document.PositionToOffset(region.BeginLine, region.BeginColumn);
			int endOffset = editor.Document.PositionToOffset(region.EndLine, region.EndColumn);
			return editor.Document.GetText(startOffset, endOffset - startOffset);
		}
		
		bool HasOnPropertyChanged(IClass currentClass)
		{
			return currentClass.DefaultReturnType.GetMethods().Any(m => m.Name == "OnPropertyChanged");
		}
		
		InvocationExpression CreateInvocation(string name, bool useEventArgs)
		{
			Expression arg = useEventArgs
				? (Expression)new ObjectCreateExpression(new TypeReference("PropertyChangedEventArgs"), new List<Expression> { new PrimitiveExpression(name) })
				: (Expression)new PrimitiveExpression(name);
			return new InvocationExpression(new IdentifierExpression("OnPropertyChanged"), new List<Expression> { arg });
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
	}
}