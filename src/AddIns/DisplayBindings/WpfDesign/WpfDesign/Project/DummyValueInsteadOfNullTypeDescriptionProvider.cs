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

namespace ICSharpCode.WpfDesign
{
	/// <summary>
	/// Description of DummyValueInsteadOfNullTypeDescriptionProvider.
	/// </summary>
	public sealed class DummyValueInsteadOfNullTypeDescriptionProvider : TypeDescriptionProvider
	{
		// By using a TypeDescriptionProvider, we can intercept all access to the property that is
		// using a PropertyDescriptor. WpfDesign.XamlDom uses a PropertyDescriptor for accessing
		// properties (except for attached properties), so even DesignItemProperty/XamlProperty.ValueOnInstance
		// will report null when the actual value is the dummy value.
		
		readonly string _propertyName;
		readonly object _dummyValue;
		
		/// <summary>
		/// Initializes a new instance of <see cref="DummyValueInsteadOfNullTypeDescriptionProvider"/>.
		/// </summary>
		public DummyValueInsteadOfNullTypeDescriptionProvider(TypeDescriptionProvider existingProvider,
		                                                      string propertyName, object dummyValue)
			: base(existingProvider)
		{
			this._propertyName = propertyName;
			this._dummyValue = dummyValue;
		}
		
		/// <inheritdoc/>
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
