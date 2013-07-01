// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows.Input;
using NuGet;

namespace ICSharpCode.PackageManagement
{
	public class FileConflictViewModel
	{
		FileConflictResolution resolution = FileConflictResolution.Ignore;
		
		public FileConflictViewModel(string message)
		{
			this.Message = message;
			CreateCommands();
		}
		
		void CreateCommands()
		{
			YesCommand = new DelegateCommand(param => UpdateResolution(FileConflictResolution.Overwrite));
			YesToAllCommand = new DelegateCommand(param => UpdateResolution(FileConflictResolution.OverwriteAll));
			NoCommand = new DelegateCommand(param => UpdateResolution(FileConflictResolution.Ignore));
			NoToAllCommand = new DelegateCommand(param => UpdateResolution(FileConflictResolution.IgnoreAll));
		}
		
		void UpdateResolution(FileConflictResolution resolution)
		{
			this.resolution = resolution;
			Close(this, new EventArgs());
		}
		
		public event EventHandler Close;
		
		public ICommand YesCommand { get; private set; }
		public ICommand YesToAllCommand { get; private set; }
		public ICommand NoCommand { get; private set; }
		public ICommand NoToAllCommand { get; private set; }
		
		public string Message { get; private set; }
		
		public FileConflictResolution GetResolution()
		{
			return resolution;
		}
	}
}
