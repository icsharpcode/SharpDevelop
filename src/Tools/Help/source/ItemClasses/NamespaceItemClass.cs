//
// Help 2.0 Registration Utility
// Copyright (c) 2005 Mathias Simmack. All rights reserved.
//
namespace HtmlHelp2Registration.ItemClasses
{
	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Globalization;
	using System.IO;
	using System.Xml;
	using System.Xml.XPath;
	using System.Xml.Serialization;
	using HtmlHelp2Registration.HelperClasses;

	public class NamespaceItemClass
	{
		private string name;
		private string description;
		private string collection;
		private bool update;
		private bool merge = true;
		private bool noremove = true;
		private List<DocumentItemClass> documents = new List<DocumentItemClass>();
		private List<FilterItemClass> filters = new List<FilterItemClass>();
		private List<PluginChildItem> plugins = new List<PluginChildItem>();
		private ReadOnlyCollection<string> connections = null;

		public NamespaceItemClass(XPathNavigator rootNode)
		{
			if (rootNode == null)
			{
				throw new ArgumentNullException("rootNode");
			}
			
			this.name        = XmlHelperClass.GetXmlStringValue(rootNode, "@name");
			this.description = XmlHelperClass.GetXmlStringValue(rootNode, "@description");
			this.collection  = XmlHelperClass.GetXmlStringValue(rootNode, "@file");
			this.update      = XmlHelperClass.GetXmlBoolValue(rootNode, "@update");
			this.merge       = XmlHelperClass.GetXmlBoolValue(rootNode, "@merge", true);
			this.noremove    = XmlHelperClass.GetXmlBoolValue(rootNode, "@noremove");
			this.connections = PluginSearch.FindPluginAsGenericList(this.name);

			this.Initialize(rootNode);
		}

		void Initialize(XPathNavigator rootNode)
		{
			// get all related documents
			XPathNodeIterator files =
				rootNode.SelectChildren("file", ApplicationHelpers.Help2NamespaceUri);
			while (files.MoveNext())
			{
				this.documents.Add(new DocumentItemClass(files.Current));
			}

			// get all related filters
			XPathNodeIterator filters =
				rootNode.SelectChildren("filter", ApplicationHelpers.Help2NamespaceUri);
			while (filters.MoveNext())
			{
				this.filters.Add(new FilterItemClass(filters.Current));
			}

			// get all related plugins
			XPathNodeIterator p =
				rootNode.SelectChildren("plugin", ApplicationHelpers.Help2NamespaceUri);
			while (p.MoveNext())
			{
				XPathNodeIterator child =
					p.Current.SelectChildren("child", ApplicationHelpers.Help2NamespaceUri);
				while (child.MoveNext())
				{
					this.plugins.Add(new PluginChildItem(child.Current));
				}
			}
		}

		#region Properties
		public string Name
		{
			get { return this.name; }
		}

		public string Description
		{
			get { return this.description; }
		}

		public string CollectionLevelFile
		{
			get { return this.collection; }
		}

		public bool ForceCreation
		{
			get { return !this.update; }
		}

		public bool Merge
		{
			get { return this.merge; }
		}

		public bool Remove
		{
			get { return !this.noremove; }
		}

		public ReadOnlyCollection<DocumentItemClass> Documents
		{
			get
			{
				ReadOnlyCollection<DocumentItemClass> roDocuments =
					new ReadOnlyCollection<DocumentItemClass>(this.documents);
				return roDocuments;
			}
		}

		public ReadOnlyCollection<FilterItemClass> Filters
		{
			get
			{
				ReadOnlyCollection<FilterItemClass> roFilters =
					new ReadOnlyCollection<FilterItemClass>(this.filters);
				return roFilters;
			}
		}

		public ReadOnlyCollection<PluginChildItem> Plugins
		{
			get
			{
				ReadOnlyCollection<PluginChildItem> roPlugins =
					new ReadOnlyCollection<PluginChildItem>(this.plugins);
				return roPlugins;
			}
		}

		public ReadOnlyCollection<string> ConnectedNamespaces
		{
			get { return this.connections; }
		}
		#endregion

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "[Help 2.0 Namespace; {0}, {1}]", this.name, this.description);
		}
	}
}
