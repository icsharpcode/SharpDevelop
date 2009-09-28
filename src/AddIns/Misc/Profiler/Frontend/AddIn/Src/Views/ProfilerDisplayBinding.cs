// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.Core;
using System;
using System.IO;
using ICSharpCode.Profiler.Controller;
using ICSharpCode.Profiler.Controller.Data;
using ICSharpCode.SharpDevelop;

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
			try {
				return new WpfViewer(file);
			} catch (IncompatibleDatabaseException e) {
				MessageService.ShowErrorFormatted("${res:AddIns.Profiler.DatabaseTooNewError}", e.ActualVersion.ToString(), e.ExpectedVersion.ToString());
				return null;
			}
		}
	}
}
