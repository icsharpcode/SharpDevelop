// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
