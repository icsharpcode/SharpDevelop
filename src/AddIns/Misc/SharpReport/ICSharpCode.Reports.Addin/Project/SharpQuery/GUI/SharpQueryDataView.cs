// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Luc Morin" email=""/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Data;
using System.Windows.Forms;

using ICSharpCode.SharpDevelop.Gui;
using SharpQuery.Collections;
using SharpQuery.SchemaClass;

namespace SharpQuery.Gui.DataView
{
	/// <summary>
	/// Description of the view content
	/// </summary>
	public class SharpQueryDataView : AbstractViewContent
	{
		DataGrid pDataGrid;
		ISchemaClass pSchema;

		#region AbstractViewContent requirements
		/// <summary>
		/// The <see cref="System.Windows.Forms.Control"/> representing the view
		/// </summary>
		/// 
		public override Control Control
		{
			get
			{
				return pDataGrid;
			}
		}

		/// <summary>
		/// Creates a new MyView object
		/// </summary>
		public SharpQueryDataView(ISchemaClass entity, int lines, SharpQuerySchemaClassCollection parameters)
		{
			if (entity == null)
			{
				throw new ArgumentNullException("entity");
			}

			TitleName = SharpQuery.SchemaClass.AbstractSharpQuerySchemaClass.RemoveBracket(entity.NormalizedName);

			this.pDataGrid = new DataGrid();
			this.pDataGrid.CaptionVisible = true;
			this.pDataGrid.DataMember = "";
			this.pDataGrid.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pDataGrid.HeaderForeColor = System.Drawing.SystemColors.ControlText;
			this.pDataGrid.Location = new System.Drawing.Point(0, 0);
			this.pDataGrid.Name = "dataGrid";
			this.pDataGrid.Size = new System.Drawing.Size(292, 266);
			this.pDataGrid.TabIndex = 0;
			this.Schema = entity;
			this.Datatable = this.Schema.Execute(lines, parameters);
			//			if ( this.Datatable == null )
			//			{
			//				WorkbenchSingleton.Workbench.ViewContentCollection.Remove( this );
			//			}
		}

		/// <summary>
		/// Loads a new file into MyView
		/// </summary>
		public override void Load(ICSharpCode.SharpDevelop.OpenedFile file, System.IO.Stream stream)
		{
			throw new System.NotImplementedException();
			//base.Load(file, stream);
		}
//		public override void Load(string fileName)
//		{
//			// TODO
//			throw new System.NotImplementedException();
//		}

		/// <summary>
		/// Refreshes the view
		/// </summary>
		public override void RedrawContent()
		{
			// TODO: Refresh the whole view control here, renew all resource strings whatever
			//       Note that you do not need to recreate the control.
		}

		/// <summary>
		/// Cleans up all used resources
		/// </summary>
		public override void Dispose()
		{
			// TODO: Clean up resources in this method
			Control.Dispose();
		}
		#endregion

		public ISchemaClass Schema
		{
			get
			{
				return this.pSchema;
			}
			set
			{
				this.pSchema = value;
			}
		}

		///<summary>
		/// <see cref="System.Data.DataTable">DataTable</see> to display in the
		/// <see cref="System.Windows.Forms.DataGrid">DataGrid</see>
		/// </summary>
		public object Datatable
		{
			get
			{
				return this.pDataGrid.DataSource;
			}

			set
			{
				removeChangedHandler();

				this.pDataGrid.DataSource = value;
				this.pDataGrid.DataMember = null;
				this.pDataGrid.AllowNavigation = true;

				DataSet dataset = this.pDataGrid.DataSource as DataSet;

				if (dataset != null)
				{
					if (dataset.Tables.Count == 1)
					{
						this.pDataGrid.DataMember = dataset.Tables[0].TableName;
						this.pDataGrid.AllowNavigation = false;
					}
				}

				addChangedHandler();
			}
		}

		private void removeChangedHandler()
		{
			if (this.pDataGrid.DataSource != null)
			{
				DataSet dataset = this.pDataGrid.DataSource as DataSet;

				if (dataset != null)
				{
					foreach (DataTable table in dataset.Tables)
					{
						table.RowChanged -= new DataRowChangeEventHandler(this.UpdateTable);
						table.RowDeleted -= new DataRowChangeEventHandler(this.DeleteRow);
					}
				}
				else
				{
					(this.pDataGrid.DataSource as DataTable).RowChanged -= new DataRowChangeEventHandler(this.UpdateTable);
					(this.pDataGrid.DataSource as DataTable).RowDeleted -= new DataRowChangeEventHandler(this.DeleteRow);
				}
			}
		}

		private void addChangedHandler()
		{
			if (this.pDataGrid.DataSource != null)
			{
				DataSet dataset = this.pDataGrid.DataSource as DataSet;

				if (dataset != null)
				{
					foreach (DataTable table in dataset.Tables)
					{
						table.RowChanged += new DataRowChangeEventHandler(this.UpdateTable);
						table.RowDeleted += new DataRowChangeEventHandler(this.DeleteRow);
					}
				}
				else
				{
					(this.pDataGrid.DataSource as DataTable).RowChanged += new DataRowChangeEventHandler(this.UpdateTable);
					(this.pDataGrid.DataSource as DataTable).RowDeleted += new DataRowChangeEventHandler(this.DeleteRow);
				}
			}
		}

		public void UpdateTable(object sender, DataRowChangeEventArgs e)
		{
			if (Datatable != null)
			{
				switch (e.Action)
				{
					case DataRowAction.Add:
						this.Schema.Connection.InsertRow(this.Schema, e.Row);
						break;
					case DataRowAction.Change:
						this.Schema.Connection.UpDateRow(this.Schema, e.Row);
						break;
				}
			}
		}

		public void DeleteRow(object sender, DataRowChangeEventArgs e)
		{
			switch (e.Action)
			{
				case DataRowAction.Delete:
					this.Schema.Connection.DeleteRow(this.Schema, e.Row);
					break;
			}

		}

		public override void Save(ICSharpCode.SharpDevelop.OpenedFile file, System.IO.Stream stream)
		{
			throw new System.NotImplementedException();
			//base.Save(file, stream);
		}
		/*
		public override void Save(string fileName)
		{
			DataSet dataset = this.pDataGrid.DataSource as DataSet;
			if (dataset == null)
			{
				DataTable dataTable = this.pDataGrid.DataSource as DataTable;
				if (dataTable != null)
				{
					dataset = new DataSet();
					dataset.Tables.Add(dataTable);
				}
			}

			dataset.WriteXml(fileName);
		}
		*/
	}

}
