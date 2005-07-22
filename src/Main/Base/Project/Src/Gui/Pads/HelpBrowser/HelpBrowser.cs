// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using Microsoft.Win32;
using System;
using System.Windows.Forms;
using System.Drawing;
using System.CodeDom.Compiler;
using System.IO;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.Xml;
using System.Reflection;

using ICSharpCode.Core;

using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpDevelop.BrowserDisplayBinding;

namespace ICSharpCode.SharpDevelop.Gui
{
	public class HelpBrowserWindow : BrowserPane
	{
		public HelpBrowserWindow() : base(true)
		{
			TitleName = "Help";
		}
	}
	
	class HelpLinkInformation
	{
		string link;
		bool isMSDN = false;
		
		public string Link {
			get {
				return link;
			}
			set {
				link = value;
			}
		}
		public bool IsMSDN {
			get {
				return isMSDN;
			}
			set {
				isMSDN = value;
			}
		}
		
		public HelpLinkInformation(string link, bool isMSDN)
		{
			this.link = link;
			this.isMSDN = isMSDN;
		}
		
	}
	
	public class HelpBrowser : AbstractPadContent
	{
		static readonly string helpPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) +
		                                  Path.DirectorySeparatorChar + ".." +
		                                  Path.DirectorySeparatorChar + "doc" +
		                                  Path.DirectorySeparatorChar + "help" +
		                                  Path.DirectorySeparatorChar;
		
		static readonly string helpFileName = helpPath + "SharpDevelopHelp.zip";
		static readonly string mainTOCFile  = "HelpConv.xml";
		string HelpPrefix = "ms-help://MS.NETFrameworkSDK";
		
		
		Panel     browserPanel = new Panel();
		TreeView  treeView     = new TreeView();
		
		HelpBrowserWindow helpBrowserWindow = null;
		
		public override Control Control {
			get {
				return browserPanel;
			}
		}
		
		public HelpBrowser()
		{
			treeView.Dock = DockStyle.Fill;
			treeView.ImageList = new ImageList();
			treeView.ImageList.ColorDepth = ColorDepth.Depth32Bit;
			
			
			treeView.ImageList.Images.Add(ResourceService.GetBitmap("Icons.16x16.HelpClosedFolder"));
			treeView.ImageList.Images.Add(ResourceService.GetBitmap("Icons.16x16.HelpOpenFolder"));
			
			treeView.ImageList.Images.Add(ResourceService.GetBitmap("Icons.16x16.HelpTopic"));
			treeView.BeforeExpand   += new TreeViewCancelEventHandler(BeforeExpand);			
			treeView.BeforeCollapse += new TreeViewCancelEventHandler(BeforeCollapse);
			treeView.AfterSelect    += new TreeViewEventHandler(SelectNode);
			browserPanel.Controls.Add(treeView);
			if (File.Exists(helpFileName))
			{
				LoadHelpfile();
				ScanForLocalizedHelpPrefix();
			}
		}
		
		void ScanForLocalizedHelpPrefix()
		{
			string localHelp = String.Concat("0x", Thread.CurrentThread.CurrentCulture.LCID.ToString("X4"));
			RegistryKey helpKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\MSDN\7.0\Help");
			if (helpKey == null) {
				return;
			}
			
			RegistryKey k = helpKey.OpenSubKey(localHelp);
			bool found = false;
			if (k != null) {
				string v = ScanSubKeys(k);
				if (v != null) {
					HelpPrefix = v;
					found = true;
				}
			}
			
			if (!found) {
				// use default english subkey
				k = helpKey.OpenSubKey("0x0409");
				string v = k != null ? ScanSubKeys(k) : null;
				if (v != null) {
					HelpPrefix = v;
				} else {
					string[] subKeys = helpKey.GetSubKeyNames();
					foreach (string subKey in subKeys) {
						if (subKey.StartsWith("0x")) {
							HelpPrefix = ScanSubKeys(helpKey.OpenSubKey(subKey));
							break;
						}
					}
				}
			}
		}
		
		string ScanSubKeys(RegistryKey key)
		{
			if (key != null) {
				string[] subKeys = key.GetSubKeyNames();
				if (subKeys != null) {
					foreach (string subKey in subKeys) {
						RegistryKey sub = key.OpenSubKey(subKey);
						if (sub == null) {
							continue;
						}
						object o = sub.GetValue(null);
						if (o == null) {
							continue;
						}
						if (o.ToString().StartsWith("Microsoft .NET Framework SDK")) {
							return sub.GetValue("Filename").ToString();
						}
					}
				}
			}
			return null;
		}
		protected string GetHelpString(string word)
		{
			int i = 0;
			while ((i = word.IndexOf('.')) != -1) {
				word = word.Remove(i,1);
			}
			return word;
		}
		
		public void ShowHelpFromType(string type)
		{
			string url = String.Format("{0}/cpref/html/frlrf{1}ClassTopic.htm", 
			                           HelpPrefix, 
			                           GetHelpString(type));
			ShowHelpBrowser(url);
		}
		
		public void ShowHelpFromType(string type, string member)
		{
			string url = String.Format("{0}/cpref/html/frlrf{1}Class{2}Topic.htm", 
			                           HelpPrefix,
			                           GetHelpString(type),
			                           member);
			ShowHelpBrowser(url);
		}
		
		/// <remarks>
		/// Parses the xml tree and generates a TreeNode tree out of it.
		/// </remarks>
		void ParseTree(TreeNodeCollection nodeCollection, XmlNode parentNode)
		{
			foreach (XmlNode node in parentNode.ChildNodes) {
				switch (node.Name) {
					case "HelpFolder":
						TreeNode newFolderNode = new TreeNode(node.Attributes["name"].InnerText);
						newFolderNode.ImageIndex = newFolderNode.SelectedImageIndex = 0;
						ParseTree(newFolderNode.Nodes, node);
						
						bool isMSDNLink = node.Attributes["ismsdn"] != null && node.Attributes["ismsdn"].Value.ToLower() == "true";
						newFolderNode.Tag = node.Attributes["link"] != null ? new HelpLinkInformation(node.Attributes["link"].InnerText , isMSDNLink) : null;
						
						nodeCollection.Add(newFolderNode);
						break;
					case "HelpTopic":
						TreeNode newNode = new TreeNode(node.Attributes["name"].InnerText);
						newNode.ImageIndex = newNode.SelectedImageIndex = 2;
						
						isMSDNLink = node.Attributes["ismsdn"] != null && node.Attributes["ismsdn"].Value.ToLower() == "true";
						newNode.Tag = new HelpLinkInformation(node.Attributes["link"].InnerText, isMSDNLink);
						nodeCollection.Add(newNode);
						break;
					case "HelpReference":
						TreeNode newReferenceNode = new TreeNode("Reference");
						newReferenceNode.Tag = node.Attributes["reference"].InnerText;
						nodeCollection.Add(newReferenceNode);
						break;
				}
			}
		}
		
		XmlDocument LoadCompressedXmlDocument(string requestedFile)
		{
			ZipInputStream s = new ZipInputStream(File.OpenRead(helpFileName));
		
			ZipEntry theEntry;
			while ((theEntry = s.GetNextEntry()) != null) {
				if (theEntry.Name == requestedFile) {
					
					StringBuilder sb = new StringBuilder();
					int size = 2048;
					byte[] data = new byte[2048];
					while (true) {
						size = s.Read(data, 0, data.Length);
						if (size > 0) {
							sb.Append(Encoding.UTF8.GetString(data, 0, size));
						} else {
							break;
						}
					}
					s.Close();
					XmlDocument doc = new XmlDocument();
					doc.LoadXml(sb.ToString());
					return doc;
				}
			}
			s.Close();
			System.Diagnostics.Debug.Assert(false);
			return null;
		}
		
		void LoadHelpfile()
		{
			XmlDocument doc = LoadCompressedXmlDocument(mainTOCFile);new XmlDocument();
			ParseTree(treeView.Nodes, doc.DocumentElement);
		}
		
		void HelpBrowserClose(object sender, EventArgs e)
		{
			helpBrowserWindow = null;
		}
		
		public void ShowHelpBrowser(string url)
		{
			if (helpBrowserWindow == null) {
				helpBrowserWindow = new HelpBrowserWindow();
				WorkbenchSingleton.Workbench.ShowView(helpBrowserWindow);
				helpBrowserWindow.WorkbenchWindow.CloseEvent += new EventHandler(HelpBrowserClose);
			}
			helpBrowserWindow.Load(url);
			helpBrowserWindow.WorkbenchWindow.SelectWindow();
		}
		
		
		
		void ShowHelp(TreeNode node)
		{
			if (node == null || node.Tag == null) {
				return;
			}
			string navigationName;
			
			if(((HelpLinkInformation)node.Tag).IsMSDN == true) {
				navigationName = ((HelpLinkInformation)node.Tag).Link;
			} else {
				navigationName = "mk:@MSITStore:" + helpPath + ((HelpLinkInformation)node.Tag).Link;
			}
			
			ShowHelpBrowser(navigationName);
		}
		
		void SelectNode(object sender, TreeViewEventArgs e)
		{
			ShowHelp(e.Node);
		}
		
		void BeforeExpand(object sender, TreeViewCancelEventArgs e)
		{
			if (e.Node.ImageIndex < 2) {
				e.Node.ImageIndex = e.Node.SelectedImageIndex = 1;
			}
			
			TreeNode[] nodes = new TreeNode[e.Node.Nodes.Count];
			e.Node.Nodes.CopyTo(nodes, 0);
			e.Node.Nodes.Clear();
			
			foreach (TreeNode node in nodes) {
				if (node.Tag is string) {
					
					XmlDocument doc = LoadCompressedXmlDocument(node.Tag.ToString());
					ParseTree(e.Node.Nodes, doc.DocumentElement);
				} else {
					e.Node.Nodes.Add(node);
				}
			}
			ShowHelp(e.Node);
		}
		
		void BeforeCollapse(object sender, TreeViewCancelEventArgs e)
		{ 
			if (e.Node.ImageIndex < 2) {
				e.Node.ImageIndex = e.Node.SelectedImageIndex = 0;
			}
		}
	}
}
