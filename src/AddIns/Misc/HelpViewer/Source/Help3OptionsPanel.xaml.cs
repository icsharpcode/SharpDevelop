// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
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
			Load();
		}

		void Load()
		{
			HelpLibraryAgent.Start();
			DataContext = Help3Service.Items;
			if (Help3Service.Items.Count > 0)
				groupBox1.Header = string.Format("{0} ({1})", StringParser.Parse("${res:AddIns.HelpViewer.InstalledHelpCatalogsLabel}"), Help3Service.Items.Count);
			if (Help3Service.ActiveCatalog != null)
				help3Catalogs.SelectedValue = Help3Service.ActiveCatalog.ShortName;
			help3Catalogs.IsEnabled = (Help3Service.Items.Count > 1 && Help3Service.Config.OfflineMode);
			onlineMode.IsChecked = !Help3Service.Config.OfflineMode;
			externalHelp.IsChecked = Help3Service.Config.ExternalHelp;
			onlineMode.IsEnabled = Help3Environment.IsHelp3ProtocolRegistered;
			offlineMode.IsEnabled = Help3Environment.IsHelp3ProtocolRegistered;
			externalHelp.IsEnabled = Help3Environment.IsHelp3ProtocolRegistered;
		}

		void Help3CatalogsSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			string item = (string)help3Catalogs.SelectedValue;
			if (!string.IsNullOrEmpty(item)) {
				Help3Service.ActiveCatalogId = item;
			}
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

		void Help3UseExternalHelpClicked(object sender, RoutedEventArgs e)
		{
			Help3Service.Config.ExternalHelp = (bool)externalHelp.IsChecked;
			LoggingService.Info(string.Format("Help 3.0: {0} external help", (Help3Service.Config.ExternalHelp)?"Enabling":"Disabling"));
		}

		public override bool SaveOptions()
		{
			Help3Service.SaveHelpConfiguration();
			return true;
		}
	}
}
