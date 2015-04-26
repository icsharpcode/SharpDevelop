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
		
		public static string[] DataTypeList (){
			return (string[])dataTypeList.Clone();
		}
		
		
		static readonly string[] dataTypeList = {
			"System.String",
			"System.DateTime",
			"System.TimeSpan",
			"System.Decimal",
			"System.Int32"};
		
		#endregion
		
		#region Formats
      
		public static string[] Formats ()
		{
			return (string[])FormatList.Clone();
		}
        
        private static readonly string[] FormatList = new string[] { "",
            "#,##0",
            "#,##0.00",
            "0",
            "0.00",
            "",
            "dd/MM/yy",
            "dd/MM/yyyy",
            "MM/dd/yyyy",
            "dddd, MMMM dd, yyyy",
            "dddd, MMMM dd, yyyy HH:mm",
            "dddd, MMMM dd, yyyy HH:mm:ss",
            "MM/dd/yyyy HH:mm",
           
            "MM/dd/yyyy HH:mm:ss", "MMMM dd",
            "Ddd, dd MMM yyyy HH\':\'mm\'\"ss \'GMT\'",
            "yyyy-MM-dd HH:mm:ss",
            "yyyy-MM-dd HH:mm:ss GMT",
            "HH:mm",
            "HH:mm:ss",
            "hh:mm:ss",
            "yyyy-MM-dd HH:mm:ss", "html"};
 
       #endregion
	}
}
