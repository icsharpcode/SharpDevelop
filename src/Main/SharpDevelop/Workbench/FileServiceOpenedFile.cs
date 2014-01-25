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
			IsUntitled = false;
			fileChangeWatcher = new FileChangeWatcher(this);
		}
		
		internal FileServiceOpenedFile(FileService fileService, byte[] fileData)
		{
			this.fileService = fileService;
			this.FileName = null;
			SetData(fileData);
			IsUntitled = true;
			MakeDirty();
			fileChangeWatcher = new FileChangeWatcher(this);
		}
		
		/// <summary>
		/// Gets the list of view contents registered with this opened file.
		/// </summary>
		public override IList<IViewContent> RegisteredViewContents {
			get { return registeredViews.AsReadOnly(); }
		}
		
		public override void ForceInitializeView(IViewContent view)
		{
			if (view == null)
				throw new ArgumentNullException("view");
			if (!registeredViews.Contains(view))
				throw new ArgumentException("registeredViews must contain view");
			
			base.ForceInitializeView(view);
		}
		
		public override void RegisterView(IViewContent view)
		{
			if (view == null)
				throw new ArgumentNullException("view");
			if (registeredViews.Contains(view))
				throw new ArgumentException("registeredViews already contains view");
			
			registeredViews.Add(view);
			
			if (SD.Workbench != null) {
				SD.Workbench.ActiveViewContentChanged += WorkbenchActiveViewContentChanged;
				if (SD.Workbench.ActiveViewContent == view) {
					SwitchedToView(view);
				}
			}
			#if DEBUG
			view.Disposed += ViewDisposed;
			#endif
		}
		
		public override void UnregisterView(IViewContent view)
		{
			if (view == null)
				throw new ArgumentNullException("view");
			Debug.Assert(registeredViews.Contains(view));
			
			if (SD.Workbench != null) {
				SD.Workbench.ActiveViewContentChanged -= WorkbenchActiveViewContentChanged;
			}
			#if DEBUG
			view.Disposed -= ViewDisposed;
			#endif
			
			registeredViews.Remove(view);
			if (registeredViews.Count > 0) {
				if (currentView == view) {
					SaveCurrentView();
					currentView = null;
				}
			} else {
				// all views to the file were closed
				CloseIfAllViewsClosed();
			}
		}
		
		public override void CloseIfAllViewsClosed()
		{
			if (registeredViews.Count == 0) {
				bool wasDirty = this.IsDirty;
				fileService.OpenedFileClosed(this);
				
				FileClosed(this, EventArgs.Empty);
				
				if (fileChangeWatcher != null) {
					fileChangeWatcher.Dispose();
					fileChangeWatcher = null;
				}
				
				if (wasDirty) {
					// We discarded some information when closing the file,
					// so we need to re-parse it.
					if (File.Exists(this.FileName))
						SD.ParserService.ParseAsync(this.FileName).FireAndForget();
					else
						SD.ParserService.ClearParseInformation(this.FileName);
				}
			}
		}
		
		#if DEBUG
		void ViewDisposed(object sender, EventArgs e)
		{
			Debug.Fail("View was disposed while still registered with OpenedFile!");
		}
		#endif
		
		void WorkbenchActiveViewContentChanged(object sender, EventArgs e)
		{
			IViewContent newView = SD.Workbench.ActiveViewContent;
			
			if (!registeredViews.Contains(newView))
				return;
			
			SwitchedToView(newView);
		}
		
		public override void SaveToDisk()
		{
			try {
				if (fileChangeWatcher != null)
					fileChangeWatcher.Enabled = false;
				base.SaveToDisk();
			} finally {
				if (fileChangeWatcher != null)
					fileChangeWatcher.Enabled = true;
			}
		}
		
		public override event EventHandler FileClosed = delegate {};
	}
}
