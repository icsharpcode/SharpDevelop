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

// 

using System;
using System.ComponentModel;

namespace ICSharpCode.SharpDevelop.Gui
{
	/// <summary>
	/// GlobalizedObject implements ICustomTypeDescriptor to enable
	/// required functionality to describe a type (class).<br></br>
	/// The main task of this class is to instantiate our own property descriptor
	/// of type GlobalizedPropertyDescriptor.
	/// </summary>
	public class LocalizedObject : ICustomTypeDescriptor
	{
		PropertyDescriptorCollection globalizedProps;
		
		string ICustomTypeDescriptor.GetClassName()
		{
			return TypeDescriptor.GetClassName(this,true);
		}

		AttributeCollection ICustomTypeDescriptor.GetAttributes()
		{
			return TypeDescriptor.GetAttributes(this,true);
		}

		string ICustomTypeDescriptor.GetComponentName()
		{
			return TypeDescriptor.GetComponentName(this, true);
		}

		TypeConverter ICustomTypeDescriptor.GetConverter()
		{
			return TypeDescriptor.GetConverter(this, true);
		}

		EventDescriptor ICustomTypeDescriptor.GetDefaultEvent()
		{
			return TypeDescriptor.GetDefaultEvent(this, true);
		}

		PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty()
		{
			return TypeDescriptor.GetDefaultProperty(this, true);
		}

		object ICustomTypeDescriptor.GetEditor(Type editorBaseType)
		{
			return TypeDescriptor.GetEditor(this, editorBaseType, true);
		}

		EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes)
		{
			return TypeDescriptor.GetEvents(this, attributes, true);
		}

		EventDescriptorCollection ICustomTypeDescriptor.GetEvents()
		{
			return TypeDescriptor.GetEvents(this, true);
		}

		/// <summary>
		/// Causes the list of property descriptors to be recreated.
		/// </summary>
		protected void ReFilterProperties()
		{
			globalizedProps = null;
			if (SD.Workbench != null) {
				SD.MainThread.InvokeAsyncAndForget(delegate {
					PropertyPad.RefreshItem(this);
				});
			}
		}
		
		protected virtual void FilterProperties(PropertyDescriptorCollection globalizedProps)
		{
		}
		
		/// <summary>
		/// Called to get the properties of a type.
		/// </summary>
		/// <param name="attributes"></param>
		/// <returns></returns>
		PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes)
		{
			if ( globalizedProps == null) {
				// Get the collection of properties
				PropertyDescriptorCollection baseProps = TypeDescriptor.GetProperties(this, attributes, true);

				globalizedProps = new PropertyDescriptorCollection(null);

				// For each property use a property descriptor of our own that is able to be globalized
				foreach (PropertyDescriptor oProp in baseProps) {
					globalizedProps.Add(new LocalizedPropertyDescriptor(oProp));
				}
				FilterProperties(globalizedProps);
			}
			return globalizedProps;
		}

		PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties()
		{
			// Only do once
			if (globalizedProps == null) {
				// Get the collection of properties
				PropertyDescriptorCollection baseProps = TypeDescriptor.GetProperties(this, true);
				globalizedProps = new PropertyDescriptorCollection(null);

				// For each property use a property descriptor of our own that is able to be globalized
				foreach (PropertyDescriptor oProp in baseProps) {
					globalizedProps.Add(new LocalizedPropertyDescriptor(oProp));
				}
				FilterProperties(globalizedProps);
			}
			return globalizedProps;
		}

		object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd)
		{
			return this;
		}
		
		protected internal virtual void InformSetValue(PropertyDescriptor propertyDescriptor, object value)
		{
			
		}
	}
}
