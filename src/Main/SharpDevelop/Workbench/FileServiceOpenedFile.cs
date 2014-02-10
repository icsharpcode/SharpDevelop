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
using System.Diagnostics;
using System.IO;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.Workbench
{
	sealed class FileServiceOpenedFile : OpenedFile
	{
		readonly FileService fileService;
		List<IViewContent> registeredViews = new List<IViewContent>();
		FileChangeWatcher fileChangeWatcher;
		
		protected override void ChangeFileName(FileName newValue)
		{
			fileService.OpenedFileFileNameChange(this, this.FileName, newValue);
			base.ChangeFileName(newValue);
		}
		
		internal FileServiceOpenedFile(FileService fileService, FileName fileName)
		{
			this.fileService = fileService;
			this.FileName = fileName;
			fileChangeWatcher = new FileChangeWatcher(this);
		}
		
		protected override void UnloadFile()
		{
			bool wasDirty = this.IsDirty;
			fileService.OpenedFileClosed(this);
			base.UnloadFile();
			
			//FileClosed(this, EventArgs.Empty);
			
			if (fileChangeWatcher != null) {
				fileChangeWatcher.Dispose();
				fileChangeWatcher = null;
			}
			
			if (wasDirty && SD.ParserService.GetExistingUnresolvedFile(this.FileName) != null) {
				// We discarded some information when closing the file,
				// so we need to re-parse it.
				if (SD.FileSystem.FileExists(this.FileName))
					SD.ParserService.ParseAsync(this.FileName).FireAndForget();
				else
					SD.ParserService.ClearParseInformation(this.FileName);
			}
		}
		
		public override void SaveToDisk(FileName fileName, FileSaveOptions options)
		{
			try {
				if (fileChangeWatcher != null)
					fileChangeWatcher.Enabled = false;
				base.SaveToDisk(fileName, options);
			} finally {
				if (fileChangeWatcher != null)
					fileChangeWatcher.Enabled = true;
			}
		}
		
		//public override event EventHandler FileClosed = delegate {};
	}
}
