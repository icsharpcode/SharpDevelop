#region Usings

using System.Windows.Controls;
using ICSharpCode.Data.Core.DatabaseObjects;
using ICSharpCode.Data.Core.Interfaces;
using ICSharpCode.Data.SQLServer.ControlPanel;
using System.Windows;
using System.Windows.Threading;
using System;

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
    }
}
