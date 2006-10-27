/*
 * User: dickon
 * Date: 04/08/2006
 * Time: 22:21
 */

using System;

namespace SharpDbTools.Forms
{
	/// <summary>
	/// Description of GetConnectionLogicalNameDialog.
	/// </summary>
	public partial class GetConnectionLogicalNameDialog
	{
		public GetConnectionLogicalNameDialog()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
		}
		
		public string LogicalConnectionName {
			get {
				return this.connectionName.Text;
			}				
		}
		
		void ConnectionNameOKButtonClick(object sender, System.EventArgs e)
		{
			this.Close();
		}
		
		void ConnectionNameCancelButtonClick(object sender, System.EventArgs e)
		{
			this.Close();
		}
	}
}
