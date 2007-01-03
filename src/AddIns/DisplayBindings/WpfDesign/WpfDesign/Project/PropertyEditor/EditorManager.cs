// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;

namespace ICSharpCode.WpfDesign.PropertyEditor
{
	/// <summary>
	/// Manages registered type and property editors.
	/// </summary>
	public sealed class EditorManager
	{
		// property return type => editor type
		Dictionary<Type, Type> _typeEditors = new Dictionary<Type, Type>();
		// property full name => editor type
		Dictionary<string, Type> _propertyEditors = new Dictionary<string, Type>();
		
		/// <summary>
		/// Creates an editor for the specified property.
		/// </summary>
		public UIElement CreateEditor(IPropertyEditorDataProperty property)
		{
			try {
				return (UIElement)Activator.CreateInstance(GetEditorType(property), property);
			} catch (Exception ex) {
				Debug.WriteLine(ex.ToString());
				throw;
			}
		}
		
		/// <summary>
		/// Creates the fallback editor for the specified property.
		/// </summary>
		public static UIElement CreateFallbackEditor(IPropertyEditorDataProperty property)
		{
			return (UIElement)Activator.CreateInstance(GetFallbackEditorType(property), property);
		}
		
		/// <summary>
		/// Gets the type of the editor that can edit the specified property.
		/// </summary>
		public Type GetEditorType(IPropertyEditorDataProperty property)
		{
			if (property == null)
				throw new ArgumentNullException("property");
			
			Type editorType;
			if (_propertyEditors.TryGetValue(property.DeclaringType.FullName + "." + property.Name, out editorType))
				return editorType;
			else if (_typeEditors.TryGetValue(property.ReturnType, out editorType))
				return editorType;
			else
				return GetFallbackEditorType(property);
		}
		
		/// <summary>
		/// Gets the type of the fallback editor used for the specified property.
		/// </summary>
		public static Type GetFallbackEditorType(IPropertyEditorDataProperty property)
		{
			if (property == null)
				throw new ArgumentNullException("property");
			
			Type returnType = property.ReturnType;
			if (returnType.IsEnum) {
				return typeof(StandardValuesComboBoxEditor);
			} else if (returnType == typeof(bool)) {
				return typeof(BooleanEditor);
			} else {
				TypeConverter c = property.TypeConverter;
				if (c != null && c.CanConvertFrom(typeof(string)) && c.CanConvertTo(typeof(string)))
					return typeof(TextBoxEditor);
				else
					return typeof(FallbackEditor);
			}
		}
		
		/// <summary>
		/// Registers property editors defined in the specified assembly.
		/// </summary>
		public void RegisterAssembly(Assembly assembly)
		{
			if (assembly == null)
				throw new ArgumentNullException("assembly");
			
			foreach (Type type in assembly.GetExportedTypes()) {
				foreach (TypeEditorAttribute editorAttribute in type.GetCustomAttributes(typeof(TypeEditorAttribute), false)) {
					CheckValidEditor(type);
					_typeEditors[editorAttribute.SupportedPropertyType] = type;
				}
				foreach (PropertyEditorAttribute editorAttribute in type.GetCustomAttributes(typeof(PropertyEditorAttribute), false)) {
					CheckValidEditor(type);
					string propertyName = editorAttribute.PropertyDeclaringType.FullName + "." + editorAttribute.PropertyName;
					_propertyEditors[propertyName] = type;
				}
			}
		}
		
		static readonly Type[] typeArrayWithPropertyEditorDataProperty = { typeof(IPropertyEditorDataProperty) };
		
		static void CheckValidEditor(Type type)
		{
			if (!typeof(UIElement).IsAssignableFrom(type)) {
				throw new DesignerException("Editor types must derive from UIElement!");
			}
			if (type.GetConstructor(typeArrayWithPropertyEditorDataProperty) == null) {
				throw new DesignerException("Editor types must have a constructor that takes a IPropertyEditorDataProperty as argument!");
			}
		}
	}
}
