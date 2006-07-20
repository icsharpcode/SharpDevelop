//
// Help 2.0 Registration Utility
// Copyright (c) 2005 Mathias Simmack. All rights reserved.
//
namespace HtmlHelp2Registration.ItemClasses
{
	using System;
	using System.Globalization;
	using System.Collections.Generic;
	using System.Xml;
	using System.Xml.XPath;
	using HtmlHelp2Registration.HelperClasses;

	public class PluginParentItem
	{
		private string realName;
		private string matchingName;
		private bool merge;
		private List<PluginChildItem> children = new List<PluginChildItem>();

		public PluginParentItem(XPathNavigator pluginRootnode)
		{
			if (pluginRootnode == null)
			{
				throw new ArgumentNullException("pluginRootnode");
			}
			this.realName = XmlHelperClass.GetXmlStringValue(pluginRootnode, "@parent");
			this.matchingName = PluginSearch.GetFirstMatchingNamespaceName(this.realName);
			this.merge = XmlHelperClass.GetXmlBoolValue(pluginRootnode, "@merge", true);
			string childName = XmlHelperClass.GetXmlStringValue(pluginRootnode, "@child");

			if (!string.IsNullOrEmpty(childName))
			{
				this.children.Add(new PluginChildItem(childName));
			}
			else
			{
				XPathNodeIterator pChild =
					pluginRootnode.SelectChildren("child", ApplicationHelpers.Help2NamespaceUri);
				while (pChild.MoveNext())
				{
					this.children.Add(new PluginChildItem(pChild.Current));
				}
			}
		}

		#region Properties
		public string Name
		{
			get { return this.realName; }
		}

		public string MatchingName
		{
			get { return this.matchingName; }
		}

		public bool Merge
		{
			get { return this.merge; }
		}
		#endregion

		public System.Collections.IEnumerator GetEnumerator()
		{
			foreach (PluginChildItem child in this.children)
			{
				yield return child;
			}
		}
	}

	public class PluginChildItem
	{
		private string realName;
		private string matchingName;

		public PluginChildItem(XPathNavigator pluginRootnode)
		{
			if (pluginRootnode == null)
			{
				throw new ArgumentNullException("pluginRootnode");
			}
			this.realName = XmlHelperClass.GetXmlStringValue(pluginRootnode, "@name");
			this.matchingName = PluginSearch.GetFirstMatchingNamespaceName(this.realName);
		}

		public PluginChildItem(string childName)
		{
			if (string.IsNullOrEmpty(childName))
			{
				throw new ArgumentNullException("childName");
			}
			this.realName = childName;
			this.matchingName = PluginSearch.GetFirstMatchingNamespaceName(this.realName);
		}

		#region Properties
		public string Name
		{
			get { return this.realName; }
		}

		public string MatchingName
		{
			get { return this.matchingName; }
		}
		#endregion

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "[Help 2.0 Plug-in; {0}]", this.matchingName);
		}
	}
}
