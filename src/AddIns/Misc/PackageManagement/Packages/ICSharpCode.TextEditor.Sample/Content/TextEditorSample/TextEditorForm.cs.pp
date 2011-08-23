
using System;
using System.Drawing;
using System.Windows.Forms;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;
using ICSharpCode.TextEditor.Gui.CompletionWindow;

namespace $rootnamespace$.TextEditorSample
{
	public partial class TextEditorForm : Form
	{
		string currentFileName;
		TextArea textArea;
		CodeCompletionWindow completionWindow;
		
		public TextEditorForm()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			Init();
		}
		
		void Init()
		{
			propertyGridComboBox.SelectedIndex = 1;
			textArea = textEditorControl.ActiveTextAreaControl.TextArea;
			textArea.KeyEventHandler += ProcessKey;
			
			PopulateHighlightingComboBox();
			
			highlightingToolStripComboBox.Text = "C#";
		}

		bool ProcessKey(char ch)
		{
			if (ch == '.') {
				ShowCompletionWindow();
			}
			return false;
		}
		
		void ShowCompletionWindow()
		{
			CompletionDataProvider completionDataProvider = new CompletionDataProvider();
			completionWindow = CodeCompletionWindow.ShowCompletionWindow(this, textEditorControl, String.Empty, completionDataProvider, '.');
			if (completionWindow != null) {
				completionWindow.Closed += CompletionWindowClosed;
			}
		}
		
		void CompletionWindowClosed(object source, EventArgs e)
		{
			if (completionWindow != null) {
				completionWindow.Closed -= CompletionWindowClosed;
				completionWindow.Dispose();
				completionWindow = null;
			}
		}
		
		void PopulateHighlightingComboBox()
		{
			foreach (string highlighting in HighlightingManager.Manager.HighlightingDefinitions.Keys) {
				highlightingToolStripComboBox.Items.Add(highlighting);
			}
			highlightingToolStripComboBox.Sorted = true;
		}
		
		void ExitToolStripMenuItemClick(object sender, EventArgs e)
		{
			Close();
		}
		
		void PropertyGridComboBoxSelectedIndexChanged(object sender, EventArgs e)
		{
			switch (propertyGridComboBox.SelectedIndex) {
				case 0:
					propertyGrid.SelectedObject = textEditorControl.TextEditorProperties;
					break;
				case 1:
					propertyGrid.SelectedObject = textEditorControl;
					break;
			}
		}
		
		void OpenToolStripMenuItemClick(object sender, EventArgs e)
		{
			using (OpenFileDialog openFileDialog = new OpenFileDialog()) {
				openFileDialog.CheckFileExists = true;
				if (openFileDialog.ShowDialog() == DialogResult.OK) {
					currentFileName = openFileDialog.FileName;
					textEditorControl.LoadFile(currentFileName);
					highlightingToolStripComboBox.Text = textEditorControl.Document.HighlightingStrategy.Name;
				}
			}
		}
		
		void SaveToolStripMenuItemClick(object sender, EventArgs e)
		{
			if (currentFileName == null) {
				using (SaveFileDialog saveFileDialog = new SaveFileDialog()) {
					if (saveFileDialog.ShowDialog() == DialogResult.OK) {
						currentFileName = saveFileDialog.FileName;
					} else {
						return;
					}
				}
			}
			textEditorControl.SaveFile(currentFileName);
		}
		
		void CutToolStripMenuItemClick(object sender, EventArgs e)
		{
			textArea.ClipboardHandler.Cut(sender, e);
		}
		
		void UndoToolStripMenuItemClick(object sender, EventArgs e)
		{
			textEditorControl.Undo();
		}
		
		void RedoToolStripMenuItemClick(object sender, EventArgs e)
		{
			textEditorControl.Redo();
		}
		
		void CopyToolStripMenuItemClick(object sender, EventArgs e)
		{
			textArea.AutoClearSelection = false;
			textArea.ClipboardHandler.Copy(sender, e);
		}
		
		void PasteToolStripMenuItemClick(object sender, EventArgs e)
		{
			textArea.ClipboardHandler.Paste(sender, e);
		}
		
		void DeleteToolStripMenuItemClick(object sender, EventArgs e)
		{
			textArea.ClipboardHandler.Delete(sender, e);
		}
		
		void HighlightingToolStripComboBoxSelectedIndexChanged(object sender, EventArgs e)
		{
			string highlighter = highlightingToolStripComboBox.Text;
			textEditorControl.SetHighlighting(highlighter);
			textEditorControl.Refresh();
		}
	}
}
