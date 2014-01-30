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
using System.Linq;
using System.Windows.Media;
using System.Windows.Controls;
using ICSharpCode.WpfDesign.Extensions;

namespace ICSharpCode.WpfDesign.Designer.Extensions
{
	/// <summary>
	/// Instance factory used to create Panel instances.
	/// Sets the panels Brush to a transparent brush, and modifies the panel's type descriptor so that
	/// the property value is reported as null when the transparent brush is used, and
	/// setting the Brush to null actually restores the transparent brush.
	/// </summary>
	[ExtensionFor(typeof(Panel))]
	public sealed class PanelInstanceFactory : CustomInstanceFactory
	{
		Brush _transparentBrush = new SolidColorBrush(Colors.Transparent);
		
		/// <summary>
		/// Creates an instance of the specified type, passing the specified arguments to its constructor.
		/// </summary>
		public override object CreateInstance(Type type, params object[] arguments)
		{
			object instance = base.CreateInstance(type, arguments);
			Panel panel = instance as Panel;
			if (panel != null) {
				if (panel.Background == null) {
					panel.Background = _transparentBrush;
				}
				TypeDescriptionProvider provider = new DummyValueInsteadOfNullTypeDescriptionProvider(
					TypeDescriptor.GetProvider(panel), "Background", _transparentBrush);
				TypeDescriptor.AddProvider(provider, panel);
			}
			return instance;
		}
	}
	
	[ExtensionFor(typeof(HeaderedContentControl))]
	public sealed class HeaderedContentControlInstanceFactory : CustomInstanceFactory
	{
		Brush _transparentBrush = new SolidColorBrush(Colors.Transparent);
		
		/// <summary>
		/// Creates an instance of the specified type, passing the specified arguments to its constructor.
		/// </summary>
		public override object CreateInstance(Type type, params object[] arguments)
		{
			object instance = base.CreateInstance(type, arguments);
			Control control = instance as Control;
			if (control != null) {
				if (control.Background == null) {
					control.Background = _transparentBrush;
				}
				TypeDescriptionProvider provider = new DummyValueInsteadOfNullTypeDescriptionProvider(
					TypeDescriptor.GetProvider(control), "Background", _transparentBrush);
				TypeDescriptor.AddProvider(provider, control);
			}
			return instance;
		}
	}
	
	[ExtensionFor(typeof(Border))]
	public sealed class BorderInstanceFactory : CustomInstanceFactory
	{
		Brush _transparentBrush = new SolidColorBrush(Colors.Transparent);

		/// <summary>
		/// Creates an instance of the specified type, passing the specified arguments to its constructor.
		/// </summary>
		public override object CreateInstance(Type type, params object[] arguments)
		{
			object instance = base.CreateInstance(type, arguments);
			Border panel = instance as Border;
			if (panel != null)
			{
				if (panel.Background == null)
				{
					panel.Background = _transparentBrush;
				}
				TypeDescriptionProvider provider = new DummyValueInsteadOfNullTypeDescriptionProvider(
					TypeDescriptor.GetProvider(panel), "Background", _transparentBrush);
				TypeDescriptor.AddProvider(provider, panel);
			}
			return instance;
		}
	}
	
	sealed class DummyValueInsteadOfNullTypeDescriptionProvider : TypeDescriptionProvider
	{
		// By using a TypeDescriptionProvider, we can intercept all access to the property that is
		// using a PropertyDescriptor. WpfDesign.XamlDom uses a PropertyDescriptor for accessing
		// properties (except for attached properties), so even DesignItemProperty/XamlProperty.ValueOnInstance
		// will report null when the actual value is the dummy value.
		
		readonly string _propertyName;
		readonly object _dummyValue;
		
		public DummyValueInsteadOfNullTypeDescriptionProvider(TypeDescriptionProvider existingProvider,
		                                                      string propertyName, object dummyValue)
			: base(existingProvider)
		{
			this._propertyName = propertyName;
			this._dummyValue = dummyValue;
		}
		
		public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance)
		{
			return new ShadowTypeDescriptor(this, base.GetTypeDescriptor(objectType, instance));
		}
		
		sealed class ShadowTypeDescriptor : CustomTypeDescriptor
		{
			readonly DummyValueInsteadOfNullTypeDescriptionProvider _parent;
			
			public ShadowTypeDescriptor(DummyValueInsteadOfNullTypeDescriptionProvider parent,
			                            ICustomTypeDescriptor existingDescriptor)
				: base(existingDescriptor)
			{
				this._parent = parent;
			}
			
			public override PropertyDescriptorCollection GetProperties()
			{
				return Filter(base.GetProperties());
			}
			
			public override PropertyDescriptorCollection GetProperties(Attribute[] attributes)
			{
				return Filter(base.GetProperties(attributes));
			}
			
			PropertyDescriptorCollection Filter(PropertyDescriptorCollection properties)
			{
				PropertyDescriptor property = properties[_parent._propertyName];
				if (property != null) {
					if ((properties as System.Collections.IDictionary).IsReadOnly) {
						properties = new PropertyDescriptorCollection(properties.Cast<PropertyDescriptor>().ToArray());
					}
					properties.Remove(property);
					properties.Add(new ShadowPropertyDescriptor(_parent, property));
				}
				return properties;
			}
		}
		
		sealed class ShadowPropertyDescriptor : PropertyDescriptor
		{
			readonly DummyValueInsteadOfNullTypeDescriptionProvider _parent;
			readonly PropertyDescriptor _baseDescriptor;
			
			public ShadowPropertyDescriptor(DummyValueInsteadOfNullTypeDescriptionProvider parent,
			                                PropertyDescriptor existingDescriptor)
				: base(existingDescriptor)
			{
				this._parent = parent;
				this._baseDescriptor = existingDescriptor;
			}
			
			public override Type ComponentType {
				get { return _baseDescriptor.ComponentType; }
			}
			
			public override bool IsReadOnly {
				get { return _baseDescriptor.IsReadOnly; }
			}
			
			public override Type PropertyType {
				get { return _baseDescriptor.PropertyType; }
			}
			
			public override bool CanResetValue(object component)
			{
				return _baseDescriptor.CanResetValue(component);
			}
			
			public override object GetValue(object component)
			{
				object value = _baseDescriptor.GetValue(component);
				if (value == _parent._dummyValue)
					return null;
				else
					return value;
			}
			
			public override void ResetValue(object component)
			{
				_baseDescriptor.SetValue(component, _parent._dummyValue);
			}
			
			public override void SetValue(object component, object value)
			{
				_baseDescriptor.SetValue(component, value ?? _parent._dummyValue);
			}
			
			public override bool ShouldSerializeValue(object component)
			{
				return _baseDescriptor.ShouldSerializeValue(component)
					&& _baseDescriptor.GetValue(component) != _parent._dummyValue;
			}
		}
	}
}
