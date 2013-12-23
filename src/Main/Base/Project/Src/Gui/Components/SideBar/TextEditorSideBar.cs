// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Templates;
using ICSharpCode.SharpDevelop.Widgets.SideBar;

namespace ICSharpCode.SharpDevelop.Gui
{
	/// <summary>
	/// The tools box shown when using the text editor.
	/// </summary>
	public sealed class TextEditorSideBar : SharpDevelopSideBar
	{
		static TextEditorSideBar instance;
		
		public static TextEditorSideBar Instance {
			get {
				SD.MainThread.VerifyAccess();
				if (instance == null) {
					instance = new TextEditorSideBar();
					instance.Initialize();
				}
				return instance;
			}
		}
		
		SideTab clipboardRing;
		
		private TextEditorSideBar()
		{
		}
		
		private void Initialize()
		{
			foreach (TextTemplateGroup template in SD.Templates.TextTemplates) {
				SideTab tab = new SideTab(this, template.Name);
				tab.DisplayName = StringParser.Parse(tab.Name);
				tab.CanSaved  = false;
				foreach (TextTemplate entry in template.Entries)  {
					tab.Items.Add(SideTabItemFactory.CreateSideTabItem(entry.Display, entry.Value));
				}
				tab.CanBeDeleted = tab.CanDragDrop = false;
				Tabs.Add(tab);
			}
			
			try {
				XmlDocument doc = new XmlDocument();
				
				doc.Load(Path.Combine(PropertyService.ConfigDirectory, "SideBarConfig.xml"));
				if (doc.DocumentElement.GetAttribute("version") != "1.0") {
					GenerateStandardSideBar();
				} else {
					LoadSideBarConfig(doc.DocumentElement["SideBar"]);
				}
			} catch (FileNotFoundException) {
				// do not show a warning when the side bar file does not exist
				GenerateStandardSideBar();
			} catch (Exception ex) {
				MessageService.ShowWarning(ex.ToString());
				GenerateStandardSideBar();
			}
			
			WorkbenchSingleton.WorkbenchUnloaded += delegate { SaveSideBarViewConfig(); };
		}
		
		void GenerateStandardSideBar()
		{
			clipboardRing = new SideTab(this, "${res:SharpDevelop.SideBar.ClipboardRing}");
			clipboardRing.DisplayName = StringParser.Parse(clipboardRing.Name);
			clipboardRing.CanBeDeleted = false;
			clipboardRing.CanDragDrop  = false;
			this.Tabs.Add(clipboardRing);
			this.ActiveTab = clipboardRing;
		}
		
		public List<string> GetClipboardRingItems()
		{
			var list = new List<string>();
			foreach (var item in clipboardRing.Items) {
				string itemData = item.Tag as string;
				if (itemData != null)
					list.Add(itemData);
			}
			return list;
		}
		
		public void PutInClipboardRing(string text)
		{
			if (clipboardRing != null) {
				string shortenedText = text.Trim();
				if (shortenedText == String.Empty)
					return;
				
				if (shortenedText.Length > 50)
					shortenedText = shortenedText.Substring(0, 47) + "...";
				
				RemoveFromClipboardRing(text);
				clipboardRing.Items.Insert(0, shortenedText, text);
				if (clipboardRing.Items.Count > 20) {
					clipboardRing.Items.RemoveAt(clipboardRing.Items.Count - 1);
				}
			}
			Refresh();
		}
		
		void RemoveFromClipboardRing(string text) 
		{
			int pos = 0;
			foreach (var item in clipboardRing.Items) {
				string itemData = item.Tag as string;
				if(itemData != null && itemData.Equals(text))
					break;
				pos++;
			}
				
			if (pos < clipboardRing.Items.Count) {
				clipboardRing.Items.RemoveAt(pos);
			}
		}
		
		public void SaveSideBarViewConfig()
		{
			XmlDocument doc = new XmlDocument();
			doc.LoadXml("<SideBarConfig version=\"1.0\"/>");
			doc.DocumentElement.AppendChild(WriteConfig(doc));
			
			FileUtility.ObservedSave(fileName => doc.Save(fileName),
			                         FileName.Create(Path.Combine(PropertyService.ConfigDirectory, "SideBarConfig.xml")),
			                         FileErrorPolicy.ProvideAlternative);
		}
		
		void LoadSideBarConfig(XmlElement el)
		{
			foreach (XmlElement sideTabEl in el.ChildNodes) {
				SideTab tab = new SideTab(this, sideTabEl.GetAttribute("text"));
				tab.DisplayName = StringParser.Parse(tab.Name);
				if (tab.Name == el.GetAttribute("activetab")) {
					ActiveTab = tab;
				} else {
					if (ActiveTab == null) {
						ActiveTab = tab;
					}
				}
				
				foreach (XmlElement sideTabItemEl in sideTabEl.ChildNodes) {
					tab.Items.Add(SideTabItemFactory.CreateSideTabItem(sideTabItemEl.GetAttribute("text"),
					                                                   sideTabItemEl.GetAttribute("value")));
				}
				
				if (sideTabEl.GetAttribute("clipboardring") == "true") {
					tab.CanBeDeleted = false;
					tab.CanDragDrop  = false;
					tab.Name         = "${res:SharpDevelop.SideBar.ClipboardRing}";
					tab.DisplayName  = StringParser.Parse(tab.Name);
					clipboardRing = tab;
				}
				Tabs.Add(tab);
			}
		}
		
		XmlElement WriteConfig(XmlDocument doc)
		{
			if (doc == null) {
				throw new ArgumentNullException("doc");
			}
			XmlElement el = doc.CreateElement("SideBar");
			el.SetAttribute("activetab", ActiveTab.Name);
			
			foreach (SideTab tab in Tabs) {
				if (tab.CanSaved) {
					XmlElement child = doc.CreateElement("SideTab");
					
					if (tab == clipboardRing) {
						child.SetAttribute("clipboardring", "true");
					}
					
					child.SetAttribute("text", tab.Name);
					
					foreach (SideTabItem item in tab.Items) {
						XmlElement itemChild = doc.CreateElement("SideTabItem");
						
						itemChild.SetAttribute("text",  item.Name);
						itemChild.SetAttribute("value", item.Tag.ToString());
						
						child.AppendChild(itemChild);
					}
					el.AppendChild(child);
				}
			}
			
			return el;
		}
		
		protected override object StartItemDrag(SideTabItem draggedItem)
		{
			if (this.ActiveTab.ChoosedItem != draggedItem && this.ActiveTab.Items.Contains(draggedItem)) {
				this.ActiveTab.ChoosedItem = draggedItem;
			}
			var dataObject = new System.Windows.DataObject();
			dataObject.SetText(draggedItem.Tag.ToString());
			return dataObject;
		}
	}
}
