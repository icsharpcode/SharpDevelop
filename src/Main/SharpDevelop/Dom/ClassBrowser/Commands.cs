// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using System.Linq;
using Microsoft.Win32;

namespace ICSharpCode.SharpDevelop.Dom.ClassBrowser
{
	/// <summary>
	/// Description of OpenAssemblyFromFileCommand.
	/// </summary>
	class OpenAssemblyFromFileCommand : SimpleCommand
	{
		public override void Execute(object parameter)
		{
			var classBrowser = SD.GetService<IClassBrowser>();
			if (classBrowser != null) {
				OpenFileDialog openFileDialog = new OpenFileDialog();
				openFileDialog.Filter = "Assembly files (*.exe, *.dll)|*.exe;*.dll";
				openFileDialog.CheckFileExists = true;
				openFileDialog.CheckPathExists = true;
				if (openFileDialog.ShowDialog() ?? false)
				{
					classBrowser.AssemblyList.Assemblies.Add(ClassBrowserPad.CreateAssemblyModelFromFile(openFileDialog.FileName));
				}
			}
		}
	}
	
	/// <summary>
	/// Description of OpenAssemblyFromGACCommand.
	/// </summary>
	class OpenAssemblyFromGACCommand : SimpleCommand
	{
		public override void Execute(object parameter)
		{
//			throw new NotImplementedException();
		}
	}
	
	/// <summary>
	/// Description of RemoveAssemblyCommand.
	/// </summary>
	class RemoveAssemblyCommand : SimpleCommand
	{
		public override bool CanExecute(object parameter)
		{
			return parameter is AssemblyModel;
		}
		
		public override void Execute(object parameter)
		{
			var classBrowser = SD.GetService<IClassBrowser>();
			if (classBrowser != null) {
				IAssemblyModel assemblyModel = (IAssemblyModel) parameter;
				classBrowser.AssemblyList.Assemblies.Remove(assemblyModel);
			}
		}
	}
	
	/// <summary>
	/// Description of RemoveAssemblyCommand.
	/// </summary>
	class ClassBrowserCollapseAllCommand : SimpleCommand
	{
		public override void Execute(object parameter)
		{
//			var classBrowser = SD.GetService<IClassBrowser>() as ClassBrowserPad;
//			if (classBrowser != null) {
//				classBrowser.TreeView
//			}
		}
	}
}
