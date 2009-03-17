// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace ICSharpCode.PythonBinding
{
	/// <summary>
	/// Contains the default values for all properties on a Control, Form or UserControl.
	/// </summary>
	public class PythonControlDefaultPropertyValues
	{
		Dictionary<string, object> defaultPropertyValues = new Dictionary<string, object>();
		
		public PythonControlDefaultPropertyValues()
		{
			defaultPropertyValues.Add("Text", String.Empty);
			defaultPropertyValues.Add("AutoValidate", AutoValidate.EnablePreventFocusChange);
			defaultPropertyValues.Add("Enabled", true);
			defaultPropertyValues.Add("Visible", true);
			defaultPropertyValues.Add("AutoScaleMode",  AutoScaleMode.Inherit);
			defaultPropertyValues.Add("DoubleBuffered", false);
			defaultPropertyValues.Add("ImeMode", ImeMode.NoControl);
			defaultPropertyValues.Add("RightToLeft", RightToLeft.No);
			defaultPropertyValues.Add("Cursor", Cursors.Default);
			defaultPropertyValues.Add("MinimumSize", new Size(0, 0));
			defaultPropertyValues.Add("AutoScrollMinSize", new Size(0, 0));
			defaultPropertyValues.Add("AutoScrollMargin", new Size(0, 0));
			defaultPropertyValues.Add("Location", new Point(0, 0));
			defaultPropertyValues.Add("Padding", Padding.Empty);
		}
		
		/// <summary>
		/// Determines if the property value has its default value.
		/// </summary>
		/// <param name="propertyName">The name of the property.</param>
		/// <param name="obj">The object that has the property.</param>
		/// <returns>False if the property does not exist.</returns>
		public bool IsDefaultValue(string propertyName, object obj)
		{
			PropertyInfo propertyInfo = obj.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
			if (propertyInfo != null) {
				object propertyValue = propertyInfo.GetValue(obj, null);
				return IsDefaultValue(propertyInfo, propertyValue);
			}
			return false;
		}

		/// <summary>
		/// Determines if the property value is the default value by checking the DefaultValueAttribute.
		/// </summary>
		/// <param name="propertyDescriptor">The property descriptor for the property.</param>
		/// <param name="obj">The object that has the property.</param>
		/// <remarks>
		/// For some properties such as Form.AutoValidate there is no default value specified by the
		/// DefaultValueAttribute.
		/// </remarks>
		public bool IsDefaultValue(PropertyDescriptor propertyDescriptor, object obj)
		{
			PropertyInfo propertyInfo = obj.GetType().GetProperty(propertyDescriptor.Name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
			if (propertyInfo != null) {
				return IsDefaultValue(propertyInfo, propertyDescriptor.GetValue(obj));
			}
			return false;
		}
		
		/// <summary>
		/// Determines if the property value is the default value by checking the DefaultValueAttribute.
		/// </summary>
		/// <remarks>
		/// For some properties such as Form.AutoValidate there is no default value specified by the
		/// DefaultValueAttribute.
		/// </remarks>
		public bool IsDefaultValue(PropertyInfo propertyInfo, object propertyValue)
		{
			// Check default attribute.
			DefaultValueAttribute defaultValueAttribute = (DefaultValueAttribute)Attribute.GetCustomAttribute(propertyInfo, typeof(DefaultValueAttribute), true);
			if (defaultValueAttribute != null) {
				if (defaultValueAttribute.Value != null) {
					return defaultValueAttribute.Value.Equals(propertyValue);
				}
				return defaultValueAttribute.Value == propertyValue;
			}
			
			object defaultPropertyValue = null;
			if (defaultPropertyValues.TryGetValue(propertyInfo.Name, out defaultPropertyValue)) {
			    return defaultPropertyValue.Equals(propertyValue);
			}

			if (propertyInfo.Name == "BackColor") {
				// Default is Control.DefaultBackColor
				return true;
			} else if (propertyInfo.Name == "Icon") {
				return true;	
			} else if (propertyInfo.Name == "TransparencyKey") {
				return true;	
			} else if (propertyInfo.Name == "Font") {
				// Default is Control.DefaultFont
				return true;	
			} else if (propertyInfo.Name == "ForeColor") {
				// Default is Control.DefaultForeColor
				return true;	
			}
			return false;
		}
	}
}
