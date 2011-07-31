// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Diagnostics;
using System.Windows.Input;

namespace ICSharpCode.PackageManagement
{
	public class OpenHyperlinkCommand : ICommand
	{	
		public event EventHandler CanExecuteChanged;
		
		protected virtual void OnCanExecuteChanged()
		{
			if (CanExecuteChanged != null) {
				CanExecuteChanged(this, new EventArgs());
			}
		}
		
		public bool CanExecute(object parameter)
		{
			return true;
		}
		
		public void Execute(object parameter)
		{
			Uri uri = parameter as Uri;
			if (uri != null) {
				StartProcess(uri.AbsoluteUri);
			} else {
				StartProcess(parameter as string);
			}
		}
		
		protected virtual void StartProcess(string fileName)
		{
			System.Diagnostics.Process.Start(fileName);
		}
	}
}
