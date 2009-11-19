// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="siegfriedpammer@gmail.com" />
//     <version>$Revision$</version>
// </file>

using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

using ICSharpCode.Core;
using ICSharpCode.Core.Presentation;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.Refactoring;
using ICSharpCode.SharpDevelop.Editor;

namespace SharpRefactoring.Gui
{
	public class OverrideEqualsGetHashCodeMethodsDialog : AbstractInlineRefactorDialog
	{
		CheckBox addIEquatableCheckBox;
		CheckBox addOperatorOverrides;
		CheckBox implementAllIEquatables;
		
		IClass selectedClass;
		
		public OverrideEqualsGetHashCodeMethodsDialog(ITextEditor editor, ITextAnchor anchor, IClass selectedClass)
			: base(editor, anchor)
		{
			if (selectedClass == null)
				throw new ArgumentNullException("selectedClass");
			
			this.selectedClass = selectedClass;
			
			addIEquatableCheckBox.Content = string.Format(StringParser.Parse("${res:AddIns.SharpRefactoring.AddInterface}"), "IEquatable<" + selectedClass.Name + ">");
			addIEquatableCheckBox.IsEnabled = !selectedClass.BaseTypes.Any(type => {
			                                                               	if (!type.IsGenericReturnType)
			                                                               		return false;
			                                                               	var genericType = type.CastToGenericReturnType();
			                                                               	var boundTo = genericType.TypeParameter.BoundTo;
			                                                               	if (boundTo == null)
			                                                               		return false;
			                                                               	return boundTo.Name == selectedClass.Name;
			                                                               }
			                                                              );
			
			addIEquatableCheckBox.SetValueToExtension(CheckBox.IsCheckedProperty,
			                                          new OptionBinding(typeof(Options), "AddInterface")
			                                         );
		}
		
		protected override UIElement CreateContentElement()
		{
			addIEquatableCheckBox = new CheckBox();
			
			StackPanel panel = new StackPanel() {
				Orientation = Orientation.Vertical,
				Children = {
					addIEquatableCheckBox
				}
			};
			
			return panel;
		}
		
		protected override string GenerateCode(CodeGenerator generator, IClass currentClass)
		{
			throw new NotImplementedException();
		}
	}
	
	public interface IInlineRefactoring
	{
		void GenerateCode(ITextEditor editor);
	}
}
