// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using System.Text;
using System.Threading;
using ICSharpCode.AvalonEdit.Utils;
using ICSharpCode.Core;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.SharpDevelop.Editor;

namespace ICSharpCode.SharpDevelop.Workbench
{
	sealed class FileService : IFileService
	{
		public Encoding DefaultFileEncoding {
			get {
				return Encoding.GetEncoding(SharpDevelop.FileService.DefaultFileEncodingCodePage);
			}
		}
		
		public ITextSource GetFileContent(FileName fileName)
		{
			return GetFileContentForOpenFile(fileName) ?? GetFileContentFromDisk(fileName, CancellationToken.None);
		}
		
		public ITextSource GetFileContent(string fileName)
		{
			return GetFileContent(FileName.Create(fileName));
		}
		
		public ITextSource GetFileContentForOpenFile(FileName fileName)
		{
			return SD.MainThread.InvokeIfRequired(
				delegate {
					OpenedFile file = SharpDevelop.FileService.GetOpenedFile(fileName);
					if (file != null) {
						IFileDocumentProvider p = file.CurrentView as IFileDocumentProvider;
						if (p != null) {
							IDocument document = p.GetDocumentForFile(file);
							if (document != null) {
								return document.CreateSnapshot();
							}
						}
						
						using (Stream s = file.OpenRead()) {
							// load file
							return new StringTextSource(FileReader.ReadFileContent(s, DefaultFileEncoding));
						}
					}
					return null;
				});
		}
		
		public ITextSource GetFileContentFromDisk(FileName fileName, CancellationToken cancellationToken)
		{
			return new StringTextSource(FileReader.ReadFileContent(fileName, DefaultFileEncoding));
		}
	}
}
