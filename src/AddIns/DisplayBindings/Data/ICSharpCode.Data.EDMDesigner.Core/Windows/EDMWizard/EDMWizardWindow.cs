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

#region Usings

using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Xml.Linq;

using ICSharpCode.Data.Core.DatabaseObjects;
using ICSharpCode.Data.Core.Interfaces;
using ICSharpCode.Data.Core.UI.Windows;
using ICSharpCode.Data.EDMDesigner.Core.ObjectModelConverters;
using ICSharpCode.SharpDevelop.Workbench;

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
