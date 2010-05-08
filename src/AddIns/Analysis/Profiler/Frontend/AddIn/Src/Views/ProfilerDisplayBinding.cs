// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

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
			return Path.GetExtension(fileName) == ".sdps";
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
	}
}
