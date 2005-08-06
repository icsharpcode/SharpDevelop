/* ***********************************************************
 *
 * Help 2.0 Environment for SharpDevelop
 * Base Help 2.0 Services
 * Copyright (c) 2005, Mathias Simmack. All rights reserved.
 *
 * ********************************************************* */
namespace HtmlHelp2Service
{
	using System;
	using System.Windows.Forms;
	using System.Xml;
	using ICSharpCode.Core;
	using MSHelpServices;
	using HtmlHelp2HelperDialog;


	public sealed class HtmlHelp2Environment
	{
		static string help2EnvironmentFile               = "help2environment.xml";
		static Guid TocGuid                              = new Guid("314111B2-A502-11D2-BBCA-00C04F8EC294");
		static Guid IndexGuid                            = new Guid("314111CC-A502-11D2-BBCA-00C04F8EC294");
		static Guid QueryGuid                            = new Guid("31411193-A502-11D2-BBCA-00C04F8EC294");
		
		static HxSession session                         = null;
		static IHxRegFilterList namespaceFilters         = null;
		static IHxQuery fulltextSearch                   = null;
		static IHxQuery dynamicHelp                      = null;
		static string corsavyNamespaceName               = "Corsavy";
		static string currentSelectedFilterQuery         = "";
		static string currentSelectedFilterName          = "";
		static string defaultPage                        = "about:blank";
		static string searchPage                         = "http://msdn.microsoft.com";
		static bool dynamicHelpIsBusy                    = false;

		static HtmlHelp2Environment()
		{
			LoadHelp2Config();
			corsavyNamespaceName = Help2RegistryWalker.GetFirstNamespace(corsavyNamespaceName);
			InitializeNamespace(corsavyNamespaceName);
		}

		#region Properties
		public static bool IsReady
		{
			get {
				return session != null;
			}
		}

		public static string CurrentSelectedNamespace
		{
			get {
				return corsavyNamespaceName;
			}
		}

		public static string CurrentFilterQuery
		{
			get {
				return currentSelectedFilterQuery;
			}
		}

		public static string CurrentFilterName
		{
			get {
				return currentSelectedFilterName;
			}
		}

		public static string DefaultPage
		{
			get {
				return defaultPage;
			}
		}

		public static string SearchPage
		{
			get {
				return searchPage;
			}
		}

		public static IHxQuery FTS
		{
			get {
				return fulltextSearch;
			}
		}

		public static bool DynamicHelpIsBusy
		{
			get {
				return dynamicHelpIsBusy;
			}
		}
		#endregion

		#region Namespace Functions
		private static void LoadHelp2Config()
		{
			try {
				XmlDocument xmldoc = new XmlDocument();
				xmldoc.Load(PropertyService.ConfigDirectory + help2EnvironmentFile);

				XmlNode node = xmldoc.SelectSingleNode("/help2environment/collection");
				if(node != null) corsavyNamespaceName = node.InnerText;
			}
			catch {
			}
		}

		public static void ReloadNamespace()
		{
			LoadHelp2Config();
			corsavyNamespaceName = Help2RegistryWalker.GetFirstNamespace(corsavyNamespaceName);
			InitializeNamespace(corsavyNamespaceName);
			OnNamespaceReloaded(EventArgs.Empty);
		}

		private static void InitializeNamespace(string namespaceName)
		{
			if(namespaceName == null || namespaceName == "") {
				return;
			}

			if(session != null) session = null;

			try {
				currentSelectedFilterQuery = "";
				currentSelectedFilterName  = "";

				HtmlHelp2Dialog initDialog = new HtmlHelp2Dialog();
				initDialog.Text            = StringParser.Parse("${res:AddIns.HtmlHelp2.HelpUpdateCaption}");
				initDialog.ActionLabel     = StringParser.Parse("${res:AddIns.HtmlHelp2.HelpUpdateInProgress}");
				initDialog.Show();
				Application.DoEvents();
				
				session                    = new HxSession();
				session.Initialize(String.Format("ms-help://{0}", namespaceName), 0);
				namespaceFilters           = session.GetFilterList();

				ReloadDefaultPages();
				ReloadFTSSystem();
				ReloadDynamicHelpSystem();

				initDialog.Dispose();
			}
			catch {
				session = null;
			}
		}

		private static void ReloadFTSSystem()
		{
			try {
				fulltextSearch = (IHxQuery)session.GetNavigationInterface("!DefaultFullTextSearch", currentSelectedFilterQuery, ref QueryGuid);
			}
			catch {
				fulltextSearch = null;
			}
		}

		private static void ReloadDynamicHelpSystem()
		{
			try {
				dynamicHelp = (IHxQuery)session.GetNavigationInterface("!DefaultContextWindowIndex", currentSelectedFilterQuery, ref QueryGuid);
			}
			catch {
				dynamicHelp = null;
			}
		}

		private static void ReloadDefaultPages()
		{
			defaultPage = GetDefaultPage("HomePage", "DefaultPage", "about:blank");
			searchPage  = GetDefaultPage("SearchHelpPage", "SearchWebPage", "http://msdn.microsoft.com");
		}

		private static string GetDefaultPage(string pageName, string alternatePageName, string defaultValue)
		{
			string resultString = "";

			try {
				IHxIndex namedUrlIndex = (IHxIndex)session.GetNavigationInterface("!DefaultNamedUrlIndex", "", ref IndexGuid);
				IHxTopicList topics = null;

				topics = namedUrlIndex.GetTopicsFromString(pageName, 0);

				if(topics.Count == 0 && (alternatePageName != null && alternatePageName != "")) {
					topics = namedUrlIndex.GetTopicsFromString(alternatePageName, 0);
				}

				if(topics.Count > 0)
					resultString = topics.ItemAt(1).URL;
				if (resultString == null || resultString.Length == 0)
					resultString = defaultValue;

				return resultString;
			}
			catch {
				return defaultValue;
			}
		}

		public static IHxHierarchy GetTocHierarchy(string filterQuery)
		{
			try {
				IHxHierarchy defaultToc = (IHxHierarchy)session.GetNavigationInterface("!DefaultTOC", filterQuery, ref TocGuid);
				return defaultToc;
			}
			catch {
				return null;
			}
		}

		public static IHxIndex GetIndex(string filterQuery)
		{
			try {
				IHxIndex defaultIndex   = (IHxIndex)session.GetNavigationInterface("!DefaultKeywordIndex", filterQuery, ref IndexGuid);
				return defaultIndex;
			}
			catch {
				return null;
			}
		}

		public static void BuildFilterList(ComboBox filterCombobox)
		{
			filterCombobox.Items.Clear();
			filterCombobox.BeginUpdate();

			try {
				for(int i = 1; i <= namespaceFilters.Count; i++) {
					IHxRegFilter filter = namespaceFilters.ItemAt(i);
					string filterName   = (string)filter.GetProperty(HxRegFilterPropId.HxRegFilterName);
					filterCombobox.Items.Add(filterName);

					if(currentSelectedFilterName == "" && i == 1) {
						currentSelectedFilterName = filterName;
					}
				}

				if(namespaceFilters.Count == 0) filterCombobox.Items.Add(StringParser.Parse("${res:AddIns.HtmlHelp2.DefaultEmptyFilter}"));

				if(currentSelectedFilterName == "") filterCombobox.SelectedIndex = 0;
				else filterCombobox.SelectedIndex = filterCombobox.Items.IndexOf(currentSelectedFilterName);
			}
			catch {
			}
			filterCombobox.EndUpdate();
		}

		public static string FindFilterQuery(string filterName)
		{
			if(String.Compare(filterName, currentSelectedFilterName) == 0) {
				return currentSelectedFilterQuery;
			}

			try {
				IHxRegFilter filter        = namespaceFilters.FindFilter(filterName);
				currentSelectedFilterQuery      = (string)filter.GetProperty(HxRegFilterPropId.HxRegFilterQuery);
				currentSelectedFilterName  = filterName;

				OnFilterQueryChanged(EventArgs.Empty);

				ReloadFTSSystem();
				ReloadDynamicHelpSystem();
				ReloadDefaultPages();
				return currentSelectedFilterQuery;
			}
			catch {
				return "";
			}
		}

		public static IHxTopicList GetMatchingTopicsForDynamicHelp(string searchTerm)
		{
			if(dynamicHelpIsBusy) {
				return null;
			}

			try {
				dynamicHelpIsBusy = true;
				IHxTopicList topics = ((IHxIndex)dynamicHelp).GetTopicsFromString(searchTerm, 0);
				return topics;
			}
			finally {
				dynamicHelpIsBusy = false;
			}
		}
		#endregion

		#region Event Handling
		public static event EventHandler FilterQueryChanged;
		public static event EventHandler NamespaceReloaded;

		private static void OnFilterQueryChanged(EventArgs e)
		{
			if(FilterQueryChanged != null) {
				FilterQueryChanged(null, e);
			}
		}

		private static void OnNamespaceReloaded(EventArgs e)
		{
			if(NamespaceReloaded != null) {
				NamespaceReloaded(null, e);
			}
		}
		#endregion
	}
}
