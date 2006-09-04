/*
 * User: dickon
 * Date: 30/08/2006
 * Time: 07:27
 * 
 */

using System;
using System.Drawing;
using System.Windows.Forms;

using System.Data;

namespace SharpDbTools.Viewer
{
	/// <summary>
	/// Description of TableDescribeForm.
	/// </summary>
	public partial class TableDescribeForm
	{
		DataTable tableInfo;
		
		public TableDescribeForm(DataTable tableInfo)
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			this.tableInfo = tableInfo;
			this.tableInfoDataGridView.DataSource = tableInfo;
		}
	}
}
