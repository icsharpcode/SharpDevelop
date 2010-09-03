// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using ICSharpCode.Core;
using MSHelpSystem.Helper;

namespace MSHelpSystem.Core
{
	public sealed class Help3Service
	{
		Help3Service()
		{
			HelpLibraryAgent.Start();
		}

		static Help3Service()
		{
			Help3Service.CatalogsUpdated      += new EventHandler(Help3HasUpdatedTheCatalogs);
			Help3Service.CatalogChanged       += new EventHandler(Help3ActiveCatalogIdChanged);
			Help3Service.ConfigurationUpdated += new EventHandler(Help3ConfigurationUpdated);
			LoadHelpConfiguration();
			UpdateCatalogs();
		}

		static List<Help3Catalog> catalogs = new List<Help3Catalog>();
		static Help3Catalog activeCatalog  = null;
		static Help3Configuration config   = new Help3Configuration();

		#region Read help catalogs

		static void UpdateCatalogs()
		{
			catalogs.Clear();
			if (Help3Environment.IsHelp3ProtocolRegistered && !string.IsNullOrEmpty(Help3Environment.ManifestFolder))
			{
				try {
					DirectoryInfo folder = new DirectoryInfo(Help3Environment.ManifestFolder);
					FileInfo[] files = folder.GetFiles(@"queryManifest*.xml");
					foreach (FileInfo file in files) {
						XmlDocument xml = new XmlDocument();
						xml.Load(file.FullName);
						XmlNodeList n = xml.SelectNodes("/queryManifest[@version=\"1.0\"]/catalogs/catalog");
						foreach (XmlNode node in n) {
							catalogs.Add(
								new Help3Catalog(node.Attributes["productId"].InnerText,
								                 node.Attributes["productVersion"].InnerText,
								                 node.Attributes["productLocale"].InnerText,
								                 node.Attributes["productDisplayName"].InnerText,
								                 node.SelectSingleNode("catalogPath").InnerText,
								                 node.SelectSingleNode("contentPath").InnerText,
								                 node.SelectSingleNode("brandingPackageFileName").InnerText)
							);
						}
					}
					LoggingService.Debug(string.Format("Help 3.0: {0} {1} loaded", catalogs.Count, (catalogs.Count == 1)?"catalog":"catalogs"));
				}
				catch (Exception ex) {
					LoggingService.Error(string.Format("Help 3.0: {0}", ex.ToString()));
				}				
			}
			OnCatalogsUpdated(EventArgs.Empty);
		}

		static void Help3HasUpdatedTheCatalogs(object sender, EventArgs e)
		{
			if (string.IsNullOrEmpty(config.ActiveCatalogId)) {
				config.ActiveCatalogId = (catalogs.Count > 0) ? catalogs[0].ShortName:string.Empty;
			}
			activeCatalog = (string.IsNullOrEmpty(config.ActiveCatalogId)) ? null:FindCatalogByItsShortName(config.ActiveCatalogId);
		}

		static void Help3ActiveCatalogIdChanged(object sender, EventArgs e)
		{
			if (string.IsNullOrEmpty(config.ActiveCatalogId)) {
				config.ActiveCatalogId = (catalogs.Count > 0) ? catalogs[0].ShortName:string.Empty;
			}
			activeCatalog = (string.IsNullOrEmpty(config.ActiveCatalogId)) ? null:FindCatalogByItsShortName(config.ActiveCatalogId);
		}

		static void Help3ConfigurationUpdated(object sender, EventArgs e)
		{
			if (config.OfflineMode) HelpClientWatcher.EnableLocalHelp();
			else HelpClientWatcher.EnableOnlineHelp();
		}

		#endregion


		public static ReadOnlyCollection<Help3Catalog> Items
		{
			get
			{
				ReadOnlyCollection<Help3Catalog> c = new ReadOnlyCollection<Help3Catalog>(catalogs);
				return c;
			}
		}

		public static Help3Catalog FindCatalogByProductCode(string productCode)
		{
			foreach (Help3Catalog catalog in catalogs) {
				if (productCode.Equals(catalog.ProductCode, StringComparison.InvariantCultureIgnoreCase)) {
					return catalog;
				}
			}
			return null;
		}

		public static Help3Catalog FindCatalogByDisplayName(string displayName)
		{
			foreach (Help3Catalog catalog in catalogs) {
				if (displayName.Equals(catalog.DisplayName, StringComparison.InvariantCultureIgnoreCase)) {
					return catalog;
				}
			}
			return null;
		}

		public static Help3Catalog FindCatalogByItsShortName(string catalogString)
		{
			foreach (Help3Catalog catalog in catalogs) {
				if (catalogString.Equals(catalog.ShortName, StringComparison.InvariantCultureIgnoreCase)) {
					return catalog;
				}
			}
			return null;
		}

		public static string ActiveCatalogId
		{
			get
			{
				return config.ActiveCatalogId;
			}
			set
			{
				config.ActiveCatalogId = value;
				OnCatalogChanged(EventArgs.Empty);
			}
		}
		
		public static Help3Catalog ActiveCatalog
		{
			get { return activeCatalog; }
		}

		public static Help3Configuration Config
		{
			get { return config; }
		}

		#region Configuration

		public static void LoadHelpConfiguration()
		{
			string configFile = System.IO.Path.Combine(PropertyService.ConfigDirectory, "mshelpsystem.xml");
			if (!File.Exists(configFile)) {
				return;
			}
			try {
				XmlSerializer serialize = new XmlSerializer(typeof(Help3Configuration));
				TextReader file = new StreamReader(configFile);
				config = (Help3Configuration)serialize.Deserialize(file);
				file.Close();
				LoggingService.Info("Help 3.0: Configuration successfully loaded");
			}
			catch (Exception ex) {
				LoggingService.Error(string.Format("Help 3.0: {0}", ex.ToString()));
			}
			OnConfigurationUpdated(EventArgs.Empty);
		}

		public static void SaveHelpConfiguration()
		{
			string configFile = System.IO.Path.Combine(PropertyService.ConfigDirectory, "mshelpsystem.xml");
			try {
				XmlSerializer serialize = new XmlSerializer(typeof(Help3Configuration));
				TextWriter file = new StreamWriter(configFile);
				serialize.Serialize(file, config);
				file.Close();
				LoggingService.Info("Help 3.0: Configuration successfully saved");
			}
			catch (Exception ex) {
				LoggingService.Error(string.Format("Help 3.0: {0}", ex.ToString()));
			}
			OnConfigurationUpdated(EventArgs.Empty);
		}

		#endregion

		#region Event handling

		public static event EventHandler CatalogsUpdated;
		public static event EventHandler CatalogChanged;
		public static event EventHandler ConfigurationUpdated;

		static void OnCatalogsUpdated(EventArgs e)
		{
			LoggingService.Debug("Help 3.0: \"OnCatalogsUpdated\" event raised");
			if (CatalogsUpdated != null) CatalogsUpdated(null, e);
		}

		static void OnCatalogChanged(EventArgs e)
		{
			LoggingService.Debug("Help 3.0: \"OnCatalogChanged\" event raised");
			if (CatalogChanged != null) CatalogChanged(null, e);
		}

		static void OnConfigurationUpdated(EventArgs e)
		{
			LoggingService.Debug("Help 3.0: \"OnConfigurationUpdated\" event raised");
			if (ConfigurationUpdated != null) ConfigurationUpdated(null, e);
		}
		
		#endregion
	}
}
