// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mathias Simmack" email="mathias@simmack.de"/>
//     <version>$Revision$</version>
// </file>

namespace HtmlHelp2.Environment
{
	using System;
	using System.Globalization;
	using System.IO;
	using System.Windows.Forms;
	using System.Xml.Serialization;
	
	using ICSharpCode.Core;
	using MSHelpServices;

	public sealed class HtmlHelp2Environment
	{
		static Guid TocGuid                              = new Guid("314111B2-A502-11D2-BBCA-00C04F8EC294");
		static Guid IndexGuid                            = new Guid("314111CC-A502-11D2-BBCA-00C04F8EC294");
		static Guid QueryGuid                            = new Guid("31411193-A502-11D2-BBCA-00C04F8EC294");
		static HxSession session;
		static IHxRegFilterList namespaceFilters;
		static IHxQuery fulltextSearch;
		static IHxIndex dynamicHelp;
		static string defaultNamespaceName;
		static string currentSelectedFilterQuery;
		static string currentSelectedFilterName;
		static string defaultPage                        = "about:blank";
		static string searchPage                         = "http://msdn.microsoft.com";
		static bool dynamicHelpIsBusy;
		static HtmlHelp2Options config                   = new HtmlHelp2Options();
		
		HtmlHelp2Environment()
		{
		}

		static HtmlHelp2Environment()
		{
			InitializeNamespace();
		}

		#region Properties
		public static bool SessionIsInitialized
		{
			get { return session != null; }
		}

		public static string DefaultNamespaceName
		{
			get { return defaultNamespaceName; }
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

		public static IHxQuery Fts
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
			if (string.IsNullOrEmpty(defaultNamespaceName))
			{
				defaultNamespaceName =
					Help2RegistryWalker.GetFirstMatchingNamespaceName("MS.NETFramework.v20*");
			}
			else
			{
				defaultNamespaceName = Help2RegistryWalker.GetFirstNamespace(defaultNamespaceName);
			}
			
			LoadConfiguration();

			if (!string.IsNullOrEmpty(config.SelectedCollection))
			{
				defaultNamespaceName =
					Help2RegistryWalker.GetFirstNamespace(config.SelectedCollection);
			}
		}

		public static void ReloadNamespace()
		{
			InitializeNamespace();
			OnNamespaceReloaded(EventArgs.Empty);
		}

		private static void InitializeNamespace()
		{
			LoadHelp2Config();
			
			if (string.IsNullOrEmpty(defaultNamespaceName))
			{
				return;
			}

			session = null;

			HtmlHelp2Dialog initDialog = new HtmlHelp2Dialog();
			try
			{
				initDialog.Text            = StringParser.Parse("${res:AddIns.HtmlHelp2.HelpUpdateCaption}");
				initDialog.ActionLabel     = StringParser.Parse("${res:AddIns.HtmlHelp2.HelpUpdateInProgress}");
				initDialog.Show();
				Application.DoEvents();

				currentSelectedFilterQuery = string.Empty;
				currentSelectedFilterName  = string.Empty;

				session                    = new HxSession();
				session.Initialize(String.Format(CultureInfo.InvariantCulture, "ms-help://{0}", defaultNamespaceName), 0);
				namespaceFilters           = session.GetFilterList();

				ReloadDefaultPages();
				ReloadFTSSystem();
				ReloadDynamicHelpSystem();

				LoggingService.Info("Help 2.0: Service sucessfully loaded");
			}
			catch (System.Runtime.InteropServices.COMException cEx)
			{
				LoggingService.Error("Help 2.0: Cannot not initialize service; " + cEx.ToString());
				session = null;
			}
			finally
			{
				initDialog.Dispose();
			}
		}

		private static void ReloadFTSSystem()
		{
			try
			{
				fulltextSearch = (IHxQuery)session.GetNavigationInterface
					("!DefaultFullTextSearch", currentSelectedFilterQuery, ref QueryGuid);
			}
			catch (System.Runtime.InteropServices.COMException)
			{
				fulltextSearch = null;
			}
		}

		private static void ReloadDynamicHelpSystem()
		{
			try
			{
				dynamicHelp = (IHxIndex)session.GetNavigationInterface
					("!DefaultContextWindowIndex", currentSelectedFilterQuery, ref IndexGuid);
			}
			catch (System.Runtime.InteropServices.COMException)
			{
				dynamicHelp = null;
			}
		}

		private static void ReloadDefaultPages()
		{
			defaultPage = GetDefaultPage("HomePage", "DefaultPage");
			searchPage  = GetDefaultPage("SearchHelpPage", "SearchWebPage", "http://msdn.microsoft.com");
		}

		private static string GetDefaultPage(string pageName, string alternatePageName)
		{
			return GetDefaultPage(pageName, alternatePageName, "about:blank");
		}

		private static string GetDefaultPage(string pageName, string alternatePageName, string defaultValue)
		{
			if (string.IsNullOrEmpty(pageName) && string.IsNullOrEmpty(alternatePageName))
			{
				throw new ArgumentNullException("pageName & alternatePageName");
			}
			try
			{
				string result = string.Empty;

				IHxIndex namedUrlIndex =
					(IHxIndex)session.GetNavigationInterface("!DefaultNamedUrlIndex", "", ref IndexGuid);
				IHxTopicList topics = namedUrlIndex.GetTopicsFromString(pageName, 0);

				if (topics.Count == 0 && !string.IsNullOrEmpty(alternatePageName))
				{
					topics = namedUrlIndex.GetTopicsFromString(alternatePageName, 0);
				}

				if (topics.Count > 0) result = topics.ItemAt(1).URL;
				if (string.IsNullOrEmpty(result)) result = defaultValue;

				return result;
			}
			catch (System.Runtime.InteropServices.COMException)
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
			if (filterCombobox == null)
			{
				throw new ArgumentNullException("filterCombobox");
			}

			filterCombobox.Items.Clear();
			filterCombobox.BeginUpdate();

			if (namespaceFilters == null || namespaceFilters.Count == 0)
			{
				filterCombobox.Items.Add
					(StringParser.Parse("${res:AddIns.HtmlHelp2.DefaultEmptyFilter}"));
				filterCombobox.SelectedIndex = 0;
			}
			else
			{
				foreach (IHxRegFilter filter in namespaceFilters)
				{
					string filterName =
						(string)filter.GetProperty(HxRegFilterPropId.HxRegFilterName);
					filterCombobox.Items.Add(filterName);

					if (string.IsNullOrEmpty(currentSelectedFilterName))
					{
						currentSelectedFilterName = filterName;
					}
				}

				if (string.IsNullOrEmpty(currentSelectedFilterName))
					filterCombobox.SelectedIndex = 0;
				else
					filterCombobox.SelectedIndex = filterCombobox.Items.IndexOf(currentSelectedFilterName);
			}
			filterCombobox.EndUpdate();
		}

		public static string FindFilterQuery(string filterName)
		{
			if (string.Compare(filterName, currentSelectedFilterName) == 0)
			{
				return currentSelectedFilterQuery;
			}
			if (namespaceFilters == null || namespaceFilters.Count == 0)
			{
				return string.Empty;
			}

			IHxRegFilter filter = namespaceFilters.FindFilter(filterName);
			if (filter == null)
			{
				return string.Empty;
			}

			currentSelectedFilterName = filterName;
			currentSelectedFilterQuery =
				(string)filter.GetProperty(HxRegFilterPropId.HxRegFilterQuery);

			OnFilterQueryChanged(EventArgs.Empty);

			try
			{
				ReloadFTSSystem();
				ReloadDynamicHelpSystem();
				ReloadDefaultPages();
			}
			catch (System.Runtime.InteropServices.COMException cEx)
			{
				LoggingService.Error("Help 2.0: Cannot not initialize service; " + cEx.ToString());
			}
			return currentSelectedFilterQuery;
		}

		public static IHxTopicList GetMatchingTopicsForDynamicHelp(string searchTerm)
		{
			if (dynamicHelpIsBusy || dynamicHelp == null || string.IsNullOrEmpty(searchTerm))
			{
				return null;
			}
			IHxTopicList topics;
			try
			{
				dynamicHelpIsBusy = true;
				topics = dynamicHelp.GetTopicsFromString(searchTerm, 0);
				LoggingService.Info("Help 2.0: Dynamic Help called");
			}
			catch (System.Runtime.InteropServices.COMException)
			{
				LoggingService.Error("Help 2.0: Dynamic Help search failed");
				topics = null;
			}
			finally
			{
				dynamicHelpIsBusy = false;
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
			string configFile = Path.Combine(PropertyService.ConfigDirectory, "help2environment.xml");
			if (!File.Exists(configFile))
			{
				return;
			}
			try
			{
				XmlSerializer serialize = new XmlSerializer(typeof(HtmlHelp2Options));
				TextReader file = new StreamReader(configFile);
				config = (HtmlHelp2Options)serialize.Deserialize(file);
				file.Close();
	
				LoggingService.Info("Help 2.0: Configuration successfully loaded");
			}
			catch (InvalidOperationException)
			{
				LoggingService.Error("Help 2.0: Error while trying to load configuration");
			}
		}

		public static void SaveConfiguration()
		{
			string configFile = Path.Combine(PropertyService.ConfigDirectory, "help2environment.xml");
			try
			{
				XmlSerializer serialize = new XmlSerializer(typeof(HtmlHelp2Options));
				TextWriter file = new StreamWriter(configFile);
				serialize.Serialize(file, config);
				file.Close();

				LoggingService.Info("Help 2.0: Configuration successfully saved");
			}
			catch (InvalidOperationException)
			{
				LoggingService.Error("Help 2.0: Error while trying to save configuration");
			}
		}
		#endregion
	}
}
