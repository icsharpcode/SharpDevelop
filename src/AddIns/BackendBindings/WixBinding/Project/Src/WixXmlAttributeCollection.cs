// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;

namespace ICSharpCode.WixBinding
{
	public class WixXmlAttributeCollection : CollectionBase
	{
		public WixXmlAttributeCollection()
		{
		}
		
		public WixXmlAttribute this[int index] {
			get {
				return (WixXmlAttribute)InnerList[index];
			}
		}
		
		/// <summary>
		/// Gets the attribute with the specified name.
		/// </summary>
		public WixXmlAttribute this[string name] {
			get {
				foreach (WixXmlAttribute attribute in InnerList) {
					if (attribute.Name == name) {
						return attribute;
					}
				}
				return null;
			}
		}
		
		/// <summary>
		/// Adds the attribute to the collection.
		/// </summary>
		public void Add(WixXmlAttribute attribute)
		{
			InnerList.Add(attribute);
		}
		
		public void AddRange(WixXmlAttributeCollection attributes)
		{
			foreach (WixXmlAttribute attribute in attributes) {
				Add(attribute);
			}
		}
	}
}
