// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Christian Hornung" email="chhornung@googlemail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.TextEditor.Document;
using ICSharpCode.TextEditor.Util;

namespace ICSharpCode.FormsDesigner
{
	/// <summary>
	/// Manages the source code files and their content associated with
	/// an open forms designer view.
	/// </summary>
	public sealed class DesignerSourceCodeStorage : IEnumerable<KeyValuePair<OpenedFile, IDocument>>
	{
		readonly Dictionary<OpenedFile, FileContent> fileContents = new Dictionary<OpenedFile, FileContent>();
		OpenedFile designerCodeFile;
		
		public DesignerSourceCodeStorage()
		{
		}
		
		public OpenedFile DesignerCodeFile {
			get { return this.designerCodeFile; }
			set {
				if (value != null && !this.fileContents.ContainsKey(value)) {
					throw new InvalidOperationException("The specified DesignerCodeFile '" + value.FileName + "' is not registered with this DesignerSourceCodeStorage instance.");
				}
				this.designerCodeFile = value;
			}
		}
		
		/// <summary>
		/// Gets the <see cref="IDocument"/> associated with the specified file or
		/// <c>null</c> if the file is not registered with the current instance.
		/// </summary>
		public IDocument this[OpenedFile file] {
			get {
				FileContent c;
				if (this.fileContents.TryGetValue(file, out c)) {
					return c.Document;
				} else {
					return null;
				}
			}
		}
		
		public IEnumerator<KeyValuePair<OpenedFile, IDocument>> GetEnumerator()
		{
			foreach (KeyValuePair<OpenedFile, FileContent> entry in this.fileContents) {
				yield return new KeyValuePair<OpenedFile, IDocument>(entry.Key, entry.Value.Document);
			}
		}
		
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}
		
		public void LoadFile(OpenedFile file, Stream stream)
		{
			if (stream == null)
				throw new ArgumentNullException("stream");
			if (file == null)
				throw new ArgumentNullException("file");
			
			FileContent c;
			if (!this.fileContents.TryGetValue(file, out c)) {
				c = new FileContent();
				this.fileContents.Add(file, c);
			}
			c.LoadFrom(stream);
		}
		
		public void SaveFile(OpenedFile file, Stream stream)
		{
			if (stream == null)
				throw new ArgumentNullException("stream");
			if (file == null)
				throw new ArgumentNullException("file");
			
			FileContent c;
			if (!this.fileContents.TryGetValue(file, out c)) {
				throw new InvalidOperationException("Cannot save file '" + file.FileName + "' because this file is not registered with this DesignerSourceCodeStorage instance.");
			}
			c.SaveTo(stream);
		}
		
		/// <summary>
		/// Adds a file with an empty document.
		/// </summary>
		public void AddFile(OpenedFile file)
		{
			this.fileContents.Add(file, new FileContent());
		}
		
		/// <summary>
		/// Adds a file with an empty document and the specified encoding.
		/// </summary>
		public void AddFile(OpenedFile file, Encoding encoding)
		{
			this.fileContents.Add(file, new FileContent(encoding));
		}
		
		/// <summary>
		/// Adds a file with the specified document and the specified encoding.
		/// </summary>
		public void AddFile(OpenedFile file, IDocument document, Encoding encoding)
		{
			this.fileContents.Add(file, new FileContent(document, encoding));
		}
		
		/// <summary>
		/// Adds a file with the specified document and the specified encoding.
		/// </summary>
		/// <param name="doNotLoad">When true, the SourceCodeStorage will not load the stream content into the document when the view is loaded. Use this when the document content is already managed elsewhere.</param>
		public void AddFile(OpenedFile file, IDocument document, Encoding encoding, bool doNotLoad)
		{
			this.fileContents.Add(file, new FileContent(document, encoding, doNotLoad));
		}
		
		public bool ContainsFile(OpenedFile file)
		{
			return this.fileContents.ContainsKey(file);
		}
		
		public bool RemoveFile(OpenedFile file)
		{
			return this.fileContents.Remove(file);
		}
		
		public Encoding GetFileEncoding(OpenedFile file)
		{
			if (file == null)
				throw new ArgumentNullException("file");
			
			FileContent c;
			if (!this.fileContents.TryGetValue(file, out c)) {
				throw new InvalidOperationException("Cannot get Encoding for file '" + file.FileName + "' because this file is not registered with this DesignerSourceCodeStorage instance.");
			}
			return c.Encoding;
		}
		
		
		/// <summary>
		/// Stores the content of a file in an <see cref="IDocument"/>
		/// along with its encoding.
		/// </summary>
		sealed class FileContent
		{
			static readonly DocumentFactory documentFactory = new DocumentFactory();
			
			Encoding encoding;
			readonly IDocument document;
			readonly bool doNotLoad;
			
			public FileContent()
				: this(ParserService.DefaultFileEncoding)
			{
			}
			
			public FileContent(Encoding encoding)
				: this(documentFactory.CreateDocument(), encoding)
			{
			}
			
			public FileContent(IDocument document, Encoding encoding)
				: this(document, encoding, false)
			{
			}
			
			public FileContent(IDocument document, Encoding encoding, bool doNotLoad)
			{
				if (document == null)
					throw new ArgumentNullException("document");
				if (encoding == null)
					throw new ArgumentNullException("encoding");
				this.document = document;
				this.encoding = encoding;
				this.doNotLoad = doNotLoad;
			}
			
			public IDocument Document {
				get { return this.document; }
			}
			
			public Encoding Encoding {
				get { return this.encoding; }
			}
			
			public void LoadFrom(Stream stream)
			{
				if (this.doNotLoad)
					return;
				this.encoding = ParserService.DefaultFileEncoding;
				this.Document.TextContent = FileReader.ReadFileContent(stream, ref this.encoding);
			}
			
			public void SaveTo(Stream stream)
			{
				using(StreamWriter writer = new StreamWriter(stream, this.encoding)) {
					writer.Write(this.Document.TextContent);
				}
			}
		}
	}
}
