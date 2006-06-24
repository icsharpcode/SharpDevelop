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

namespace SharpReportCore{
	/// <summary>
	/// This Class checks for invalid SqlStatements
	/// </summary>
	internal class SqlQueryChecker{
		internal string noValidMessage = "Query should start with 'Select'";
		
		private SqlQueryChecker () {
		}
		
		public static void Check (string queryString) {
			if (!String.IsNullOrEmpty(queryString)) {
				queryString = queryString.ToUpper(CultureInfo.CurrentCulture);
				if (queryString.IndexOf("SELECT") < 0) {
					throw new SharpReportCore.IllegalQueryException();
				}
			}
		}
	}
}
