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
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace ICSharpCode.SharpDevelop.Gui.OptionPanels
{
	/// <summary>
	/// Interaction logic for ProjectInformation.xaml
	/// </summary>
	public partial class ProjectInformation : UserControl, INotifyPropertyChanged
	{
		public ProjectInformation()
		{
			InitializeComponent();
			this.DataContext = this;
		}
		
		private string projectFolder;
		
		public string ProjectFolder {
			get { return projectFolder; }
			set { projectFolder = value;
			RaisePropertyChanged("ProjectFolder");}
		}
		
		
		private string projectFile;
		
		public string ProjectFile {
			get { return projectFile; }
			set { projectFile = value;
				RaisePropertyChanged("ProjectFile");}
		}
		
		private string outputTypeName;
		
		public string OutputTypeName {
			get { return outputTypeName; }
			set { outputTypeName = value;
				RaisePropertyChanged("OutputTypeName");}
		}
		
		#region INotifyPropertyChanged
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		private void RaisePropertyChanged (string propertyName)
		{
			var handler = this.PropertyChanged;
			if (handler != null)
			{
				handler(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
			}
		}
		
		#endregion
	}
}
