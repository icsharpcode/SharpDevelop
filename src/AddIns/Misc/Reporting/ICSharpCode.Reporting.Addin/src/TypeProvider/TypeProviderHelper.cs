/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 17.03.2014
 * Time: 20:20
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace ICSharpCode.Reporting.Addin.TypeProvider
{
	/// <summary>
	/// Description of TypeProviderHelper.
	/// </summary>
	static class TypeProviderHelper
	{
	
		public static void RemoveProperties (IDictionary properties)
		{
			if (properties == null){
				throw new ArgumentNullException("properties");
			}
			properties.Remove("AccessibleDescription");
			properties.Remove("AccessibleName");
			properties.Remove("AccessibleRole");
			properties.Remove("AllowDrop");
			properties.Remove("Anchor");
			properties.Remove("AutoScroll");
			properties.Remove("AutoSize");
			properties.Remove("BackgroundImage");
			properties.Remove("BackgroundImageLayout");
			properties.Remove("Cursor");
			properties.Remove("CausesValidation");
			properties.Remove("ContextMenuStrip");
			properties.Remove("DataBindings");
			properties.Remove("Dock");
			
			properties.Remove("Enabled");
			
			properties.Remove("ImeMode");
			properties.Remove("Locked");
			properties.Remove("Padding");
			properties.Remove("RightToLeft");
			properties.Remove("TabIndex");
			properties.Remove("TabStop");
			properties.Remove("Tag");
			properties.Remove("UseWaitCursor");
			properties.Remove("Visible");
		}
		
		public static void Remove (IDictionary properties,string[] toRemove)
		{
			if (properties == null){
				throw new ArgumentNullException("properties");
			}
			if (toRemove == null) {
				throw new ArgumentNullException("toRemove");
			}
			foreach (String str in toRemove)
			{
				properties.Remove(str);
			}
		}
		
		public static void AddDefaultProperties (List<PropertyDescriptor> allProperties,
		                                         PropertyDescriptorCollection props )
		{
			PropertyDescriptor prop = props.Find("Location",true);
			allProperties.Add(prop);
			
			prop = props.Find("Size",true);
			allProperties.Add(prop);
			
			prop = props.Find("BackColor",true);
			allProperties.Add(prop);
			

			// need this for Contextmenu's
			prop = props.Find("ContextMenu",true);
			allProperties.Add(prop);
		}
		
		public static void AddTextBasedProperties (List<PropertyDescriptor> allProperties,
		                                          PropertyDescriptorCollection props)
		{
			PropertyDescriptor prop = props.Find("Font",true);
			allProperties.Add(prop);
			
			prop = props.Find("FormatString",true);
			allProperties.Add(prop);
			
			prop = props.Find("StringTrimming",true);
			allProperties.Add(prop);
			
			prop = props.Find("ContentAlignment",true);
			allProperties.Add(prop);
			
			prop = props.Find("TextAlignment",true);
			allProperties.Add(prop);
			
			prop = props.Find("CanGrow",true);
			allProperties.Add(prop);
			
			prop = props.Find("CanShrink",true);
			allProperties.Add(prop);
			
			prop = props.Find("DataType",true);
			allProperties.Add(prop);
		}
		
		public static void AddGraphicProperties (List<PropertyDescriptor> allProperties,
		                                         PropertyDescriptorCollection props)
		{
			PropertyDescriptor prop = null;
			prop = props.Find("ForeColor",true);
			allProperties.Add(prop);
			
			prop = props.Find("DashStyle",true);
			allProperties.Add(prop);
			
			prop = props.Find("Thickness",true);
			allProperties.Add(prop);
		}
	}
}
