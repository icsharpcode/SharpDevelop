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
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Workbench;

namespace ICSharpCode.Profiler.AddIn.Views
{
	/// <summary>
	/// Description of ProfilerDisplayBinding.
	/// </summary>
	public class ProfilerDisplayBinding : IDisplayBinding
	{
		public ProfilerDisplayBinding()
		{
		}
		
		public bool CanCreateContentForFile(string fileName)
		{
			return true; // definition in .addin does extension-based filtering
		}
		
		public IViewContent CreateContentForFile(OpenedFile file)
		{
			ProfilingDataSQLiteProvider provider;
			try {
				provider = ProfilingDataSQLiteProvider.FromFile(file.FileName);
			} catch (IncompatibleDatabaseException e) {
				if (e.ActualVersion == new Version(1, 0)) {
					if (MessageService.AskQuestion("Upgrade DB?")) {
						using (AsynchronousWaitDialog.ShowWaitDialog("Upgrading database...")) {
							provider = ProfilingDataSQLiteProvider.UpgradeFromOldVersion(file.FileName);
						}
					} else {
						return null;
					}
				} else {
					MessageService.ShowErrorFormatted("${res:AddIns.Profiler.DatabaseTooNewError}", e.ActualVersion.ToString(), e.ExpectedVersion.ToString());
					return null;
				}
			}
			return new ProfilingReportViewer(file, provider);
		}
		
		public bool IsPreferredBindingForFile(FileName fileName)
		{
			return true;
		}
		
		public bool CanCreateContentForFile(FileName fileName)
		{
			return fileName.GetExtension().Equals(".sdps", StringComparison.OrdinalIgnoreCase);
		}
		
		public double AutoDetectFileContent(FileName fileName, Stream fileContent, string detectedMimeType)
		{
			return 1;
		}
	}
}
