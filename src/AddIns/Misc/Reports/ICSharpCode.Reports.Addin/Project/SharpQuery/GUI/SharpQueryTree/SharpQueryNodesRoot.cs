// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Luc Morin" email=""/>
//     <version>$Revision$</version>
// </file>

// created on 04/11/2003 at 17:29

using System;
using System.Reflection;
using System.Windows.Forms;

using ICSharpCode.Core;
using SharpQuery.Collections;
using SharpQuery.Connection;
using SharpQuery.Exceptions;
using SharpQuery.SchemaClass;

namespace SharpQuery.Gui.TreeView
{
	///<summary>
	/// this is the root of all others nodes!
	///</summary>
	public class SharpQueryNodeDatabaseRoot : AbstractSharpQueryNode
	{
		public override string AddinContextMenu
		{
			get
			{
				return "/SharpQuery/ContextMenu/DatabaseRoot";
			}
		}

		public override string entityName
		{
			get
			{
				return StringParser.Parse("${res:SharpQuery.Label.DataBasesRoot}");
			}
		}

		public SharpQueryNodeDatabaseRoot()
			: base(null)
		{
			this.Text = this.entityName;

			this.ImageIndex = 0;
			this.SelectedImageIndex = 0;
		}

		public override void Refresh()
		{
			this.Text = this.entityName;

			foreach (ISharpQueryNode node in this.Nodes)
			{
				node.Refresh();
			}
		}

		public override void Clear()
		{
			foreach (ISharpQueryNode node in this.Nodes)
			{
				node.Clear();
			}
		}

		public override void BuildsChilds()
		{
			IConnection connection = null;
			ISharpQueryNode node = null;

			try
			{
				connection = AbstractSharpQueryConnectionWrapper.CreateFromDataConnectionLink();

				if (connection != null)
				{
					string ChildClass = "";

					if (SharpQueryTree.SchemaClassDict.ContainsKey(connection.GetType().FullName) == true)
					{
						ChildClass = SharpQueryTree.SchemaClassDict[connection.GetType().FullName];
					}

					if ((ChildClass != null) && (ChildClass != ""))
					{
						node = (ISharpQueryNode)ass.CreateInstance(ChildClass, false, BindingFlags.CreateInstance, null, new object[] { connection }, null, null);
					}
					else
					{
						node = new SharpQueryNodeNotSupported(new SharpQueryNotSupported(connection, "", "", "", connection.GetType().FullName));
					}


					//TODO : do an interface for the node connection!
					(node as SharpQueryNodeConnection).Connect();
					this.Nodes.Add(node as TreeNode);
					node.Refresh();

					if (node.Connection.IsConnectionStringWrong == true)
					{
						this.Nodes.Remove(node as TreeNode);
					}
					else
					{
						this.Expand();
					}
				}
			}
			catch (ConnectionStringException e)
			{
				if (this.Nodes.Contains(node as TreeNode) == true)
				{
					this.Nodes.Remove(node as TreeNode);
				}
				MessageService.ShowError(e.Message);
			}
		}
	}


	///<summary>
	/// Root nodes for a connection to a database
	///</summary>
	public class SharpQueryNodeConnection : AbstractSharpQueryNode
	{
		IConnection pConnection = null;

		public override string entityNormalizedName
		{
			get
			{
				if (this.pConnection != null)
				{
					return this.pConnection.NormalizedName;
				}
				else
				{
					return "";
				}
			}
		}

		public override string entityName
		{
			get
			{
				if (this.pConnection != null)
				{
					return this.pConnection.Name;
				}
				else
				{
					return "";
				}
			}
		}

		public override IConnection Connection
		{
			get
			{
				if (this.pConnection != null)
				{
					return this.pConnection;
				}
				else
				{
					return null;
				}
			}
		}

		public override SharpQueryListDictionary Entities
		{
			get
			{
				if (this.Connection != null)
				{
					return this.Connection.Entities;
				}
				else
				{
					return null;
				}
			}
		}

		public override string AddinContextMenu
		{
			get
			{
				return "/SharpQuery/ContextMenu/DatabaseConnection";
			}
		}

		public SharpQueryNodeConnection(IConnection dataConnection)
			: base(null)
		{
			this.pConnection = dataConnection;
			this.ImageIndex = 1;
			this.SelectedImageIndex = 1;
		}

		public bool IsConnected
		{
			get
			{
				return this.Connection.IsOpen;
			}
		}

		public void Disconnect()
		{
			if (this.IsConnected == true)
			{
				this.Collapse();
				this.Clear();
				this.Connection.Close();
				this.ImageIndex = 1;
				this.SelectedImageIndex = 1;
			}
		}

		public void Connect()
		{
			try
			{
				if (this.IsConnected == false)
				{
					if (this.Connection.Open())
					{
						this.Refresh();
						this.ImageIndex = 2;
						this.SelectedImageIndex = 2;
					}
				}
			}
			catch (OpenConnectionException e)
			{
				MessageService.ShowError(e.Message);
			}
		}

		protected override void OnRefresh()
		{
			if (this.IsConnected == true)
			{
				this.Clear();
				this.Connection.Refresh();
			}
		}

		public override void Clear()
		{
			if (this.Connection != null)
			{
				this.Connection.Clear();
			}

			base.Clear();
		}

		public void RemoveConnection()
		{
			this.Disconnect();
			this.pConnection = null;

			this.Parent.Nodes.Remove(this);
		}

		public void ModifyConnection()
		{
			IConnection Oldconnection = this.pConnection;
			bool error = false;
			try
			{
				IConnection connection = null;

				connection = AbstractSharpQueryConnectionWrapper.UpDateFromDataConnectionLink(this.Connection);

				if (connection != null)
				{
					this.Disconnect();
					this.pConnection = connection;
					this.Refresh();
					error = this.pConnection.IsConnectionStringWrong;
				}
			}
			catch (ConnectionStringException e)
			{
				error = true;
				MessageService.ShowError(e.Message);
			}
			finally
			{
				if (error == true)
				{
					this.pConnection = Oldconnection;
					this.Connect();
					this.Refresh();
				}
			}
		}
	}


	///<summary>
	/// Tables Root Node
	///</summary>
	public class SharpQueryNodeTableRoot : AbstractSharpQueryNode
	{
		///<summary>
		/// Addin Path of the node's context menu
		///</summary>
		public override string AddinContextMenu
		{
			get
			{
				return "/SharpQuery/ContextMenu/TablesRoot";
			}
		}

		public SharpQueryNodeTableRoot(AbstractSharpQuerySchemaClass databaseclass)
			: base(databaseclass)
		{
			this.ImageIndex = 3;
			this.SelectedImageIndex = 3;
		}

	}

	///<summary>
	/// Views Root Node
	///</summary>
	public class SharpQueryNodeViewRoot : AbstractSharpQueryNode
	{
		///<summary>
		/// Addin Path of the node's context menu
		///</summary>
		public override string AddinContextMenu
		{
			get
			{
				return "/SharpQuery/ContextMenu/ViewsRoot";
			}
		}

		public SharpQueryNodeViewRoot(AbstractSharpQuerySchemaClass databaseclass)
			: base(databaseclass)
		{
			this.ImageIndex = 4;
			this.SelectedImageIndex = 4;
		}
	}

	///<summary>
	/// Procedure Root Node
	///</summary>
	public class SharpQueryNodeProcedureRoot : AbstractSharpQueryNode
	{
		///<summary>
		/// Addin Path of the node's context menu
		///</summary>
		public override string AddinContextMenu
		{
			get
			{
				return "/SharpQuery/ContextMenu/ProceduresRoot";
			}
		}

		public SharpQueryNodeProcedureRoot(AbstractSharpQuerySchemaClass databaseclass)
			: base(databaseclass)
		{
			this.ImageIndex = 5;
			this.SelectedImageIndex = 5;
		}
	}


}
