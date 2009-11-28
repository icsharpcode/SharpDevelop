// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Reflection;
using System.Xml;

namespace WixBinding.Tests.Utils
{
	public class XmlWriterSettingsComparison
	{
		bool nullSettings;
		
		public XmlWriterSettingsComparison()
		{
		}
		
		public bool AreEqual(XmlWriterSettings lhs, XmlWriterSettings rhs)
		{
			if ((lhs == null) || (rhs == null)) {
				nullSettings = true;
				return false;
			}
			
			foreach (PropertyInfo property in typeof(XmlWriterSettings).GetProperties()) {
				object lhsValue = property.GetValue(lhs, new object[0]);
				object rhsValue = property.GetValue(rhs, new object[0]);
				
				if (property.PropertyType == typeof(bool)) {
					if ((bool)lhsValue != (bool)rhsValue) {
						PropertyName = property.Name;
						return false;
					}
				} else if (property.PropertyType == typeof(string)) {
					if ((string)lhsValue != (string)rhsValue) {
						PropertyName = property.Name;
						return false;
					}
				}
			}
			return true;
		}
		
		public string PropertyName { get; set; }
		
		public override string ToString() 
		{
			if (nullSettings) {
				return "XmlWriterSetting is null.";
			}
			
			if (String.IsNullOrEmpty(PropertyName)) {
				return String.Empty;
			}
			
			return "Property " + PropertyName + " is different.";
		}
	}
}
