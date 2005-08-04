//
// SharpDevelop Xml Editor
//
// Copyright (C) 2005 Matthew Ward
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
//
// Matthew Ward (mrward@users.sourceforge.net)

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;
using System;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Schema;

namespace ICSharpCode.XmlEditor
{
	/// <summary>
	/// Wrapper class for the XmlEditor used when displaying the xml file.
	/// </summary>
	public class XmlView : AbstractViewContent, IEditable, IClipboardHandler, IParseInformationListener, IMementoCapable, IPrintable, ITextEditorControlProvider, IPositionable
	{
		/// <summary>
		/// The language handled by this view.
		/// </summary>
		public static readonly string Language = "XML";

		delegate void RefreshDelegate(AbstractMargin margin);

		XmlEditorControl xmlEditor = new XmlEditorControl();
		FileSystemWatcher watcher;
		bool wasChangedExternally;
		MessageViewCategory category;
		
		public XmlView()
		{
			xmlEditor.Dock = DockStyle.Fill;
			
			xmlEditor.SchemaCompletionDataItems = XmlSchemaManager.SchemaCompletionDataItems;
			xmlEditor.Document.DocumentChanged += new DocumentEventHandler(DocumentChanged);
			
			xmlEditor.ActiveTextAreaControl.Caret.CaretModeChanged += new EventHandler(CaretModeChanged);
			xmlEditor.ActiveTextAreaControl.Enter += new EventHandler(CaretUpdate);
			((Form)ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.Workbench).Activated += new EventHandler(GotFocusEvent);
			
			// Listen for changes to the xml editor properties.
			XmlEditorAddInOptions.PropertyChanged += PropertyChanged;
			XmlSchemaManager.UserSchemaAdded += new EventHandler(UserSchemaAdded);
			XmlSchemaManager.UserSchemaRemoved += new EventHandler(UserSchemaRemoved);
		}
		
		/// <summary>
		/// Loads the string content into the view.
		/// </summary>
		public void LoadContent(string content)
		{
			xmlEditor.Document.TextContent = StringParser.Parse(content);
			xmlEditor.Document.HighlightingStrategy = HighlightingStrategyFactory.CreateHighlightingStrategy(XmlView.Language);
			UpdateFolding();
		}
		
		/// <summary>
		/// Can create content for the 'XML' language.
		/// </summary>
		public static bool IsLanguageHandled(string language)
		{
			return language == XmlView.Language;
		}
		
		/// <summary>
		/// Returns whether the view can handle the specified file.
		/// </summary>
		public static bool IsFileNameHandled(string fileName)
		{
			return IsXmlFileExtension(Path.GetExtension(fileName));
		}
		
		/// <summary>
		/// Gets the known xml file extensions.
		/// </summary>
		public static string[] GetXmlFileExtensions()
		{
			IHighlightingStrategy strategy = HighlightingManager.Manager.FindHighlighter(XmlView.Language);
			if (strategy != null) {
				return strategy.Extensions;
			}
			
			return new string[0];
		}
		
		/// <summary>
		/// Validates the xml against known schemas.
		/// </summary>
		public void ValidateXml()
		{
			ClearTasks();
			Category.ClearText();
			OutputWindowWriteLine(StringParser.Parse("${res:MainWindow.XmlValidationMessages.ValidationStarted}"));

			try {
				StringReader stringReader = new StringReader(xmlEditor.Document.TextContent);
				XmlTextReader xmlReader = new XmlTextReader(stringReader);
				xmlReader.XmlResolver = null;
				XmlValidatingReader reader = new XmlValidatingReader(xmlReader);
				reader.XmlResolver = null;
				
				foreach (XmlSchemaCompletionData schemaData in XmlSchemaManager.SchemaCompletionDataItems) {
					reader.Schemas.Add(schemaData.Schema);
				}
				
				XmlDocument doc = new XmlDocument();
				doc.Load(reader);
				
				OutputWindowWriteLine(String.Empty);
				OutputWindowWriteLine(StringParser.Parse("${res:MainWindow.XmlValidationMessages.ValidationSuccess}"));
				
			} catch (XmlSchemaException ex) {
				DisplayValidationError(xmlEditor.FileName, ex.Message, ex.LinePosition - 1, ex.LineNumber - 1);
			} catch (XmlException ex) {
				DisplayValidationError(xmlEditor.FileName, ex.Message, ex.LinePosition - 1, ex.LineNumber - 1);
			}
			
			// Show tasks.
			if (HasTasks && ShowTaskListAfterBuild) {
				ShowTasks();
			}
		}
		
		/// <summary>
		/// Gets the name of a new view.
		/// </summary>
		public override string UntitledName {
			get {
				return base.UntitledName;
			}
			set {
				base.UntitledName = value;
				xmlEditor.FileName = value;
				SetDefaultSchema(Path.GetExtension(xmlEditor.FileName));
			}
		}
		
		public override void Dispose()
		{
			((Form)ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.Workbench).Activated -= new EventHandler(GotFocusEvent);
			
			XmlEditorAddInOptions.PropertyChanged -= PropertyChanged;
			XmlSchemaManager.UserSchemaAdded -= new EventHandler(UserSchemaAdded);
			XmlSchemaManager.UserSchemaRemoved -= new EventHandler(UserSchemaRemoved);

			xmlEditor.Dispose();
		}
		
		/// <summary>
		/// Sets the filename associated with the view.
		/// </summary>
		public override string FileName {
			set {
				if (Path.GetExtension(FileName) != Path.GetExtension(value)) {
					if (xmlEditor.Document.HighlightingStrategy != null) {
						xmlEditor.Document.HighlightingStrategy = HighlightingStrategyFactory.CreateHighlightingStrategyForFile(value);
						xmlEditor.Refresh();
					}
				}
				base.FileName  = value;
				base.TitleName = Path.GetFileName(value);
				
				SetDefaultSchema(Path.GetExtension(xmlEditor.FileName));
			}
		}
		
		#region IEditable interface
		
		public IClipboardHandler ClipboardHandler {
			get {
				return this;
			}
		}
		
		public bool EnableUndo {
			get {
				return xmlEditor.EnableUndo;
			}
		}
		
		public bool EnableRedo {
			get {
				return xmlEditor.EnableUndo;
			}
		}
		
		public string Text {
			get {
				return xmlEditor.Document.TextContent;
			}
			set {
				xmlEditor.Document.TextContent = value;
			}
		}
		
		public void Redo()
		{
			xmlEditor.Redo();
		}
		
		public void Undo()
		{
			xmlEditor.Undo();
		}
		
		#endregion
		
		#region AbstractViewContent implementation
		
		public override Control Control {
			get {
				return xmlEditor;
			}
		}
		
		public override void Load(string fileName)
		{
			xmlEditor.IsReadOnly = IsFileReadOnly(fileName);
			xmlEditor.LoadFile(fileName);
			FileName  = fileName;
			TitleName = Path.GetFileName(fileName);
			IsDirty     = false;
			UpdateFolding();
			SetWatcher();
		}
		
		public override void Save(string fileName)
		{
			OnSaving(EventArgs.Empty);

			if (watcher != null) {
				watcher.EnableRaisingEvents = false;
			}

			xmlEditor.SaveFile(fileName);
			FileName = fileName;
			TitleName = Path.GetFileName(fileName);
			IsDirty = false;
			
			SetWatcher();
			OnSaved(new SaveEventArgs(true));
		}
		
		
		#endregion
		
		#region IClipboardHandler interface

		public bool EnableCut {
			get {
				return xmlEditor.ActiveTextAreaControl.TextArea.ClipboardHandler.EnableCut;
			}
		}
		
		public bool EnableCopy {
			get {
				return xmlEditor.ActiveTextAreaControl.TextArea.ClipboardHandler.EnableCopy;
			}
		}
		
		public bool EnablePaste {
			get {
				return xmlEditor.ActiveTextAreaControl.TextArea.ClipboardHandler.EnablePaste;
			}
		}
		
		public bool EnableDelete {
			get {
				return xmlEditor.ActiveTextAreaControl.TextArea.ClipboardHandler.EnableDelete;
			}
		}
		
		public bool EnableSelectAll {
			get {
				return xmlEditor.ActiveTextAreaControl.TextArea.ClipboardHandler.EnableSelectAll;
			}
		}
		
		public void SelectAll()
		{
			xmlEditor.ActiveTextAreaControl.TextArea.ClipboardHandler.SelectAll(null, EventArgs.Empty);
		}
		
		public void Delete()
		{
			xmlEditor.ActiveTextAreaControl.TextArea.ClipboardHandler.Delete(null, EventArgs.Empty);
		}
		
		public void Paste()
		{
			xmlEditor.ActiveTextAreaControl.TextArea.ClipboardHandler.Paste(null, EventArgs.Empty);
		}
		
		public void Copy()
		{
			xmlEditor.ActiveTextAreaControl.TextArea.ClipboardHandler.Copy(null, EventArgs.Empty);
		}
		
		public void Cut()
		{
			xmlEditor.ActiveTextAreaControl.TextArea.ClipboardHandler.Cut(null, EventArgs.Empty);
		}
		
		#endregion
		
		#region IParseInformationListener interface
		
		public void ParseInformationUpdated(ParseInformation parseInfo)
		{
			UpdateFolding();
		}
		
		#endregion
		
		#region IMementoCapable interface
		
		public void SetMemento(Properties properties)
		{
			xmlEditor.ActiveTextAreaControl.Caret.Position =  xmlEditor.Document.OffsetToPosition(Math.Min(xmlEditor.Document.TextLength, Math.Max(0, properties.Get("CaretOffset", xmlEditor.ActiveTextAreaControl.Caret.Offset))));

			if (xmlEditor.Document.HighlightingStrategy.Name != properties.Get("HighlightingLanguage", xmlEditor.Document.HighlightingStrategy.Name)) {
				IHighlightingStrategy highlightingStrategy = HighlightingStrategyFactory.CreateHighlightingStrategy(properties.Get("HighlightingLanguage", xmlEditor.Document.HighlightingStrategy.Name));
				if (highlightingStrategy != null) {
					xmlEditor.Document.HighlightingStrategy = highlightingStrategy;
				}
			}
			xmlEditor.ActiveTextAreaControl.TextArea.TextView.FirstVisibleLine = properties.Get("VisibleLine", 0);
			
			xmlEditor.Document.FoldingManager.DeserializeFromString(properties.Get("Foldings", ""));
		}
		
		public Properties CreateMemento()
		{
			Properties properties = new Properties();
			properties.Set("CaretOffset", xmlEditor.ActiveTextAreaControl.Caret.Offset);
			properties.Set("VisibleLine", xmlEditor.ActiveTextAreaControl.TextArea.TextView.FirstVisibleLine);
			properties.Set("HighlightingLanguage", xmlEditor.Document.HighlightingStrategy.Name);
			properties.Set("Foldings", xmlEditor.Document.FoldingManager.SerializeToString());
			return properties;
		}
		
		#endregion
		
		#region IPrintable interface
		
		public PrintDocument PrintDocument{
			get {
				return xmlEditor.PrintDocument;
			}
		}
		
		#endregion
		
		#region ITextEditorControlProvider interface
		
		public TextEditorControl TextEditorControl {
			get {
				return xmlEditor;
			}
		}
		#endregion
		
		#region IPositionable interface

		/// <summary>
		/// Moves the cursor to the specified line and column.
		/// </summary>
		public void JumpTo(int line, int column)
		{
			xmlEditor.ActiveTextAreaControl.JumpTo(line, column);
		}

		#endregion

		protected override void OnFileNameChanged(EventArgs e)
		{
			base.OnFileNameChanged(e);
			xmlEditor.FileName = base.FileName;
		}
		
		static bool IsFileReadOnly(string fileName)
		{
			return (File.GetAttributes(fileName) & FileAttributes.ReadOnly) == FileAttributes.ReadOnly;
		}
		
		/// <summary>
		/// Checks that the file extension refers to an xml file as
		/// specified in the SyntaxModes.xml file.
		/// </summary>
		static bool IsXmlFileExtension(string extension)
		{
			bool isXmlFileExtension = false;
			
			IHighlightingStrategy strategy = HighlightingManager.Manager.FindHighlighter(XmlView.Language);
			if (strategy != null) {
				foreach (string currentExtension in strategy.Extensions) {
					if (String.Compare(extension, currentExtension, true) == 0) {
						isXmlFileExtension = true;
						break;
					}
				}
			}
			
			return isXmlFileExtension;
		}
		
		/// <summary>
		/// Forces the editor to update its folds.
		/// </summary>
		void UpdateFolding()
		{
			xmlEditor.Document.FoldingManager.UpdateFoldings(String.Empty, null);
			RefreshMargin();
		}
		
		/// <summary>
		/// Repaints the folds in the margin.
		/// </summary>
		void RefreshMargin()
		{
			RefreshDelegate refreshDelegate = new RefreshDelegate(xmlEditor.ActiveTextAreaControl.TextArea.Refresh);
			xmlEditor.ActiveTextAreaControl.TextArea.Invoke(refreshDelegate, new object[] { xmlEditor.ActiveTextAreaControl.TextArea.FoldMargin});
		}
		
		/// <summary>
		/// Sets the dirty flag since the document has changed.
		/// </summary>
		void DocumentChanged(object sender, DocumentEventArgs e)
		{
			IsDirty = true;
		}
		
		/// <summary>
		/// Updates the line, col, overwrite/insert mode in the status bar.
		/// </summary>
		void CaretUpdate(object sender, EventArgs e)
		{
			CaretChanged(sender, e);
			CaretModeChanged(sender, e);
		}
		
		/// <summary>
		/// Updates the line, col information in the status bar.
		/// </summary>
		void CaretChanged(object sender, EventArgs e)
		{
			Point pos = xmlEditor.Document.OffsetToPosition(xmlEditor.ActiveTextAreaControl.Caret.Offset);
			LineSegment line = xmlEditor.Document.GetLineSegment(pos.Y);
			StatusBarService.SetCaretPosition(pos.X + 1, pos.Y + 1, xmlEditor.ActiveTextAreaControl.Caret.Offset - line.Offset + 1);
		}
		
		/// <summary>
		/// Updates the insert/overwrite mode text in the status bar.
		/// </summary>
		void CaretModeChanged(object sender, EventArgs e)
		{
			StatusBarService.SetInsertMode(xmlEditor.ActiveTextAreaControl.Caret.CaretMode == CaretMode.InsertMode);
		}
		
		/// <summary>
		/// Creates the file system watcher.
		/// </summary>
		void SetWatcher()
		{
			try {
				if (this.watcher == null) {
					this.watcher = new FileSystemWatcher();
					this.watcher.Changed += new FileSystemEventHandler(this.OnFileChangedEvent);
				} else {
					this.watcher.EnableRaisingEvents = false;
				}
				this.watcher.Path = Path.GetDirectoryName(xmlEditor.FileName);
				this.watcher.Filter = Path.GetFileName(xmlEditor.FileName);
				this.watcher.NotifyFilter = NotifyFilters.LastWrite;
				this.watcher.EnableRaisingEvents = true;
			} catch (Exception) {
				watcher = null;
			}
		}
		
		/// <summary>
		/// Shows the "File was changed" dialog if the file was
		/// changed externally.
		/// </summary>
		void GotFocusEvent(object sender, EventArgs e)
		{
			lock (this) {
				if (wasChangedExternally) {
					wasChangedExternally = false;
					string message = StringParser.Parse("${res:ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor.TextEditorDisplayBinding.FileAlteredMessage}", new string[,] {{"File", Path.GetFullPath(xmlEditor.FileName)}});
					if (MessageService.AskQuestion(message, "${res:MainWindow.DialogName}")) {
						Load(xmlEditor.FileName);
					} else {
						IsDirty = true;
					}
				}
			}
		}
		
		void OnFileChangedEvent(object sender, FileSystemEventArgs e)
		{
			lock (this) {
				if(e.ChangeType != WatcherChangeTypes.Deleted) {
					wasChangedExternally = true;
					if (((Form)ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.Workbench).Focused) {
						GotFocusEvent(this, EventArgs.Empty);
					}
				}
			}
		}
		
		/// <summary>
		/// Gets the xml validation output window.
		/// </summary>
		MessageViewCategory Category {
			get {
				if (category == null) {
					category = new MessageViewCategory("Xml", "Xml");
					CompilerMessageView cmv = (CompilerMessageView)WorkbenchSingleton.Workbench.GetPad(typeof(CompilerMessageView)).PadContent;
					cmv.AddCategory(category);
				}
				
				return category;
			}
		}
		
		/// <summary>
		/// Writes a line of text to the output window.
		/// </summary>
		/// <param name="message">The message to send to the output
		/// window.</param>
		void OutputWindowWriteLine(string message)
		{
			Category.AppendText(String.Concat(message, Environment.NewLine));
		}
		
		void ClearTasks()
		{
			if (HasTasks) {
				TaskService.Clear();
			}
		}
		
		bool ShowTaskListAfterBuild {
			get {
				return PropertyService.Get("SharpDevelop.ShowTaskListAfterBuild", true);
			}
		}
		
		bool HasTasks {
			get {
				bool hasTasks = false;
				if (TaskService.TaskCount > 0) {
					hasTasks = true;
				}
				return hasTasks;
			}
		}
		
		void ShowTasks()
		{
			PadDescriptor pad = WorkbenchSingleton.Workbench.GetPad(typeof(OpenTaskView));
			if (pad != null) {
				WorkbenchSingleton.Workbench.ShowPad(pad);
			}
		}
		
		void AddTask(string fileName, string message, int column, int line, TaskType taskType)
		{
			TaskService.Add(new Task(fileName, message, column, line, taskType));
		}
		
		/// <summary>
		/// Displays the validation error.
		/// </summary>
		void DisplayValidationError(string fileName, string message, int column, int line)
		{
			OutputWindowWriteLine(message);
			OutputWindowWriteLine(String.Empty);
			OutputWindowWriteLine(StringParser.Parse("${res:MainWindow.XmlValidationMessages.ValidationFailed}"));
			AddTask(fileName, message, column, line, TaskType.Error);
		}
		
		/// <summary>
		/// Updates the default schema associated with the xml editor.
		/// </summary>
		void PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			string extension = Path.GetExtension(xmlEditor.FileName).ToLower();
			if (e.Key == extension) {
				SetDefaultSchema(extension);
			} else if (e.Key == XmlEditorAddInOptions.ShowAttributesWhenFoldedPropertyName) {
				UpdateFolding();
				xmlEditor.Refresh();
			}
		}

		/// <summary>
		/// Sets the default schema and namespace prefix that the xml editor will use.
		/// </summary>
		void SetDefaultSchema(string extension)
		{
			xmlEditor.DefaultSchemaCompletionData = XmlSchemaManager.GetSchemaCompletionData(extension);
			xmlEditor.DefaultNamespacePrefix = XmlSchemaManager.GetNamespacePrefix(extension);
		}
		
		/// <summary>
		/// Updates the default schema association since the schema
		/// may have been added.
		/// </summary>
		void UserSchemaAdded(object source, EventArgs e)
		{
			SetDefaultSchema(Path.GetExtension(xmlEditor.FileName).ToLower());
		}
		
		/// <summary>
		/// Updates the default schema association since the schema
		/// may have been removed.
		/// </summary>
		void UserSchemaRemoved(object source, EventArgs e)
		{
			SetDefaultSchema(Path.GetExtension(xmlEditor.FileName).ToLower());
		}
	}
}
