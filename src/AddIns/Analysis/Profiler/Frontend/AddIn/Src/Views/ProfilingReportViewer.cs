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
using System.IO;
using ICSharpCode.Core;
using ICSharpCode.Profiler.Controller.Data;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Workbench;

namespace ICSharpCode.Profiler.AddIn.Views
{
	/// <summary>
	/// ViewContent that displays profiling session results.
	/// </summary>
	public class ProfilingReportViewer : AbstractViewContent
	{
		ProfilingDataProvider provider;
		ProfilerView dataView;
		OpenedFile file;
		
		/// <summary>
		/// Returns the ProfilerView of this instance.
		/// </summary>
		public ProfilerView DataView {
			get { return dataView; }
		}
		
		public override object Control {
			get {
				return dataView;
			}
		}
		
		public override FileName PrimaryFileName {
			get { return file.FileName; }
		}
		
		public ProfilingReportViewer(OpenedFile file, ProfilingDataSQLiteProvider provider)
		{
			// HACK : OpenedFile architecture does not allow to keep files open
			//        but it is necessary for the ProfilerView to keep the session file open.
			//        We don't want to load all session data into memory.
			// this.Files.Add(file);
			
			this.file = file;
			this.provider = provider;
			this.TabPageText = Path.GetFileName(file.FileName);
			this.TitleName = this.TabPageText;
			dataView = new ProfilerView(this.provider);
			
			// HACK : The file is not recognised by the FileService for closing if it is deleted while open.
			//        (reason see above comment)
			FileService.FileRemoving += FileServiceFileRemoving;
		}

		void FileServiceFileRemoving(object sender, FileCancelEventArgs e)
		{
			if (FileUtility.IsEqualFileName(e.FileName, file.FileName) ||
			    FileUtility.IsBaseDirectory(e.FileName, file.FileName))
				WorkbenchWindow.CloseWindow(true);
		}

		/// <summary>
		/// Cleans up all used resources
		/// </summary>
		public override void Dispose()
		{
			dataView.SaveUserState();
			provider.Close();
			
			FileService.FileRemoving -= FileServiceFileRemoving;
			base.Dispose();
		}
	}
}
