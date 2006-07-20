//
// Help 2.0 Registration Utility
// Copyright (c) 2005 Mathias Simmack. All rights reserved.
//
namespace HtmlHelp2Registration.HelperClasses
{
	using System;
	using System.Xml;
	using System.Xml.XPath;

	public sealed class XmlHelperClass
	{
		XmlHelperClass()
		{
		}

		public static bool SetXmlStringAttributeValue(XPathNavigator parentNode, string valueName, string newValue)
		{
			if (parentNode == null || string.IsNullOrEmpty(valueName))
			{
				return false;
			}
			try
			{
				XPathNavigator factory = parentNode.Clone();
				do
				{
					factory.MoveToFirstAttribute();
					
					if (string.Compare(factory.Name, valueName) == 0)
					{
						factory.SetValue(newValue);
						return true;
					}
				}
				while (!factory.MoveToNextAttribute());

				parentNode.CreateAttribute(string.Empty,
				                           valueName,
				                           string.Empty,
				                           newValue);
				return true;
			}
			catch (ArgumentNullException)
			{
			}
			catch (InvalidOperationException)
			{
			}
			return false;
		}

		public static string GetXmlStringValue(XPathNavigator parentNode, string valueName)
		{
			if (parentNode == null || string.IsNullOrEmpty(valueName))
			{
				return string.Empty;
			}
			try
			{
				XPathNavigator node = parentNode.SelectSingleNode(valueName);
				return node.Value;
			}
			catch (NullReferenceException)
			{
			}
			return string.Empty;
		}

		public static int GetXmlIntValue(XPathNavigator parentNode, string valueName, int defaultValue)
		{
			if (parentNode == null || string.IsNullOrEmpty(valueName))
			{
				return defaultValue;
			}
			try
			{
				XPathNavigator node = parentNode.SelectSingleNode(valueName);
				return node.ValueAsInt;
			}
			catch (NullReferenceException)
			{
			}
			catch (FormatException)
			{
			}
			return defaultValue;
		}

		public static bool GetXmlBoolValue(XPathNavigator parentNode, string valueName)
		{
			return GetXmlBoolValue(parentNode, valueName, false);
		}

		public static bool GetXmlBoolValue(XPathNavigator parentNode, string valueName, bool emptyIsTrue)
		{
			if (parentNode == null || string.IsNullOrEmpty(valueName))
			{
				return false;
			}
			bool result = false;

			try
			{
				XPathNavigator node = parentNode.SelectSingleNode(valueName);
				if (emptyIsTrue)
				{
					result = (node == null || (node != null && node.Value == "yes" && string.IsNullOrEmpty(node.Value)));
				}
				else
				{
					result = (node != null && node.Value == "yes");
				}
			}
			catch (NullReferenceException)
			{
			}
			return result;
		}
	}
}
