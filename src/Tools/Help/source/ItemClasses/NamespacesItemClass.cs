//
// Help 2.0 Registration Utility
// Copyright (c) 2005 Mathias Simmack. All rights reserved.
//
namespace HtmlHelp2Registration.ItemClasses
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Globalization;
	using System.IO;
	using System.Xml;
	using System.Xml.Serialization;
	using HtmlHelp2Registration.HelperClasses;

	public class NamespacesItemClass : ICommonItemClass
	{
		private List<NamespaceItemClass> helpNamespaces = new List<NamespaceItemClass>();
		private XmlDocument xmldoc = new XmlDocument();
		private XmlNamespaceManager xmlns;
		private string xmlFilename = string.Empty;
		private string xmlXpathSequence = string.Empty;

		#region Constructor
		public NamespacesItemClass(string xmlFilename) : this(xmlFilename, string.Empty)
		{
		}

		public NamespacesItemClass(string xmlFilename, string xmlXpathSequence)
		{
			this.xmlFilename      = xmlFilename;
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
				(string.Format(CultureInfo.InvariantCulture, "/help2:register/help2:namespace{0}", this.xmlXpathSequence), this.xmlns);

			foreach (XmlNode node in nodes)
			{
				this.helpNamespaces.Add(new NamespaceItemClass(node.CreateNavigator()));
			}
		}

		public void Register()
		{
			foreach (NamespaceItemClass helpNamespace in helpNamespaces)
			{
				if (string.IsNullOrEmpty(helpNamespace.Name) ||
				    string.IsNullOrEmpty(helpNamespace.CollectionLevelFile) ||
				    !File.Exists(helpNamespace.CollectionLevelFile))
				{
					continue;
				}

				using (Help2RegisterClass register = new Help2RegisterClass())
				{
					// force Help 2.0 namespace creation
					if (helpNamespace.ForceCreation)
					{
						OnLogProgress(new LoggingEventArgs(helpNamespace.ToString()));
						OnRegisterOrRemoveNamespace(new CommonRegisterEventArgs(helpNamespace.Name, true));
						register.RegisterNamespace(helpNamespace.Name,
						                           helpNamespace.CollectionLevelFile,
						                           helpNamespace.Description);
					}

					// register Help 2.0 documents
					foreach (DocumentItemClass document in helpNamespace.Documents)
					{
						if (string.IsNullOrEmpty(document.Id) ||
						    string.IsNullOrEmpty(document.Hxs) ||
						    !File.Exists(document.Hxs))
						{
							continue;
						}

						OnLogProgress(new LoggingEventArgs(document.ToString()));
						OnRegisterOrRemoveHelpDocument(new CommonRegisterEventArgs(document.Id, true));
						register.RegisterHelpFile(helpNamespace.Name,
						                          document.Id,
						                          document.LanguageId,
						                          document.Hxs,
						                          document.Hxi,
						                          document.Hxq,
						                          document.Hxr,
						                          document.HxsMediaId,
						                          document.HxqMediaId,
						                          document.HxrMediaId,
						                          document.SampleMediaId);
					}

					// register Help 2.0 filters
					foreach (FilterItemClass filter in helpNamespace.Filters)
					{
						if (string.IsNullOrEmpty(filter.Name))
						{
							continue;
						}

						OnLogProgress(new LoggingEventArgs(filter.ToString()));
						OnRegisterOrRemoveFilter(new CommonRegisterEventArgs(filter.Name, true));
						register.RegisterFilter(helpNamespace.Name, filter.Name, filter.Query);
					}

					// register Help 2.0 child plug-ins
					foreach (PluginChildItem plugin in helpNamespace.Plugins)
					{
						if (string.IsNullOrEmpty(plugin.MatchingName))
						{
							continue;
						}

						OnLogProgress(new LoggingEventArgs(plugin.ToString()));
						OnRegisterOrRemovePlugin(new PluginRegisterEventArgs(helpNamespace.Name, plugin.MatchingName, true));
						register.RegisterPlugin(helpNamespace.Name, plugin.MatchingName);

						if (string.Compare(plugin.Name, plugin.MatchingName) != 0)
						{
							this.PatchXmlNode(helpNamespace.Name, plugin.Name, plugin.MatchingName);
						}
					}

					// merge Help 2.0 namespace
					if (helpNamespace.Merge)
					{
						OnLogProgress(new LoggingEventArgs(string.Format(CultureInfo.InvariantCulture, "[merging {0}]", helpNamespace.Name)));
						OnNamespaceMerge(new MergeNamespaceEventArgs(helpNamespace.Name));
						MergeNamespace.CallMerge(helpNamespace.Name);

						foreach(string connectedNamespace in helpNamespace.ConnectedNamespaces)
						{
							OnLogProgress(new LoggingEventArgs(string.Format(CultureInfo.InvariantCulture, "[merging {0}]", connectedNamespace)));
							OnNamespaceMerge(new MergeNamespaceEventArgs(connectedNamespace));
							MergeNamespace.CallMerge(connectedNamespace);
						}
					}
				}
			}
		}

		public void Unregister()
		{
			foreach (NamespaceItemClass helpNamespace in helpNamespaces)
			{
				if (string.IsNullOrEmpty(helpNamespace.Name))
				{
					continue;
				}

				using (Help2RegisterClass register = new Help2RegisterClass())
				{
					// remove this Help 2.0 namespace, if it is a plug-in
					foreach (string connectedNamespace in helpNamespace.ConnectedNamespaces)
					{
						OnRegisterOrRemovePlugin(new PluginRegisterEventArgs(connectedNamespace, helpNamespace.Name, false));
						register.RemovePlugin(connectedNamespace, helpNamespace.Name);

						OnNamespaceMerge(new MergeNamespaceEventArgs(connectedNamespace));
						MergeNamespace.CallMerge(connectedNamespace);
					}

					// remove this namespace's child plug-ins
					foreach (PluginChildItem plugin in helpNamespace.Plugins)
					{
						OnLogProgress(new LoggingEventArgs(plugin.ToString()));
						OnRegisterOrRemovePlugin(new PluginRegisterEventArgs(helpNamespace.Name, plugin.MatchingName, false));
						register.RemovePlugin(helpNamespace.Name, plugin.MatchingName);
					}

					// remove this namespace's filters
					foreach (FilterItemClass filter in helpNamespace.Filters)
					{
						OnLogProgress(new LoggingEventArgs(filter.ToString()));
						OnRegisterOrRemoveFilter(new CommonRegisterEventArgs(filter.Name, false));
						register.RemoveFilter(helpNamespace.Name, filter.Name);
					}

					// remove this namespace's documents
					foreach (DocumentItemClass document in helpNamespace.Documents)
					{
						OnLogProgress(new LoggingEventArgs(document.ToString()));
						OnRegisterOrRemoveHelpDocument(new CommonRegisterEventArgs(document.Id, false));
						register.RemoveHelpFile(helpNamespace.Name, document.Id, document.LanguageId);
					}

					// remove this namespace, ...
					if (helpNamespace.Remove)
					{
						OnRegisterOrRemoveNamespace(new CommonRegisterEventArgs(helpNamespace.Name, false));
						register.RemoveNamespace(helpNamespace.Name);
					}
					// ... or just (re)merge it
					else
					{
						OnNamespaceMerge(new MergeNamespaceEventArgs(helpNamespace.Name));
						MergeNamespace.CallMerge(helpNamespace.Name);
					}
				}
			}
		}

		void PatchXmlNode(string namespaceName, string pluginName, string matchingName)
		{
			if (this.xmldoc == null ||
			    string.IsNullOrEmpty(namespaceName) ||
			    string.IsNullOrEmpty(pluginName) ||
			    string.IsNullOrEmpty(matchingName))
			{
				return;
			}

			XmlNode node = xmldoc.SelectSingleNode
				(string.Format(CultureInfo.InvariantCulture,
				               "/help2:register/help2:namespace[@name=\"{0}\"]/help2:plugin/help2:child[@name=\"{1}\"]",
				               namespaceName, pluginName), this.xmlns);
			XmlHelperClass.SetXmlStringAttributeValue(node.CreateNavigator(), "name", matchingName);
			xmldoc.Save(this.xmlFilename);
		}

		#region Events
		public event EventHandler<CommonRegisterEventArgs> RegisterOrRemoveNamespace;
		public event EventHandler<CommonRegisterEventArgs> RegisterOrRemoveHelpDocument;
		public event EventHandler<CommonRegisterEventArgs> RegisterOrRemoveFilter;
		public event EventHandler<PluginRegisterEventArgs> RegisterOrRemovePlugin;
		public event EventHandler<MergeNamespaceEventArgs> NamespaceMerge;
		public event EventHandler<LoggingEventArgs> LogProgress;

		private void OnRegisterOrRemoveNamespace(CommonRegisterEventArgs e)
		{
			if (RegisterOrRemoveNamespace != null)
			{
				RegisterOrRemoveNamespace(null, e);
			}
		}

		private void OnRegisterOrRemoveHelpDocument(CommonRegisterEventArgs e)
		{
			if (RegisterOrRemoveHelpDocument != null)
			{
				RegisterOrRemoveHelpDocument(null, e);
			}
		}

		private void OnRegisterOrRemoveFilter(CommonRegisterEventArgs e)
		{
			if (RegisterOrRemoveFilter != null)
			{
				RegisterOrRemoveFilter(null, e);
			}
		}

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

		private void OnLogProgress(LoggingEventArgs e)
		{
			if (LogProgress != null)
			{
				LogProgress(null, e);
			}
		}
		#endregion
	}
}
