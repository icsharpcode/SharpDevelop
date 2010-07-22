// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Peter Forstmeier" email="peter.forstmeier@t-online.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Drawing;
using System.Data;
using System.Globalization;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using SharpQuery.Gui.TreeView;
using SharpQuery.SchemaClass;


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
		private SharpQueryTree sharpQueryTree;
		private System.Windows.Forms.TextBox txtSqlString;
		private System.Windows.Forms.Label label3;
		private bool firstDrag;
		private string connectionString;
		private CommandType commandType;
		private ReportStructure reportStructure;
		private Properties customizer;		
		private ISharpQueryNode currentNode;
		
		public enum NodeType {
			
//			DataBaseRoot,
//			dataBaseConnctionClose,
//			dataBaseConnection,
//			tablesRoot,
//			viewsRoot,
//			proceduresRoot,
			TableImage,
			ViewImage,
			ProcedureImage,
			ColumnImage,
			NodeError
		}
		
		
		public PullModelPanel()
		{
			InitializeComponent();
			sharpQueryTree = new SharpQueryTree();
			sharpQueryTree.Dock = DockStyle.Fill;
			this.sharpQueryTree.AfterSelect += SharpQueryTreeAfterSelect;
			this.label2.Controls.Add(this.sharpQueryTree);

			base.EnableFinish = false;
			base.EnableNext = false;
			base.EnableCancel = true;
			this.firstDrag = true;
			base.IsLastPanel = false;
			commandType = CommandType.Text;
			this.txtSqlString.Enabled = false;
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
				if (currentNode is SharpQueryNodeProcedure) {
					commandType = CommandType.StoredProcedure;
				} else {
					commandType = CommandType.Text;
				}
				customizer.Set("SqlString", this.txtSqlString.Text.Trim());
				reportStructure.CommandType = commandType;
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
			if (e.Data.GetDataPresent(DataFormats.Text))
				e.Effect = DragDropEffects.Copy;
			else
				e.Effect = DragDropEffects.None;
		}
		
		
		private void TxtSqlStringDragDrop(object sender, System.Windows.Forms.DragEventArgs e){
			if (firstDrag == true) {
				this.txtSqlString.Clear();
				firstDrag = false;
			}
			
			switch ( CheckCurrentNode(currentNode)) {
				case NodeType.TableImage:
					// we insert Select * from.... otherwise we have to scan
					//the whole string for incorrect columnNames
					this.txtSqlString.Clear();
//					AbstractSharpQuerySchemaClass tbl = (AbstractSharpQuerySchemaClass)this.currentNode.SchemaClass;
					this.txtSqlString.Text = "SELECT * FROM " + InferColumnName(((AbstractSharpQuerySchemaClass)currentNode.SchemaClass).Name);
					
					break;
					
				case NodeType.ColumnImage:
					string colName = InferColumnName(((AbstractSharpQuerySchemaClass)currentNode.SchemaClass).Name);
					if (this.txtSqlString.Text.Length == 0) {
						this.txtSqlString.AppendText ("SELECT ");
						this.txtSqlString.AppendText (colName);
						
					} else if (this.txtSqlString.Text.ToLower(CultureInfo.InvariantCulture).IndexOf("where",StringComparison.OrdinalIgnoreCase) > 0){
						this.txtSqlString.AppendText (colName + " = ?");
					}
					else {
						this.txtSqlString.AppendText(", ");
						this.txtSqlString.AppendText(colName);
					}
					break;
					
				case NodeType.ProcedureImage:
					this.txtSqlString.Clear();
					
					// we can't use the dragobject because it returns an string like 'EXECUTE ProcName'
					this.txtSqlString.Text = currentNode.SchemaClass.Name;
					
					if (this.currentNode.SchemaClass is SharpQueryProcedure ) {
						reportStructure.SharpQueryProcedure = (SharpQueryProcedure) this.currentNode.SchemaClass;
					} else {
						throw new ArgumentException("PullModelPanel:TxtSqlStringDragDrop : currentNode is not a SharpQueryProcedure");
					}
					break;
					
				case NodeType.ViewImage:
					this.txtSqlString.Text = String.Empty;
					this.txtSqlString.Text ="No idea how to handle views";
					break;
				default:
					break;
			}
//			base.EnableNext = true;
		}
		
		
		private void SharpQueryTreeAfterSelect(object sender, System.Windows.Forms.TreeViewEventArgs e)
		{
			ISharpQueryNode node = e.Node as ISharpQueryNode;
			if (node != null){
				this.currentNode = node;
				
				// Set the connectionstring here and toogle EnableNext
				if (node.Connection != null) {
					
					if (node.Connection.ConnectionString.Length > 0) {
						this.connectionString = node.Connection.ConnectionString;
						this.txtSqlString.Enabled = true;
						
						if (this.firstDrag) {
							this.txtSqlString.Text = String.Empty;
						}
						
					} else {
						this.EnableNext = false;
					}
				}
			}
		}

		
		///<summary>
		/// We check if a ColumnName includes an "-" Character,
		/// if so, suround it with []</summary>
		///<param name="SharpQueryNodeColumn">a ColumnNode</param>
		/// <returns>a valid ColumnName</returns>
		/// 
		private static string InferColumnName(string node) {
			string colName;
			if (node != null) {
				if (node.IndexOf("-",StringComparison.Ordinal) > -1 ){
//					colName = node.Replace(".",".[") + "]";
					colName = "[" + node + "]";
				} else {
					colName = node;
				}
			} else {
				colName = String.Empty;
			}
			return colName;
		}
		
		
		
		// check witch type of node we dragg
		private static NodeType CheckCurrentNode (ISharpQueryNode node) {
			NodeType enm;
			if (node is SharpQueryNodeColumn) {
				enm = NodeType.ColumnImage;
			} else if (node is SharpQueryNodeTable) {
				enm = NodeType.TableImage;
			} else if (node is SharpQueryNodeProcedure) {
				enm = NodeType.ProcedureImage;
			} else if (node is SharpQueryNodeView) {
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

