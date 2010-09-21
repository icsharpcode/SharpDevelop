// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.ComponentModel;
using System.Reflection;

namespace ICSharpCode.Reports.Core {
	
	public class ExtendedTypeDescriptor
	{
		private static Hashtable collections = new Hashtable();

		private static bool IsAllowedProperty(string name)
		{
			return true; // alle erlaubt
		}


		public static PropertyDescriptorCollection GetProperties(Type memberType)
		{
			if (memberType == null)
				return PropertyDescriptorCollection.Empty;

			PropertyDescriptorCollection pdc;
			if ((pdc = (PropertyDescriptorCollection) collections[memberType]) != null)
				return (pdc);
 
			PropertyInfo[] allProps = memberType.GetProperties();
			int l = allProps.Length;
			for (int i = 0; i < allProps.Length; i++)
			{
				PropertyInfo pi = allProps[i];
				if (!IsAllowedProperty(pi.Name))
				{
					allProps[i] = null;
					l--;
				}
			}

			PropertyDescriptor[] descriptors = new PropertyDescriptor[l];
			
			int j = 0;
			foreach(PropertyInfo pinfo in allProps)
			{
				if (pinfo != null)
				{
					descriptors[j++] = new ExtendedPropertyDescriptor(pinfo.Name, memberType, pinfo.PropertyType);
				}		
			}								 
			PropertyDescriptorCollection result = new PropertyDescriptorCollection(descriptors);
			collections.Add(memberType, result);
			return result;			
		}
	}
}
