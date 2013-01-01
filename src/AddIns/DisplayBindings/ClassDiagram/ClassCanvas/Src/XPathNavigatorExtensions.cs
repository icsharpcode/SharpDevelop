// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Xml.XPath;

namespace ClassDiagram
{
	public static class XPathNavigatorExtensions
	{
		public static bool GetBooleanAttribute(this XPathNavigator navigator, string name, string namespaceUri = "")
		{
			string attributeValue = navigator.GetAttribute(name, namespaceUri);
			bool result = false;
			if (bool.TryParse(attributeValue, out result)) {
				return true;
			}
			return false;
		}
	}
}
