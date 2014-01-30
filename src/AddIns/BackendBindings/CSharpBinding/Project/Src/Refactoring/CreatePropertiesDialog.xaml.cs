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
using System.Linq;
using System.Windows.Input;

using ICSharpCode.AvalonEdit.Snippets;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.CSharp.Refactoring;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem.Implementation;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Refactoring;
using CSharpBinding.Parser;

namespace CSharpBinding.Refactoring
{
	/// <summary>
	/// Interaction logic for CreatePropertiesDialog.xaml
	/// </summary>
	public partial class CreatePropertiesDialog : AbstractInlineRefactorDialog
	{
		IList<FieldWrapper> fields;
		
		public CreatePropertiesDialog(InsertionContext context, ITextEditor editor, ITextAnchor anchor)
			: base(context, editor, anchor)
		{
			InitializeComponent();
		}
		
		protected override void Initialize()
		{
			base.Initialize();
			
			var typeResolveContext = refactoringContext.GetTypeResolveContext();
			if (typeResolveContext == null) {
				return;
			}
			var resolvedCurrent = typeResolveContext.CurrentTypeDefinition;
			
			fields = FindFields(resolvedCurrent).ToList();
			this.listBox.ItemsSource = fields;
			
			if (fields.Any())
				Visibility = System.Windows.Visibility.Visible;
			
			implementInterface.IsChecked = !resolvedCurrent.IsStatic && HasOnPropertyChanged(resolvedCurrent);
			if (resolvedCurrent.IsStatic)
				implementInterface.Visibility = System.Windows.Visibility.Collapsed;
			
			SelectAllUnchecked();
		}
		
		static IEnumerable<FieldWrapper> FindFields(IType sourceClass)
		{
			int i = 0;
			
			foreach (var f in sourceClass.GetFields().Where(field => !field.IsConst
			                                                && field.ReturnType != null)) {
				yield return new FieldWrapper(f) { Index = i };
				i++;
			}
		}

		
		protected override string GenerateCode(ITypeDefinition currentClass)
		{
			bool implementInterface = this.implementInterface.IsChecked == true;
			bool hasOnPropertyChanged = HasOnPropertyChanged(currentClass);
			bool useEventArgs = false;
			
			AstNode insertionAnchorElement = refactoringContext.GetNode();
			if ((insertionAnchorElement == null) || !(insertionAnchorElement.Parent is TypeDeclaration)) {
				return null;
			}
			NewLineNode newLineNode = insertionAnchorElement as NewLineNode;
			while (insertionAnchorElement.PrevSibling is NewLineNode)
				insertionAnchorElement = insertionAnchorElement.PrevSibling ?? insertionAnchorElement;
			
			using (Script script = refactoringContext.StartScript()) {
				TypeDeclaration currentClassDeclaration = insertionAnchorElement.Parent as TypeDeclaration;
				
				if (implementInterface && !currentClass.IsStatic) {
					if (!hasOnPropertyChanged) {
						var nodes = new List<AstNode>();
						if (!currentClass.GetAllBaseTypeDefinitions().Any(bt => bt.FullName == "System.ComponentModel.INotifyPropertyChanged")) {
							AstNode nodeBeforeClassBlock = currentClassDeclaration.LBraceToken;
							if (nodeBeforeClassBlock.PrevSibling is NewLineNode) {
								// There's a new line before the brace, insert before it!
								nodeBeforeClassBlock = nodeBeforeClassBlock.PrevSibling;
							}
							int insertion = editor.Document.GetOffset(nodeBeforeClassBlock.StartLocation);
							
							AstType interfaceTypeNode = refactoringContext.CreateShortType("System.ComponentModel", "INotifyPropertyChanged", 0);
							var directBaseTypes = currentClass.DirectBaseTypes.Where(t => t.FullName != "System.Object");
							if (currentClassDeclaration.BaseTypes.Count > 0) {
								script.InsertText(insertion, ", " + interfaceTypeNode + " ");
							} else {
								script.InsertText(insertion, " : " + interfaceTypeNode + " ");
							}
						}

						var rt = new GetClassTypeReference("System.ComponentModel", "INotifyPropertyChanged", 0);
						var rtResolved = rt.Resolve(refactoringContext.Compilation);
						var ev = rtResolved.GetEvents().First(e => e.Name == "PropertyChanged");
						
						EventDeclaration propertyChangedEvent = new EventDeclaration();
						propertyChangedEvent.Variables.Add(new VariableInitializer(ev.Name));
						propertyChangedEvent.Modifiers = Modifiers.Public;
						propertyChangedEvent.ReturnType = refactoringContext.CreateShortType(ev.ReturnType);
						
						nodes.Add(propertyChangedEvent);
						
						MethodDeclaration onEvent = CreateOnEventMethod(ev, currentClass);
						nodes.Add(onEvent);
						foreach (var node in nodes) {
							script.InsertAfter(insertionAnchorElement, node);
							AppendNewLine(script, insertionAnchorElement, newLineNode);
						}
						useEventArgs = false;
					} else {
						useEventArgs = currentClass.GetMethods().First(m => m.Name == "OnPropertyChanged").Parameters[0].Type.FullName != "System.String";
					}
				}
				
				foreach (FieldWrapper field in fields.Where(f => f.IsIncluded)) {
					var prop = CreateProperty(field.Field, true, field.AddSetter);
					if (!field.Field.IsStatic && !currentClass.IsStatic && field.AddSetter && implementInterface) {
						var invocation = new ExpressionStatement(CreateInvocation(field.PropertyName, useEventArgs));
						var assignment = prop.Setter.Body.Children.ElementAt(0) as Statement;
						prop.Setter.Body = new BlockStatement();
						BlockStatement elseBlock = new BlockStatement();
						elseBlock.Add(assignment.Clone());
						elseBlock.Add(invocation);
						prop.Setter.Body.Add(
							new IfElseStatement(
								new BinaryOperatorExpression(new IdentifierExpression(field.MemberName), BinaryOperatorType.InEquality, new IdentifierExpression("value")),
								elseBlock
							)
						);
					}
					
					script.InsertAfter(insertionAnchorElement, prop);
					AppendNewLine(script, insertionAnchorElement, newLineNode);
				}
			}
			
			return null;
		}
		
		MethodDeclaration CreateOnEventMethod(IEvent e, ITypeDefinition currentClass)
		{
			List<ParameterDeclaration> parameters = new List<ParameterDeclaration>();
//			bool sender = false;
//			if (e.ReturnType != null) {
//				IMethod invoke = e.ReturnType.GetMethods().SingleOrDefault(m => m.Name=="Invoke" );
//				if (invoke != null) {
//					foreach (IParameter param in invoke.Parameters) {
//						parameters.Add(new ParameterDeclaration(ConvertType(param.Type), param.Name));
//					}
//					if (parameters.Count > 0 && string.Equals(parameters[0].Name, "sender", StringComparison.InvariantCultureIgnoreCase)) {
//						sender = true;
//						parameters.RemoveAt(0);
//					}
//				}
//			}
			parameters.Add(new ParameterDeclaration(ConvertType(KnownTypeCode.String), "propertyName"));
			
			Modifiers modifier;
			if (e.IsStatic)
				modifier = Modifiers.Private | Modifiers.Static;
			else if ((e.DeclaringType.GetDefinition() != null) && e.DeclaringType.GetDefinition().IsSealed)
				modifier = Modifiers.Private;
			else
				modifier = Modifiers.Protected | Modifiers.Virtual;
			
			MethodDeclaration method = new MethodDeclaration {
				Name = "On" + e.Name,
				Modifiers = ConvertModifier(modifier, currentClass),
				ReturnType = ConvertType(KnownTypeCode.Void)
			};
			method.Parameters.AddRange(parameters);
			
			List<Expression> arguments = new List<Expression>();
//			if (sender) {
			if (e.IsStatic)
				arguments.Add(new PrimitiveExpression(null, "null"));
			else
				arguments.Add(new ThisReferenceExpression());
//			}
//			foreach (ParameterDeclaration param in parameters) {
//				arguments.Add(new IdentifierExpression(param.Name));
//			}
			arguments.Add(new ObjectCreateExpression(refactoringContext.CreateShortType("System.ComponentModel", "PropertyChangedEventArgs", 0),
			                                         new List<Expression> { new IdentifierExpression("propertyName") }));
			method.Body = new BlockStatement();
//			method.Body.Add(new RaiseEventStatement(e.Name, arguments));
			method.Body.Add(new ExpressionStatement(new InvocationExpression(new MemberReferenceExpression(new ThisReferenceExpression(), e.Name), arguments)));
			
			return method;
		}
		
		PropertyDeclaration CreateProperty(IField field, bool createGetter, bool createSetter)
		{
			IProject project = field.Compilation.GetProject();
			if (project == null)
				return null;
			
			CodeGenerator codeGenerator = project.LanguageBinding.CodeGenerator;
			string name = codeGenerator.GetPropertyName(field.Name);
			CSharpFullParseInformation tempParseInformation;
			PropertyDeclaration property = new PropertyDeclaration()
			{
				Modifiers = ConvertModifier(field.GetDeclaration(out tempParseInformation).Modifiers, field.DeclaringTypeDefinition),
				Name = name
			};
			property.ReturnType = ConvertType(field.ReturnType);
			if (createGetter) {
				property.Getter = new Accessor()
				{
					Body = new BlockStatement()
				};
				property.Getter.Body.Add(new ReturnStatement(new IdentifierExpression(field.Name)));
			}
			if (createSetter) {
				property.Setter = new Accessor()
				{
					Body = new BlockStatement()
				};
				property.Setter.Body.Add(new AssignmentExpression(new IdentifierExpression(field.Name), new IdentifierExpression("value")));
			}
			
			property.Modifiers = Modifiers.Public | (property.Modifiers & Modifiers.Static);
			return property;
		}
		
		public static Modifiers ConvertModifier(Modifiers modifiers, ITypeDefinition targetContext)
		{
			IProject project = targetContext.ParentAssembly.GetProject();
			if (targetContext != null && project != null && targetContext.DeclaringType != null) {
//				if (project.LanguageBinding.IsClassWithImplicitlyStaticMembers(targetContext.CallingClass)) {
				return modifiers & ~Modifiers.Static;
//				}
			}
			if (modifiers.HasFlag(Modifiers.Static))
				modifiers &= ~(Modifiers.Abstract | Modifiers.Sealed);
			return modifiers;
		}
		
		string GetCodeFromRegion(DomRegion region)
		{
			int startOffset = editor.Document.PositionToOffset(region.BeginLine, region.BeginColumn);
			int endOffset = editor.Document.PositionToOffset(region.EndLine, region.EndColumn);
			return editor.Document.GetText(startOffset, endOffset - startOffset);
		}
		
		bool HasOnPropertyChanged(ITypeDefinition currentClass)
		{
			return currentClass.GetMethods().Any(m => m.Name == "OnPropertyChanged");
		}
		
		InvocationExpression CreateInvocation(string name, bool useEventArgs)
		{
			Expression arg = useEventArgs
				? (Expression)new ObjectCreateExpression(refactoringContext.CreateShortType("System.ComponentModel", "PropertyChangedEventArgs", 0), new List<Expression> { new PrimitiveExpression(name) })
				: (Expression)new PrimitiveExpression(name);
			return new InvocationExpression(new IdentifierExpression("OnPropertyChanged"), new List<Expression> { arg });
		}
		
		void SelectAllChecked()
		{
			foreach (var param in fields) {
				param.IsIncluded = true;
			}
		}
		
		void SelectAllChecked(object sender, System.Windows.RoutedEventArgs e)
		{
			SelectAllChecked();
		}
		
		void SelectAllUnchecked()
		{
			foreach (var param in fields) {
				param.IsIncluded = false;
			}
		}
		
		void SelectAllUnchecked(object sender, System.Windows.RoutedEventArgs e)
		{
			SelectAllUnchecked();
		}
		
		bool AllSelected {
			get { return fields.Count(p => p.IsIncluded) == fields.Count; }
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
