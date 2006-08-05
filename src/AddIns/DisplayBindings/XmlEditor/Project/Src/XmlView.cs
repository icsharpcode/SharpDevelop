// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;
using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Schema;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace ICSharpCode.XmlEditor
{
	/// <summary>
	/// Wrapper class for the XmlEditor used when displaying the xml file.
	/// </summary>
	public class XmlView : AbstractViewContent, IEditable, IClipboardHandler, IParseInformationListener, IMementoCapable, IPrintable, ITextEditorControlProvider, IPositionable, IUndoHandler
	{
		/// <summary>
		/// The language handled by this view.
		/// </summary>
		public static readonly string Language = "XML";
		
		/// <summary>
		/// Output window category name.
		/// </summary>
		public static readonly string CategoryName = "XML";

		XmlEditorControl xmlEditor = new XmlEditorControl();
		FileSystemWatcher watcher;
		bool wasChangedExternally;
		static MessageViewCategory category;
		string stylesheetFileName;
				
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
		/// Gets the active XmlView.
		/// </summary>
		/// <returns><see langword="null"/> if the active view is not an XmlView.</returns>
		public static XmlView ActiveXmlView {
			get {
				IWorkbench workbench = WorkbenchSingleton.Workbench;
				if (workbench != null) {
					IWorkbenchWindow window = workbench.ActiveWorkbenchWindow;
					if (window != null) {
						return window.ActiveViewContent as XmlView;
					}
				}
				return null;
			}
		}
		
		/// <summary>
		/// Gets whether the active view is an XmlView.
		/// </summary>
		public static bool IsXmlViewActive {
			get {
				return ActiveXmlView != null;
			}
		}
		
		public override string TabPageText {
			get {
				return "XML";
			}
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
			foreach (ParserDescriptor parser in (ParserDescriptor[])AddInTree.BuildItems("/Workspace/Parser", null, false).ToArray(typeof(ParserDescriptor))) {
				if (parser.Codon.Id == "XmlFoldingParser") {
					return parser.Supportedextensions;
				}
			}

			// Did not find the XmlFoldingParser so default to those files defined by the
			// HighlightingManager.
			IHighlightingStrategy strategy = HighlightingManager.Manager.FindHighlighter(XmlView.Language);
			if (strategy != null) {
				return strategy.Extensions;
			}
			
			return new string[0];
		}
		
		/// <summary>
		/// Finds the xml nodes that match the specified xpath.
		/// </summary>
		/// <returns>An array of XPathNodeMatch items. These include line number 
		/// and line position information aswell as the node found.</returns>
		public static XPathNodeMatch[] SelectNodes(string xml, string xpath, ReadOnlyCollection<XmlNamespace> namespaces)
		{
			XPathDocument doc = new XPathDocument(new StringReader(xml));
			XPathNavigator navigator = doc.CreateNavigator();
			
			// Add namespaces.
			XmlNamespaceManager namespaceManager = new XmlNamespaceManager(navigator.NameTable);
			foreach (XmlNamespace xmlNamespace in namespaces) {
				namespaceManager.AddNamespace(xmlNamespace.Prefix, xmlNamespace.Uri);
			}
	
			// Run the xpath query.                                                        
			XPathNodeIterator iterator = navigator.Select(xpath, namespaceManager);
			
			List<XPathNodeMatch> nodes = new List<XPathNodeMatch>();
			while (iterator.MoveNext()) {
				nodes.Add(new XPathNodeMatch(iterator.Current));
			}			
			return nodes.ToArray();
		}
		
		/// <summary>
		/// Finds the xml nodes that match the specified xpath.
		/// </summary>
		/// <returns>An array of XPathNodeMatch items. These include line number 
		/// and line position information aswell as the node found.</returns>
		public static XPathNodeMatch[] SelectNodes(string xml, string xpath)
		{
			List<XmlNamespace> list = new List<XmlNamespace>();
			return SelectNodes(xml, xpath, new ReadOnlyCollection<XmlNamespace>(list));
		}
		
		/// <summary>
		/// Finds the xml nodes in the current document that match the specified xpath.
		/// </summary>
		/// <returns>An array of XPathNodeMatch items. These include line number 
		/// and line position information aswell as the node found.</returns>
		public XPathNodeMatch[] SelectNodes(string xpath, ReadOnlyCollection<XmlNamespace> namespaces)
		{
			return SelectNodes(Text, xpath, namespaces);
		}
		
		/// <summary>
		/// Validates the xml against known schemas.
		/// </summary>
		public void ValidateXml()
		{
			TaskService.ClearExceptCommentTasks();
			Category.ClearText();
			ShowOutputWindow();
			
			OutputWindowWriteLine(StringParser.Parse("${res:MainWindow.XmlValidationMessages.ValidationStarted}"));

			if (IsSchema) {
				if (!ValidateSchema()) {
					return;
				}
			} else {
				if (!ValidateAgainstSchema()) {
					return;
				}
			}
				
			OutputWindowWriteLine(String.Empty);
			OutputWindowWriteLine(StringParser.Parse("${res:MainWindow.XmlValidationMessages.ValidationSuccess}"));
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
			base.Dispose();
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
				string extension = Path.GetExtension(value);
				if (Path.GetExtension(FileName) != extension) {
					if (xmlEditor.Document.HighlightingStrategy != null) {
						if (XmlView.IsXmlFileExtension(extension)) {
							xmlEditor.Document.HighlightingStrategy = HighlightingStrategyFactory.CreateHighlightingStrategy(XmlView.Language);
						} else {
							xmlEditor.Document.HighlightingStrategy = HighlightingStrategyFactory.CreateHighlightingStrategyForFile(value);
						}
						xmlEditor.Refresh();
					}
				}
				base.FileName  = value;
				base.TitleName = Path.GetFileName(value);
				
				SetDefaultSchema(extension);
			}
		}
		
		/// <summary>
		/// Gets or sets the stylesheet associated with this xml file.
		/// </summary>
		public string StylesheetFileName {
			get {
				return stylesheetFileName;
			}
			set {
				stylesheetFileName = value;
			}
		}
		
		/// <summary>
		/// Applys the stylesheet to the xml and displays the resulting output.
		/// </summary>
		public void RunXslTransform(string xsl)
		{
			try {
				WorkbenchSingleton.Workbench.GetPad(typeof(CompilerMessageView)).BringPadToFront();
				
				TaskService.ClearExceptCommentTasks();
				
				if (IsWellFormed) {
					if (IsValidXsl(xsl)) {
						string transformedXml = Transform(Text, xsl);
						ShowTransformOutput(transformedXml);
					}
				}
				
				ShowErrorList();
				
			} catch (Exception ex) {
				MessageService.ShowError(ex);
			}
		}
		
		/// <summary>
		/// Pretty prints the xml.
		/// </summary>
		public void FormatXml()
		{
			TaskService.ClearExceptCommentTasks();
			
			if (IsWellFormed) {
				string xml = SimpleFormat(IndentedFormat(Text));
				xmlEditor.Document.Replace(0, xmlEditor.Document.TextLength, xml);
				UpdateFolding();
			} else {
				ShowErrorList();
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
				return xmlEditor.EnableRedo;
			}
		}
		
		// ParserUpdateThread uses the text property via IEditable, I had an exception
		// because multiple threads were accessing the GapBufferStrategy at the same time.
		internal string GetText()
		{
			return xmlEditor.Document.TextContent;
		}
		
		internal void SetText(string value)
		{
			xmlEditor.Document.TextContent = value;
		}
		
		public string Text {
			get {
				if (WorkbenchSingleton.InvokeRequired) {
					return WorkbenchSingleton.SafeThreadFunction<string>(GetText);
				} else {
					return GetText();
				}
			}
			set {
				if (WorkbenchSingleton.InvokeRequired) {
					WorkbenchSingleton.SafeThreadCall(SetText, value);
				} else {
					SetText(value);
				}
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
			xmlEditor.LoadFile(fileName, false, true);
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
			WorkbenchSingleton.SafeThreadAsyncCall(UpdateFolding);
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
		
		public int Line {
			get {
				return xmlEditor.ActiveTextAreaControl.Caret.Line;
			}
		}
		
		public int Column {
			get {
				return xmlEditor.ActiveTextAreaControl.Caret.Column;
			}
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
			foreach (string currentExtension in GetXmlFileExtensions()) {
				if (String.Compare(extension, currentExtension, true) == 0) {
					return true;
				}
			}
			return false;
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
			WorkbenchSingleton.SafeThreadAsyncCall(xmlEditor.ActiveTextAreaControl.TextArea.Refresh,
			                                       xmlEditor.ActiveTextAreaControl.TextArea.FoldMargin);
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
			if(e.ChangeType != WatcherChangeTypes.Deleted) {
				wasChangedExternally = true;
				if (xmlEditor.IsHandleCreated) {
					xmlEditor.BeginInvoke(new MethodInvoker(OnFileChangedEventInvoked));
				}
			}
		}
		
		void OnFileChangedEventInvoked()
		{
			if (((Form)ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.Workbench).Focused) {
				GotFocusEvent(this, EventArgs.Empty);
			}
		}
		
		/// <summary>
		/// Gets the xml validation output window.
		/// </summary>
		MessageViewCategory Category {
			get {
				if (category == null) {
					category = new MessageViewCategory(CategoryName);
					CompilerMessageView cmv = (CompilerMessageView)WorkbenchSingleton.Workbench.GetPad(typeof(CompilerMessageView)).PadContent;
					cmv.AddCategory(category);
				}
				
				return category;
			}
		}
		
		/// <summary>
		/// Brings output window pad to the front.
		/// </summary>
		void ShowOutputWindow()
		{
			WorkbenchSingleton.Workbench.GetPad(typeof(CompilerMessageView)).BringPadToFront();
		}
		
		/// <summary>
		/// Writes a line of text to the output window.
		/// </summary>
		/// <param name="message">The message to send to the output
		/// window.</param>
		void OutputWindowWriteLine(string message)
		{
			LoggingService.Info("WriteLine message=" + message);
			Category.AppendText(String.Concat(message, Environment.NewLine));
		}
		
		void ShowErrorList()
		{
			if (ErrorListPad.ShowAfterBuild && TaskService.SomethingWentWrong) {
				WorkbenchSingleton.Workbench.GetPad(typeof(ErrorListPad)).BringPadToFront();
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
			AddTask(fileName, message, column, line, TaskType.Error);
		}
		
		void ShowValidationFailedMessage()
		{
			OutputWindowWriteLine(String.Empty);
			OutputWindowWriteLine(StringParser.Parse("${res:MainWindow.XmlValidationMessages.ValidationFailed}"));
		}
		
		/// <summary>
		/// Updates the default schema associated with the xml editor.
		/// </summary>
		void PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			string extension = Path.GetExtension(xmlEditor.FileName).ToLowerInvariant();
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
			SetDefaultSchema(Path.GetExtension(xmlEditor.FileName).ToLowerInvariant());
		}
		
		/// <summary>
		/// Updates the default schema association since the schema
		/// may have been removed.
		/// </summary>
		void UserSchemaRemoved(object source, EventArgs e)
		{
			SetDefaultSchema(Path.GetExtension(xmlEditor.FileName).ToLowerInvariant());
		}
		
		/// <summary>
		/// Displays the transformed output.
		/// </summary>
		void ShowTransformOutput(string xml)
		{
			// Pretty print the xml.
			xml = SimpleFormat(IndentedFormat(xml));
			
			// Display the output xml.
			XslOutputView view = XslOutputView.Instance;
			if (view == null) {
				view = new XslOutputView();
				view.LoadContent(xml);
				WorkbenchSingleton.Workbench.ShowView(view);
			} else {
				// Transform output window already opened.
				view.LoadContent(xml);
				view.WorkbenchWindow.SelectWindow();
			}
		}
		
		/// <summary>
		/// Returns a formatted xml string using a simple formatting algorithm.
		/// </summary>
		static string SimpleFormat(string xml)
		{
			return xml.Replace("><", ">\r\n<");
		}
		
		/// <summary>
		/// Runs an XSL transform on the input xml.
		/// </summary>
		/// <param name="input">The input xml to transform.</param>
		/// <param name="transform">The transform xml.</param>
		/// <returns>The output of the transform.</returns>
		static string Transform(string input, string transform)
		{
			StringReader inputString = new StringReader(input);
			XPathDocument sourceDocument = new XPathDocument(inputString);

			StringReader transformString = new StringReader(transform);
			XPathDocument transformDocument = new XPathDocument(transformString);

			XslCompiledTransform xslTransform = new XslCompiledTransform();
			xslTransform.Load(transformDocument, XsltSettings.Default, new XmlUrlResolver());
			
			MemoryStream outputStream = new MemoryStream();
			XmlTextWriter writer = new XmlTextWriter(outputStream, Encoding.UTF8);
			
			xslTransform.Transform(sourceDocument, null, writer);

			int preambleLength = Encoding.UTF8.GetPreamble().Length;
			byte[] outputBytes = outputStream.ToArray();
			return UTF8Encoding.UTF8.GetString(outputBytes, preambleLength, outputBytes.Length - preambleLength);
		}
		
		/// <summary>
		/// Returns a pretty print version of the given xml.
		/// </summary>
		/// <param name="xml">Xml string to pretty print.</param>
		/// <returns>A pretty print version of the specified xml.  If the
		/// string is not well formed xml the original string is returned.
		/// </returns>
		string IndentedFormat(string xml)
		{
			string indentedText = String.Empty;

			try	{
				XmlTextReader reader = new XmlTextReader(new StringReader(xml));
				reader.WhitespaceHandling = WhitespaceHandling.None;

				StringWriter indentedXmlWriter = new StringWriter();
				XmlTextWriter writer = new XmlTextWriter(indentedXmlWriter);
				if (xmlEditor.TextEditorProperties.ConvertTabsToSpaces) {
					writer.Indentation = xmlEditor.TextEditorProperties.TabIndent;
					writer.IndentChar = ' ';
					;
				} else {
					writer.Indentation = 1;
					writer.IndentChar = '\t';
				}
				writer.Formatting = Formatting.Indented;
				writer.WriteNode(reader, false);
				writer.Flush();

				indentedText = indentedXmlWriter.ToString();
			}
			catch(Exception) {
				indentedText = xml;
			}

			return indentedText;
		}
		
		/// <summary>
		/// Checks that the xml in this view is well-formed.
		/// </summary>
		bool IsWellFormed {
			get {
				try	{
					XmlDocument Document = new XmlDocument( );
					Document.LoadXml(Text);
					return true;
				} catch(XmlException ex) {
					string fileName = FileName;
					if (fileName == null || fileName.Length == 0) {
						fileName = TitleName;
					}
					AddTask(fileName, ex.Message, ex.LinePosition - 1, ex.LineNumber - 1, TaskType.Error);
				}
				return false;
			}
		}
		
		/// <summary>
		/// Validates the given xsl string,.
		/// </summary>
		bool IsValidXsl(string xml)
		{
			try	{
				WorkbenchSingleton.Workbench.GetPad(typeof(CompilerMessageView)).BringPadToFront();

				StringReader reader = new StringReader(xml);
				XPathDocument doc = new XPathDocument(reader);

				XslCompiledTransform xslTransform = new XslCompiledTransform();
				xslTransform.Load(doc, XsltSettings.Default, new XmlUrlResolver());

				return true;
			} catch(XsltCompileException ex) {
				string message = String.Empty;
				
				if(ex.InnerException != null) {
					message = ex.InnerException.Message;
				} else {
					message = ex.ToString();
				}

				AddTask(StylesheetFileName, message, ex.LineNumber - 1, ex.LinePosition - 1, TaskType.Error);
			} catch(XsltException ex) {
				AddTask(StylesheetFileName, ex.Message, ex.LinePosition - 1, ex.LineNumber - 1, TaskType.Error);
			} catch(XmlException ex) {
				AddTask(StylesheetFileName, ex.Message, ex.LinePosition - 1, ex.LineNumber - 1, TaskType.Error);
			}

			return false;
		}
		
		/// <summary>
		/// Validates the XML in the editor against all the schemas in the
		/// schema manager.
		/// </summary>
		bool ValidateAgainstSchema()
		{
			try {
				StringReader stringReader = new StringReader(xmlEditor.Document.TextContent);
				XmlTextReader xmlReader = new XmlTextReader(stringReader);
				xmlReader.XmlResolver = null;
				XmlReaderSettings settings = new XmlReaderSettings();
				settings.ValidationType = ValidationType.Schema;
				settings.ValidationFlags = XmlSchemaValidationFlags.None;
				settings.XmlResolver = null;
				
				XmlSchemaCompletionData schemaData = null;
				try {
					for (int i = 0; i < XmlSchemaManager.SchemaCompletionDataItems.Count; ++i) {
						schemaData = XmlSchemaManager.SchemaCompletionDataItems[i];
						settings.Schemas.Add(schemaData.Schema);
					}
				} catch (XmlSchemaException ex) {
					DisplayValidationError(schemaData.FileName, ex.Message, ex.LinePosition - 1, ex.LineNumber - 1);
					ShowValidationFailedMessage();
					ShowErrorList();
					return false;
				}
			
				XmlReader reader = XmlReader.Create(xmlReader, settings);
				
				XmlDocument doc = new XmlDocument();
				doc.Load(reader);
				return true;
				
			} catch (XmlSchemaException ex) {
				DisplayValidationError(xmlEditor.FileName, ex.Message, ex.LinePosition - 1, ex.LineNumber - 1);
			} catch (XmlException ex) {
				DisplayValidationError(xmlEditor.FileName, ex.Message, ex.LinePosition - 1, ex.LineNumber - 1);
			}
			ShowValidationFailedMessage();
			ShowErrorList();
			return false;
		}
		
		/// <summary>
		/// Assumes the content in the editor is a schema and validates it using
		/// the XmlSchema class.  This is used instead of validating against the 
		/// XMLSchema.xsd file since it gives us better error information.
		/// </summary>
		bool ValidateSchema()
		{
			StringReader stringReader = new StringReader(xmlEditor.Document.TextContent);
			XmlTextReader xmlReader = new XmlTextReader(stringReader);
			xmlReader.XmlResolver = null;

			try	{
				XmlSchema schema = XmlSchema.Read(xmlReader, new ValidationEventHandler(SchemaValidation));
				schema.Compile(new ValidationEventHandler(SchemaValidation));
			} catch (XmlSchemaException ex) {
				DisplayValidationError(xmlEditor.FileName, ex.Message, ex.LinePosition - 1, ex.LineNumber - 1);
			} catch (XmlException ex) {
				DisplayValidationError(xmlEditor.FileName, ex.Message, ex.LinePosition - 1, ex.LineNumber - 1);
			} finally {
				xmlReader.Close();
			}
			if (TaskService.SomethingWentWrong) {
				ShowValidationFailedMessage();
				ShowErrorList();
				return false;
			}
			return true;
		}
		
		void SchemaValidation(object source, ValidationEventArgs e)
		{
			if (e.Severity == XmlSeverityType.Error) {
				DisplayValidationError(xmlEditor.FileName, e.Message, e.Exception.LinePosition - 1, e.Exception.LineNumber - 1);
			} else {
				DisplayValidationWarning(xmlEditor.FileName, e.Message, e.Exception.LinePosition - 1, e.Exception.LineNumber - 1);
			}
		}
		
		/// <summary>
		/// Displays the validation warning.
		/// </summary>
		void DisplayValidationWarning(string fileName, string message, int column, int line)
		{
			OutputWindowWriteLine(message);
			AddTask(fileName, message, column, line, TaskType.Warning);
		}

		bool IsSchema {
			get {
				string extension = Path.GetExtension(xmlEditor.FileName);
				if (extension != null) {
					return String.Compare(".xsd", extension, true) == 0;
				}
				return false;
			}
		}
	}
}
