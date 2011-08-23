// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using ICSharpCode.Core;
using ICSharpCode.Profiler.Controller.Data;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;

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
		
		public ICSharpCode.SharpDevelop.Gui.IViewContent CreateContentForFile(OpenedFile file)
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
			return new WpfViewer(file, provider);
		}
		
		public bool IsPreferredBindingForFile(string fileName)
		{
			return true;
		}
		
		public double AutoDetectFileContent(string fileName, Stream fileContent, string detectedMimeType)
		{
			return 1;
		}
	}
}
