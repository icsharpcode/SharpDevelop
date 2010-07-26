using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using ICSharpCode.SharpDevelop.Gui.ClassBrowser;

namespace ICSharpCode.CodeQualityAnalysis.Controls
{
	public class MatrixControl<TValue> : DataGridView
	{
		Matrix<TValue> matrix;
		object [,] cache;
		
		public Matrix<TValue> Matrix 
		{ 
			get
			{
				return matrix;
			}
			
			set 
			{
				Rows.Clear();
				Columns.Clear();
				cache = new object[value.HeaderRows.Count, value.HeaderColumns.Count];
				matrix = value;
			}
		}
		
		public MatrixControl()
		{
			BackgroundColor = Color.White;
			BorderStyle = BorderStyle.None;
			
			AllowUserToAddRows = false;
			AllowUserToDeleteRows = false;
			AllowUserToResizeRows = false;
			AllowUserToResizeColumns = false;
			AllowUserToOrderColumns = false;
			AllowDrop = false;
			
			EnableHeadersVisualStyles = false;
			
			EditMode = DataGridViewEditMode.EditProgrammatically;
			
			AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
			AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.DisplayedCells;
			
			CellBorderStyle = DataGridViewCellBorderStyle.None;
			
			ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.AutoSizeToDisplayedHeaders;
			ColumnHeadersDefaultCellStyle.BackColor = Color.White;
			RowHeadersDefaultCellStyle.BackColor = Color.White;
			RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
			ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
			
			// disable selection
			DefaultCellStyle.SelectionForeColor = Color.Black;
			DefaultCellStyle.SelectionBackColor = Color.White;
			
			ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.Black;
			ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.White;
			
			RowHeadersDefaultCellStyle.SelectionForeColor = Color.Black;
			RowHeadersDefaultCellStyle.SelectionBackColor = Color.White;
			
			SelectionMode = DataGridViewSelectionMode.CellSelect;
			
			
			ReadOnly = true;
			VirtualMode = true;
			EditMode = DataGridViewEditMode.EditProgrammatically;
		}

		public void DrawMatrix()
		{
			DrawHeaders();
		}

		protected void DrawHeaders()
		{
			foreach (var headerColumn in Matrix.HeaderColumns) {
				var column = new DataGridViewTextBoxColumn();
				column.HeaderText = headerColumn.Value.ToString();
				this.Columns.Add(column);
			}
			
			foreach (var headerRow in Matrix.HeaderRows) {
				var row = new DataGridViewRow();
				row.HeaderCell.Value = headerRow.Value.ToString();
				this.Rows.Add(row);
				
				//if (!row.Displayed) // crashes sharpdevelop debugger
				//	break;
			}
		}
		
		protected override void OnCellValueNeeded(DataGridViewCellValueEventArgs e)
		{
			if (cache[e.RowIndex, e.ColumnIndex] != null)
				e.Value = cache[e.RowIndex, e.ColumnIndex];
			if (e.RowIndex < Matrix.HeaderRows.Count && e.ColumnIndex < Matrix.HeaderColumns.Count) {
				e.Value = Matrix.EvaluateCell(Matrix.HeaderRows[e.RowIndex], Matrix.HeaderColumns[e.ColumnIndex]);
			}
		}
	}
}
