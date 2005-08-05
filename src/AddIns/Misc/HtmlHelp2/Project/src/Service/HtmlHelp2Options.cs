/* ***********************************************************
 * 
 * Help 2.0 Environment for SharpDevelop
 * Base Help 2.0 Services (Options Panel)
 * Copyright (c) 2005, Mathias Simmack. All rights reserved.
 * 
 * ********************************************************* */
namespace HtmlHelp2Service
{
	using System;
	using System.Drawing;
	using System.Collections;
	using System.IO;
	using System.Reflection;
	using System.Windows.Forms;
	using System.Xml;
	using ICSharpCode.Core;
	using ICSharpCode.SharpDevelop.Gui;
	using MSHelpServices;

	public class HtmlHelp2OptionsPanel : AbstractOptionPanel
	{
		static string help2EnvironmentFile = "help2environment.xml";
		bool Help2EnvIsReady               = false;
		ComboBox help2Collections          = null;
		string selectedHelp2Collection     = "Fidalgo";

		public override void LoadPanelContents()
		{
			SetupFromXmlStream(this.GetType().Assembly.GetManifestResourceStream("HtmlHelp2.Resources.HtmlHelp2Options.xfrm"));
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
			Help2EnvIsReady                  = HtmlHelp2Environment.IsReady;

			try {
				help2Collections             = (ComboBox)ControlDictionary["help2Collections"];
				help2Collections.Enabled     = Help2EnvIsReady;
				help2Collections.SelectedIndexChanged += new EventHandler(this.NamespaceNameChanged);
				selectedHelp2Collection      = HtmlHelp2Environment.CurrentSelectedNamespace;

				Help2RegistryWalker.BuildNamespacesList(help2Collections, selectedHelp2Collection);
			}
			catch {
			}
		}

		private void NamespaceNameChanged(object sender, EventArgs e)
		{
			if(help2Collections.SelectedItem != null) {
				try {
					selectedHelp2Collection = Help2RegistryWalker.GetNamespaceName(help2Collections.SelectedItem.ToString());
				}
				catch {
				}
			}
		}

		private void SaveHelp2Config()
		{
			try {
				XmlDocument xmldoc    = new XmlDocument();
				XmlNode node          = null;
				XmlCDataSection cdata = null;
				xmldoc.LoadXml("<?xml version=\"1.0\" encoding=\"ISO-8859-1\"?><help2environment/>");

				node                  = xmldoc.CreateElement("collection");
				cdata                 = xmldoc.CreateCDataSection(selectedHelp2Collection);
				node.AppendChild(cdata);
				xmldoc.DocumentElement.AppendChild(node);

				xmldoc.Save(PropertyService.ConfigDirectory + help2EnvironmentFile);
			}
			catch {
			}
		}
	}
}
