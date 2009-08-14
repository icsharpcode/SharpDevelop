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
        private Exception _datasourceException = null;

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
                _selectedDatabaseDriver = value;               
                OnPropertyChanged("SelectedDatabaseDriver");
            }
        }

        public IDatasource SelectedDatasource
        {
            get { return _selectedDatasource; }
            set 
            {
                _selectedDatasource = value;
               OnPropertyChanged("SelectedDatasource");
            }
        }

        public IDatabase SelectedDatabase
        {
            get { return _selectedDatabase; }
            set 
            { 
                _selectedDatabase = value;
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

        public Exception DatasourceException
        {
            get { return _datasourceException; }
            set
            {
                _datasourceException = value;
                OnPropertyChanged("DatasourceException");
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

        private void SetException(Exception exception)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => { DatasourceException = exception; }));
        }

        private void PopulateDatasources()
        {
            Thread thread = new Thread(new ThreadStart(delegate()
                {
                    if (SelectedDatabaseDriver != null)
                    {
                        SetIsLoading(true);
                        SelectedDatabaseDriver.PopulateDatasources();
                        SetIsLoading(false);
                    }
                }
            ));

            thread.SetApartmentState(ApartmentState.STA);
            thread.IsBackground = true;
            thread.Start();

        }

        private void PopulateDatabases()
        {
            SetException(null);
   
            Thread thread = new Thread(new ThreadStart(delegate()
            {
                if (SelectedDatabaseDriver != null)
                {
                    SetIsLoading(true);

                    try
                    {
                        SelectedDatabaseDriver.PopulateDatabases();
                    }
                    catch (Exception ex)
                    {
                        SetException(ex);
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

        private void cboDatabaseDriver_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PopulateDatasources();
        }

        private void cboDatasources_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PopulateDatabases();
        }

        private void erbDatasources_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            PopulateDatabases();
        }

        private void cboDatasources_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (SelectedDatabaseDriver != null)
                {
                    IDatasource newDatasource = SelectedDatabaseDriver.AddNewDatasource(cboDatasources.Text);
                    newDatasource.PopulateDatabases();
                }
            }
        }

        private void cboDatabases_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            btnAdd.IsEnabled = true;
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