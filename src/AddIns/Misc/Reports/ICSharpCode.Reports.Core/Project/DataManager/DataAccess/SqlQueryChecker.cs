// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Data;
using System.Globalization;

namespace ICSharpCode.Reports.Core.DataAccess
{
	/// <summary>
	/// This Class checks for invalid SqlStatements
	/// </summary>
	public sealed class SqlQueryChecker{
		
		private SqlQueryChecker () {
		}
		
		public static void Check (CommandType commandType,string commandText) {
			if (commandType == CommandType.Text) {
				if (!String.IsNullOrEmpty(commandText)) {
					commandText = commandText.ToUpper(CultureInfo.CurrentCulture);

					if (!commandText.StartsWith("SELECT",StringComparison.Ordinal)) {
						throw new IllegalQueryException();
					}
				}
			}
		}
	}
}
