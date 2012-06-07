// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.XPath;
using System.Xml.Xsl;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.XmlEditor
{
	public class XmlView
	{
		static readonly Dictionary<OpenedFile, XmlView> mapping = new Dictionary<OpenedFile, XmlView>();
		XmlSchemaCompletionCollection schemas;
		
		XmlView()
		{
			schemas = XmlEditorService.RegisteredXmlSchemas.Schemas;
		}
		
		/// <summary>
		/// Retrieves additional data for a XML file.
		/// </summary>
		/// <param name="file">The file to retrieve the data for.</param>
		/// <returns>null if the file is not a valid XML file, otherwise a XmlView instance with additional data used by the XML editor.</returns>
		public static XmlView ForFile(OpenedFile file)
		{
			if (file == null) {
				return null;
			}
			if (!XmlDisplayBinding.IsFileNameHandled(file.FileName)) {
				return null;
			}
			
			XmlView instance;
			if (!mapping.TryGetValue(file, out instance)) {
				file.FileClosed += new EventHandler(FileClosedHandler);
				instance = new XmlView() { File = file };
				mapping.Add(file, instance);
			}
			
			return instance;
		}
		
		public static XmlView ForFileName(string fileName)
		{
			return ForFile(FileService.GetOpenedFile(fileName));
		}
		
		public OpenedFile File { get; set; }
		
		public static XmlView ForViewContent(IViewContent view)
		{
			if ((view == null) || (view.PrimaryFile == null)) {
				return null;
			}
			return ForFile(view.PrimaryFile);
		}

		static void FileClosedHandler(object sender, EventArgs e)
		{
			if (sender is OpenedFile) {
				mapping.Remove(sender as OpenedFile);
			}
		}
		
		public string StylesheetFileName { get; set; }
				
		public ITextEditor TextEditor
		{
			get {
				foreach (IViewContent view in File.RegisteredViewContents) {
					ITextEditorProvider provider = view as ITextEditorProvider;
					if (provider != null) {
						IDocument document = provider.GetDocumentForFile(File);
						if (document != null) {
							return provider.TextEditor;
						}
					}
				}
				return null;
			}
		}
		
		public XmlTreeView View
		{
			get {
				foreach (IViewContent view in File.RegisteredViewContents) {
					XmlTreeView tree = view as XmlTreeView;
					if (tree != null) {
						return tree;
					}
				}
				return null;
			}
		}
		
		public static XmlView ActiveXmlView {
			get { return XmlView.ForViewContent(WorkbenchSingleton.Workbench.ActiveViewContent); }
		}
		
		public void GoToSchemaDefinition()
		{
			// Find schema object for selected xml element or attribute.
			XmlSchemaCompletion currentSchemaCompletion = schemas.GetSchemaFromFileName(File.FileName);
			
			ITextEditor editor = TextEditor;
			if (editor == null) {
				return;
			}
			
			XmlSchemaDefinition schemaDefinition = new XmlSchemaDefinition(schemas, currentSchemaCompletion);
			XmlSchemaObjectLocation schemaObjectLocation = schemaDefinition.GetSelectedSchemaObjectLocation(editor.Document.Text, editor.Caret.Offset);
			schemaObjectLocation.JumpToFilePosition();
		}
		
		/// <summary>
		/// Checks that the xml in this view is well-formed.
		/// </summary>
		public bool IsWellFormed {
			get { return CheckIsWellFormed(); }
		}
		
		public bool CheckIsWellFormed()
		{
			return CheckIsWellFormed(TextEditor);
		}

		public static bool CheckIsWellFormed(ITextEditor editor)
		{
			if (editor == null) return false;
			try {
				XmlDocument document = new XmlDocument();
				document.XmlResolver = null;
				document.LoadXml(editor.Document.Text);
				return true;
			} catch (XmlException ex) {
				AddTask(editor.FileName, ex.Message, ex.LinePosition, ex.LineNumber, TaskType.Error);
			} catch (WebException ex) {
				AddTask(editor.FileName, ex.Message, 0, 0, TaskType.Error);
			}
			return false;
		}
		
		static void AddTask(string fileName, string message, int column, int line, TaskType taskType)
		{
			TaskService.Add(new Task(FileName.Create(fileName), message, column, line, taskType));
		}
		
		#region XmlView methods
		
		static void ShowErrorList()
		{
			if (ErrorListPad.ShowAfterBuild && TaskService.SomethingWentWrong) {
				WorkbenchSingleton.Workbench.GetPad(typeof(ErrorListPad)).BringPadToFront();
			}
		}
		
		/// <summary>
		/// Converts a set of schemas to a string array, each array item
		/// contains the schema converted to a string.
		/// </summary>
		static string[] GetSchemas(XmlSchemaSet schemaSet, ITextEditor editor)
		{
			List<string> schemas = new List<string>();
			foreach (XmlSchema schema in schemaSet.Schemas()) {
				using (EncodedStringWriter writer = new EncodedStringWriter(Encoding.Default)) { // TODO : use encoding used in ITextEditor (property missing?)
					using (XmlTextWriter xmlWriter = CreateXmlTextWriter(writer, editor)) {
						schema.Write(xmlWriter);
						schemas.Add(writer.ToString());
					}
				}
			}
			return schemas.ToArray();
		}
		
		static XmlTextWriter CreateXmlTextWriter(TextWriter textWriter, ITextEditor editor)
		{
			XmlTextWriter writer = new XmlTextWriter(textWriter);
			if (editor.Options.ConvertTabsToSpaces) {
				writer.Indentation = editor.Options.IndentationSize;
				writer.IndentChar = ' ';
			} else {
				writer.Indentation = 1;
				writer.IndentChar = '\t';
			}
			writer.Formatting = Formatting.Indented;
			return writer;
		}
		
		/// <summary>
		/// Creates a schema based on the xml content.
		/// </summary>
		/// <returns>A set of generated schemas or null if the xml content is not
		/// well formed.</returns>
		public string[] InferSchema()
		{
			ITextEditor editor = TextEditor;
			if (editor == null)	return null;
			
			TaskService.ClearExceptCommentTasks();
			if (IsWellFormed) {
				try {
					using (XmlTextReader reader = new XmlTextReader(new StringReader(editor.Document.Text))) {
						XmlSchemaInference schemaInference = new XmlSchemaInference();
						XmlSchemaSet schemaSet = schemaInference.InferSchema(reader);
						return GetSchemas(schemaSet, editor);
					}
				} catch (XmlSchemaInferenceException ex) {
					AddTask(editor.FileName, ex.Message, ex.LinePosition, ex.LineNumber, TaskType.Error);
				}
			}
			ShowErrorList();
			return null;
		}
		
		/// <summary>
		/// Pretty prints the xml.
		/// </summary>
		public static void FormatXml(ITextEditor editor)
		{
			if (editor == null) return;
			
			TaskService.ClearExceptCommentTasks();
			if (CheckIsWellFormed(editor)) {
				ReplaceAll(editor.Document.Text, editor);
			} else {
				ShowErrorList();
			}
		}
		
		/// <summary>
		/// Replaces the entire text of the xml view with the xml in the
		/// specified. The xml will be formatted.
		/// </summary>
		public static void ReplaceAll(string xml, ITextEditor editor)
		{
			if (editor == null) return;
			
			string formattedXml = SimpleFormat(IndentedFormat(xml, editor));
			editor.Document.Text = formattedXml;
			//UpdateFolding(); // TODO : add again when folding is implemented in AvalonEdit
		}
		
		/// <summary>
		/// Returns a formatted xml string using a simple formatting algorithm.
		/// </summary>
		static string SimpleFormat(string xml)
		{
			return xml.Replace("><", ">\r\n<");
		}
		
		/// <summary>
		/// Returns a pretty print version of the given xml.
		/// </summary>
		/// <param name="xml">Xml string to pretty print.</param>
		/// <returns>A pretty print version of the specified xml.  If the
		/// string is not well formed xml the original string is returned.
		/// </returns>
		static string IndentedFormat(string xml, ITextEditor editor)
		{
			string indentedText = string.Empty;
			if (editor == null) return indentedText;

			try	{
				XmlTextReader reader = new XmlTextReader(new StringReader(xml));
				reader.WhitespaceHandling = WhitespaceHandling.None;
				reader.XmlResolver = null;

				StringWriter indentedXmlWriter = new StringWriter();
				XmlTextWriter writer = CreateXmlTextWriter(indentedXmlWriter, editor);
				writer.WriteNode(reader, false);
				writer.Flush();

				indentedText = indentedXmlWriter.ToString();
			} catch(Exception) {
				indentedText = xml;
			}
			return indentedText;
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
			} else if (!ValidateAgainstSchema()) {
				return;
			}
			
			OutputWindowWriteLine(String.Empty);
			OutputWindowWriteLine(StringParser.Parse("${res:MainWindow.XmlValidationMessages.ValidationSuccess}"));
		}
		
		bool IsSchema {
			get {
				string extension = Path.GetExtension(File.FileName);
				if (extension != null) {
					return String.Compare(".xsd", extension, StringComparison.OrdinalIgnoreCase) == 0;
				}
				return false;
			}
		}
		
		/// <summary>
		/// Validates the XML in the editor against all the schemas in the
		/// schema manager.
		/// </summary>
		bool ValidateAgainstSchema()
		{
			ITextEditor editor = TextEditor;
			if (editor == null) return false;
			
			try {
				StringReader stringReader = new StringReader(editor.Document.Text);
				XmlTextReader xmlReader = new XmlTextReader(stringReader);
				xmlReader.XmlResolver = null;
				XmlReaderSettings settings = new XmlReaderSettings();
				settings.ValidationType = ValidationType.Schema;
				settings.ValidationFlags = XmlSchemaValidationFlags.None;
				settings.XmlResolver = null;
				
				XmlSchemaCompletion schemaData = null;
				try {
					for (int i = 0; i < XmlEditorService.RegisteredXmlSchemas.Schemas.Count; ++i) {
						schemaData = XmlEditorService.RegisteredXmlSchemas.Schemas[i];
						settings.Schemas.Add(schemaData.Schema);
					}
				} catch (XmlSchemaException ex) {
					DisplayValidationError(schemaData.FileName, ex.Message, ex.LinePosition, ex.LineNumber);
					ShowValidationFailedMessage();
					ShowErrorList();
					return false;
				}
				
				XmlReader reader = XmlReader.Create(xmlReader, settings);
				
				XmlDocument doc = new XmlDocument();
				doc.Load(reader);
				return true;
				
			} catch (XmlSchemaException ex) {
				DisplayValidationError(File.FileName, ex.Message, ex.LinePosition, ex.LineNumber);
			} catch (XmlException ex) {
				DisplayValidationError(File.FileName, ex.Message, ex.LinePosition, ex.LineNumber);
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
			ITextEditor editor = TextEditor;
			if (editor == null) return false;
			
			StringReader stringReader = new StringReader(editor.Document.Text);
			XmlTextReader xmlReader = new XmlTextReader(stringReader);
			xmlReader.XmlResolver = null;

			try	{
				XmlSchema schema = XmlSchema.Read(xmlReader, new ValidationEventHandler(SchemaValidation));
				if (schema != null) {
					XmlSchemaSet schemaSet = new XmlSchemaSet();
					schemaSet.Add(schema);
					schemaSet.ValidationEventHandler += SchemaValidation;
					schemaSet.Compile();
				}
			} catch (XmlSchemaException ex) {
				DisplayValidationError(File.FileName, ex.Message, ex.LinePosition, ex.LineNumber);
			} catch (XmlException ex) {
				DisplayValidationError(File.FileName, ex.Message, ex.LinePosition, ex.LineNumber);
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
		
		/// <summary>
		/// Displays the validation error.
		/// </summary>
		static void DisplayValidationError(string fileName, string message, int column, int line)
		{
			OutputWindowWriteLine(message);
			AddTask(fileName, message, column, line, TaskType.Error);
		}
		
		static void ShowValidationFailedMessage()
		{
			OutputWindowWriteLine(string.Empty);
			OutputWindowWriteLine(StringParser.Parse("${res:MainWindow.XmlValidationMessages.ValidationFailed}"));
		}
		
		void SchemaValidation(object source, ValidationEventArgs e)
		{
			if (e.Severity == XmlSeverityType.Error) {
				DisplayValidationError(File.FileName, e.Message, e.Exception.LinePosition, e.Exception.LineNumber);
			} else {
				DisplayValidationWarning(File.FileName, e.Message, e.Exception.LinePosition, e.Exception.LineNumber);
			}
		}
		
		/// <summary>
		/// Displays the validation warning.
		/// </summary>
		static void DisplayValidationWarning(string fileName, string message, int column, int line)
		{
			OutputWindowWriteLine(message);
			AddTask(fileName, message, column, line, TaskType.Warning);
		}
		
		/// <summary>
		/// Applys the stylesheet to the xml and displays the resulting output.
		/// </summary>
		public void RunXslTransform(string xsl)
		{
			try {
				WorkbenchSingleton.Workbench.GetPad(typeof(CompilerMessageView)).BringPadToFront();
				
				TaskService.ClearExceptCommentTasks();
				
				ITextEditor editor = TextEditor;
				if (editor == null) return;
				
				if (IsWellFormed) {
					if (IsValidXsl(xsl)) {
						try {
							string transformedXml = Transform(editor.Document.Text, xsl);
							ShowTransformOutput(transformedXml);
						} catch (XsltException ex) {
							AddTask(GetFileNameFromInnerException(ex, StylesheetFileName), GetInnerExceptionErrorMessage(ex), ex.LineNumber, ex.LinePosition, TaskType.Error);
						}
					}
				}
				
				ShowErrorList();
			} catch (Exception ex) {
				MessageService.ShowException(ex);
			}
		}
		
		/// <summary>
		/// Displays the transformed output.
		/// </summary>
		void ShowTransformOutput(string xml)
		{
			// Pretty print the xml.
			xml = SimpleFormat(IndentedFormat(xml, TextEditor));
			
			// Display the output xml.
			XslOutputView.EditorInstance.Document.Text = xml;
			XslOutputView.Instance.WorkbenchWindow.SelectWindow();
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
				AddTask(StylesheetFileName, GetInnerExceptionErrorMessage(ex), ex.LineNumber, ex.LinePosition, TaskType.Error);
			} catch(XsltException ex) {
				AddTask(StylesheetFileName, ex.Message, ex.LinePosition, ex.LineNumber, TaskType.Error);
			} catch(XmlException ex) {
				AddTask(StylesheetFileName, ex.Message, ex.LinePosition, ex.LineNumber, TaskType.Error);
			}

			return false;
		}
		
		/// <summary>
		/// Brings output window pad to the front.
		/// </summary>
		static void ShowOutputWindow()
		{
			WorkbenchSingleton.Workbench.GetPad(typeof(CompilerMessageView)).BringPadToFront();
		}
		
		/// <summary>
		/// Writes a line of text to the output window.
		/// </summary>
		/// <param name="message">The message to send to the output
		/// window.</param>
		static void OutputWindowWriteLine(string message)
		{
			Category.AppendText(string.Concat(message, Environment.NewLine));
		}
		
		static MessageViewCategory category;
		
		/// <summary>
		/// Gets the xml validation output window.
		/// </summary>
		static MessageViewCategory Category {
			get {
				if (category == null) {
					MessageViewCategory.Create(ref category, CategoryName);
				}
				
				return category;
			}
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
		/// Tries to get the filename from the inner exception otherwise returns the default filename.
		/// </summary>
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
		
		/// <summary>
		/// Output window category name.
		/// </summary>
		public static readonly string CategoryName = "XML";
		#endregion
	}
}
