// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using ICSharpCode.Core;
using ICSharpCode.Core.Presentation;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;
using ICSharpCode.SharpDevelop.Project;
using System.Windows.Media;

namespace ICSharpCode.SharpDevelop.Gui
{
	public partial class GotoDialog : Window
	{
		static GotoDialog Instance = null;
		
		public static void ShowSingleInstance()
		{
			if (Instance == null) {
				Instance = new GotoDialog();
				Instance.Owner = WorkbenchSingleton.MainWindow;
				Instance.Show();
			} else {
				Instance.Activate();
			}
		}
		
		public GotoDialog()
		{
			InitializeComponent();
			FormLocationHelper.ApplyWindow(this, "ICSharpCode.SharpDevelop.Gui.GotoDialog.Bounds", true);
			ParserService.LoadSolutionProjectsThreadEnded += ParserService_LoadSolutionProjectsThreadEnded;
			textBox.Focus();
		}
		
		protected override void OnClosed(EventArgs e)
		{
			Instance = null;
			ParserService.LoadSolutionProjectsThreadEnded -= ParserService_LoadSolutionProjectsThreadEnded;
			base.OnClosed(e);
		}
		
		void ParserService_LoadSolutionProjectsThreadEnded(object sender, EventArgs e)
		{
			// refresh the list box contents when parsing has completed
			Dispatcher.BeginInvoke(
				System.Windows.Threading.DispatcherPriority.Background,
				new Action(delegate { textBoxTextChanged(null, null); }));
		}
		
		class GotoEntry : IComparable<GotoEntry>
		{
			public object Tag;
			public string Text { get; private set; }
			IImage image;
			
			public ImageSource ImageSource {
				get { return image.ImageSource; }
			}
			
			public GotoEntry(string text, IImage image)
			{
				this.Text = text;
				this.image = image;
			}
			
			public int CompareTo(GotoEntry other)
			{
				return Text.CompareTo(other.Text);
			}
		}
		
		void textBoxPreviewKeyDown(object sender, KeyEventArgs e)
		{
			if (listBox.SelectedItem == null)
				return;
			if (e.Key == Key.Up) {
				e.Handled = true;
				ChangeIndex(-1);
			} else if (e.Key == Key.Down) {
				e.Handled = true;
				ChangeIndex(+1);
			} else if (e.Key == Key.PageUp) {
				e.Handled = true;
				ChangeIndex((int)Math.Round(-listBox.ActualHeight / 20));
			} else if (e.Key == Key.PageDown) {
				e.Handled = true;
				ChangeIndex((int)Math.Round(+listBox.ActualHeight / 20));
			}
		}
		
		void ChangeIndex(int increment)
		{
			int index = listBox.SelectedIndex;
			index = Math.Max(0, Math.Min(listBox.Items.Count - 1, index + increment));
			listBox.SelectedIndex = index;
			listBox.ScrollIntoView(listBox.Items[index]);
		}
		
		Dictionary<string, object> visibleEntries = new Dictionary<string, object>();
		int bestMatchType;
		double bestPriority;
		List<GotoEntry> newItems = new List<GotoEntry>();
		GotoEntry bestItem;
		
		void textBoxTextChanged(object sender, TextChangedEventArgs e)
		{
			string text = textBox.Text.Trim();
			listBox.ItemsSource = null;
			newItems.Clear();
			visibleEntries.Clear();
			bestItem = null;
			if (text.Length == 0) {
				return;
			}
			if (text.Length == 1 && !char.IsDigit(text, 0)) {
				return;
			}
			int dotPos = text.IndexOf('.');
			int commaPos = text.IndexOf(',');
			if (commaPos < 0) {
				// use "File, ##" or "File: ##" syntax for line numbers
				commaPos = text.IndexOf(':');
			}
			if (char.IsDigit(text, 0)) {
				ShowLineNumberItem(text);
			} else if (commaPos > 0) {
				string file = text.Substring(0, commaPos).Trim();
				string line = text.Substring(commaPos + 1).Trim();
				if (line.StartsWith("line")) {
					// remove the word "line"
					line = line.Substring(4).Trim();
				}
				int lineNr;
				if (!int.TryParse(line, out lineNr))
					lineNr = 0;
				AddSourceFiles(file, lineNr);
			} else {
				AddSourceFiles(text, 0);
				foreach (IClass c in SearchClasses(text)) {
					AddItem(c, GetMatchType(text, c.Name));
				}
				AddAllMembersMatchingText(text);
			}
			newItems.Sort();
			listBox.ItemsSource = newItems;
			if (bestItem != null) {
				listBox.SelectedItem = bestItem;
				listBox.ScrollIntoView(bestItem);
			}
		}
		
		void AddAllMembersMatchingText(string text)
		{
			ITextEditor editor = GetEditor();
			if (editor != null) {
				ParseInformation parseInfo = ParserService.GetExistingParseInformation(editor.FileName);
				if (parseInfo != null) {
					foreach (IClass c in parseInfo.CompilationUnit.Classes) {
						AddAllMembersMatchingText(c, text);
					}
				}
			}
		}

		void AddAllMembersMatchingText(IClass c, string text)
		{
			foreach (IClass innerClass in c.InnerClasses) {
				AddAllMembersMatchingText(innerClass, text);
			}
			foreach (IMethod m in c.Methods) {
				if (!m.IsConstructor) {
					AddItemIfMatchText(text, m, ClassBrowserIconService.GetIcon(m));
				}
			}
			foreach (IField f in c.Fields) {
				AddItemIfMatchText(text, f, ClassBrowserIconService.GetIcon(f));
			}
			foreach (IProperty p in c.Properties) {
				AddItemIfMatchText(text, p, ClassBrowserIconService.GetIcon(p));
			}
			foreach (IEvent evt in c.Events) {
				AddItemIfMatchText(text, evt, ClassBrowserIconService.GetIcon(evt));
			}
		}
		
		void AddSourceFiles(string text, int lineNumber)
		{
			if (ProjectService.OpenSolution != null) {
				foreach (IProject project in ProjectService.OpenSolution.Projects) {
					foreach (ProjectItem item in project.Items) {
						if (item is FileProjectItem && item.ItemType != ItemType.Folder) {
							AddSourceFile(text, lineNumber, item);
						}
					}
				}
			}
		}
		
		void AddSourceFile(string text, int lineNumber, ProjectItem item)
		{
			string fileName = item.FileName;
			string display = Path.GetFileName(fileName);
			if (display.Length >= text.Length) {
				int matchType = GetMatchType(text, display);
				if (matchType >= 0) {
					if (lineNumber > 0) {
						display += ", line " + lineNumber;
					}
					if (item.Project != null) {
						display += StringParser.Parse(" ${res:MainWindow.Windows.SearchResultPanel.In} ") + item.Project.Name;
					}
					AddItem(display, ClassBrowserIconService.GotoArrow, new FileLineReference(fileName, lineNumber), 0.5, matchType);
				}
			}
		}
		
		void ShowLineNumberItem(string text)
		{
			int num;
			if (int.TryParse(text, out num)) {
				ITextEditor editor = GetEditor();
				if (editor != null) {
					num = Math.Min(editor.Document.TotalNumberOfLines, Math.Max(1, num));
					AddItem(StringParser.Parse("${res:Dialog.Goto.GotoLine} ") + num, ClassBrowserIconService.GotoArrow, num, 0, int.MaxValue);
				}
			}
		}
		
		void ShowCompletionData(IList<ICompletionItem> dataList, string text)
		{
			string lowerText = text.ToLowerInvariant();
			foreach (CodeCompletionItem data in dataList.OfType<CodeCompletionItem>()) {
				string dataText = data.Text;
				int matchType = GetMatchType(text, dataText);
				if (matchType >= 0) {
					AddItem(data.Entity, data.Image, data.Priority, matchType);
				}
			}
		}
		
		ArrayList SearchClasses(string text)
		{
			ArrayList list = new ArrayList();
			if (ProjectService.OpenSolution != null) {
				foreach (IProject project in ProjectService.OpenSolution.Projects) {
					IProjectContent projectContent = ParserService.GetProjectContent(project);
					if (projectContent != null) {
						AddClasses(text, list, projectContent.Classes);
					}
				}
			}
			return list;
		}
		
		void AddClasses(string text, ArrayList list, IEnumerable<IClass> classes)
		{
			foreach (IClass c in classes) {
				string className = c.Name;
				if (className.Length >= text.Length) {
					if (className.IndexOf(text, StringComparison.OrdinalIgnoreCase) >= 0) {
						list.Add(c);
					}
				}
				AddClasses(text, list, c.InnerClasses);
			}
		}
		
		const int MatchType_NoMatch = -1;
		const int MatchType_ContainsMatch_CaseInsensitive = 0;
		const int MatchType_ContainsMatch = 1;
		const int MatchType_StartingMatch_CaseInsensitive = 2;
		const int MatchType_StartingMatch = 3;
		const int MatchType_FullMatch_CaseInsensitive = 4;
		const int MatchType_FullMatch = 5;
		
		static int GetMatchType(string searchText, string itemText)
		{
			if (itemText.Length < searchText.Length)
				return MatchType_NoMatch;
			int indexInsensitive = itemText.IndexOf(searchText, StringComparison.OrdinalIgnoreCase);
			if (indexInsensitive < 0)
				return MatchType_NoMatch;
			// This is a case insensitive match
			int indexSensitive = itemText.IndexOf(searchText, StringComparison.Ordinal);
			if (itemText.Length == searchText.Length) {
				// this is a full match
				if (indexSensitive == 0)
					return MatchType_FullMatch;
				else
					return MatchType_FullMatch_CaseInsensitive;
			} else if (indexInsensitive == 0) {
				// This is a starting match
				if (indexSensitive == 0)
					return MatchType_StartingMatch;
				else
					return MatchType_StartingMatch_CaseInsensitive;
			} else {
				if (indexSensitive >= 0)
					return MatchType_ContainsMatch;
				else
					return MatchType_ContainsMatch_CaseInsensitive;
			}
		}
		
		void AddItem(string text, IImage image, object tag, double priority, int matchType)
		{
			if (visibleEntries.ContainsKey(text))
				return;
			visibleEntries.Add(text, null);
			GotoEntry item = new GotoEntry(text, image);
			item.Tag = tag;
			if (bestItem == null
			    || (tag is IMember && bestItem.Tag is IClass)
			    || (!(tag is IClass && bestItem.Tag is IMember)
			        && (matchType > bestMatchType || matchType == bestMatchType && priority > bestPriority)))
			{
				bestItem = item;
				bestPriority = priority;
				bestMatchType = matchType;
			}
			newItems.Add(item);
		}
		
		void AddItem(IClass c, int matchType)
		{
			AddItem(c, ClassBrowserIconService.GetIcon(c), CodeCompletionDataUsageCache.GetPriority(c.DotNetName, true), matchType);
		}
		
		void AddItemIfMatchText(string text, IMember member, IImage image)
		{
			string name = member.Name;
			int matchType = GetMatchType(text, name);
			if (matchType >= 0) {
				AddItem(member, image, CodeCompletionDataUsageCache.GetPriority(member.DotNetName, true), matchType);
			}
		}
		
		void AddItem(IEntity e, IImage image, double priority, int matchType)
		{
			AddItem(e.Name + " (" + e.FullyQualifiedName + ")", image, e, priority, matchType);
		}
		
		void cancelButtonClick(object sender, RoutedEventArgs e)
		{
			Close();
		}
		
		void GotoRegion(DomRegion region, string fileName)
		{
			if (fileName != null && !region.IsEmpty) {
				FileService.JumpToFilePosition(fileName, region.BeginLine, region.BeginColumn);
			}
		}
		
		ITextEditor GetEditor()
		{
			IViewContent viewContent = WorkbenchSingleton.Workbench.ActiveViewContent;
			if (viewContent is ITextEditorProvider) {
				return ((ITextEditorProvider)viewContent).TextEditor;
			}
			return null;
		}
		
		void okButtonClick(object sender, RoutedEventArgs e)
		{
			try {
				if (listBox.SelectedItem == null)
					return;
				object tag = ((GotoEntry)listBox.SelectedItem).Tag;
				if (tag is int) {
					ITextEditor editor = GetEditor();
					if (editor != null) {
						int i = Math.Min(editor.Document.TotalNumberOfLines, Math.Max(1, (int)tag));
						editor.JumpTo(i, int.MaxValue);
					}
				} else if (tag is IClass) {
					IClass c = tag as IClass;
					CodeCompletionDataUsageCache.IncrementUsage(c.DotNetName);
					GotoRegion(c.Region, c.CompilationUnit.FileName);
				} else if (tag is IMember) {
					IMember m = tag as IMember;
					CodeCompletionDataUsageCache.IncrementUsage(m.DotNetName);
					GotoRegion(m.Region, m.DeclaringType.CompilationUnit.FileName);
				} else if (tag is FileLineReference) {
					FileLineReference flref = (FileLineReference)tag;
					if (flref.Line <= 0) {
						FileService.OpenFile(flref.FileName);
					} else {
						FileService.JumpToFilePosition(flref.FileName, flref.Line, flref.Column);
					}
				} else {
					throw new NotImplementedException("Unknown tag: " + tag);
				}
			} finally {
				Close();
			}
		}
	}
}
