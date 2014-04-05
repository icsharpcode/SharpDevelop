/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 04.04.2014
 * Time: 20:51
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace ICSharpCode.Reporting.Addin.Globals
{
	/// <summary>
	/// Description of GlobalLists.
	/// </summary>
	static class GlobalLists
	{
		#region DataTypes
		
		public static string[] DataTypeList ()
		{
			return (string[])dataTypeList.Clone();
		}
		
		
		static readonly string[] dataTypeList = {
			"System.String",
			"System.DateTime",
			"System.TimeSpan",
			"System.Decimal",
			"System.Int"};
		
		#endregion
	}
}
