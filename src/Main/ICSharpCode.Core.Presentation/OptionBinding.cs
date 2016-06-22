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
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace ICSharpCode.Core.Presentation
{
	/// <summary>
	/// Custom binding to allow direct bindings of option properties to WPF controls.
	/// </summary>
	/// <remarks>
	/// Properties accessed by this binding have to be managed by a custom
	/// settings class, which contains all settings as static properties or fields,
	/// or is a singleton class with the standard 'Instance' property.<br />
	/// Do not use PropertyService directly!<br />
	/// This markup extension can only be used in OptionPanels or other <br />containers implementing IOptionBindingContainer!
	/// </remarks>
	/// <example>
	/// <code>
	/// {sd:OptionBinding addin:XmlEditorAddInOptions.ShowAttributesWhenFolded}
	/// </code>
	/// <br />
	/// Whereas 'sd' is the xml namespace of ICSharpCode.Core.Presentation.OptionBinding and 'addin'<br />
	/// is the xml namespace, in which your settings class is defined.
	/// </example>
	public class OptionBinding : MarkupExtension
	{
		string fullPropertyName;
		
		public string FullPropertyName {
			get { return fullPropertyName; }
			set {
				if (!regex.IsMatch(value))
					throw new ArgumentException("parameter must have the following format: namespace:ClassName.FieldOrProperty", "propertyName");

				fullPropertyName = value;
			}
		}
		
		public IValueConverter Converter { get; set; }
		public object ConverterParameter { get; set; }
		public CultureInfo ConverterCulture { get; set; }
		
		static readonly Regex regex = new Regex("^.+\\:.+\\..+$", RegexOptions.Compiled);
		
		DependencyObject target;
		DependencyProperty dp;
		bool isStatic;
		Type propertyDeclaringType;
		string propertyName;
		
		MemberInfo propertyInfo;
		
		public OptionBinding(string propertyName)
		{
			if (!regex.IsMatch(propertyName))
				throw new ArgumentException("parameter must have the following format: namespace:ClassName.FieldOrProperty", "propertyName");
			
			this.FullPropertyName = propertyName;
		}
		
		public OptionBinding(Type container, string propertyName)
		{
			this.propertyDeclaringType = container;
			this.propertyName = propertyName;
		}
		
		public override object ProvideValue(IServiceProvider provider)
		{
			IProvideValueTarget service = (IProvideValueTarget)provider.GetService(typeof(IProvideValueTarget));
			
			if (service == null)
				return null;
			
			target = service.TargetObject as DependencyObject;
			dp = service.TargetProperty as DependencyProperty;
			
			if (target == null || dp == null)
				return null;
			
			if (FullPropertyName != null) {
				string[] name =  FullPropertyName.Split('.');
				IXamlTypeResolver typeResolver = provider.GetService(typeof(IXamlTypeResolver)) as IXamlTypeResolver;
				propertyDeclaringType = typeResolver.Resolve(name[0]);
				propertyName = name[1];
			}
			
			this.propertyInfo = propertyDeclaringType.GetProperty(propertyName);
			if (this.propertyInfo != null) {
				isStatic = (propertyInfo as PropertyInfo).GetGetMethod().IsStatic;
			} else {
				this.propertyInfo = propertyDeclaringType.GetField(propertyName);
				if (this.propertyInfo != null) {
					isStatic = (propertyInfo as FieldInfo).IsStatic;
				} else {
					throw new ArgumentException("Could not find property " + propertyName);
				}
			}
			
			IOptionBindingContainer container = TryFindContainer(target as FrameworkElement);
			
			if (container == null)
				throw new InvalidOperationException("This extension can be used in OptionPanels only!");
			
			container.AddBinding(this);
			
			object instance = isStatic ? null : FetchInstance(propertyDeclaringType);
			try {
				object result = null;
				
				if (this.propertyInfo is PropertyInfo) {
					result = (propertyInfo as PropertyInfo).GetValue(instance, null);
				} else {
					if (this.propertyInfo is FieldInfo)
						result = (propertyInfo as FieldInfo).GetValue(instance);
				}

				return ConvertOnDemand(result, dp.PropertyType);
			} catch (Exception e) {
				throw new Exception("Failing to convert " + this.FullPropertyName + " to " +
				                    dp.OwnerType.Name + "." + dp.Name + " (" + dp.PropertyType + ")", e);
			}
		}
		
		/// <summary>
		/// Gets the 'Instance' from a singleton type.
		/// </summary>
		static object FetchInstance(Type type)
		{
			PropertyInfo instanceProp = type.GetProperty("Instance", type);
			if (instanceProp != null)
				return instanceProp.GetValue(null, null);
			FieldInfo instanceField = type.GetField("Instance");
			if (instanceField != null)
				return instanceField.GetValue(null);
			throw new ArgumentException("Type " + type.FullName + " has no 'Instance' property. Only singletons can be used with OptionBinding.");
		}

		object ConvertOnDemand(object result, Type returnType, bool convertBack = false)
		{
			if (Converter != null) {
				if (convertBack)
					result = Converter.ConvertBack(result, returnType, ConverterParameter, ConverterCulture ?? CultureInfo.CurrentCulture);
				else
					result = Converter.Convert(result, returnType, ConverterParameter, ConverterCulture ?? CultureInfo.CurrentCulture);
			}
			if (returnType.IsInstanceOfType(result) || returnType == typeof(object))
				return result;
			
			if (returnType == typeof(string)) {
				var converter = TypeDescriptor.GetConverter(result.GetType());
				return converter.ConvertToString(result);
			}
			
			if (result is string) {
				var converter = TypeDescriptor.GetConverter(returnType);
				return converter.ConvertFromString(result as string);
			}
			
			return Convert.ChangeType(result, returnType);
		}
		
		IOptionBindingContainer TryFindContainer(DependencyObject start)
		{
			if (start == null)
				return null;
			
			while (start != null && !(start is IOptionBindingContainer))
				start = LogicalTreeHelper.GetParent(start);
			
			return start as IOptionBindingContainer;
		}
		
		public bool Save()
		{
			object value = target.GetValue(dp);
			
			Type returnType = null;
			
			if (propertyInfo is PropertyInfo)
				returnType = (propertyInfo as PropertyInfo).PropertyType;
			if (propertyInfo is FieldInfo)
				returnType = (propertyInfo as FieldInfo).FieldType;
			
			if (returnType == null)
				return false;
			
			value = ConvertOnDemand(value, returnType, true);
			
			object instance = isStatic ? null : FetchInstance(propertyDeclaringType);
			if (propertyInfo is PropertyInfo) {
				// Set only if the value is different from current value or default
				if (!(propertyInfo as PropertyInfo).GetValue(instance, null).Equals(value)) {
					(propertyInfo as PropertyInfo).SetValue(instance, value, null);
				}
				return true;
			}
			
			if (propertyInfo is FieldInfo) {
				// Set only if the value is different from current value or default
				if (!(propertyInfo as FieldInfo).GetValue(instance).Equals(value)) {
					(propertyInfo as FieldInfo).SetValue(instance, value);
				}
				return true;
			}
			
			return false;
		}
	}
}
