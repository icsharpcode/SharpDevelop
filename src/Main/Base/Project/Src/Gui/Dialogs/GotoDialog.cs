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
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

using ICSharpCode.Core;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem.Implementation;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;
using ICSharpCode.SharpDevelop.Parser;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Gui
{
	public partial class GotoDialog : Window
	{
		static GotoDialog Instance = null;
		
		public static void ShowSingleInstance()
		{
			if (Instance == null) {
				Instance = new GotoDialog();
				Instance.Owner = SD.Workbench.MainWindow;
				Instance.Show();
			} else {
				Instance.Activate();
			}
		}
		
		public GotoDialog()
		{
			InitializeComponent();
			FormLocationHelper.ApplyWindow(this, "ICSharpCode.SharpDevelop.Gui.GotoDialog.Bounds", true);
			SD.ParserService.LoadSolutionProjectsThread.Finished += ParserService_LoadSolutionProjectsThreadEnded;
			textBox.Focus();
		}
		
		protected override void OnClosed(EventArgs e)
		{
			Instance = null;
			SD.ParserService.LoadSolutionProjectsThread.Finished -= ParserService_LoadSolutionProjectsThreadEnded;
			base.OnClosed(e);
		}
		
		void ParserService_LoadSolutionProjectsThreadEnded(object sender, EventArgs e)
		{
			// refresh the list box contents when parsing has completed
			textBoxTextChanged(null, null);
		}
		
		class GotoEntry : IComparable<GotoEntry>
		{
			public object Tag;
			public string Text { get; private set; }
			public bool InCurrentFile { get; private set; }
			IImage image;
			int matchType;
			
			public ImageSource ImageSource {
				get { return image.ImageSource; }
			}
			
			public GotoEntry(string text, IImage image, int matchType, bool inCurrentFile)
			{
				this.Text = text;
				this.image = image;
				this.matchType = matchType;
				this.InCurrentFile = inCurrentFile;
			}
			
			public int CompareTo(GotoEntry other)
			{
				if ((matchType < MatchType_FullMatch_CaseInsensitive) && (other.matchType < MatchType_FullMatch_CaseInsensitive))
				{
					if (InCurrentFile && !other.InCurrentFile)
						return -1;
					if (!InCurrentFile && other.InCurrentFile)
						return 1;
				}
				int r = matchType.CompareTo(other.matchType);
				if (r != 0)
					return -r;
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
			} else if (e.Key == Key.Tab) {
				e.Handled = true;
				CompleteSelectedEntry();
			}
		}
		
		private void CompleteSelectedEntry()
		{
			if (listBox.SelectedItem == null) {
				return;
			}
			
			string completed = null;
			bool partlyComplete = false;
			
			object tag = ((GotoEntry) listBox.SelectedItem).Tag;
			if (tag is IUnresolvedEntity) {
				IUnresolvedEntity c = tag as IUnresolvedEntity;
				completed = c.Name;
				partlyComplete = (tag is IUnresolvedMember);
			} else if (tag is IEntity) {
				IEntity m = tag as IEntity;
				completed = m.Name;
				partlyComplete = (tag is IMember);
			} else if (tag is FileLineReference) {
				FileLineReference flref = tag as FileLineReference;
				// Only complete if we are not matching with a concrete number
				if (flref.Line == 0) {
					completed = Path.GetFileName(flref.FileName);
				} else {
					return;
				}
			} else {
				// Unsupported list item, do nothing
				return;
			}
			
			if (completed != null) {
				string currentText = textBox.Text;
				int dotPos = currentText.IndexOf('.');
				string needle = currentText;
				string member = null;
				if (dotPos > 0) {
					needle = currentText.Substring(0, dotPos).Trim();
					member = currentText.Substring(dotPos + 1).Trim();
				}
				
				// Replace the text, set caret to end, so user can continue typing
				if (partlyComplete && (member != null)) {
					// Only replace the part after the dot
					textBox.Text = needle + "." + completed;
				} else {
					textBox.Text = completed;
				}
				textBox.CaretIndex = textBox.Text.Length;
			}
		}
		
		void ChangeIndex(int increment)
		{
			int index = listBox.SelectedIndex;
			index = Math.Max(0, Math.Min(listBox.Items.Count - 1, index + increment));
			listBox.SelectedIndex = index;
			listBox.ScrollIntoView(listBox.Items[index]);
		}
		
		HashSet<string> visibleEntries = new HashSet<string>();
		List<GotoEntry> newItems = new List<GotoEntry>();
		
		void textBoxTextChanged(object sender, TextChangedEventArgs e)
		{
			string text = textBox.Text.Trim();
			listBox.ItemsSource = null;
			newItems.Clear();
			visibleEntries.Clear();
			if (text.Length == 0) {
				return;
			}
			if (text.Length == 1 && !char.IsDigit(text, 0)) {
				return;
			}
			
			int dotPos = text.IndexOf('.');
			string needle = text;
			string member = null;
			if (dotPos > 0) {
				needle = text.Substring(0, dotPos).Trim();
				member = text.Substring(dotPos + 1).Trim();
			}
			
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
				Match m = Regex.Match(line, @"^\w+ (\d+)$");
				if (m.Success) {
					// remove the word "line" (or some localized version of it)
					line = m.Groups[1].Value;
				}
				int lineNr;
				if (!int.TryParse(line, out lineNr))
					lineNr = 0;
				AddSourceFiles(file, lineNr);
			} else {
				AddSourceFiles(text, 0);
				foreach (IUnresolvedTypeDefinition c in SearchClasses(needle)) {
					if (!String.IsNullOrEmpty(member)) {
						AddAllMembersMatchingText(c, member, false);
					} else {
						AddItem(c, GetMatchType(needle, c.Name), false);
					}
				}
				AddAllMembersMatchingText(text);
				AddAllExtensionMethodsMatchingText(text);
			}
			newItems.Sort();
			listBox.ItemsSource = newItems;
			if (newItems.Count > 0) {
				listBox.SelectedItem = newItems[0];
			}
		}
		
		void AddAllExtensionMethodsMatchingText(string text)
		{
			List<IUnresolvedTypeDefinition> list = new List<IUnresolvedTypeDefinition>();
			if (ProjectService.OpenSolution != null) {
				foreach (IProject project in ProjectService.OpenSolution.Projects) {
					IProjectContent projectContent = project.ProjectContent;
					if (projectContent != null) {
						foreach (IUnresolvedTypeDefinition c in projectContent.GetAllTypeDefinitions()) {
							AddAllExtensionMethodsMatchingText(c, text);
						}
					}
				}
			}
		}
		
		void AddAllExtensionMethodsMatchingText(IUnresolvedTypeDefinition c, string text)
		{
			foreach (IUnresolvedMember m in c.Members) {
				var defMethod = m as DefaultUnresolvedMethod;
				if ((defMethod != null) && defMethod.IsExtensionMethod)
					AddItemIfMatchText(text, m, ClassBrowserIconService.GetIcon(m), false);
			}
		}
		
		void AddAllMembersMatchingText(string text)
		{
			ITextEditor editor = GetEditor();
			if (editor != null) {
				IUnresolvedFile parseInfo = SD.ParserService.GetExistingUnresolvedFile(editor.FileName);
				if (parseInfo != null) {
					foreach (IUnresolvedTypeDefinition c in parseInfo.TopLevelTypeDefinitions) {
						AddAllMembersMatchingText(c, text, true);
					}
				}
			}
		}

		void AddAllMembersMatchingText(IUnresolvedTypeDefinition c, string text, bool inCurrentFile)
		{
			foreach (IUnresolvedTypeDefinition innerClass in c.NestedTypes) {
				AddAllMembersMatchingText(innerClass, text, inCurrentFile);
			}
			foreach (IUnresolvedMember m in c.Members) {
				if (m.SymbolKind != SymbolKind.Constructor) {
					AddItemIfMatchText(text, m, ClassBrowserIconService.GetIcon(m), inCurrentFile);
				}
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
					AddItem(display, ClassBrowserIconService.GotoArrow, new FileLineReference(fileName, lineNumber), matchType, false);
				}
			}
		}
		
		void ShowLineNumberItem(string text)
		{
			int num;
			if (int.TryParse(text, out num)) {
				ITextEditor editor = GetEditor();
				if (editor != null) {
					num = Math.Min(editor.Document.LineCount, Math.Max(1, num));
					AddItem(StringParser.Parse("${res:Dialog.Goto.GotoLine} ") + num, ClassBrowserIconService.GotoArrow, num, int.MaxValue, false);
				}
			}
		}
		
		List<IUnresolvedTypeDefinition> SearchClasses(string text)
		{
			List<IUnresolvedTypeDefinition> list = new List<IUnresolvedTypeDefinition>();
			if (ProjectService.OpenSolution != null) {
				foreach (IProject project in ProjectService.OpenSolution.Projects) {
					IProjectContent projectContent = project.ProjectContent;
					if (projectContent != null) {
						foreach (IUnresolvedTypeDefinition c in projectContent.GetAllTypeDefinitions()) {
							string className = c.Name;
							if (className.Length >= text.Length) {
								if (GotoUtils.AutoCompleteWithCamelHumpsMatch(className, text)) {
									list.Add(c);
								}
							}
						}
					}
				}
			}
			return list;
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
			if (indexInsensitive < 0) {
				if (GotoUtils.AutoCompleteWithCamelHumpsMatch(itemText, searchText)) {
					return MatchType_ContainsMatch_CaseInsensitive;
				}
				return MatchType_NoMatch;
			}
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
		
		void AddItem(string text, IImage image, object tag, int matchType, bool inCurrentFile)
		{
			if (!visibleEntries.Add(text))
				return;
			GotoEntry item = new GotoEntry(text, image, matchType, inCurrentFile);
			item.Tag = tag;
			newItems.Add(item);
		}
		
		void AddItem(IUnresolvedTypeDefinition c, int matchType, bool inCurrentFile)
		{
			AddItem(c, ClassBrowserIconService.GetIcon(c), matchType, inCurrentFile);
		}
		
		void AddItemIfMatchText(string text, IUnresolvedMember member, IImage image, bool inCurrentFile)
		{
			string name = member.Name;
			int matchType = GetMatchType(text, name);
			if (matchType >= 0) {
				AddItem(member, image, matchType, inCurrentFile);
			}
		}
		
		void AddItem(IUnresolvedEntity e, IImage image, int matchType, bool inCurrentFile)
		{
			AddItem(e.Name + " (" + e.FullName + ")", image, e, matchType, inCurrentFile);
		}
		
		void cancelButtonClick(object sender, RoutedEventArgs e)
		{
			Close();
		}
		
		void GotoRegion(DomRegion region)
		{
			if (!region.IsEmpty && !string.IsNullOrEmpty(region.FileName)) {
				FileService.JumpToFilePosition(region.FileName, region.BeginLine, region.BeginColumn);
			}
		}
		
		ITextEditor GetEditor()
		{
			return SD.GetActiveViewContentService<ITextEditor>();
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
						int i = Math.Min(editor.Document.LineCount, Math.Max(1, (int)tag));
						editor.JumpTo(i, int.MaxValue);
					}
				} else if (tag is IUnresolvedEntity) {
					IUnresolvedEntity c = tag as IUnresolvedEntity;
					CodeCompletionDataUsageCache.IncrementUsage(c.ReflectionName);
					GotoRegion(c.Region);
				} else if (tag is IEntity) {
					IEntity m = tag as IEntity;
					CodeCompletionDataUsageCache.IncrementUsage(m.ReflectionName);
					GotoRegion(m.Region);
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
		
		void ListItem_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			if (e.ClickCount == 2) {
				okButtonClick(sender, e);
			}
		}
	}
	
	public class GotoUtils
	{
		public static bool AutoCompleteWithCamelHumpsMatch(string entityName, string entityPartName)
		{
			string camelHumpsPrefix = new string(entityName.Where(Char.IsUpper).ToArray());
			return AutoCompleteMatch(entityName, entityPartName) || AutoCompleteMatch(camelHumpsPrefix, entityPartName);
		}
		
		public static bool AutoCompleteMatch(string entityName, string entityPartName)
		{
			return entityName.IndexOf(entityPartName, StringComparison.OrdinalIgnoreCase) >= 0;
		}
	}
}
