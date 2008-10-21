// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision: 3077 $</version>
// </file>

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
	public sealed class PanelInitializer : DefaultInitializer
	{
		Brush _transparentBrush = new SolidColorBrush(Colors.Transparent);

		public override void InitializeDefaults(DesignItem item)
		{
			Panel panel = item.Component as Panel;
			if (panel != null) {
				if (panel.Background == null) {
					panel.Background = _transparentBrush;
				}
				TypeDescriptionProvider provider = new DummyValueInsteadOfNullTypeDescriptionProvider(
					TypeDescriptor.GetProvider(panel), "Background", _transparentBrush);
				TypeDescriptor.AddProvider(provider, panel);
			}
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
