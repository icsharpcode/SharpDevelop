// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using System.Linq;
using Microsoft.Win32;

namespace ICSharpCode.SharpDevelop.Dom.ClassBrowser
{
	/// <summary>
	/// OpenAssemblyFromFileCommand.
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
					try {
						classBrowser.AssemblyList.Assemblies.Add(ClassBrowserPad.CreateAssemblyModelFromFile(openFileDialog.FileName));
					} catch (BadImageFormatException) {
						SD.MessageService.ShowWarningFormatted("{0} is not a valid .NET assembly.", Path.GetFileName(openFileDialog.FileName));
					} catch (FileNotFoundException) {
						SD.MessageService.ShowWarningFormatted("{0} is not accessible or doesn't exist anymore.", openFileDialog.FileName);
					}
				}
			}
		}
	}
	
	/// <summary>
	/// OpenAssemblyFromGACCommand.
	/// </summary>
	class OpenAssemblyFromGACCommand : SimpleCommand
	{
		public override void Execute(object parameter)
		{
			var classBrowser = SD.GetService<IClassBrowser>();
			if (classBrowser != null) {
				OpenFromGacDialog gacDialog = new OpenFromGacDialog();
				if (gacDialog.ShowDialog() ?? false)
				{
					foreach (string assemblyFile in gacDialog.SelectedFileNames) {
						try {
							classBrowser.AssemblyList.Assemblies.Add(ClassBrowserPad.CreateAssemblyModelFromFile(assemblyFile));
						} catch (BadImageFormatException) {
							SD.MessageService.ShowWarningFormatted("{0} is not a valid .NET assembly.", Path.GetFileName(assemblyFile));
						} catch (FileNotFoundException) {
							SD.MessageService.ShowWarningFormatted("{0} is not accessible or doesn't exist anymore.", assemblyFile);
						}
					}
				}
			}
		}
	}
	
	/// <summary>
	/// RemoveAssemblyCommand.
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
}
