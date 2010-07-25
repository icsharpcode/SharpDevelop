using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace ICSharpCode.CodeQualityAnalysis.Controls
{
	public class MatrixControl<TValue> : DataGridView
	{
		public Matrix<TValue> Matrix { get; set; }
		
		public MatrixControl()
		{
			AllowUserToAddRows = false;
			AllowUserToDeleteRows = false;
			AllowUserToResizeRows = false;
			EnableHeadersVisualStyles = false;
		}

		public void DrawMatrix()
		{
			DrawHeaders();

			for (int i = 0; i < Matrix.HeaderRows.Count; i++) {
				
				for (int j = 0; j < Matrix.HeaderColumns.Count; j++) {
					var val = Matrix.EvaluateCell(Matrix.HeaderRows[i], Matrix.HeaderColumns[j]);
					
					this[i, j].Value = val.ToString();
				}
			}
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
			}
		}
	}
}
