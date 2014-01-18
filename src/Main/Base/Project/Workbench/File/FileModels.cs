// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using ICSharpCode.AvalonEdit.Document;

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
		public static readonly IFileModelProvider<IBinaryModel> Binary = new BinaryFileModelProvider();
		
		/// <summary>
		/// The text document file model provider.
		/// </summary>
		public static readonly IFileModelProvider<TextDocument> TextDocument = new TextDocumentFileModelProvider();
	}
	
	/// <summary>
	/// Represents the binary contents of a file.
	/// </summary>
	public interface IBinaryModel
	{
		/// <summary>
		/// Creates a stream that reads from the binary model.
		/// </summary>
		/// <returns>A readable and seekable stream.</returns>
		/// <exception cref="IOException">Error opening the file.</exception>
		Stream OpenRead();
	}
}
