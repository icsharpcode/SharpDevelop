/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 23.09.2012
 * Time: 16:31
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
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