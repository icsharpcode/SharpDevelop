// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mathias Simmack" email="mathias@simmack.de"/>
//     <version>$Revision$</version>
// </file>

namespace HtmlHelp2.OptionsPanel
{
	using System;
	using System.Drawing;
	using System.Diagnostics;
	using System.Collections;
	using System.IO;
	using System.Reflection;
	using System.Windows.Forms;
	using System.Xml;
	using ICSharpCode.Core;
	using ICSharpCode.SharpDevelop.Gui;
	using HtmlHelp2.Environment;
	using HtmlHelp2.RegistryWalker;
	using MSHelpServices;

	public class HtmlHelp2OptionsPanel : AbstractOptionPanel
	{
		static string help2EnvironmentFile = "help2environment.xml";
		ComboBox help2Collections          = null;
		string selectedHelp2Collection     = HtmlHelp2Environment.DefaultNamespaceName;

		public override void LoadPanelContents()
		{
			SetupFromXmlStream(this.GetType().Assembly.GetManifestResourceStream("HtmlHelp2.Resources.HtmlHelp2Options.xfrm"));
//			ControlDictionary["reregisterButton"].Click += ReregisterButtonClick;
//			ControlDictionary["reregisterButton"].Visible = false;
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
			try
			{
				help2Collections             = (ComboBox)ControlDictionary["help2Collections"];
				help2Collections.Enabled     = HtmlHelp2Environment.IsReady;
				help2Collections.SelectedIndexChanged += new EventHandler(this.NamespaceNameChanged);
				selectedHelp2Collection      = HtmlHelp2Environment.CurrentSelectedNamespace;

				Help2RegistryWalker.BuildNamespacesList(help2Collections, selectedHelp2Collection);
			}
			catch(Exception ex)
			{
				LoggingService.Error("Help 2.0: Cannot initialize options panel; " + ex.Message);
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
			if (selectedHelp2Collection.Length == 0)
			{
				return;
			}

			try
			{
				XmlDocument xmldoc    = new XmlDocument();
				xmldoc.LoadXml("<?xml version=\"1.0\" encoding=\"ISO-8859-1\"?><help2environment/>");

				XmlNode node          = xmldoc.CreateElement("collection");
				XmlCDataSection cdata = xmldoc.CreateCDataSection(selectedHelp2Collection);
				node.AppendChild(cdata);
				xmldoc.DocumentElement.AppendChild(node);

				xmldoc.Save(Path.Combine(PropertyService.ConfigDirectory, help2EnvironmentFile));

				LoggingService.Info("Help 2.0: new configuration saved");
			}
			catch
			{
				LoggingService.Error("Help 2.0: error while trying to save configuration");
			}
		}

		#region ReRegister
//		void ReregisterButtonClick(object sender, EventArgs e)
//		{
//			System.Threading.ThreadPool.QueueUserWorkItem(DoReregister);
//		}
//		
//		void DoReregister(object state)
//		{
//			try
//			{
//				ProcessStartInfo info = new ProcessStartInfo("cmd", "/c call echo Unregistering... & unregister.bat & echo. & echo Registering... & call register.bat & pause");
//				info.WorkingDirectory = Path.Combine(FileUtility.ApplicationRootPath, "bin\\setup\\help");
//				Process p = Process.Start(info);
//				p.WaitForExit(45000);
//				WorkbenchSingleton.SafeThreadAsyncCall(typeof(HtmlHelp2Environment), "ReloadNamespace");
//			}
//			catch (Exception ex)
//			{
//				MessageService.ShowError(ex);
//			}
//		}
		#endregion
	}
}
