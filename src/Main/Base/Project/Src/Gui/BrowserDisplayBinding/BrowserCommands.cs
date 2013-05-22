// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.BrowserDisplayBinding
{
	public class GoBack : AbstractCommand
	{
		public override void Run()
		{
			((HtmlViewPane)Owner).WebBrowser.GoBack();
		}
	}
	
	public class GoForward : AbstractCommand
	{
		public override void Run()
		{
			((HtmlViewPane)Owner).WebBrowser.GoForward();
		}
	}
	
	public class Stop : AbstractCommand
	{
		public override void Run()
		{
			((HtmlViewPane)Owner).WebBrowser.Stop();
		}
	}
	
	public class Refresh : AbstractCommand
	{
		public override void Run()
		{
			if ((Control.ModifierKeys & Keys.Control) == Keys.Control)
				((HtmlViewPane)Owner).WebBrowser.Refresh(WebBrowserRefreshOption.Completely);
			else
				((HtmlViewPane)Owner).WebBrowser.Refresh();
		}
	}
	
	public class GoHome : AbstractCommand
	{
		public override void Run()
		{
			((HtmlViewPane)Owner).GoHome();
		}
	}
	
	public class GoSearch : AbstractCommand
	{
		public override void Run()
		{
			((HtmlViewPane)Owner).GoSearch();
		}
	}
	
	public class UrlComboBoxBuilder : IMenuItemBuilder
	{
		public IEnumerable<object> BuildItems(Codon codon, object parameter)
		{
			ToolStripComboBox toolbarItem = new ToolStripComboBox();
			ComboBox comboBox = toolbarItem.ComboBox;
			comboBox.Width *= 3;
			comboBox.DropDownStyle = ComboBoxStyle.DropDown;
			comboBox.Items.Clear();
			foreach (string url in PropertyService.GetList<string>("Browser.URLBoxHistory"))
				comboBox.Items.Add(url);
			comboBox.AutoCompleteMode   = AutoCompleteMode.Suggest;
			comboBox.AutoCompleteSource = AutoCompleteSource.HistoryList;
			((HtmlViewPane)parameter).SetUrlBox(comboBox);
			return new[] { toolbarItem };
		}
	}
	
	public class NewWindow : AbstractCommand
	{
		public override void Run()
		{
			SD.Workbench.ShowView(new BrowserPane());
		}
	}
}
