/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Peter Forstmeier
 * Datum: 04.12.2007
 * Zeit: 08:56
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */

using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;

namespace ICSharpCode.Reports.Addin
{
	/// <summary>
	/// Description of DesignerHelper.
	/// </summary>
	public sealed class DesignerHelper
	{
		
		private DesignerHelper()
		{
		}
		
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
		
		public static void AddTextbasedProperties (List<PropertyDescriptor> allProperties,
		                                          PropertyDescriptorCollection props)
		{
			PropertyDescriptor prop = prop = props.Find("Font",true);
			allProperties.Add(prop);
			
			prop = props.Find("FormatString",true);
			allProperties.Add(prop);
			
			prop = props.Find("StringTrimming",true);
			allProperties.Add(prop);
			
			prop = props.Find("ContentAlignment",true);
			allProperties.Add(prop);
			
			prop = props.Find("CanGrow",true);
			allProperties.Add(prop);
			
			prop = props.Find("CanShrink",true);
			allProperties.Add(prop);
			
			prop = props.Find("DataType",true);
			allProperties.Add(prop);
		}
	}
}
