//
// Help 2.0 Registration Utility
// Copyright (c) 2005 Mathias Simmack. All rights reserved.
//
namespace HtmlHelp2Registration.ItemClasses
{
	using System;
	using System.Collections.Generic;
	using System.Globalization;
	using System.IO;
	using System.Xml;
	using System.Xml.XPath;
	using HtmlHelp2Registration.HelperClasses;

	public class PluginsItemClass : ICommonItemClass
	{
		private List<PluginParentItem> plugins = new List<PluginParentItem>();
		private XmlDocument xmldoc = new XmlDocument();
		private XmlNamespaceManager xmlns;
		private string xmlFilename = string.Empty;
		private string xmlXpathSequence = string.Empty;

		#region Constructor
		public PluginsItemClass(string xmlFilename) : this(xmlFilename, string.Empty)
		{
		}

		public PluginsItemClass(string xmlFilename, string xmlXpathSequence)
		{
			this.xmlFilename = xmlFilename;
			this.xmlXpathSequence = xmlXpathSequence;
			this.Initialize();
		}
		#endregion

		void Initialize()
		{
			this.xmlns = new XmlNamespaceManager(this.xmldoc.NameTable);
			this.xmlns.AddNamespace("help2", ApplicationHelpers.Help2NamespaceUri);
			
			this.xmldoc.Load(this.xmlFilename);
			XmlNodeList nodes = this.xmldoc.SelectNodes
				(string.Format(CultureInfo.InvariantCulture, "/help2:register/help2:plugin{0}", this.xmlXpathSequence), this.xmlns);

			foreach (XmlNode node in nodes)
			{
				this.plugins.Add(new PluginParentItem(node.CreateNavigator()));
			}
		}

		public void Register()
		{
			foreach (PluginParentItem plugin in this.plugins)
			{
				if (string.IsNullOrEmpty(plugin.MatchingName))
				{
					continue;
				}

				using (Help2RegisterClass register = new Help2RegisterClass())
				{
					foreach (PluginChildItem child in plugin)
					{
						if (string.IsNullOrEmpty(child.MatchingName))
						{
							continue;
						}

						OnRegisterOrRemovePlugin(new PluginRegisterEventArgs(plugin.MatchingName, child.MatchingName, true));
						register.RegisterPlugin(plugin.MatchingName, child.MatchingName);

						if (string.Compare(plugin.Name, plugin.MatchingName) != 0)
						{
							this.PatchParentXmlNode(plugin.Name, plugin.MatchingName);
						}
						if (string.Compare(child.Name, child.MatchingName) != 0)
						{
							this.PatchChildXmlNode(plugin.Name, child.Name, child.MatchingName);
						}
					}

					if (plugin.Merge)
					{
						OnNamespaceMerge(new MergeNamespaceEventArgs(plugin.MatchingName));
						MergeNamespace.CallMerge(plugin.MatchingName);
					}
				}
			}
		}

		public void Unregister()
		{
			foreach (PluginParentItem plugin in this.plugins)
			{
				if (string.IsNullOrEmpty(plugin.MatchingName))
				{
					continue;
				}

				using (Help2RegisterClass register = new Help2RegisterClass())
				{
					foreach (PluginChildItem child in plugin)
					{
						if (string.IsNullOrEmpty(child.MatchingName))
						{
							continue;
						}

						OnRegisterOrRemovePlugin(new PluginRegisterEventArgs(plugin.MatchingName, child.MatchingName, false));
						register.RemovePlugin(plugin.MatchingName, child.MatchingName);
					}

					if (plugin.Merge)
					{
						OnNamespaceMerge(new MergeNamespaceEventArgs(plugin.MatchingName));
						MergeNamespace.CallMerge(plugin.MatchingName);
					}
				}
			}
		}

		private void PatchParentXmlNode(string realParentName, string matchingParentName)
		{
			if (this.xmldoc == null ||
			    string.IsNullOrEmpty(realParentName) ||
			    string.IsNullOrEmpty(matchingParentName))
			{
				return;
			}

			XmlNode node = this.xmldoc.SelectSingleNode
				(string.Format(CultureInfo.InvariantCulture, "/register/plugin[@parent=\"{0}\"]", realParentName));
			XmlHelperClass.SetXmlStringAttributeValue(node.CreateNavigator(), "parent", matchingParentName);
			this.xmldoc.Save(this.xmlFilename);
		}

		private void PatchChildXmlNode(string realParentName, string realChildName, string matchingChildName)
		{
			if (this.xmldoc == null ||
			    string.IsNullOrEmpty(realParentName) ||
			    string.IsNullOrEmpty(realChildName) ||
			    string.IsNullOrEmpty(matchingChildName))
			{
				return;
			}

			XmlNode node = this.xmldoc.SelectSingleNode
				(string.Format(CultureInfo.InvariantCulture, "/register/plugin[@parent=\"{0}\"]", realParentName));
			if(node != null)
			{
				string childName = XmlHelperClass.GetXmlStringValue(node.CreateNavigator(), "@child");
				if(childName.Length > 0 && String.Compare(childName, realChildName) == 0)
				{
					XmlHelperClass.SetXmlStringAttributeValue(node.CreateNavigator(), "child", matchingChildName);
				}
				else
				{
					XmlNode child = node.SelectSingleNode(string.Format(CultureInfo.InvariantCulture, "child[@name=\"{0}\"]", realChildName));
					XmlHelperClass.SetXmlStringAttributeValue(child.CreateNavigator(), "name", matchingChildName);
				}
				this.xmldoc.Save(this.xmlFilename);
			}
		}

		#region Events
		public event EventHandler<PluginRegisterEventArgs> RegisterOrRemovePlugin;
		public event EventHandler<MergeNamespaceEventArgs> NamespaceMerge;

		private void OnRegisterOrRemovePlugin(PluginRegisterEventArgs e)
		{
			if (RegisterOrRemovePlugin != null)
			{
				RegisterOrRemovePlugin(null, e);
			}
		}

		private void OnNamespaceMerge(MergeNamespaceEventArgs e)
		{
			if (NamespaceMerge != null)
			{
				NamespaceMerge(null, e);
			}
		}
		#endregion
	}
}
