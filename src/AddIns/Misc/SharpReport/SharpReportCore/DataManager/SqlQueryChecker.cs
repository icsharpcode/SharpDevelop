/*
 * Created by SharpDevelop.
 * User: Forstmeier Helmut
 * Date: 20.01.2006
 * Time: 13:44
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Data;
using System.Globalization;
using System.Windows.Forms;

namespace SharpReportCore{
	/// <summary>
	/// This Class checks for invalid SqlStatements
	/// </summary>
	public sealed class SqlQueryChecker{
		
		private SqlQueryChecker () {
		}
		
		public static void Check (CommandType commandType,string queryString) {
			if (commandType == CommandType.Text) {
				if (!String.IsNullOrEmpty(queryString)) {
					queryString = queryString.ToUpper(CultureInfo.CurrentCulture);

					if (!queryString.StartsWith("SELECT")) {
						throw new SharpReportCore.IllegalQueryException();
					}
				}
			}
		}
	}
}
