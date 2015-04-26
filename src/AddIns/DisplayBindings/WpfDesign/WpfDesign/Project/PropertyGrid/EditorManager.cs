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
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace ICSharpCode.WpfDesign.PropertyGrid
{
	/// <summary>
	/// Manages registered type and property editors.
	/// </summary>
	public static class EditorManager
	{
		// property return type => editor type
		static Dictionary<Type, Type> typeEditors = new Dictionary<Type, Type>();
		// property full name => editor type
		static Dictionary<string, Type> propertyEditors = new Dictionary<string, Type>();

		static Type defaultComboboxEditor;
		
		static Type defaultTextboxEditor;
		
		/// <summary>
		/// Creates a property editor for the specified <paramref name="property"/>
		/// </summary>
		public static FrameworkElement CreateEditor(DesignItemProperty property)
		{
			Type editorType;
			if (!propertyEditors.TryGetValue(property.FullName, out editorType)) {
				var type = property.ReturnType;
				while (type != null) {
					if (typeEditors.TryGetValue(type, out editorType)) {
						break;
					}
					type = type.BaseType;
				}
				
				foreach (var t in typeEditors) {
					if (t.Key.IsAssignableFrom(property.ReturnType)) {
						return (FrameworkElement)Activator.CreateInstance(t.Value);
					}
				}
				
				if (editorType == null) {
					var standardValues = Metadata.GetStandardValues(property.ReturnType);
					if (standardValues != null) {
						var itemsControl = (ItemsControl)Activator.CreateInstance(defaultComboboxEditor);
						itemsControl.ItemsSource = standardValues;
						if (Nullable.GetUnderlyingType(property.ReturnType) != null) {
							itemsControl.GetType().GetProperty("IsNullable").SetValue(itemsControl, true, null); //In this Class we don't know the Nullable Combo Box
						}
						return itemsControl;
					}
					return (FrameworkElement)Activator.CreateInstance(defaultTextboxEditor);
				}
			}
			return (FrameworkElement)Activator.CreateInstance(editorType);
		}
		
		/// <summary>
		/// Registers the Textbox Editor.
		/// </summary>
		public static void SetDefaultTextBoxEditorType(Type type)
		{
			defaultTextboxEditor = type;
		}
		
		/// <summary>
		/// Registers the Combobox Editor.
		/// </summary>
		public static void SetDefaultComboBoxEditorType(Type type)
		{
			defaultComboboxEditor = type;
		}
		
		/// <summary>
		/// Registers property editors defined in the specified assembly.
		/// </summary>
		public static void RegisterAssembly(Assembly assembly)
		{
			if (assembly == null)
				throw new ArgumentNullException("assembly");
			
			foreach (Type type in assembly.GetExportedTypes()) {
				foreach (TypeEditorAttribute editorAttribute in type.GetCustomAttributes(typeof(TypeEditorAttribute), false)) {
					CheckValidEditor(type);
					typeEditors[editorAttribute.SupportedPropertyType] = type;
				}
				foreach (PropertyEditorAttribute editorAttribute in type.GetCustomAttributes(typeof(PropertyEditorAttribute), false)) {
					CheckValidEditor(type);
					string propertyName = editorAttribute.PropertyDeclaringType.FullName + "." + editorAttribute.PropertyName;
					propertyEditors[propertyName] = type;
				}
			}
		}
		
		static void CheckValidEditor(Type type)
		{
			if (!typeof(FrameworkElement).IsAssignableFrom(type)) {
				throw new DesignerException("Editor types must derive from FrameworkElement!");
			}
		}
	}
}
