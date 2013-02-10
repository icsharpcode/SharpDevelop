// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

#region Usings

using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using ICSharpCode.Data.Core.Interfaces;
using System;
using System.Threading;
using System.Windows.Threading;

#endregion

namespace ICSharpCode.Data.Core.UI.Windows
{
	/// <summary>
	/// Interaction logic for ConnectionWizardWindow.xaml
	/// </summary>

	public partial class ConnectionWizardWindow : Window, INotifyPropertyChanged
	{
		#region Fields

		private Action _addAction = null;
		private IDatabaseDriver _selectedDatabaseDriver = null;
		private IDatasource _selectedDatasource = null;
		private IDatabase _selectedDatabase = null;
		private bool _isLoading = false;
		IDatasource defaultDataSource = null;

		#endregion

		#region Properties

		public Action AddAction
		{
			get { return _addAction; }
			set { _addAction = value; }
		}

		public IDatabaseDriver SelectedDatabaseDriver
		{
			get { return _selectedDatabaseDriver; }
			set
			{
				if (_selectedDatabaseDriver != value) {
					defaultDataSource = GetDefaultDatasource(value);
					_selectedDatasource = defaultDataSource;
				} else if (value == null) {
					defaultDataSource = null;
					_selectedDatasource = null;
				}
				_selectedDatabaseDriver = value;
				OnPropertyChanged("SelectedDatabaseDriver");
				OnPropertyChanged("SelectedDatasource");
				OnPropertyChanged("CanConnect");
			}
		}
		
		public bool CanConnect {
			get { return _selectedDatabaseDriver != null; }
		}
		
		IDatasource GetDefaultDatasource(IDatabaseDriver driver)
		{
			if (driver != null) {
				return driver.CreateNewIDatasource("");
			}
			return null;
		}
		
		public IDatasource SelectedDatasource {
			get { return _selectedDatasource; }
			set {
				if (value != null)
					_selectedDatasource = value;
				else
					_selectedDatasource = defaultDataSource;
				OnPropertyChanged("SelectedDatasource");
				OnPropertyChanged("CanConnect");
			}
		}

		public IDatabase SelectedDatabase
		{
			get { return _selectedDatabase; }
			set
			{
				_selectedDatabase = value;
				btnAdd.IsEnabled = (_selectedDatabase != null);
				OnPropertyChanged("SelectedDatabase");
			}
		}
		
		public bool IsLoading
		{
			get { return _isLoading; }
			set
			{
				_isLoading = value;
				OnPropertyChanged("IsLoading");
			}
		}

		#endregion

		#region Constructor

		public ConnectionWizardWindow()
		{
			InitializeComponent();
		}

		#endregion

		#region Private methods

		private void SetIsLoading(bool value)
		{
			Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => { IsLoading = value; }));
		}
		
		private void SetSelectedDatasource(IDatasource datasource)
		{
			Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => { SelectedDatasource = datasource; }));
		}

		private void PopulateDatasources()
		{
			Thread thread = new Thread(
				new ThreadStart(
					delegate() {
						try {
							if (SelectedDatabaseDriver != null) {
								SetIsLoading(true);
								SelectedDatabaseDriver.PopulateDatasources();
							}
						} catch (Exception ex) {
							Dispatcher.BeginInvoke(DispatcherPriority.Background,
							                       new Action(() => {
							                                  	MessageBox.Show(this, ex.Message, this.Title, MessageBoxButton.OK, MessageBoxImage.Error);
							                                  }));
						} finally {
							SetIsLoading(false);
						}
					}
				)
			);

			thread.SetApartmentState(ApartmentState.STA);
			thread.IsBackground = true;
			thread.Start();
		}

		private void PopulateDatabases()
		{
			Thread thread = new Thread(new ThreadStart(
				delegate() {
					if (SelectedDatabaseDriver != null)
					{
						SetIsLoading(true);

						try
						{
							SelectedDatabaseDriver.PopulateDatabases(_selectedDatasource);
						}
						catch (Exception ex)
						{
							Dispatcher.BeginInvoke(DispatcherPriority.Background,
							                       new Action(() => {
							                                  	MessageBox.Show(this, ex.Message, this.Title, MessageBoxButton.OK, MessageBoxImage.Error);
							                                  }));
						}

						SetIsLoading(false);
					}
				}));

			thread.SetApartmentState(ApartmentState.STA);
			thread.IsBackground = true;
			thread.Start();
		}

		#endregion

		#region Event handlers

		private void btnAutoDiscover_Click(object sender, RoutedEventArgs e)
		{
			PopulateDatasources();
		}

		private void btnConnect_Click(object sender, RoutedEventArgs e)
		{
			if (!_selectedDatabaseDriver.IDatasources.Contains(_selectedDatasource))
				_selectedDatasource.Name = cboDatasources.Text;
			PopulateDatabases();
		}

		private void cboDatasources_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				if (SelectedDatabaseDriver != null)
				{
					SelectedDatasource = SelectedDatabaseDriver.AddNewDatasource(cboDatasources.Text);
				}
			}
		}

		private void btnAdd_Click(object sender, RoutedEventArgs e)
		{
			if (_addAction == null)
			{
				DialogResult = true;
				Close();
			}
			else
			{
				_addAction.Invoke();
			}
		}

		private void btnCancel_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = false;
			Close();
		}

		#endregion

		#region INotifyPropertyChanged

		public event PropertyChangedEventHandler PropertyChanged;

		protected void OnPropertyChanged(string property)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(property));
			}
		}

		#endregion
	}
}
