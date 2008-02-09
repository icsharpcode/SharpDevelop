using System;
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
using System.Data;
using System.Data.OleDb;
using System.Data.Common;
using System.Configuration;
using System.Collections.Generic;

using ICSharpCode.DataTools;
using log = ICSharpCode.Core.LoggingService;

namespace ICSharpCode.ServerTools
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class ServerControl : UserControl
    {
    	private DbConnectionsNode _dbConnectionsNode;
    	private ServersNode _serversNode;
    	public const int DATA_CONNECTIONS_VIEW_INDEX = 0;
    	public const int SERVERS_VIEW_INDEX = 1;
    	
        public ServerControl()
        {
            InitializeComponent();
            _dbConnectionsNode = new DbConnectionsNode();
            _serversNode = new ServersNode();
            this.serverTree.Items.Add(_dbConnectionsNode);
            this.serverTree.Items.Add(_serversNode);
            DbControlController dbControlController = new DbControlController(_dbConnectionsNode);
            this.addConnectionButton.Click += dbControlController.AddConnectionButton_Clicked;
            this.refreshButton.Click += dbControlController.RefreshButton_Clicked;
            dbControlController.UpdateDbConnectionsNode();
        }
    }
}
