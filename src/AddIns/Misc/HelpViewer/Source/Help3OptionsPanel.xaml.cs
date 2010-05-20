using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using MSHelpSystem.Core;
using MSHelpSystem.Helper;

namespace MSHelpSystem
{
	public partial class Help3OptionsPanel : OptionPanel
	{
		public Help3OptionsPanel()
		{
			InitializeComponent();
			LoadCatalogs();
			onlineMode.IsChecked = !Help3Service.Config.OfflineMode;
		}

		void LoadCatalogs()
		{
			if (Help3Environment.IsLocalHelp) HelpLibraryAgent.Start();

			help3Catalogs.Items.Clear();
			if (Help3Service.Count > 0) {
				foreach (Help3Catalog catalog in Help3Service.Items) {
					LoggingService.Debug(string.Format("Help 3.0: Found help catalog \"{0}\"", catalog.ToString()));
					ComboBoxItem cbi = new ComboBoxItem();
					cbi.Content = catalog.ToString();
					help3Catalogs.Items.Add(cbi);
				}
			}
			// TODO: localization needed for "Installed help catalogs"
			groupBox1.Header = string.Format("{0} ({1})", "Installed Help catalogs", Help3Service.Count);

			if (help3Catalogs.Items.Count == 0) help3Catalogs.SelectedIndex = 0;
			else {
				foreach (ComboBoxItem item in help3Catalogs.Items) {
					if (string.Compare((string)item.Content, Help3Service.ActiveCatalog.ToString(), true) == 0) {
						help3Catalogs.SelectedItem = item;
						break;
					}
				}
				// TODO: There has to be a much more elegant way to select the catalog?!
			}
			help3Catalogs.IsEnabled = (help3Catalogs.Items.Count > 1 && Help3Service.Config.OfflineMode);
		}

		void Help3CatalogsSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			ComboBoxItem selectedItem = (ComboBoxItem)help3Catalogs.SelectedItem;
			if (selectedItem == null) return;
			string activeCatalog      = (string)selectedItem.Content;
			if (!string.IsNullOrEmpty(activeCatalog)) Help3Service.ActiveCatalogId = activeCatalog;
		}

		void Help3OfflineModeClicked(object sender, RoutedEventArgs e)
		{
			LoggingService.Info("Help 3.0: Setting help mode to \"offline\"");
			Help3Service.Config.OfflineMode = true;
			help3Catalogs.IsEnabled = (help3Catalogs.Items.Count > 1 && Help3Service.Config.OfflineMode);
		}

		void Help3OnlineModeClicked(object sender, RoutedEventArgs e)
		{
			LoggingService.Info("Help 3.0: Setting help mode to \"online\"");
			Help3Service.Config.OfflineMode = false;
			help3Catalogs.IsEnabled = false;
		}

		public override bool SaveOptions()
		{
			Help3Service.SaveHelpConfiguration();
			return true;
		}
	}
}