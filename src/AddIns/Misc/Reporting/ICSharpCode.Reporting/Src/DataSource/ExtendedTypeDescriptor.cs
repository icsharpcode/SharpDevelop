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
using System.Collections;
using System.ComponentModel;
using System.Reflection;

namespace ICSharpCode.Reporting.DataSource
{
	/// <summary>
	/// Description of ExtendedTypeDescriptor.
	/// </summary>
	class ExtendedTypeDescriptor
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
