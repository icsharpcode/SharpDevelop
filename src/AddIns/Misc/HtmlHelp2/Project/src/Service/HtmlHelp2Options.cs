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
	using System.Xml.Serialization;
	using ICSharpCode.Core;
	using ICSharpCode.SharpDevelop.Gui;
	using HtmlHelp2.Environment;
	using HtmlHelp2.RegistryWalker;
	using MSHelpServices;

	public class HtmlHelp2OptionsPanel : AbstractOptionPanel
	{
		ComboBox help2Collections = null;
		CheckBox tocPictures = null;
		string selectedHelp2Collection = HtmlHelp2Environment.DefaultNamespaceName;

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
				help2Collections = (ComboBox)ControlDictionary["help2Collections"];
				help2Collections.Enabled = HtmlHelp2Environment.IsReady;
				help2Collections.SelectedIndexChanged += new EventHandler(this.NamespaceNameChanged);
				selectedHelp2Collection = HtmlHelp2Environment.CurrentSelectedNamespace;

				tocPictures = (CheckBox)ControlDictionary["tocPictures"];
				tocPictures.Enabled = HtmlHelp2Environment.IsReady;
				tocPictures.Checked = HtmlHelp2Environment.Config.TocPictures;

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
			HtmlHelp2Environment.Config.SelectedCollection = selectedHelp2Collection;
			HtmlHelp2Environment.Config.TocPictures = tocPictures.Checked;
			HtmlHelp2Environment.SaveConfiguration();
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

	[XmlRoot("help2environment")]
	public class HtmlHelp2Options
	{
		private string selectedCollection = string.Empty;
		private bool tocPictures = true;

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
	}
}
