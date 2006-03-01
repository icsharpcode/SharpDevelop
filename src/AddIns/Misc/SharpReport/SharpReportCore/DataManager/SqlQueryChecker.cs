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
	internal class SqlQueryChecker{
		internal string UPDATE = "UPDATE";
		internal string DELETE = "DELETE";
		internal string INSERT = "INSERT";
		internal string noValidMessage = "is no valid Member of SqlString";
		
		
		public SqlQueryChecker(){
		}
		
		public void Check (string queryString) {
			if (!String.IsNullOrEmpty(queryString)) {
				queryString = queryString.ToUpper(CultureInfo.CurrentCulture);
				
				if (queryString.IndexOf (this.UPDATE) > -1) {
					this.Invalid (this.UPDATE);
				}
				
				if (queryString.IndexOf(this.DELETE) > -1)  {
					this.Invalid (this.DELETE);
				}
				
				if (queryString.IndexOf(this.INSERT) > -1)  {
					this.Invalid (this.INSERT);
				}
			}
		}
		
		private void Invalid (string invalidArgument) {
			
			string str = String.Format(CultureInfo.CurrentCulture,
			                           "{0} {1}",invalidArgument,this.noValidMessage);
			throw new SharpReportCore.SharpReportException(str);
		}
	}
}
