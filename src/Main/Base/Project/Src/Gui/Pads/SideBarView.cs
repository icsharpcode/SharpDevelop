// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using System.Drawing;

using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.Core;

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
			AxSideTab tab = new AxSideTab(sideBar, "${res:SharpDevelop.SideBar.GeneralCategory}");
			
			sideBar.Tabs.Add(tab);
			sideBar.ActiveTab = tab;
			
			tab = new AxSideTab(sideBar, "${res:SharpDevelop.SideBar.ClipboardRing}");
			tab.IsClipboardRing = true;
			tab.CanBeDeleted = false;
			tab.CanDragDrop  = false;
			sideBar.Tabs.Add(tab);
		}
		
		public static void PutInClipboardRing(string text)
		{
			if (sideBar != null) {
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
