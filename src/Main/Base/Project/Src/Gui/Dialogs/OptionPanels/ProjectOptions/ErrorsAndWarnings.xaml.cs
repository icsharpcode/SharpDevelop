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

using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Gui.OptionPanels
{
	public partial class ErrorsAndWarnings : UserControl, INotifyPropertyChanged
	{
		private List<KeyItemPair> warnLevel;
		private ProjectOptionPanel projectOptions;
		
		public ErrorsAndWarnings()
		{
			InitializeComponent();
			this.DataContext = this;
			
			this.warnLevel = new List<KeyItemPair>();
			this.warnLevel.Add(new KeyItemPair("0","0"));
			this.warnLevel.Add(new KeyItemPair("1","1"));
			this.warnLevel.Add(new KeyItemPair("2","2"));
			this.warnLevel.Add(new KeyItemPair("3","3"));
			this.warnLevel.Add(new KeyItemPair("4","4"));
			this.WarnLevel = warnLevel;
		}
		
		#region IProjectUserControl
		
		public void Initialize (ProjectOptionPanel projectOptions)
		{
			if (projectOptions == null) {
				throw new ArgumentNullException("projectOptions");
			}
			this.projectOptions = projectOptions;
		}
		
		#endregion
		
		public ProjectOptionPanel.ProjectProperty<string> WarningLevel {
			get {return projectOptions.GetProperty("WarningLevel","4",TextBoxEditMode.EditEvaluatedProperty ); }
		}
		
		
		public ProjectOptionPanel.ProjectProperty<string> NoWarn {
			get {return projectOptions.GetProperty("NoWarn","",TextBoxEditMode.EditRawProperty ); }
		}
		
		
		public List<KeyItemPair> WarnLevel {
			get { return warnLevel; }
			set { warnLevel = value;
				RaisePropertyChanged("WarnLevel");
			}
		}
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		private void RaisePropertyChanged (string propertyName)
		{
			var handler = this.PropertyChanged;
			if (handler != null)
			{
				handler(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
			}
		}
	}
}
