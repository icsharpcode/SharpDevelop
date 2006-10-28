// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using System.Windows.Forms;
using System.Xml;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Widgets.SideBar;

namespace ICSharpCode.SharpDevelop.Gui
{
	public class SideBarView : IPadContent, IDisposable
	{
		public Control Control {
			get {
				return sideBar;
			}
		}
		
		public void RedrawContent()
		{
			if (sideBar != null) {
				sideBar.Refresh();
			}
		}
		
		public void Dispose()
		{
			if (sideBar != null) {
				SaveSideBarViewConfig();
				sideBar.Dispose();
				sideBar = null;
			}
		}
		
		public static SharpDevelopSideBar sideBar = null;
		
		public SideBarView()
		{
			try {
				XmlDocument doc = new XmlDocument();
				
				doc.Load(Path.Combine(PropertyService.ConfigDirectory, "SideBarConfig.xml"));
				if (doc.DocumentElement.Attributes["version"] == null || doc.DocumentElement.Attributes["version"].InnerText != "1.0") {
					GenerateStandardSideBar();
				} else {
					sideBar = new SharpDevelopSideBar(doc.DocumentElement["SideBar"]);
				}
			} catch (Exception) {
				GenerateStandardSideBar();
			}
			
			sideBar.Dock = DockStyle.Fill;
		}
		
		void GenerateStandardSideBar()
		{
			sideBar = new SharpDevelopSideBar();
			SideTab tab = new SideTab(sideBar, "${res:SharpDevelop.SideBar.GeneralCategory}");
			tab.DisplayName = StringParser.Parse(tab.Name);
			
			sideBar.Tabs.Add(tab);
			sideBar.ActiveTab = tab;
			
			tab = new SideTab(sideBar, "${res:SharpDevelop.SideBar.ClipboardRing}");
			tab.DisplayName = StringParser.Parse(tab.Name);
			tab.IsClipboardRing = true;
			tab.CanBeDeleted = false;
			tab.CanDragDrop  = false;
			sideBar.Tabs.Add(tab);
		}
		
		public static void PutInClipboardRing(string text)
		{
			if (sideBar == null) {
				WorkbenchSingleton.Workbench.GetPad(typeof(SideBarView)).CreatePad();
			} else {
				sideBar.PutInClipboardRing(text);
				sideBar.Refresh();
			}
		}
		
		public void SaveSideBarViewConfig()
		{
			if (sideBar != null) {
				XmlDocument doc = new XmlDocument();
				doc.LoadXml("<SideBarConfig version=\"1.0\"/>");
				doc.DocumentElement.AppendChild(sideBar.ToXmlElement(doc));
				
				FileUtility.ObservedSave(new NamedFileOperationDelegate(doc.Save),
				                         Path.Combine(PropertyService.ConfigDirectory, "SideBarConfig.xml"),
				                         FileErrorPolicy.ProvideAlternative);
			}
		}
	}
}
