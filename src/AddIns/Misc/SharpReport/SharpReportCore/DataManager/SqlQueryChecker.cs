/*
 * Created by SharpDevelop.
 * User: Forstmeier Helmut
 * Date: 20.01.2006
 * Time: 13:44
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Globalization;
using System.Windows.Forms;

namespace SharpReportCore
{
	/// <summary>
	/// This Class checks for invalid SqlStatements
	/// </summary>
	internal class SqlQueryCkecker{
		internal string UPDATE = "UPDATE";
		internal string DELETE = "DELETE";
		internal string INSERT = "INSERT";
		internal string noValidMessage = "is no valid Member of SqlString";
		private string queryString;
		
		
		public SqlQueryCkecker(){
		}
		
		public void Check (string queryString) {
			if (queryString != "") {
				this.queryString = queryString.ToUpper(CultureInfo.CurrentCulture);
				
				if (this.queryString.IndexOf (this.UPDATE) > -1) {
					string str = String.Format("{0} is no valid Member of SqlString",this.UPDATE);
					this.Invalid (this.UPDATE);
				}
				
				if (this.queryString.IndexOf(this.DELETE) > -1)  {
					this.Invalid (this.DELETE);
					string str = String.Format("{0} is no valid Member of SqlString",this.DELETE);
				}
				
				if (this.queryString.IndexOf(this.INSERT) > -1)  {
					this.Invalid (this.INSERT);
					string str = String.Format("{0} is no valid Member of SqlString",this.DELETE);
				}
			}
		}
		
		private void Invalid (string invalidArgument) {
			string str = String.Format("{0} {1}",invalidArgument,this.noValidMessage);
			throw new SharpReportCore.SharpReportException(str);
		}
	}
}
