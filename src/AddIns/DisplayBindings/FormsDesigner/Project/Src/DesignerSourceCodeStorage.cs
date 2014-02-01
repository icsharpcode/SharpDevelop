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
using System.Text;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.Core;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Refactoring;
using ICSharpCode.SharpDevelop.Workbench;

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
				c = new FileContent(file.FileName);
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
			this.fileContents.Add(file, new FileContent(file.FileName));
		}
		
		/// <summary>
		/// Adds a file with an empty document and the specified encoding.
		/// </summary>
		public void AddFile(OpenedFile file, Encoding encoding)
		{
			this.fileContents.Add(file, new FileContent(file.FileName, encoding));
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
			Encoding encoding;
			readonly IDocument document;
			readonly bool doNotLoad;
			
			public FileContent(FileName fileName)
				: this(fileName, SD.FileService.DefaultFileEncoding)
			{
			}
			
			public FileContent(FileName fileName, Encoding encoding)
				: this(new TextDocument { FileName = fileName }, encoding)
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
				using (StreamReader r = ICSharpCode.AvalonEdit.Utils.FileReader.OpenStream(stream, SD.FileService.DefaultFileEncoding)) {
					this.Document.Text = r.ReadToEnd();
					this.encoding = r.CurrentEncoding;
				}
			}
			
			public void SaveTo(Stream stream)
			{
				using(StreamWriter writer = new StreamWriter(stream, this.encoding)) {
					writer.Write(this.Document.Text);
				}
			}
		}
	}
}
