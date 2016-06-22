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
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui.XmlForms;
using ICSharpCode.SharpDevelop.Parser;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.WinForms;
using ICSharpCode.SharpDevelop.Workbench;

namespace ICSharpCode.SharpDevelop.Gui
{
	// TODO: rewrite without XMLForms
	#pragma warning disable 618
	class WordCountDialog : BaseSharpDevelopForm
	{
		List<Report> items;
		Report total;
		
		internal class Report
		{
			public string name;
			public long chars;
			public long words;
			public long lines;
			
			public Report(string name, long chars, long words, long lines)
			{
				this.name  = name;
				this.chars = chars;
				this.words = words;
				this.lines = lines;
			}
			
			public ListViewItem ToListItem()
			{
				return new ListViewItem(new string[] {Path.GetFileName(name), chars.ToString(), words.ToString(), lines.ToString()});
			}
			
			public static Report operator+(Report r, Report s)
			{
				
				Report tmp = new Report(ResourceService.GetString("Dialog.WordCountDialog.TotalText"), s.chars, s.words, s.lines);
				tmp.chars += r.chars;
				tmp.words += r.words;
				tmp.lines += r.lines;
				return tmp;
			}
		}
		
		Report GetReport(string filename)
		{
			if (!File.Exists(filename)) return null;
			
			using (var reader = SD.FileService.GetFileContent(filename).CreateReader()) {
				return GetReport(filename, reader);
			}
		}
		
		Report GetReport(IViewContent content, TextReader reader)
		{
			OpenedFile file = content.PrimaryFile;
			if (file != null && file.IsUntitled == false)
				return GetReport(file.FileName, reader);
			else
				return GetReport(content.TitleName, reader);
		}
		
		Report GetReport(string filename, TextReader reader)
		{
			long numLines = 0;
			long numWords = 0;
			long numChars = 0;
			
			
			string line = reader.ReadLine();
			while (line != null) {
				++numLines;
				numChars += line.Length;
				string[] words = line.Split(null);
				numWords += words.Length;
				line = reader.ReadLine();
			}
			
			return new Report(filename, numChars, numWords, numLines);
		}
		
		void startEvent(object sender, System.EventArgs e)
		{
			items = new List<Report>();
			total = null;
			
			switch (((ComboBox)ControlDictionary["locationComboBox"]).SelectedIndex) {
					case 0: {// current file
						IViewContent viewContent = SD.Workbench.ActiveViewContent;
						if (viewContent != null) {
							IEditable editable = viewContent.GetService<IEditable>();
							if (editable == null) {
								MessageService.ShowWarning("${res:Dialog.WordCountDialog.IsNotTextFile}");
							} else {
								Report r = GetReport(viewContent, editable.CreateSnapshot().CreateReader());
								if (r != null) items.Add(r);
							}
						}
						break;
					}
					case 1: {// all open files
						if (SD.Workbench.ViewContentCollection.Count > 0) {
							total = new Report(StringParser.Parse("${res:Dialog.WordCountDialog.TotalText}"), 0, 0, 0);
							foreach (IViewContent content in SD.Workbench.ViewContentCollection) {
								IEditable editable = content.GetService<IEditable>();
								if (editable != null) {
									Report r = GetReport(content, editable.CreateSnapshot().CreateReader());
									if (r != null) {
										total += r;
										items.Add(r);
									}
								}
							}
						}
						break;
					}
					case 2: {// whole project
						
						if (ProjectService.CurrentProject == null) {
							MessageService.ShowError("${res:Dialog.WordCountDialog.MustBeInProtectedModeWarning}");
							break;
						}
						total = new Report(StringParser.Parse("${res:Dialog.WordCountDialog.TotalText}"), 0, 0, 0);
						CountProject(ProjectService.CurrentProject, ref total);
						break;
					}
					case 3: { // whole solution
						if (ProjectService.OpenSolution == null) {
							MessageService.ShowError("${res:Dialog.WordCountDialog.MustBeInProtectedModeWarning}");
							break;
						}
						total = new Report(StringParser.Parse("${res:Dialog.WordCountDialog.TotalText}"), 0, 0, 0);
						CountSolution(ProjectService.OpenSolution, ref total);
						break;
					}
			}
			UpdateList(0);
		}
		
		void CountSolution(ISolution solution, ref Report all)
		{
			foreach (IProject project in solution.Projects) {
				CountProject(project, ref all);
			}
		}
		
		void CountProject(IProject project, ref Report all)
		{
			foreach (ProjectItem item in project.Items) {
				if (item.ItemType == ItemType.Compile) {
					Report r = GetReport(item.FileName);
					if (r != null) {
						all += r;
						items.Add(r);
					}
				}
			}
		}
		
		void UpdateList(int SortKey)
		{
			if (items == null) {
				return;
			}
			((ListView)ControlDictionary["resultListView"]).BeginUpdate();
			((ListView)ControlDictionary["resultListView"]).Items.Clear();
			
			if (items.Count == 0) {
				goto endupdate;
			}
			
			ReportComparer rc = new ReportComparer(SortKey);
			items.Sort(rc);
			
			for (int i = 0; i < items.Count; ++i) {
				((ListView)ControlDictionary["resultListView"]).Items.Add(((Report)items[i]).ToListItem());
			}
			
			if (total != null) {
				((ListView)ControlDictionary["resultListView"]).Items.Add(new ListViewItem(""));
				((ListView)ControlDictionary["resultListView"]).Items.Add(total.ToListItem());
			}
			
		endupdate:
			((ListView)ControlDictionary["resultListView"]).EndUpdate();
			
		}
		
		internal class ReportComparer : IComparer<Report>
		{
			int sortKey;
			
			public ReportComparer(int SortKey)
			{
				sortKey = SortKey;
			}
			
			public int Compare(Report x, Report y)
			{
				if (x == null || y == null) return 1;
				
				switch (sortKey) {
					case 0:  // files
						return String.Compare(x.name, y.name);
					case 1:  // chars
						return x.chars.CompareTo(y.chars);
					case 2:  // words
						return x.words.CompareTo(y.words);
					case 3:  // lines
						return x.lines.CompareTo(y.lines);
					default:
						return 1;
				}
			}
		}
		
		void SortEvt(object sender, ColumnClickEventArgs e)
		{
			UpdateList(e.Column);
		}
		
		public WordCountDialog()
		{
			InitializeComponents();
		}
		
		void InitializeComponents()
		{
			SetupFromXmlStream(this.GetType().Assembly.GetManifestResourceStream("ICSharpCode.SharpDevelop.Resources.WordCountDialog.xfrm"));
			
			((Button)ControlDictionary["startButton"]).Click += new System.EventHandler(startEvent);
			((ListView)ControlDictionary["resultListView"]).ColumnClick += new ColumnClickEventHandler(SortEvt);
			
			Icon = SD.ResourceService.GetIcon("Icons.16x16.FindIcon");
			
			((ComboBox)ControlDictionary["locationComboBox"]).Items.Add(StringParser.Parse("${res:Dialog.NewProject.SearchReplace.LookIn.CurrentDocument536D2AC6-E704-40BD-9790-0EB02ED6D8A9}"));
			((ComboBox)ControlDictionary["locationComboBox"]).Items.Add(StringParser.Parse("${res:Dialog.NewProject.SearchReplace.LookIn.AllOpenDocuments}"));
			((ComboBox)ControlDictionary["locationComboBox"]).Items.Add(StringParser.Parse("${res:Dialog.NewProject.SearchReplace.LookIn.WholeProject}"));
			((ComboBox)ControlDictionary["locationComboBox"]).Items.Add(StringParser.Parse("${res:Dialog.NewProject.SearchReplace.LookIn.WholeSolution}"));
			((ComboBox)ControlDictionary["locationComboBox"]).SelectedIndex = 0;
		}
	}
}
