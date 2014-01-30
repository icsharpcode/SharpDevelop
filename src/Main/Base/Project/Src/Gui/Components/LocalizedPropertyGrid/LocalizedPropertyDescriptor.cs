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
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Gui
{
	/// <summary>
	/// LocalizedPropertyDescriptor enhances the base class bay obtaining the display name for a property
	/// from the resource.
	/// </summary>
	public class LocalizedPropertyDescriptor : PropertyDescriptor
	{
		PropertyDescriptor basePropertyDescriptor;
		
		string localizedName        = String.Empty;
		string localizedDescription = String.Empty;
		string localizedCategory    = String.Empty;
		
		TypeConverter customTypeConverter = null;
		
		public override bool IsReadOnly {
			get {
				return this.basePropertyDescriptor.IsReadOnly;
			}
		}

		public override string Name {
			get {
				return this.basePropertyDescriptor.Name;
			}
		}

		public override Type PropertyType {
			get {
				return this.basePropertyDescriptor.PropertyType;
			}
		}
		
		public override Type ComponentType {
			get {
				return basePropertyDescriptor.ComponentType;
			}
		}
		
		public override string DisplayName {
			get  {
				return StringParser.Parse(localizedName);
			}
		}
		
		public override string Description {
			get {
				return StringParser.Parse(localizedDescription);
			}
		}
		
		public override string Category {
			get {
				return StringParser.Parse(localizedCategory);
			}
		}
		
		public override TypeConverter Converter {
			get {
				if (customTypeConverter != null) {
					return customTypeConverter;
				}
				return base.Converter;
			}
		}
		
		public LocalizedPropertyDescriptor(PropertyDescriptor basePropertyDescriptor) : base(basePropertyDescriptor)
		{
			LocalizedPropertyAttribute localizedPropertyAttribute = null;
			
			foreach (Attribute attr in basePropertyDescriptor.Attributes) {
				localizedPropertyAttribute = attr as LocalizedPropertyAttribute;
				if (localizedPropertyAttribute != null) {
					break;
				}
			}
			
			if (localizedPropertyAttribute != null) {
				localizedName        = localizedPropertyAttribute.Name;
				localizedDescription = localizedPropertyAttribute.Description;
				localizedCategory    = localizedPropertyAttribute.Category;
			} else {
				localizedName        = basePropertyDescriptor.Name;
				localizedDescription = basePropertyDescriptor.Description;
				localizedCategory    = basePropertyDescriptor.Category;
			}
			
			this.basePropertyDescriptor = basePropertyDescriptor;
			
			// "Booleans" get a localized type converter
			if (basePropertyDescriptor.PropertyType == typeof(System.Boolean)) {
				customTypeConverter = new BooleanTypeConverter();
			}
		}

		public override bool CanResetValue(object component)
		{
			return basePropertyDescriptor.CanResetValue(component);
		}
		
		public override object GetValue(object component)
		{
			return this.basePropertyDescriptor.GetValue(component);
		}
		
		public override void ResetValue(object component)
		{
			this.basePropertyDescriptor.ResetValue(component);
			if (component is LocalizedObject) {
				((LocalizedObject)component).InformSetValue(this, null);
			}
		}

		public override bool ShouldSerializeValue(object component)
		{
			return this.basePropertyDescriptor.ShouldSerializeValue(component);
		}
		
		public override void SetValue(object component, object value)
		{
			if (this.customTypeConverter != null && value.GetType() != PropertyType) {
				this.basePropertyDescriptor.SetValue(component, this.customTypeConverter.ConvertFrom(value));
			} else {
				this.basePropertyDescriptor.SetValue(component, value);
			}
			if (component is LocalizedObject) {
				((LocalizedObject)component).InformSetValue(this, value);
			}
		}
	}
}
