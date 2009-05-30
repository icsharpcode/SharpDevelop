// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

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
		
		/// <summary>
		/// Retrieves additional data for a XML file.
		/// </summary>
		/// <param name="file">The file to retrieve the data for.</param>
		/// <returns>null if the file is not a valid XML file, otherwise a XmlView instance with additional data used by the XML editor.</returns>
		public static XmlView ForFile(OpenedFile file)
		{
			if (!XmlDisplayBinding.IsFileNameHandled(file.FileName))
				return null;
			
			XmlView instance;
			
			if (!mapping.TryGetValue(file, out instance)) {
				file.FileClosed += new EventHandler(FileClosedHandler);
				instance = new XmlView() { File = file };
				mapping.Add(file, instance);
			}
			
			return instance;
		}
		
		public OpenedFile File { get; set; }
		
		public static XmlView ForView(IViewContent view)
		{
			if (view == null)
				return null;
			if (view.PrimaryFile == null)
				return null;
			return ForFile(view.PrimaryFile);
		}

		static void FileClosedHandler(object sender, EventArgs e)
		{
			if (sender is OpenedFile)
				mapping.Remove(sender as OpenedFile);
		}
		
		public string StylesheetFileName { get; set; }
		
		public XmlCompletionDataProvider GetProvider()
		{
			return GetProvider(Path.GetExtension(File.FileName));
		}
		
		public static XmlCompletionDataProvider GetProvider(string extension)
		{
			string defaultNamespacePrefix = XmlSchemaManager.GetNamespacePrefix(extension);
			XmlSchemaCompletionData defaultSchemaCompletionData = XmlSchemaManager.GetSchemaCompletionData(extension);
			return new XmlCompletionDataProvider(XmlSchemaManager.SchemaCompletionDataItems,
			                                     defaultSchemaCompletionData,
			                                     defaultNamespacePrefix);
		}
		
		public ITextEditor GetTextEditor()
		{
			foreach (IViewContent view in File.RegisteredViewContents) {
				ITextEditorProvider provider = view as ITextEditorProvider;
				
				if (provider != null) {
					IDocument document = provider.GetDocumentForFile(File);
					if (document != null)
						return provider.TextEditor;
				}
			}
			
			return null;
		}
		
		public XmlTreeView GetXmlView()
		{
			foreach (IViewContent view in File.RegisteredViewContents) {
				if (view is XmlTreeView)
					return view as XmlTreeView;
			}
			
			return null;
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
			return SelectNodes(GetXmlView().XmlContent, xpath, namespaces);
		}
		
		public void GoToSchemaDefinition()
		{
			// Find schema object for selected xml element or attribute.
			XmlCompletionDataProvider provider = GetProvider();
			XmlSchemaCompletionData currentSchemaCompletionData = provider.FindSchemaFromFileName(File.FileName);
			ITextEditor editor = GetTextEditor();
			
			if (editor == null)
				return;
			
			XmlSchemaObject schemaObject = GetSchemaObjectSelected(editor.Document.Text, editor.Caret.Offset, provider, currentSchemaCompletionData);
			
			// Open schema.
			if (schemaObject != null && schemaObject.SourceUri != null && schemaObject.SourceUri.Length > 0) {
				string fileName = schemaObject.SourceUri.Replace("file:///", String.Empty);
				FileService.JumpToFilePosition(fileName, schemaObject.LineNumber, schemaObject.LinePosition);
			}
		}
		
		/// <summary>
		/// Checks that the xml in this view is well-formed.
		/// </summary>
		public bool IsWellFormed {
			get {
				ITextEditor editor = GetTextEditor();
				if (editor == null)
					return false;
				
				try	{
					XmlDocument Document = new XmlDocument();
					Document.LoadXml(editor.Document.Text);
					return true;
				} catch(XmlException ex) {
					AddTask(editor.FileName, ex.Message, ex.LinePosition - 1, ex.LineNumber - 1, TaskType.Error);
				} catch (WebException ex) {
					AddTask(editor.FileName, ex.Message, 0, 0, TaskType.Error);
				}
				return false;
			}
		}
		
		static void AddTask(string fileName, string message, int column, int line, TaskType taskType)
		{
			TaskService.Add(new Task(fileName, message, column, line, taskType));
		}
		
		#region XmlView methods
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
		
		void ShowErrorList()
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
			writer.Indentation = editor.Options.IndentationString.Length;
			writer.IndentChar = (editor.Options.IndentationString.Length > 0) ? editor.Options.IndentationString[0] : ' ';
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
			ITextEditor editor = GetTextEditor();
			if (editor == null)
				return null;
			
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
		public void FormatXml()
		{
			ITextEditor editor = GetTextEditor();
			if (editor == null)
				return;
			
			TaskService.ClearExceptCommentTasks();
			
			if (IsWellFormed) {
				ReplaceAll(editor.Document.Text);
			} else {
				ShowErrorList();
			}
		}
		
		/// <summary>
		/// Replaces the entire text of the xml view with the xml in the
		/// specified. The xml will be formatted.
		/// </summary>
		public void ReplaceAll(string xml)
		{
			ITextEditor editor = GetTextEditor();
			if (editor == null)
				return;
			
			string formattedXml = SimpleFormat(IndentedFormat(xml));
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
		string IndentedFormat(string xml)
		{
			string indentedText = string.Empty;
			
			ITextEditor editor = GetTextEditor();
			if (editor == null)
				return indentedText;

			try	{
				XmlTextReader reader = new XmlTextReader(new StringReader(xml));
				reader.WhitespaceHandling = WhitespaceHandling.None;

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
			} else {
				if (!ValidateAgainstSchema()) {
					return;
				}
			}
			
			OutputWindowWriteLine(String.Empty);
			OutputWindowWriteLine(StringParser.Parse("${res:MainWindow.XmlValidationMessages.ValidationSuccess}"));
		}
		
		bool IsSchema {
			get {
				string extension = Path.GetExtension(File.FileName);
				if (extension != null) {
					return String.Compare(".xsd", extension, true) == 0;
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
			ITextEditor editor = GetTextEditor();
			if (editor == null)
				return false;
			
			try {
				StringReader stringReader = new StringReader(editor.Document.Text);
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
				DisplayValidationError(File.FileName, ex.Message, ex.LinePosition - 1, ex.LineNumber - 1);
			} catch (XmlException ex) {
				DisplayValidationError(File.FileName, ex.Message, ex.LinePosition - 1, ex.LineNumber - 1);
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
			ITextEditor editor = GetTextEditor();
			if (editor == null)
				return false;
			
			StringReader stringReader = new StringReader(editor.Document.Text);
			XmlTextReader xmlReader = new XmlTextReader(stringReader);
			xmlReader.XmlResolver = null;

			try	{
				XmlSchema schema = XmlSchema.Read(xmlReader, new ValidationEventHandler(SchemaValidation));
				schema.Compile(new ValidationEventHandler(SchemaValidation));
			} catch (XmlSchemaException ex) {
				DisplayValidationError(File.FileName, ex.Message, ex.LinePosition - 1, ex.LineNumber - 1);
			} catch (XmlException ex) {
				DisplayValidationError(File.FileName, ex.Message, ex.LinePosition - 1, ex.LineNumber - 1);
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
				DisplayValidationError(File.FileName, e.Message, e.Exception.LinePosition - 1, e.Exception.LineNumber - 1);
			} else {
				DisplayValidationWarning(File.FileName, e.Message, e.Exception.LinePosition - 1, e.Exception.LineNumber - 1);
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
		
		/// <summary>
		/// Applys the stylesheet to the xml and displays the resulting output.
		/// </summary>
		public void RunXslTransform(string xsl)
		{
			try {
				WorkbenchSingleton.Workbench.GetPad(typeof(CompilerMessageView)).BringPadToFront();
				
				TaskService.ClearExceptCommentTasks();
				
				ITextEditor editor = GetTextEditor();
				if (editor == null)
					return;
				
				if (IsWellFormed) {
					if (IsValidXsl(xsl)) {
						try {
							string transformedXml = Transform(editor.Document.Text, xsl);
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
		/// Displays the transformed output.
		/// </summary>
		void ShowTransformOutput(string xml)
		{
			// Pretty print the xml.
			xml = SimpleFormat(IndentedFormat(xml));
			
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
				AddTask(StylesheetFileName, GetInnerExceptionErrorMessage(ex), ex.LineNumber - 1, ex.LinePosition - 1, TaskType.Error);
			} catch(XsltException ex) {
				AddTask(StylesheetFileName, ex.Message, ex.LinePosition - 1, ex.LineNumber - 1, TaskType.Error);
			} catch(XmlException ex) {
				AddTask(StylesheetFileName, ex.Message, ex.LinePosition - 1, ex.LineNumber - 1, TaskType.Error);
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
			Category.AppendText(String.Concat(message, Environment.NewLine));
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
		
		/// <summary>
		/// Output window category name.
		/// </summary>
		public static readonly string CategoryName = "XML";
		#endregion
	}
}
