// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mathias Simmack" email="mathias@simmack.de"/>
//     <version>$Revision$</version>
// </file>

namespace HtmlHelp2.Environment
{
	using System;
	using System.IO;
	using System.Windows.Forms;
	using System.Xml;
	using System.Xml.Serialization;
	using ICSharpCode.Core;
	using MSHelpServices;
	using HtmlHelp2.RegistryWalker;
	using HtmlHelp2.HelperDialog;
	using HtmlHelp2.OptionsPanel;

	public sealed class HtmlHelp2Environment
	{
		static Guid TocGuid                              = new Guid("314111B2-A502-11D2-BBCA-00C04F8EC294");
		static Guid IndexGuid                            = new Guid("314111CC-A502-11D2-BBCA-00C04F8EC294");
		static Guid QueryGuid                            = new Guid("31411193-A502-11D2-BBCA-00C04F8EC294");
		
		static HxSession session                         = null;
		static IHxRegFilterList namespaceFilters         = null;
		static IHxQuery fulltextSearch                   = null;
		static IHxIndex dynamicHelp                      = null;
		public static string DefaultNamespaceName        = "MS.NETFramework.v20*";
		static string currentSelectedFilterQuery         = "";
		static string currentSelectedFilterName          = "";
		static string defaultPage                        = "about:blank";
		static string searchPage                         = "http://msdn.microsoft.com";
		static bool dynamicHelpIsBusy                    = false;
		static HtmlHelp2Options config                   = new HtmlHelp2Options();


		static HtmlHelp2Environment()
		{
			LoadHelp2Config();
			DefaultNamespaceName = Help2RegistryWalker.GetFirstMatchingNamespaceName(DefaultNamespaceName);
			InitializeNamespace(DefaultNamespaceName);
		}

		#region Properties
		public static bool IsReady
		{
			get { return session != null; }
		}

		public static string CurrentSelectedNamespace
		{
			get { return DefaultNamespaceName; }
		}

		public static string CurrentFilterQuery
		{
			get { return currentSelectedFilterQuery; }
		}

		public static string CurrentFilterName
		{
			get { return currentSelectedFilterName; }
		}

		public static string DefaultPage
		{
			get { return defaultPage; }
		}

		public static string SearchPage
		{
			get { return searchPage; }
		}

		public static IHxQuery FTS
		{
			get { return fulltextSearch; }
		}

		public static bool DynamicHelpIsBusy
		{
			get { return dynamicHelpIsBusy; }
		}

		public static HtmlHelp2Options Config
		{
			get { return config; }
		}
		#endregion

		#region Namespace Functions
		private static void LoadHelp2Config()
		{
			LoadConfiguration();
			if (!string.IsNullOrEmpty(config.SelectedCollection))
			{
				DefaultNamespaceName = config.SelectedCollection;
			}

//			try
//			{
//				XmlDocument xmldoc = new XmlDocument();
//				xmldoc.Load(Path.Combine(PropertyService.ConfigDirectory, help2EnvironmentFile));
//
//				XmlNode node = xmldoc.SelectSingleNode("/help2environment/collection");
//				if (node != null) {
//					if (!string.IsNullOrEmpty(node.InnerText)) {
//						DefaultNamespaceName = node.InnerText;
//					}
//				}
//
//				LoggingService.Info("Help 2.0: using last configuration");
//			}
//			catch
//			{
//				LoggingService.Info("Help 2.0: using default configuration");
//			}
		}

		public static void ReloadNamespace()
		{
			LoadHelp2Config();
			DefaultNamespaceName = Help2RegistryWalker.GetFirstNamespace(DefaultNamespaceName);
			InitializeNamespace(DefaultNamespaceName);
			OnNamespaceReloaded(EventArgs.Empty);
		}

		private static void InitializeNamespace(string namespaceName)
		{
			if(namespaceName == null || namespaceName == "")
				return;

			if(session != null) session = null;

			HtmlHelp2Dialog initDialog = new HtmlHelp2Dialog();
			try
			{
				initDialog.Text            = StringParser.Parse("${res:AddIns.HtmlHelp2.HelpUpdateCaption}");
				initDialog.ActionLabel     = StringParser.Parse("${res:AddIns.HtmlHelp2.HelpUpdateInProgress}");
				initDialog.Show();
				Application.DoEvents();

				currentSelectedFilterQuery = "";
				currentSelectedFilterName  = "";

				session                    = new HxSession();
				session.Initialize(String.Format("ms-help://{0}", namespaceName), 0);
				namespaceFilters           = session.GetFilterList();

				ReloadDefaultPages();
				ReloadFTSSystem();
				ReloadDynamicHelpSystem();

				LoggingService.Info("Help 2.0: service sucessfully loaded");
			}
			catch(Exception ex)
			{
				LoggingService.Error("Help 2.0: not initialize service; " + ex.ToString());
				session = null;
			}
			finally
			{
				initDialog.Dispose();
			}
		}

		private static void ReloadFTSSystem()
		{
			fulltextSearch = (IHxQuery)session.GetNavigationInterface("!DefaultFullTextSearch", currentSelectedFilterQuery, ref QueryGuid);
		}

		private static void ReloadDynamicHelpSystem()
		{
			dynamicHelp = (IHxIndex)session.GetNavigationInterface("!DefaultContextWindowIndex", currentSelectedFilterQuery, ref IndexGuid);
		}

		private static void ReloadDefaultPages()
		{
			defaultPage = GetDefaultPage("HomePage", "DefaultPage", "about:blank");
			searchPage  = GetDefaultPage("SearchHelpPage", "SearchWebPage", "http://msdn.microsoft.com");
		}

		private static string GetDefaultPage(string pageName, string alternatePageName, string defaultValue)
		{
			string resultString = "";

			try
			{
				IHxIndex namedUrlIndex = (IHxIndex)session.GetNavigationInterface("!DefaultNamedUrlIndex",
				                                                                  "",
				                                                                  ref IndexGuid);
				IHxTopicList topics = null;

				topics = namedUrlIndex.GetTopicsFromString(pageName, 0);

				if(topics.Count == 0 && (alternatePageName != null && alternatePageName != ""))
				{
					topics = namedUrlIndex.GetTopicsFromString(alternatePageName, 0);
				}

				if(topics.Count > 0)
					resultString = topics.ItemAt(1).URL;

				if (resultString == null || resultString.Length == 0)
					resultString = defaultValue;

				return resultString;
			}
			catch
			{
				return defaultValue;
			}
		}

		public static IHxHierarchy GetTocHierarchy(string filterQuery)
		{
			IHxHierarchy defaultToc =
				(IHxHierarchy)session.GetNavigationInterface("!DefaultTOC", filterQuery, ref TocGuid);
			return defaultToc;
		}

		public static IHxIndex GetIndex(string filterQuery)
		{
			IHxIndex defaultIndex =
				(IHxIndex)session.GetNavigationInterface("!DefaultKeywordIndex", filterQuery, ref IndexGuid);
			return defaultIndex;
		}

		public static void BuildFilterList(ComboBox filterCombobox)
		{
			filterCombobox.Items.Clear();
			filterCombobox.BeginUpdate();

			try
			{
				foreach(IHxRegFilter filter in namespaceFilters)
				{
					string filterName = (string)filter.GetProperty(HxRegFilterPropId.HxRegFilterName);
					filterCombobox.Items.Add(filterName);
					if(currentSelectedFilterName == "") currentSelectedFilterName = filterName;
				}

				if(namespaceFilters.Count == 0)
					filterCombobox.Items.Add(StringParser.Parse("${res:AddIns.HtmlHelp2.DefaultEmptyFilter}"));

				if(currentSelectedFilterName == "")	filterCombobox.SelectedIndex = 0;
				else filterCombobox.SelectedIndex = filterCombobox.Items.IndexOf(currentSelectedFilterName);
			}
			catch(Exception ex)
			{
				LoggingService.Error("Help 2.0: cannot build filters; " + ex.Message);
			}

			filterCombobox.EndUpdate();
		}

		public static string FindFilterQuery(string filterName)
		{
			if(String.Compare(filterName, currentSelectedFilterName) == 0)
				return currentSelectedFilterQuery;

			try
			{
				IHxRegFilter filter        = namespaceFilters.FindFilter(filterName);
				currentSelectedFilterQuery = (string)filter.GetProperty(HxRegFilterPropId.HxRegFilterQuery);
				currentSelectedFilterName  = filterName;

				OnFilterQueryChanged(EventArgs.Empty);

				ReloadFTSSystem();
				ReloadDynamicHelpSystem();
				ReloadDefaultPages();
				return currentSelectedFilterQuery;
			}
			catch
			{
				return "";
			}
		}

		public static IHxTopicList GetMatchingTopicsForDynamicHelp(string searchTerm)
		{
			if(dynamicHelpIsBusy) return null;
			IHxTopicList topics = null;
			try
			{
				dynamicHelpIsBusy = true;
				topics = dynamicHelp.GetTopicsFromString(searchTerm, 0);
				LoggingService.Info("Help 2.0: Dynamic Help called");
			}
			catch
			{
				LoggingService.Error("Help 2.0: Dynamic Help search failed");
			}
			finally
			{
				dynamicHelpIsBusy   = false;
			}
			return topics;
		}
		#endregion

		#region Event Handling
		public static event EventHandler FilterQueryChanged;
		public static event EventHandler NamespaceReloaded;

		private static void OnFilterQueryChanged(EventArgs e)
		{
			if(FilterQueryChanged != null)
			{
				FilterQueryChanged(null, e);
			}
		}

		private static void OnNamespaceReloaded(EventArgs e)
		{
			if(NamespaceReloaded != null)
			{
				NamespaceReloaded(null, e);
			}
		}
		#endregion

		#region Configuration
		public static void LoadConfiguration()
		{
			try
			{
				string configFile =
					Path.Combine(PropertyService.ConfigDirectory, "help2environment.xml");

				if(!File.Exists(configFile))
				{
					return;
				}
				
				XmlSerializer serialize = new XmlSerializer(typeof(HtmlHelp2Options));
				StreamReader file = new StreamReader(configFile);
				config = (HtmlHelp2Options)serialize.Deserialize(file);
				file.Close();

				LoggingService.Info("Help 2.0: Configuration successfully loaded.");
			}
			catch
			{
				LoggingService.Error("Help 2.0: Error while trying to load configuration.");
			}
		}

		public static void SaveConfiguration()
		{
			try
			{
				string configFile =
					Path.Combine(PropertyService.ConfigDirectory, "help2environment.xml");

				XmlSerializer serialize = new XmlSerializer(typeof(HtmlHelp2Options));
				StreamWriter file = new StreamWriter(configFile);
				serialize.Serialize(file, config);
				file.Close();

				LoggingService.Info("Help 2.0: Configuration successfully saved.");
			}
			catch
			{
				LoggingService.Error("Help 2.0: Error while trying to save configuration.");
			}
		}
		#endregion
	}
}
