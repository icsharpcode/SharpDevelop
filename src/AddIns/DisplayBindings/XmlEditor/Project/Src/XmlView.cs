// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing.Printing;
using System.IO;
using System.Net;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Schema;
using System.Xml.XPath;
using System.Xml.Xsl;

using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Actions;
using ICSharpCode.TextEditor.Document;
using Bookmarks = ICSharpCode.SharpDevelop.Bookmarks;

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

		/// <summary>
		/// Edit actions addin tree path for the xml editor control.
		/// </summary>
		static readonly string editActionsPath = "/AddIns/XmlEditor/EditActions";
		
		/// <summary>
		/// Right click menu addin tree path for the xml editor control.
		/// </summary>
		static readonly string contextMenuPath = "/SharpDevelop/ViewContent/XmlEditor/ContextMenu";

		XmlEditorControl xmlEditor;
		static MessageViewCategory category;
		string stylesheetFileName;
		XmlTreeView xmlTreeView;
		bool isInUnitTest;
		
		/// <summary>
		/// Sets the primary file. Public because it is used by some unit tests.
		/// </summary>
		public void SetPrimaryFileUnitTestMode(OpenedFile file)
		{
			if (PrimaryFile != null)
				throw new InvalidOperationException("primary file is already set");
			
			isInUnitTest = true;
			
			this.Files.Add(file);
			OnFileNameChanged(file);
			file.ForceInitializeView(this);
		}
		
		public XmlView(OpenedFile file)
			: this()
		{
			this.Files.Add(file);
			OnFileNameChanged(file);
			file.ForceInitializeView(this);
			
			xmlTreeView = new XmlTreeView(this);
			SecondaryViewContents.Add(xmlTreeView);
		}
		
		public XmlView()
			: this(SharpDevelopTextEditorProperties.Instance, XmlSchemaManager.SchemaCompletionDataItems)
		{
			xmlEditor.AddEditActions(GetEditActions());
			xmlEditor.TextAreaContextMenuStrip = MenuService.CreateContextMenu(xmlEditor, contextMenuPath);
			
			// Add event handlers so we can update the status bar when
			// the cursor position changes.
			xmlEditor.ActiveTextAreaControl.Caret.CaretModeChanged += CaretModeChanged;
			xmlEditor.ActiveTextAreaControl.Caret.PositionChanged += CaretChanged;
			xmlEditor.ActiveTextAreaControl.Enter += CaretUpdate;
			
			// Listen for changes to the xml editor properties.
			XmlEditorAddInOptions.PropertyChanged += PropertyChanged;
			XmlSchemaManager.UserSchemaAdded += UserSchemaAdded;
			XmlSchemaManager.UserSchemaRemoved += UserSchemaRemoved;
		}
		
		/// <summary>
		/// Creates an XmlView that is independent of SharpDevelop. This
		/// constructor does rely on SharpDevelop being available and is
		/// only used for testing the XmlView.
		/// </summary>
		public XmlView(ITextEditorProperties textEditorProperties, XmlSchemaCompletionDataCollection schemas)
		{
			this.TabPageText = "XML";
			
			TextEditorDisplayBinding.InitializeSyntaxModes();
			
			xmlEditor = new XmlEditorControl();
			xmlEditor.Dock = DockStyle.Fill;
			
			xmlEditor.TextEditorProperties = textEditorProperties;
			xmlEditor.SchemaCompletionDataItems = schemas;
			xmlEditor.Document.DocumentChanged += DocumentChanged;
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
		
		public XmlEditorControl XmlEditor {
			get {
				return xmlEditor;
			}
		}
		
		public override bool IsReadOnly {
			get {
				return xmlEditor.IsReadOnly;
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
			foreach (ParserDescriptor parser in AddInTree.BuildItems<ParserDescriptor>("/Workspace/Parser", null, false)) {
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
			XmlTextReader xmlReader = new XmlTextReader(new StringReader(xml));
			xmlReader.XmlResolver = null;
			XPathDocument doc = new XPathDocument(xmlReader);
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
		/// Gets the XmlSchemaObject that defines the currently selected xml element or
		/// attribute.
		/// </summary>
		/// <param name="text">The complete xml text.</param>
		/// <param name="index">The current cursor index.</param>
		/// <param name="provider">The completion data provider</param>
		public static XmlSchemaObject GetSchemaObjectSelected(string xml, int index, XmlCompletionDataProvider provider)
		{
			return GetSchemaObjectSelected(xml, index, provider, null);
		}
		
		/// <summary>
		/// Gets the XmlSchemaObject that defines the currently selected xml element or
		/// attribute.
		/// </summary>
		/// <param name="text">The complete xml text.</param>
		/// <param name="index">The current cursor index.</param>
		/// <param name="provider">The completion data provider</param>
		/// <param name="currentSchemaCompletionData">This is the schema completion data for the
		/// schema currently being displayed. This can be null if the document is
		/// not a schema.</param>
		public static XmlSchemaObject GetSchemaObjectSelected(string xml, int index, XmlCompletionDataProvider provider, XmlSchemaCompletionData currentSchemaCompletionData)
		{
			// Find element under cursor.
			XmlElementPath path = XmlParser.GetActiveElementStartPathAtIndex(xml, index);
			string attributeName = XmlParser.GetAttributeNameAtIndex(xml, index);
			
			// Find schema definition object.
			XmlSchemaCompletionData schemaCompletionData = provider.FindSchema(path);
			XmlSchemaObject schemaObject = null;
			if (schemaCompletionData != null) {
				XmlSchemaElement element = schemaCompletionData.FindElement(path);
				schemaObject = element;
				if (element != null) {
					if (attributeName.Length > 0) {
						XmlSchemaAttribute attribute = schemaCompletionData.FindAttribute(element, attributeName);
						if (attribute != null) {
							if (currentSchemaCompletionData != null) {
								schemaObject = GetSchemaObjectReferenced(xml, index, provider, currentSchemaCompletionData, element, attribute);
							} else {
								schemaObject = attribute;
							}
						}
					}
					return schemaObject;
				}
			}
			return null;
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
		
		public override void Dispose()
		{
			base.Dispose();
			XmlEditorAddInOptions.PropertyChanged -= PropertyChanged;
			XmlSchemaManager.UserSchemaAdded -= UserSchemaAdded;
			XmlSchemaManager.UserSchemaRemoved -= UserSchemaRemoved;
			xmlEditor.Dispose();
		}
		
		
		protected override void OnFileNameChanged(OpenedFile file)
		{
			base.OnFileNameChanged(file);
			
			string oldFileName = xmlEditor.FileName;
			string newFileName = file.FileName;
			
			string extension = Path.GetExtension(newFileName);
			if (Path.GetExtension(oldFileName) != extension) {
				if (xmlEditor.Document.HighlightingStrategy != null) {
					if (XmlView.IsXmlFileExtension(extension)) {
						xmlEditor.Document.HighlightingStrategy = HighlightingStrategyFactory.CreateHighlightingStrategy(XmlView.Language);
					} else {
						xmlEditor.Document.HighlightingStrategy = HighlightingStrategyFactory.CreateHighlightingStrategyForFile(newFileName);
					}
					xmlEditor.Refresh();
				}
			}
			SetDefaultSchema(extension);
			
			xmlEditor.FileName = newFileName;
			ICSharpCode.SharpDevelop.Bookmarks.SDBookmarkFactory factory = (ICSharpCode.SharpDevelop.Bookmarks.SDBookmarkFactory)xmlEditor.Document.BookmarkManager.Factory;
			factory.ChangeFilename(newFileName);
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
						try {
							string transformedXml = Transform(Text, xsl);
							ShowTransformOutput(transformedXml);
						} catch (XsltException ex) {
							AddTask(GetFileNameFromInnerException(ex, StylesheetFileName), GetInnerExceptionErrorMessage(ex), ex.LineNumber - 1, ex.LinePosition - 1, TaskType.Error);
						}
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
				ReplaceAll(Text);
			} else {
				ShowErrorList();
			}
		}
		
		/// <summary>
		/// Creates a schema based on the xml content.
		/// </summary>
		/// <returns>A set of generated schemas or null if the xml content is not
		/// well formed.</returns>
		public string[] InferSchema()
		{
			TaskService.ClearExceptCommentTasks();
			if (IsWellFormed) {
				try {
					using (XmlTextReader reader = new XmlTextReader(new StringReader(Text))) {
						XmlSchemaInference schemaInference = new XmlSchemaInference();
						XmlSchemaSet schemaSet = schemaInference.InferSchema(reader);
						return GetSchemas(schemaSet);
					}
				} catch (XmlSchemaInferenceException ex) {
					AddTask(xmlEditor.FileName, ex.Message, ex.LinePosition, ex.LineNumber, TaskType.Error);
				}
			}
			ShowErrorList();
			return null;
		}
		
		/// <summary>
		/// Finds the definition of the xml element or attribute under the cursor
		/// in the corresponding schema and then displays that schema and the definition
		/// found.
		/// </summary>
		public void GoToSchemaDefinition()
		{
			// Find schema object for selected xml element or attribute.
			XmlCompletionDataProvider provider = new XmlCompletionDataProvider(xmlEditor.SchemaCompletionDataItems, xmlEditor.DefaultSchemaCompletionData, xmlEditor.DefaultNamespacePrefix);
			XmlSchemaCompletionData currentSchemaCompletionData = provider.FindSchemaFromFileName(PrimaryFileName);
			XmlSchemaObject schemaObject = GetSchemaObjectSelected(Text, xmlEditor.ActiveTextAreaControl.Caret.Offset, provider, currentSchemaCompletionData);
			
			// Open schema.
			if (schemaObject != null && schemaObject.SourceUri != null && schemaObject.SourceUri.Length > 0) {
				string fileName = schemaObject.SourceUri.Replace("file:///", String.Empty);
				FileService.JumpToFilePosition(fileName, schemaObject.LineNumber - 1, schemaObject.LinePosition - 1);
			}
		}
		
		/// <summary>
		/// Checks that the xml is well formed. Any errors are displayed in the
		/// errors list.
		/// </summary>
		public void CheckIsWellFormed()
		{
			TaskService.ClearExceptCommentTasks();
			if (!IsWellFormed) {
				ShowErrorList();
			}
		}
		
		/// <summary>
		/// Replaces the entire text of the xml view with the xml in the
		/// specified. The xml will be formatted.
		/// </summary>
		public void ReplaceAll(string xml)
		{
			string formattedXml = SimpleFormat(IndentedFormat(xml));
			xmlEditor.Document.Replace(0, xmlEditor.Document.TextLength, formattedXml);
			UpdateFolding();
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
		
		public override void Load(OpenedFile file, Stream stream)
		{
			if (!file.IsUntitled) {
				xmlEditor.IsReadOnly = IsFileReadOnly(file.FileName);
			}
			
			xmlEditor.LoadFile(file.FileName, stream, false, true);
			foreach (Bookmarks.SDBookmark mark in Bookmarks.BookmarkManager.GetBookmarks(file.FileName)) {
				mark.Document = xmlEditor.Document;
				xmlEditor.Document.BookmarkManager.AddMark(mark);
			}
			UpdateFolding();
		}
		
		public override void Save(OpenedFile file, Stream stream)
		{
			xmlEditor.SaveFile(stream);
		}
		
		public override INavigationPoint BuildNavPoint()
		{
			int line = Line;
			LineSegment lineSegment = xmlEditor.Document.GetLineSegment(line);
			string text = xmlEditor.Document.GetText(lineSegment);
			return new TextNavigationPoint(PrimaryFileName, line, Column, text);
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
			xmlEditor.Document.FoldingManager.DeserializeFromString(properties.Get("Foldings", String.Empty));
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
		
		public IDocument GetDocumentForFile(OpenedFile file)
		{
			if (file == this.PrimaryFile) {
				return this.TextEditorControl.Document;
			} else {
				return null;
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
			if (isInUnitTest) // SafeThreadAsyncCall doesn't work in unit testing
				return;
			WorkbenchSingleton.SafeThreadAsyncCall(xmlEditor.ActiveTextAreaControl.TextArea.Refresh,
			                                       xmlEditor.ActiveTextAreaControl.TextArea.FoldMargin);
		}
		
		/// <summary>
		/// Sets the dirty flag since the document has changed.
		/// </summary>
		void DocumentChanged(object sender, DocumentEventArgs e)
		{
			if (PrimaryFile != null) {
				PrimaryFile.MakeDirty();
			}
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
			TextAreaControl activeTextAreaControl = xmlEditor.ActiveTextAreaControl;
			int line = activeTextAreaControl.Caret.Line;
			int col = activeTextAreaControl.Caret.Column;
			StatusBarService.SetCaretPosition(activeTextAreaControl.TextArea.TextView.GetVisualColumn(line, col) + 1, line + 1, col + 1);
		}
		
		/// <summary>
		/// Updates the insert/overwrite mode text in the status bar.
		/// </summary>
		void CaretModeChanged(object sender, EventArgs e)
		{
			StatusBarService.SetInsertMode(xmlEditor.ActiveTextAreaControl.Caret.CaretMode == CaretMode.InsertMode);
		}
		
		/// <summary>
		/// Gets the xml validation output window.
		/// </summary>
		MessageViewCategory Category {
			get {
				if (category == null) {
					MessageViewCategory.Create(ref category, CategoryName);
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
			if (e.Key == String.Concat("ext", extension)) {
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
			XmlTextReader sourceDocument = new XmlTextReader(inputString);

			StringReader transformString = new StringReader(transform);
			XPathDocument transformDocument = new XPathDocument(transformString);

			XslCompiledTransform xslTransform = new XslCompiledTransform();
			xslTransform.Load(transformDocument, XsltSettings.TrustedXslt, new XmlUrlResolver());
			
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
				XmlTextWriter writer = CreateXmlTextWriter(indentedXmlWriter);
				writer.WriteNode(reader, false);
				writer.Flush();

				indentedText = indentedXmlWriter.ToString();
			} catch(Exception) {
				indentedText = xml;
			}

			return indentedText;
		}
		
		XmlTextWriter CreateXmlTextWriter(TextWriter textWriter)
		{
			XmlTextWriter writer = new XmlTextWriter(textWriter);
			if (xmlEditor.TextEditorProperties.ConvertTabsToSpaces) {
				writer.Indentation = xmlEditor.TextEditorProperties.IndentationSize;
				writer.IndentChar = ' ';
			} else {
				writer.Indentation = 1;
				writer.IndentChar = '\t';
			}
			writer.Formatting = Formatting.Indented;
			return writer;
		}
		
		/// <summary>
		/// Checks that the xml in this view is well-formed.
		/// </summary>
		bool IsWellFormed {
			get {
				try	{
					XmlDocument Document = new XmlDocument();
					Document.LoadXml(Text);
					return true;
				} catch(XmlException ex) {
					AddTask(xmlEditor.FileName, ex.Message, ex.LinePosition - 1, ex.LineNumber - 1, TaskType.Error);
				} catch (WebException ex) {
					AddTask(xmlEditor.FileName, ex.Message, 0, 0, TaskType.Error);
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
				AddTask(StylesheetFileName, GetInnerExceptionErrorMessage(ex), ex.LineNumber - 1, ex.LinePosition - 1, TaskType.Error);
			} catch(XsltException ex) {
				AddTask(StylesheetFileName, ex.Message, ex.LinePosition - 1, ex.LineNumber - 1, TaskType.Error);
			} catch(XmlException ex) {
				AddTask(StylesheetFileName, ex.Message, ex.LinePosition - 1, ex.LineNumber - 1, TaskType.Error);
			}

			return false;
		}
		
		/// <summary>
		/// Returns the inner exception message if there is one otherwise returns the exception's error message.
		/// </summary>
		static string GetInnerExceptionErrorMessage(Exception ex)
		{
			if(ex.InnerException != null) {
				return ex.InnerException.Message;
			}
			return ex.Message;
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
		
		/// <summary>
		/// Checks whether the element belongs to the XSD namespace.
		/// </summary>
		static bool IsXmlSchemaNamespace(XmlSchemaElement element)
		{
			XmlQualifiedName qualifiedName = element.QualifiedName;
			if (qualifiedName != null) {
				return XmlSchemaManager.IsXmlSchemaNamespace(qualifiedName.Namespace);
			}
			return false;
		}
		
		/// <summary>
		/// If the attribute value found references another item in the schema
		/// return this instead of the attribute schema object. For example, if the
		/// user can select the attribute value and the code will work out the schema object pointed to by the ref
		/// or type attribute:
		///
		/// xs:element ref="ref-name"
		/// xs:attribute type="type-name"
		/// </summary>
		/// <returns>
		/// The <paramref name="attribute"/> if no schema object was referenced.
		/// </returns>
		static XmlSchemaObject GetSchemaObjectReferenced(string xml, int index, XmlCompletionDataProvider provider, XmlSchemaCompletionData currentSchemaCompletionData, XmlSchemaElement element, XmlSchemaAttribute attribute)
		{
			XmlSchemaObject schemaObject = null;
			if (IsXmlSchemaNamespace(element)) {
				// Find attribute value.
				string attributeValue = XmlParser.GetAttributeValueAtIndex(xml, index);
				if (attributeValue.Length == 0) {
					return attribute;
				}
				
				if (attribute.Name == "ref") {
					schemaObject = FindSchemaObjectReference(attributeValue, provider, currentSchemaCompletionData, element.Name);
				} else if (attribute.Name == "type") {
					schemaObject = FindSchemaObjectType(attributeValue, provider, currentSchemaCompletionData, element.Name);
				}
			}
			
			if (schemaObject != null) {
				return schemaObject;
			}
			return attribute;
		}
		
		/// <summary>
		/// Attempts to locate the reference name in the specified schema.
		/// </summary>
		/// <param name="name">The reference to look up.</param>
		/// <param name="schemaCompletionData">The schema completion data to use to
		/// find the reference.</param>
		/// <param name="elementName">The element to determine what sort of reference it is
		/// (e.g. group, attribute, element).</param>
		/// <returns><see langword="null"/> if no match can be found.</returns>
		static XmlSchemaObject FindSchemaObjectReference(string name, XmlCompletionDataProvider provider, XmlSchemaCompletionData schemaCompletionData, string elementName)
		{
			QualifiedName qualifiedName = schemaCompletionData.CreateQualifiedName(name);
			XmlSchemaCompletionData qualifiedNameSchema = provider.FindSchema(qualifiedName.Namespace);
			if (qualifiedNameSchema != null) {
				schemaCompletionData = qualifiedNameSchema;
			}
			switch (elementName) {
				case "element":
					return schemaCompletionData.FindElement(qualifiedName);
				case "attribute":
					return schemaCompletionData.FindAttribute(qualifiedName.Name);
				case "group":
					return schemaCompletionData.FindGroup(qualifiedName.Name);
				case "attributeGroup":
					return schemaCompletionData.FindAttributeGroup(qualifiedName.Name);
			}
			return null;
		}
		
		/// <summary>
		/// Attempts to locate the type name in the specified schema.
		/// </summary>
		/// <param name="name">The type to look up.</param>
		/// <param name="schemaCompletionData">The schema completion data to use to
		/// find the type.</param>
		/// <param name="elementName">The element to determine what sort of type it is
		/// (e.g. group, attribute, element).</param>
		/// <returns><see langword="null"/> if no match can be found.</returns>
		static XmlSchemaObject FindSchemaObjectType(string name, XmlCompletionDataProvider provider, XmlSchemaCompletionData schemaCompletionData, string elementName)
		{
			QualifiedName qualifiedName = schemaCompletionData.CreateQualifiedName(name);
			XmlSchemaCompletionData qualifiedNameSchema = provider.FindSchema(qualifiedName.Namespace);
			if (qualifiedNameSchema != null) {
				schemaCompletionData = qualifiedNameSchema;
			}
			switch (elementName) {
				case "element":
					return schemaCompletionData.FindComplexType(qualifiedName);
				case "attribute":
					return schemaCompletionData.FindSimpleType(qualifiedName.Name);
			}
			return null;
		}
		
		/// <summary>
		/// Converts a set of schemas to a string array, each array item
		/// contains the schema converted to a string.
		/// </summary>
		string[] GetSchemas(XmlSchemaSet schemaSet)
		{
			List<string> schemas = new List<string>();
			foreach (XmlSchema schema in schemaSet.Schemas()) {
				using (EncodedStringWriter writer = new EncodedStringWriter(xmlEditor.TextEditorProperties.Encoding)) {
					using (XmlTextWriter xmlWriter = CreateXmlTextWriter(writer)) {
						schema.Write(xmlWriter);
						schemas.Add(writer.ToString());
					}
				}
			}
			return schemas.ToArray();
		}
		
		/// <summary>
		/// Gets the edit actions for the xml editor from the addin tree.
		/// </summary>
		IEditAction[] GetEditActions()
		{
			return AddInTree.BuildItems<IEditAction>(editActionsPath, this, false).ToArray();
		}
		
		/// <summary>
		/// Tries to get the filename from the inner exception otherwise returns the default filename.
		/// </summary>
		/// <param name="ex"></param>
		/// <param name="defaultFileName"></param>
		/// <returns></returns>
		static string GetFileNameFromInnerException(Exception ex, string defaultFileName)
		{
			XmlException innerException = ex.InnerException as XmlException;
			if (innerException != null) {
				string fileName = innerException.SourceUri.Replace("file:///", String.Empty);
				if (!String.IsNullOrEmpty(fileName)) {
					return fileName;
				}
			}
			return defaultFileName;
		}
	}
}
