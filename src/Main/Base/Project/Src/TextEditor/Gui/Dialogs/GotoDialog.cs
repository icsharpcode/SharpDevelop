// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.IO;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using System.Resources;

using ICSharpCode.TextEditor.Document;
using ICSharpCode.TextEditor.Gui.CompletionWindow;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;

using ICSharpCode.TextEditor;
using ICSharpCode.SharpDevelop.Gui.XmlForms;

namespace ICSharpCode.SharpDevelop.Gui
{
	public class GotoDialog : BaseSharpDevelopForm
	{
		static GotoDialog Instance = null;
		
		public static void ShowSingleInstance()
		{
			if (Instance == null) {
				Instance = new GotoDialog();
				Instance.Show(WorkbenchSingleton.MainForm);
			} else {
				Instance.Focus();
			}
		}
		
		ListView listView;
		TextBox textBox;
		
		public GotoDialog()
		{
			SetupFromXmlStream(this.GetType().Assembly.GetManifestResourceStream("Resources.GotoDialog.xfrm"));
			ControlDictionary["okButton"].Click     += new EventHandler(OKButtonClick);
			ControlDictionary["cancelButton"].Click += new EventHandler(CancelButtonClick);
			listView = (ListView)ControlDictionary["listView"];
			textBox = (TextBox)ControlDictionary["textBox"];
			textBox.TextChanged += TextBoxTextChanged;
			textBox.KeyDown += TextBoxKeyDown;
			listView.SmallImageList = ClassBrowserIconService.ImageList;
			listView.ItemActivate += OKButtonClick;
			listView.Sorting = SortOrder.Ascending;
			listView.SizeChanged += ListViewSizeChanged;
			listView.HideSelection = false;
			ListViewSizeChanged(null, null);
			Owner = WorkbenchSingleton.MainForm;
			Icon = null;
			this.StartPosition = FormStartPosition.Manual;
			this.Bounds = PropertyService.Get("ICSharpCode.SharpDevelop.Gui.GotoDialog.Bounds", GetDefaultBounds());
		}
		
		Rectangle GetDefaultBounds()
		{
			Rectangle parent = WorkbenchSingleton.MainForm.Bounds;
			Size size = this.Size;
			return new Rectangle(parent.Left + (parent.Width - size.Width) / 2,
			                     parent.Top + (parent.Height - size.Height) / 2,
			                     size.Width, size.Height);
		}
		
		protected override void OnClosing(CancelEventArgs e)
		{
			base.OnClosing(e);
			PropertyService.Set("ICSharpCode.SharpDevelop.Gui.GotoDialog.Bounds", this.Bounds);
		}
		
		void ListViewSizeChanged(object sender, EventArgs e)
		{
			listView.Columns[0].Width = listView.Width - 24;
		}
		
		void TextBoxKeyDown(object sender, KeyEventArgs e)
		{
			if (listView.SelectedItems.Count == 0)
				return;
			if (e.KeyData == Keys.Up) {
				e.Handled = true;
				ChangeIndex(-1);
			} else if (e.KeyData == Keys.Down) {
				e.Handled = true;
				ChangeIndex(+1);
			} else if (e.KeyData == Keys.PageUp) {
				e.Handled = true;
				ChangeIndex(-listView.ClientSize.Height / listView.Items[0].Bounds.Height);
			} else if (e.KeyData == Keys.PageDown) {
				e.Handled = true;
				ChangeIndex(+listView.ClientSize.Height / listView.Items[0].Bounds.Height);
			}
		}
		
		void ChangeIndex(int increment)
		{
			int index = listView.SelectedIndices[0];
			index = Math.Max(0, Math.Min(listView.Items.Count - 1, index + increment));
			listView.Items[index].Selected = true;
			listView.EnsureVisible(index);
		}
		
		ICompletionData[] ctrlSpaceCompletionData;
		
		ICompletionData[] GetCompletionData()
		{
			if (ctrlSpaceCompletionData != null)
				return ctrlSpaceCompletionData;
			TextEditorControl editor = GetEditor();
			if (editor != null) {
				CtrlSpaceCompletionDataProvider cdp = new CtrlSpaceCompletionDataProvider(ExpressionContext.Default);
				ctrlSpaceCompletionData = cdp.GenerateCompletionData(editor.FileName, editor.ActiveTextAreaControl.TextArea, '\0');
				return ctrlSpaceCompletionData;
			}
			return new ICompletionData[0];
		}
		
		ICompletionData[] Resolve(string expression)
		{
			TextEditorControl editor = GetEditor();
			if (editor != null) {
				CodeCompletionDataProvider cdp = new CodeCompletionDataProvider(new ExpressionResult(expression));
				return cdp.GenerateCompletionData(editor.FileName, editor.ActiveTextAreaControl.TextArea, '.');
			}
			return new ICompletionData[0];
		}
		
		protected override void OnClosed(EventArgs e)
		{
			Instance = null;
			base.OnClosed(e);
		}
		
		Hashtable visibleEntries = new Hashtable();
		double bestPriority;
		ListViewItem bestItem;
		
		void TextBoxTextChanged(object sender, EventArgs e)
		{
			string text = textBox.Text.Trim();
			listView.Items.Clear();
			visibleEntries.Clear();
			bestItem = null;
			if (text.Length == 0)
				return;
			if (text.Length == 1 && !char.IsDigit(text, 0))
				return;
			listView.BeginUpdate();
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
			} else if (dotPos > 0) {
				AddSourceFiles(text, 0);
				string expression = text.Substring(0, dotPos).Trim();
				text = text.Substring(dotPos + 1).Trim();
				ShowCompletionData(Resolve(expression), text);
				foreach (IClass c in SearchClasses(expression)) {
					if (c.Name.Equals(expression, StringComparison.InvariantCultureIgnoreCase)) {
						foreach (IMethod m in c.DefaultReturnType.GetMethods()) {
							if (!m.IsConstructor) {
								AddItemIfMatchText(text, m, ClassBrowserIconService.GetIcon(m));
							}
						}
						foreach (IField f in c.DefaultReturnType.GetFields()) {
							AddItemIfMatchText(text, f, ClassBrowserIconService.GetIcon(f));
						}
						foreach (IProperty p in c.DefaultReturnType.GetProperties()) {
							AddItemIfMatchText(text, p, ClassBrowserIconService.GetIcon(p));
						}
						foreach (IEvent evt in c.DefaultReturnType.GetEvents()) {
							AddItemIfMatchText(text, evt, ClassBrowserIconService.GetIcon(evt));
						}
					}
				}
			} else {
				AddSourceFiles(text, 0);
				ShowCtrlSpaceCompletion(text);
			}
			if (bestItem != null) {
				bestItem.Selected = true;
				listView.EnsureVisible(bestItem.Index);
			}
			listView.EndUpdate();
		}
		
		void AddSourceFiles(string text, int lineNumber)
		{
			if (ProjectService.OpenSolution != null) {
				foreach (IProject project in ProjectService.OpenSolution.Projects) {
					foreach (ProjectItem item in project.Items) {
						switch (item.ItemType) {
							case ItemType.Compile:
							case ItemType.Content:
							case ItemType.EmbeddedResource:
							case ItemType.None:
								AddSourceFile(text, lineNumber, item);
								break;
						}
					}
				}
			}
		}
		
		void AddSourceFile(string text, int lineNumber, ProjectItem item)
		{
			string lowerText = text.ToLowerInvariant();
			string fileName = item.FileName;
			string display = Path.GetFileName(fileName);
			if (display.Length >= text.Length) {
				if (display.ToLowerInvariant().IndexOf(lowerText) >= 0) {
					if (lineNumber > 0) {
						display += ", line " + lineNumber;
					}
					if (item.Project != null) {
						display += " in " + item.Project.Name;
					}
					AddItem(display, ClassBrowserIconService.GotoArrowIndex, new FileLineReference(fileName, lineNumber), 0.5);
				}
			}
		}
		
		void ShowLineNumberItem(string text)
		{
			int num;
			if (int.TryParse(text, out num)) {
				TextEditorControl editor = GetEditor();
				if (editor != null) {
					num = Math.Min(editor.Document.TotalNumberOfLines, Math.Max(1, num));
					AddItem(StringParser.Parse("${res:Dialog.Goto.GotoLine} ") + num, ClassBrowserIconService.GotoArrowIndex, num, 0);
				}
			}
		}
		
		void ShowCompletionData(ICompletionData[] dataList, string text)
		{
			string lowerText = text.ToLowerInvariant();
			foreach (ICompletionData data in dataList) {
				CodeCompletionData ccd = data as CodeCompletionData;
				if (ccd == null)
					return;
				string dataText = ccd.Text;
				if (dataText.Length >= text.Length) {
					if (dataText.ToLowerInvariant().IndexOf(lowerText) >= 0) {
						if (ccd.Class != null)
							AddItem(ccd.Class, data.ImageIndex, data.Priority);
						else if (ccd.Member != null)
							AddItem(ccd.Member, data.ImageIndex, data.Priority);
					}
				}
			}
		}
		
		void ShowCtrlSpaceCompletion(string text)
		{
			ShowCompletionData(GetCompletionData(), text);
			foreach (IClass c in SearchClasses(text)) {
				AddItem(c);
			}
		}
		
		ArrayList SearchClasses(string text)
		{
			string lowerText = text.ToLowerInvariant();
			ArrayList list = new ArrayList();
			if (ProjectService.OpenSolution != null) {
				foreach (IProject project in ProjectService.OpenSolution.Projects) {
					IProjectContent projectContent = ParserService.GetProjectContent(project);
					if (projectContent != null) {
						foreach (IClass c in projectContent.Classes) {
							string className = c.Name;
							if (className.Length >= text.Length) {
								if (className.ToLowerInvariant().IndexOf(lowerText) >= 0) {
									list.Add(c);
								}
							}
						}
					}
				}
			}
			return list;
		}
		
		void AddItem(string text, int imageIndex, object tag, double priority)
		{
			if (visibleEntries.ContainsKey(text))
				return;
			visibleEntries.Add(text, null);
			ListViewItem item = new ListViewItem(text, imageIndex);
			item.Tag = tag;
			if (bestItem == null
			    || (priority > bestPriority && !(tag is IClass && bestItem.Tag is IMember))
			    || (tag is IMember && bestItem.Tag is IClass))
			{
				bestItem = item;
				bestPriority = priority;
			}
			listView.Items.Add(item);
		}
		
		void AddItem(IClass c)
		{
			AddItem(c, ClassBrowserIconService.GetIcon(c), CodeCompletionDataUsageCache.GetPriority(c.DotNetName, true));
		}
		
		void AddItemIfMatchText(string text, IMember member, int imageIndex)
		{
			string name = member.Name;
			if (name.Length >= text.Length) {
				if (text.Equals(name.Substring(0, text.Length), StringComparison.OrdinalIgnoreCase)) {
					AddItem(member, imageIndex, CodeCompletionDataUsageCache.GetPriority(member.DotNetName, true));
				}
			}
		}
		
		void AddItem(IClass c, int imageIndex, double priority)
		{
			AddItem(c.Name + " (" + c.FullyQualifiedName + ")", imageIndex, c, priority);
		}
		
		void AddItem(IMember m, int imageIndex, double priority)
		{
			AddItem(m.Name + " (" + m.FullyQualifiedName + ")", imageIndex, m, priority);
		}
		
		void CancelButtonClick(object sender, EventArgs e)
		{
			Close();
		}
		
		void GotoRegion(DomRegion region, string fileName)
		{
			if (fileName != null && !region.IsEmpty) {
				FileService.JumpToFilePosition(fileName, region.BeginLine - 1, region.BeginColumn - 1);
			}
		}
		
		TextEditorControl GetEditor()
		{
			IWorkbenchWindow window = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;
			if (window != null && window.ViewContent is ITextEditorControlProvider) {
				return ((ITextEditorControlProvider)window.ViewContent).TextEditorControl;
			}
			return null;
		}
		
		void OKButtonClick(object sender, EventArgs e)
		{
			try {
				if (listView.SelectedItems.Count == 0)
					return;
				object tag = listView.SelectedItems[0].Tag;
				if (tag is int) {
					TextEditorControl editor = GetEditor();
					if (editor != null) {
						int i = Math.Min(editor.Document.TotalNumberOfLines, Math.Max(1, (int)tag));
						editor.ActiveTextAreaControl.Caret.Line = i - 1;
						editor.ActiveTextAreaControl.ScrollToCaret();
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
						FileService.JumpToFilePosition(flref.FileName, flref.Line - 1, flref.Column);
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
