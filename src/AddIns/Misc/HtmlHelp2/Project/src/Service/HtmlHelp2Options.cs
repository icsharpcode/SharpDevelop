// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mathias Simmack" email="mathias@simmack.de"/>
//     <version>$Revision$</version>
// </file>

namespace HtmlHelp2.Environment
{
	using System;
	using System.Windows.Forms;
	using System.Xml.Serialization;
	
	using ICSharpCode.SharpDevelop.Gui;

	public class HtmlHelp2OptionsPanel : AbstractOptionPanel
	{
		ComboBox help2Collections;
		CheckBox tocPictures;
		string selectedHelp2Collection = string.Empty;

		public override void LoadPanelContents()
		{
			SetupFromXmlStream
				(this.GetType().Assembly.GetManifestResourceStream("HtmlHelp2.Resources.HtmlHelp2Options.xfrm"));
			this.InitializeComponents();
		}

		public override bool StorePanelContents()
		{
			this.SaveHelp2Config();
			HtmlHelp2Environment.ReloadNamespace();
			return true;
		}

		private void InitializeComponents()
		{
			selectedHelp2Collection = HtmlHelp2Environment.DefaultNamespaceName;

			help2Collections = (ComboBox)ControlDictionary["help2Collections"];
			help2Collections.SelectedIndexChanged += new EventHandler(this.NamespaceNameChanged);

			tocPictures = (CheckBox)ControlDictionary["tocPictures"];
			tocPictures.Checked = HtmlHelp2Environment.Config.TocPictures;
			
			if (!Help2RegistryWalker.BuildNamespacesList(help2Collections, selectedHelp2Collection))
			{
				help2Collections.Enabled = false;
				tocPictures.Enabled = false;
			}
		}

		private void NamespaceNameChanged(object sender, EventArgs e)
		{
			if (help2Collections.SelectedItem != null)
			{
				selectedHelp2Collection =
					Help2RegistryWalker.GetNamespaceName(help2Collections.SelectedItem.ToString());
			}
		}

		private void SaveHelp2Config()
		{
			HtmlHelp2Environment.Config.SelectedCollection = selectedHelp2Collection;
			HtmlHelp2Environment.Config.TocPictures = tocPictures.Checked;
			HtmlHelp2Environment.SaveConfiguration();
		}
	}

	[XmlRoot("help2environment")]
	public class HtmlHelp2Options
	{
		private string selectedCollection = string.Empty;
		private bool tocPictures;
		private bool dynamicHelpDebugInfo;

		[XmlElement("collection")]
		public string SelectedCollection
		{
			get { return selectedCollection; }
			set { selectedCollection = value; }
		}

		[XmlElement("tocpictures")]
		public bool TocPictures
		{
			get { return tocPictures; }
			set { tocPictures = value; }
		}

		[XmlElement("dhdebuginfos")]
		public bool DynamicHelpDebugInfos
		{
			get { return this.dynamicHelpDebugInfo; }
			set { this.dynamicHelpDebugInfo = value; }
		}
	}
}
