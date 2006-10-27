/*
 * Created by SharpDevelop.
 * User: Forstmeier Peter
 * Date: 31.01.2005
 * Time: 11:28
 *
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Data;
using System.Globalization;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using SharpQuery.Gui.TreeView;
using SharpQuery.SchemaClass;

namespace ReportGenerator
{
	/// <summary>
	/// Description of PullModelPanel.
	/// </summary>
	public class PullModelPanel : AbstractWizardPanel
	{
		private System.ComponentModel.IContainer components;
		private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.Label label1;
		private SharpQuery.Gui.TreeView.SharpQueryTree sharpQueryTree;
		private System.Windows.Forms.TextBox txtSqlString;
		private System.Windows.Forms.Label label3;
		
		
		public enum enmNodeType {
			
			dataBaseRoot,
//			dataBaseConnctionClose,
//			dataBaseConnection,
//			tablesRoot,
//			viewsRoot,
//			proceduresRoot,
			tableImage,
			viewImage,
			procedureImage,
			columnImage,
			nodeError
		}
		
		private bool firstDrag;
		private string connectionString;

		private CommandType commandType;
		
		private ReportGenerator generator;
		private Properties customizer;
		
		private ISharpQueryNode currentNode;
		
		
		public PullModelPanel()
		{
			InitializeComponent();
			sharpQueryTree = new SharpQueryTree();
			base.EnableFinish = false;
			base.EnableNext = false;
			base.EnableCancel = true;
			this.firstDrag = true;
			commandType = CommandType.Text;
			this.txtSqlString.Enabled = false;
			Localise();
		}
		
		private void Localise() {
			this.label1.Text = ResourceService.GetString("SharpQuery.Label.SharpQuery");
			this.label3.Text = ResourceService.GetString("SharpReport.Wizard.PullModel.CommandText");
			this.toolTip1.SetToolTip(this.txtSqlString,
			                         ResourceService.GetString("SharpReport.Wizard.PullModel.CommandText.ToolTip"));
		}
		
		#region overrides
		
		public override bool ReceiveDialogMessage(DialogMessage message){
			
			if (message == DialogMessage.Next) {
				if (currentNode is SharpQueryNodeProcedure) {
					commandType = CommandType.StoredProcedure;
				} else {
					commandType = CommandType.Text;
				}
				customizer.Set("SqlString", this.txtSqlString.Text.Trim());
				generator.CommandType = commandType;
				generator.SqlString = this.txtSqlString.Text.Trim();
				generator.ConnectionString = connectionString;
				base.EnableFinish = true;
			}
			return true;
		}
		
		public override object CustomizationObject {
			get { return customizer; }
			set {
				if (value == null) {
					throw new ArgumentNullException("value");
				}
				this.customizer = (Properties)value;
				generator = (ReportGenerator)customizer.Get("Generator");
			}
		}
		
		#endregion
		
		#region events
		void TxtSqlStringChanged (object sender,EventArgs e) {
			
			if ((this.txtSqlString.Text.Length == 0) && (this.connectionString.Length > 0)) {
				base.EnableNext = false;
			} else {
				base.EnableNext = true;
			}
			
		}
		void TxtSqlStringDragEnter(object sender, System.Windows.Forms.DragEventArgs e){
			// Handle the Drag effect when the listbox is entered
			if (e.Data.GetDataPresent(DataFormats.Text))
				e.Effect = DragDropEffects.Copy;
			else
				e.Effect = DragDropEffects.None;
		}
		
		void TxtSqlStringDragDrop(object sender, System.Windows.Forms.DragEventArgs e){
			if (firstDrag == true) {
				this.txtSqlString.Text = "";
				firstDrag = false;
				this.EnableNext = true;
			}
			switch ( CheckCurrentNode(currentNode)  ) {
				case enmNodeType.tableImage:
					// we insert Select * from.... otherwise we have to scan
					//the whole string for incorrect columnNames
					if (currentNode is TreeNode) {
						TreeNode treeNode = (TreeNode)currentNode;
						this.txtSqlString.Text = "";
						this.txtSqlString.Text = "SELECT * FROM " + treeNode.Text;
					}
					
					break;
				case enmNodeType.columnImage:
					string colName = MakeProperColumnName((SharpQueryNodeColumn)currentNode);
					if (this.txtSqlString.Text.Length == 0) {
						this.txtSqlString.AppendText ("SELECT ");
						this.txtSqlString.AppendText (colName);
						
					} else if (this.txtSqlString.Text.ToLower(CultureInfo.InvariantCulture).IndexOf("where") > 0){
						this.txtSqlString.AppendText (colName + " = ?");
					}
					else {
						this.txtSqlString.AppendText(", ");
						this.txtSqlString.AppendText(colName);
					}
					
					break;
				case enmNodeType.procedureImage:
					this.txtSqlString.Text = "";
					
					// we can't use the dragobject because it returns an string like 'EXECUTE ProcName'
					this.txtSqlString.Text = currentNode.SchemaClass.Name;
					
					if (this.currentNode.SchemaClass is SharpQueryProcedure ) {
						generator.SharpQueryProcedure = (SharpQueryProcedure) this.currentNode.SchemaClass;
					} else {
						throw new ArgumentException("PullModelPanel:TxtSqlStringDragDrop : currentNode is not a SharpQueryProcedure");
					}
					
					break;
				case enmNodeType.viewImage:
					this.txtSqlString.Text = "";
					this.txtSqlString.Text ="No idea how to handle views";
					break;
				default:
					break;
			}
		}
		
		
		
		void SharpQueryTreeAfterSelect(object sender, System.Windows.Forms.TreeViewEventArgs e){
			
			ISharpQueryNode node = e.Node as ISharpQueryNode;
		
			if (node != null){
				try {
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
				
				catch (Exception){
					throw;
				}
			}
		}
		
		///<summary>
		/// We check if a ColumnName includes an "-" Character,
		/// if so, suround it with []</summary>
		///<param name="SharpQueryNodeColumn">a ColumnNode</param>
		/// <returns>a valid ColumnName</returns>
		/// 
		private static string MakeProperColumnName(SharpQueryNodeColumn node) {
			string colName;
			if (node != null) {
				if (node.SchemaClass.NormalizedName.IndexOf("-") > -1 ){
					colName = node.SchemaClass.NormalizedName.Replace(".",".[") + "]";
				} else {
					colName = node.SchemaClass.NormalizedName;
				}
				
			} else {
				colName = String.Empty;
			}
			return colName;
		}
		
		// check witch type of node we dragg
		private static enmNodeType CheckCurrentNode (ISharpQueryNode node) {
			enmNodeType enm;
			if (node == null) {
				enm = enmNodeType.nodeError;
				return enm;
			}
			
			if (node is SharpQueryNodeColumn) {
				enm = enmNodeType.columnImage;
			} else if (node is SharpQueryNodeTable) {
				enm = enmNodeType.tableImage;
			} else if (node is SharpQueryNodeProcedure) {
				enm = enmNodeType.procedureImage;
			} else if (node is SharpQueryNodeView) {
				enm = enmNodeType.viewImage;
			}
			else {
				enm = enmNodeType.nodeError;
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
			this.sharpQueryTree = new SharpQuery.Gui.TreeView.SharpQueryTree();
			this.label1 = new System.Windows.Forms.Label();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
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
			this.txtSqlString.Location = new System.Drawing.Point(120, 192);
			this.txtSqlString.Multiline = true;
			this.txtSqlString.Name = "txtSqlString";
			this.txtSqlString.Size = new System.Drawing.Size(264, 144);
			this.txtSqlString.TabIndex = 8;
			this.txtSqlString.Text = "";
			this.txtSqlString.DragDrop += new System.Windows.Forms.DragEventHandler(this.TxtSqlStringDragDrop);
			this.txtSqlString.DragEnter += new System.Windows.Forms.DragEventHandler(this.TxtSqlStringDragEnter);
			this.txtSqlString.TextChanged += new System.EventHandler (this.TxtSqlStringChanged);
			// 
			// sharpQueryTree
			// 
			this.sharpQueryTree.Location = new System.Drawing.Point(120, 8);
			this.sharpQueryTree.Name = "sharpQueryTree";
//			this.sharpQueryTree.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
//						((SharpQuery.Gui.TreeView.SharpQueryNodeDatabaseRoot)(new System.Windows.Forms.TreeNode("Datenbankverbindungen", 0, 0)))});
			this.sharpQueryTree.Size = new System.Drawing.Size(264, 176);
			this.sharpQueryTree.TabIndex = 12;
			this.sharpQueryTree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.SharpQueryTreeAfterSelect);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(8, 16);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(104, 16);
			this.label1.TabIndex = 9;
			// 
			// PullModelPanel
			// 
			this.Controls.Add(this.sharpQueryTree);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.txtSqlString);
			this.Name = "PullModelPanel";
			this.Size = new System.Drawing.Size(432, 344);
			this.ResumeLayout(false);
		}
		#endregion
		
		
		
	}
}
