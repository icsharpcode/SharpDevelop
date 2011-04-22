// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Data;
using System.Globalization;
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Forms.Integration;

using ICSharpCode.Core;
using ICSharpCode.Data.Core.Interfaces;
using ICSharpCode.Data.Core.UI.UserControls;
using ICSharpCode.SharpDevelop;


namespace ICSharpCode.Reports.Addin.ReportWizard
{
	/// <summary>
	/// Description of PullModelPanel.
	/// </summary>
	public class PullModelPanel : AbstractWizardPanel
	{
	private System.ComponentModel.IContainer components;
		private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox txtSqlString;
		private System.Windows.Forms.Label label3;
		private bool firstDrag;
		private string connectionString;
		private ReportStructure reportStructure;
		private Properties customizer;		
		private IDatabaseObjectBase currentNode;
        private ElementHost databasesTreeHost;
        private DatabasesTreeView databasesTree;
		
		private enum NodeType
		{
			TableImage,
			ViewImage,
			ProcedureImage,
			ColumnImage,
			NodeError
		}
		
		
		public PullModelPanel()
		{
			InitializeComponent();
          
			base.EnableFinish = false;
			base.EnableNext = false;
			base.EnableCancel = true;
			this.firstDrag = true;
			base.IsLastPanel = false;
			this.txtSqlString.Enabled = false;

            this.databasesTreeHost = new ElementHost() { Dock = DockStyle.Fill };
            this.databasesTree = new DatabasesTreeView();
            this.databasesTree.SelectedItemChanged += new System.Windows.RoutedPropertyChangedEventHandler<object>(databasesTree_SelectedItemChanged);
            this.databasesTreeHost.Child = this.databasesTree;
            this.label2.Controls.Add(databasesTreeHost);

			Localize();
		}	
	
		private void Localize() {
			this.label1.Text = ResourceService.GetString("SharpQuery.Label.SharpQuery");
			this.label3.Text = ResourceService.GetString("SharpReport.Wizard.PullModel.CommandText");
			this.toolTip1.SetToolTip(this.txtSqlString,
			                         ResourceService.GetString("SharpReport.Wizard.PullModel.CommandText.ToolTip"));
		}
		

		#region overrides
		
		public override bool ReceiveDialogMessage(DialogMessage message)
		{
			if (customizer == null) {
				customizer = (Properties)base.CustomizationObject;
				reportStructure = (ReportStructure)customizer.Get("Generator");
			}
			
			if (message == DialogMessage.Next) {
				customizer.Set("SqlString", this.txtSqlString.Text.Trim());
				reportStructure.SqlString = this.txtSqlString.Text.Trim();
				reportStructure.ConnectionString = connectionString;
				base.EnableFinish = true;
			}
			return true;
		}
		
		#endregion
		
		#region events
		
		private  void TxtSqlStringChanged (object sender,EventArgs e) {
			
			if ((this.txtSqlString.Text.Length == 0) && (this.connectionString.Length > 0)) {
				base.EnableNext = false;
			} else {
				base.EnableNext = true;
			}
			
		}
		
		
		private void TxtSqlStringDragEnter(object sender, System.Windows.Forms.DragEventArgs e){
			// Handle the Drag effect when the listbox is entered
            if (e.Data.GetFormats().Length > 0)
            {
                string draggedFormat = e.Data.GetFormats()[0];
                
                String str = String.Format("drag {0}",draggedFormat);
                System.Diagnostics.Trace.WriteLine(str);

                Type draggedType = null;

                // I'm doing this ugly thing because we are checking if the IDatabaseObjectBase is implemented,
                // obviously Microsoft hasn't really considered using interfaces or base classes for drag and drop
                AppDomain.CurrentDomain.GetAssemblies().ForEach(assembly =>
                {
                    if (draggedType == null && assembly.GetName().Name == "ICSharpCode.Data.Core")
                        draggedType = assembly.GetType(draggedFormat);
                });

                if (draggedType != null && draggedType.GetInterface("IDatabaseObjectBase") != null)
                {
                    e.Effect = DragDropEffects.Copy;
                    return;
                }
            }
			
            e.Effect = DragDropEffects.None;
		}
		
		
		private void TxtSqlStringDragDrop(object sender, System.Windows.Forms.DragEventArgs e){
			if (firstDrag == true) {
				this.txtSqlString.Clear();
				firstDrag = false;
			}

			// Drag and drop isn't working via e.Data.GetData, so I'm using reflection here - took me a lot of time to figure out how this works...
			// Still don't know why they implemented dnd so buggy and uncomfortable...
			string draggedFormat = e.Data.GetFormats()[0];
			IDatabaseObjectBase draggedObject = null;

			if (e.Data.GetDataPresent(draggedFormat))
			{
				object tempDraggedObject = null;
				FieldInfo info;
				info = e.Data.GetType().GetField("innerData", BindingFlags.NonPublic | BindingFlags.Instance);
				tempDraggedObject = info.GetValue(e.Data);
				info = tempDraggedObject.GetType().GetField("innerData", BindingFlags.NonPublic | BindingFlags.Instance);
				System.Windows.DataObject dataObject = info.GetValue(tempDraggedObject) as System.Windows.DataObject;
				draggedObject = dataObject.GetData(draggedFormat) as IDatabaseObjectBase;
			}

			switch (CheckCurrentNode(draggedObject))
			{
				case NodeType.TableImage:
					// we insert Select * from.... otherwise we have to scan
					//the whole string for incorrect columnNames
					this.txtSqlString.Clear();
					ITable table = draggedObject as ITable;
					this.txtSqlString.Text = "SELECT * FROM " + table.Name;
					reportStructure.CommandType = CommandType.Text;
					reportStructure.IDatabaseObjectBase = table;
					break;

				case NodeType.ColumnImage:
					IColumn column = draggedObject as IColumn;
					string colName = column.Name;
					
					if (this.txtSqlString.Text.Length == 0)
					{
						this.txtSqlString.AppendText("SELECT ");
						this.txtSqlString.AppendText(colName);
					}
					
					else if (this.txtSqlString.Text.ToUpper(CultureInfo.InvariantCulture).IndexOf("where", StringComparison.OrdinalIgnoreCase) > 0)
					{
						this.txtSqlString.AppendText(colName + " = ?");
					}
					else
					{
						this.txtSqlString.AppendText(", ");
						this.txtSqlString.AppendText(colName);
					}
					reportStructure.CommandType = CommandType.Text;
					if (reportStructure.IDatabaseObjectBase == null)
					{
						reportStructure.IDatabaseObjectBase = column;
					}
					break;

				case NodeType.ProcedureImage:
					this.txtSqlString.Clear();

					// we can't use the dragobject because it returns an string like 'EXECUTE ProcName'
					IProcedure procedure = draggedObject as IProcedure;
					this.txtSqlString.Text = "[" + procedure.Name + "]";
					reportStructure.CommandType = CommandType.StoredProcedure;
					reportStructure.IDatabaseObjectBase = procedure;
					break;
				default:
					break;
			}
			base.EnableNext = true;
		}


        private void databasesTree_SelectedItemChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<object> e)
        {
        	if (e.NewValue is IDatabaseObjectBase)
            {
                IDatabase parentDatabase = e.NewValue as IDatabase;

                if (parentDatabase == null)
                {
                    IDatabaseObjectBase currentDatabaseObject = e.NewValue as IDatabaseObjectBase;

                    while (parentDatabase == null)
                    {
                        if (currentDatabaseObject.Parent == null)
                            break;
                        else if (currentDatabaseObject.Parent is IDatabase)
                        {
                            parentDatabase = currentDatabaseObject.Parent as IDatabase;
                            break;
                        }
                        else
                            currentDatabaseObject = currentDatabaseObject.Parent;                        
                    }
                }


                if (parentDatabase != null)
                    this.currentNode = parentDatabase;

                if (this.currentNode is IDatabase)
                {
                	if (parentDatabase != null)
                	{
                		this.connectionString = "Provider=" + parentDatabase.Datasource.DatabaseDriver.ODBCProviderName + ";" + parentDatabase.ConnectionString;
                		this.txtSqlString.Enabled = true;

                		if (this.firstDrag)
                			this.txtSqlString.Text = string.Empty;
                		
                		firstDrag = false;
                	}
                }
                else
                {
                    this.EnableNext = false;
                }
            }
        }
		
		// check witch type of node we dragg
		private static NodeType CheckCurrentNode (IDatabaseObjectBase node) {
			NodeType enm;
			if (node is IColumn) {
				enm = NodeType.ColumnImage;
			} else if (node is ITable) {
				enm = NodeType.TableImage;
			} else if (node is IProcedure) {
				enm = NodeType.ProcedureImage;
			} else if (node is IView) {
				enm = NodeType.ViewImage;
			}
			else {
				enm = NodeType.NodeError;
			}
			return enm;
		}
		
		#endregion
		
		
		#region Windows Forms Designer generated code
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method connectiontents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			this.label3 = new System.Windows.Forms.Label();
			this.txtSqlString = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(8, 200);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(104, 24);
			this.label3.TabIndex = 11;
			// 
			// txtSqlString
			// 
			this.txtSqlString.AllowDrop = true;
			this.txtSqlString.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.txtSqlString.Location = new System.Drawing.Point(120, 192);
			this.txtSqlString.Multiline = true;
			this.txtSqlString.Name = "txtSqlString";
			this.txtSqlString.Size = new System.Drawing.Size(264, 144);
			this.txtSqlString.TabIndex = 8;
			this.txtSqlString.TextChanged += new System.EventHandler(this.TxtSqlStringChanged);
			this.txtSqlString.DragDrop += new System.Windows.Forms.DragEventHandler(this.TxtSqlStringDragDrop);
			this.txtSqlString.DragEnter += new System.Windows.Forms.DragEventHandler(this.TxtSqlStringDragEnter);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(8, 16);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(104, 16);
			this.label1.TabIndex = 9;
			// 
			// textBox1
			// 
			this.textBox1.Location = new System.Drawing.Point(0, 0);
			this.textBox1.Name = "textBox1";
			this.textBox1.Size = new System.Drawing.Size(100, 20);
			this.textBox1.TabIndex = 0;
			// 
			// label2
			// 
			this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.label2.Location = new System.Drawing.Point(116, 16);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(267, 159);
			this.label2.TabIndex = 12;
			this.label2.Text = "label2";
			// 
			// PullModelPanel
			// 
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.txtSqlString);
			this.Name = "PullModelPanel";
			this.Size = new System.Drawing.Size(432, 344);
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox textBox1;
		#endregion
	}
}
