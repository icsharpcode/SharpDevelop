// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using ICSharpCode.Data.Core.DatabaseObjects;
using ICSharpCode.Data.Core.Interfaces;
using ICSharpCode.Data.Core.UI.Windows;
using System.IO;
using System.Xml.Linq;
using ICSharpCode.Data.EDMDesigner.Core.ObjectModelConverters;
using ICSharpCode.SharpDevelop;

#endregion

namespace ICSharpCode.Data.EDMDesigner.Core.Windows.EDMWizard
{
    public partial class EDMWizardWindow : WizardWindow, INotifyPropertyChanged
    {
        #region Fields

        private DatabaseObjectsCollection<IDatabase> _databases = new DatabaseObjectsCollection<IDatabase>();
        private IDatabase _selectedDatabase = null;
        private string _modelNamespace = string.Empty;
        private string _objectContextName = string.Empty;
        private string _projectStandardNamespace = string.Empty;
        private OpenedFile _file = null;
        private XDocument _edmxDocument = null;

        #endregion

        #region Properties

        public DatabaseObjectsCollection<IDatabase> Databases
        {
            get { return _databases; }
            set
            {
                _databases = value;
                OnPropertyChanged("Databases");
            }
        }

        public IDatabase SelectedDatabase
        {
            get { return _selectedDatabase; }
            set
            {
                _selectedDatabase = value;
                OnPropertyChanged("SelectedDatabase");
                OnPropertyChanged("EntityConnectionString");
                OnPropertyChanged("ModelNamespace");
                OnPropertyChanged("ObjectContextName");
            }
        }

        public OpenedFile File
        {
            get { return _file; }
        }
        
        public XDocument EDMXDocument
        {
        	get { return _edmxDocument; }
        }

        public string EntityConnectionString
        {
            get
            {
                if (_selectedDatabase == null)
                    return string.Empty;

                FileInfo fileInfo = new FileInfo(_file.FileName);

                return string.Format(@"metadata=res://*/{0}.csdl|res://*/{0}.ssdl|res://*/{0}.msl;provider={1};provider connection string=""{2}""",
                    fileInfo.Name.Replace(fileInfo.Extension, string.Empty), _selectedDatabase.Datasource.DatabaseDriver.ProviderName, _selectedDatabase.ConnectionString);
            }
        }

        public string ModelNamespace
        {
            get
            {
                if (_selectedDatabase != null)
                {
                    if (string.IsNullOrEmpty(_modelNamespace))
                        return _selectedDatabase.Name + "Model";
                    else
                        return _modelNamespace;
                }
                else
                    return string.Empty;
            }
            set 
            {
                _modelNamespace = value;
                OnPropertyChanged("ModelNamespace");
            }
        }

        public string ObjectContextName
        {
            get 
            {
                if (_selectedDatabase != null)
                {
                    if (string.IsNullOrEmpty(_objectContextName))
                        return _selectedDatabase.Name.Replace(".", string.Empty) + "ObjectContext";
                    else
                        return _objectContextName;
                }
                else
                    return string.Empty; 
            }
            set
            {
                _objectContextName = value;
                OnPropertyChanged("ObjectContextName");
            }
        }

        #endregion

        #region Constructor

        public EDMWizardWindow(OpenedFile file, string projectStandardNamespace)
        {
            Title = "Entity Framework Wizard";
            Width = 640;
            Height = 480;
            WizardWindowInnards.WizardErrorUserControl = new EDMWizardErrorUserControl();            
            AddWizardUserControl<ChooseDatabaseConnectionUserControl>();
            AddWizardUserControl<ChooseDatabaseObjectsUserControl>();

            _file = file;
            _projectStandardNamespace = projectStandardNamespace;
        }

        #endregion

        #region Methods

        public override void OnFinished()
        {          
            if (SelectedDatabase != null)
            {
                _edmxDocument = EDMConverter.CreateEDMXFromIDatabase(SelectedDatabase, ModelNamespace, _projectStandardNamespace, ObjectContextName);
            }
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
