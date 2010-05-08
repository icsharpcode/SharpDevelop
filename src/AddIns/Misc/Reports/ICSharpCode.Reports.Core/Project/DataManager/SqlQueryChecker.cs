// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Peter Forstmeier" email="peter.forstmeier@t-online.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Data;
using System.Globalization;

namespace ICSharpCode.Reports.Core
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

					if (!commandText.StartsWith("SELECT")) {
						throw new IllegalQueryException();
					}
				}
			}
		}
	}
}
