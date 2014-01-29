// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
