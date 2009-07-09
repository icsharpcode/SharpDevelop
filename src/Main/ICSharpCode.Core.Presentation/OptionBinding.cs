// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at" />
//     <version>$Revision$</version>
// </file>

using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace ICSharpCode.Core.Presentation
{
	/// <summary>
	/// Custom binding to allow direct bindings of option properties to WPF controls.
	/// </summary>
	/// <remarks>
	/// Properties accessed by this binding have to be managed by a custom<br />
	/// settings class, which contains all settings as static properties or fields.<br />
	/// Do not use PropertyService directly!<br />
	/// This markup extension can only be used in OptionPanels or other <br />classes implementing IOptionBindingContainer!
	/// <br />
	/// Example:
	/// <code>
	/// {sd:OptionBinding addin:XmlEditorAddInOptions.ShowAttributesWhenFolded}
	/// </code>
	/// <br />
	/// Whereas 'sd' is the xml namespace of ICSharpCode.Core.Presentation.OptionBinding and 'addin'<br />
	/// is the xml namespace, in which your settings class is defined.
	/// </remarks>
	public class OptionBinding : MarkupExtension
	{
		public string PropertyName { get; set; }
		
		DependencyObject target;
		DependencyProperty dp;
		
		object propertyInfo;
		
		public OptionBinding(string propertyName)
		{
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
			
			if (this.propertyInfo is PropertyInfo)
				return (propertyInfo as PropertyInfo).GetValue(null, null);
			else {
				this.propertyInfo = t.GetField(name[1]);
				if (this.propertyInfo is FieldInfo)
					return (propertyInfo as FieldInfo).GetValue(null);
			}
			
			return null;
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
			if (propertyInfo is PropertyInfo) {
				(propertyInfo as PropertyInfo).SetValue(null, target.GetValue(dp), null);
				return true;
			}
			
			if (propertyInfo is FieldInfo) {
				(propertyInfo as FieldInfo).SetValue(null, target.GetValue(dp));
				return true;
			}
			
			return false;
		}
	}
}
