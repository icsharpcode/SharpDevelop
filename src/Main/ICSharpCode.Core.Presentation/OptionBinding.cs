// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at" />
//     <version>$Revision$</version>
// </file>

using System;
using System.Linq;
using System.ComponentModel;
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
	/// settings class, which contains all settings as static properties or fields.<br />
	/// Do not use PropertyService directly!<br />
	/// This markup extension can only be used in OptionPanels or other <br />classes implementing IOptionBindingContainer!
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
		public string PropertyName { get; set; }
		
		static readonly Regex regex = new Regex("^.+\\:.+\\..+$", RegexOptions.Compiled);
		
		DependencyObject target;
		DependencyProperty dp;
		
		object propertyInfo;
		
		public OptionBinding(string propertyName)
		{
			if (!regex.IsMatch(propertyName))
				throw new ArgumentException("parameter must have the following format: namespace:ClassName.FieldOrProperty", "propertyName");
			
			this.PropertyName = propertyName;
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
			
			string[] name =  PropertyName.Split('.');
			IXamlTypeResolver typeResolver = provider.GetService(typeof(IXamlTypeResolver)) as IXamlTypeResolver;
			Type t = typeResolver.Resolve(name[0]);
			
			this.propertyInfo = t.GetProperty(name[1]);
			
			IOptionBindingContainer container = TryFindContainer(target as FrameworkElement);
			
			if (container == null)
				throw new InvalidOperationException("This extension can only be used in OptionPanels");
			
			container.AddBinding(this);
			
			object result = null;
			
			if (this.propertyInfo is PropertyInfo)
				result = (propertyInfo as PropertyInfo).GetValue(null, null);
			else {
				this.propertyInfo = t.GetField(name[1]);
				if (this.propertyInfo is FieldInfo)
					result = (propertyInfo as FieldInfo).GetValue(null);
			}
			
			Type returnType = dp.PropertyType;
			
			if (dp.PropertyType.IsGenericType && dp.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>)) {
				returnType = dp.PropertyType.GetGenericArguments().First();
			}
			
			return Convert.ChangeType(result, returnType);
		}
		
		IOptionBindingContainer TryFindContainer(FrameworkElement start)
		{
			if (start == null)
				return null;
			
			while (start != null && !(start is IOptionBindingContainer))
				start = start.Parent as FrameworkElement;
			
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
			
			value = Convert.ChangeType(value, returnType);
			
			if (propertyInfo is PropertyInfo) {
				(propertyInfo as PropertyInfo).SetValue(null, value, null);
				return true;
			}
			
			if (propertyInfo is FieldInfo) {
				(propertyInfo as FieldInfo).SetValue(null, value);
				return true;
			}
			
			return false;
		}
	}
}
