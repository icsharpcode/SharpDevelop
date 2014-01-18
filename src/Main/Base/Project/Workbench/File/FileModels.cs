// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;

namespace ICSharpCode.SharpDevelop.Workbench
{
	/// <summary>
	/// Pre-defined file models.
	/// </summary>
	public static class FileModels
	{
		/// <summary>
		/// The binary file model provider.
		/// </summary>
		public static readonly IFileModelProvider<IBinaryFileModel> Binary = new BinaryFileModelProvider();
		
		/// <summary>
		/// The text document file model provider.
		/// </summary>
		public static readonly IFileModelProvider<ICSharpCode.AvalonEdit.Document.TextDocument> TextDocument = new TextDocumentFileModelProvider();
		
		/// <summary>
		/// The XDocument file model provider.
		/// </summary>
		public static readonly IFileModelProvider<System.Xml.Linq.XDocument> XDocument = new XDocumentFileModelProvider();
	}
	
	/// <summary>
	/// Represents the binary contents of a file.
	/// </summary>
	public interface IBinaryFileModel
	{
		/// <summary>
		/// Creates a stream that reads from the binary model.
		/// </summary>
		/// <returns>A readable and seekable stream.</returns>
		/// <exception cref="IOException">Error opening the file.</exception>
		Stream OpenRead();
	}
	
	/// <summary>
	/// Simple IBinaryFileModel implementation that uses a byte array.
	/// </summary>
	public class BinaryFileModel : IBinaryFileModel
	{
		readonly byte[] data;
		
		public BinaryFileModel(byte[] data)
		{
			this.data = data;
		}
		
		public Stream OpenRead()
		{
			return new MemoryStream(data);
		}
	}
}
