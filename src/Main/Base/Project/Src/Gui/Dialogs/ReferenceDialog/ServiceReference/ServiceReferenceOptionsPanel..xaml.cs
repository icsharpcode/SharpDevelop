// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.ComponentModel;
using System.Windows.Input;

using ICSharpCode.SharpDevelop.Gui.Dialogs.ReferenceDialog.ServiceReference;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Widgets;
using Microsoft.Win32;

namespace ICSharpCode.SharpDevelop.Gui.OptionPanels.ServiceReference
{
	public partial class ServiceReferenceOptionsPanel : OptionPanel
	{
		string svcUtilPath;
		bool changed;
		
		public ServiceReferenceOptionsPanel()
		{
			InitializeComponent();
			BrowseCommand = new RelayCommand(Browse);
			svcUtilPath = ServiceReferenceOptions.SvcUtilPath;
			DataContext = this;
		}
		
		public ICommand BrowseCommand { get; private set; }
		
		void Browse()
		{
			string fileName = OptionsHelper.OpenFile(string.Empty,String.Empty,String.Empty,TextBoxEditMode.EditRawProperty); 
			if (!String.IsNullOrEmpty(fileName)) {
				SvcUtilPath = fileName;
			}
		}
		
		
		public string SvcUtilPath {
			get { return svcUtilPath; }
			set {
				svcUtilPath = value;
				changed = true;
				base.RaisePropertyChanged(() => SvcUtilPath);
			}
		}
		
		public override bool SaveOptions()
		{
			if (changed) {
				ServiceReferenceOptions.SvcUtilPath = svcUtilPath;
			}
			return true;
		}
	}
}