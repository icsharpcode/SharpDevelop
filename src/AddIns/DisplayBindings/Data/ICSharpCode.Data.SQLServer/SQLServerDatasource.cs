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

using System.Windows.Controls;
using ICSharpCode.Data.Core.DatabaseObjects;
using ICSharpCode.Data.Core.Interfaces;
using ICSharpCode.Data.SQLServer.ControlPanel;
using System.Windows;
using System.Windows.Threading;
using System;
using System.Data.SqlClient;

#endregion

namespace ICSharpCode.Data.Core.DatabaseDrivers.SQLServer
{
    public class SQLServerDatasource : Datasource
    {
        #region Fields

        private SQLServerControlPanel _controlPanel = null;

        #endregion

        #region Properties

        public override UserControl ControlPanel
        {
            get {  return _controlPanel; }
        }

        public string UserId
        {
            get { return GetConnectionStringSetting("User Id"); }
            set 
            { 
                SetConnectionStringSetting("User Id", value);
                OnPropertyChanged("UserId");
            }
        }

        public string Password
        {
            get { return GetConnectionStringSetting("Password"); }
            set 
            { 
                SetConnectionStringSetting("Password", value);
                OnPropertyChanged("Password");
            }
        }

        public bool IntegratedSecurity
        {
            get
            {
                if (GetConnectionStringSetting("Integrated Security") == "SSPI")
                    return true;
                else
                    return false;
            }
            set
            {
                if (value)
                    SetConnectionStringSetting("Integrated Security", "SSPI");
                else
                    SetConnectionStringSetting("Integrated Security", null);

                OnPropertyChanged("IntegratedSecurity");
            }
        }

        public string InitialCatalog
        {
            get { return GetConnectionStringSetting("InitialCatalog", "master"); }
            set
            {
                SetConnectionStringSetting("InitialCatalog", value);
                OnPropertyChanged("InitialCatalog");
            }
        }

        #endregion

        #region Constructor

        public SQLServerDatasource(IDatabaseDriver databaseDriver)
            : base(databaseDriver)
        {            
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
            {
                _controlPanel = new SQLServerControlPanel(this);
            }));
        }

        #endregion

        #region Methods

        protected override bool HandlePopulateDatabasesException(Exception exception)
        {
        	if (exception is SqlException) {
        		SqlException sqlException = exception as SqlException;
        		
        		if (sqlException.Number == 67) {
        			DatabaseDriver.RemoveDatasource(Name);
        			MessageBox.Show("Error while trying to populate databases.\n\n" + exception.Message, DatabaseDriver.Name, MessageBoxButton.OK, MessageBoxImage.Exclamation);
        			return false;
        		} else {
        			throw exception;
        		}
        	} else if (exception is NotSupportedException) {
        		DatabaseDriver.RemoveDatasource(Name);
        		MessageBox.Show("Error while trying to populate databases.\n\n" + exception.Message, DatabaseDriver.Name, MessageBoxButton.OK, MessageBoxImage.Exclamation);
        		return false;
        	} else {
        		throw exception;
        	}
        }

        #endregion
    }
}
