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
