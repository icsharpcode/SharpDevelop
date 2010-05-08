/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Peter
 * Datum: 18.01.2009
 * Zeit: 11:08
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */
using System;
using System.Collections;


/// <summary>
/// Use this code to have the same functionnames as SLQReportingServer
/// See <see cref="http://www.fyireporting.com/"></see>
/// </summary>
namespace ICSharpCode.Reports.Core
{
	/// <summary>
	/// Description of GlobalLists.
	/// </summary>
	public class GlobalLists
	{
		
		public enum FunctionTypes {
			Globals,
			User,
			Fields,
			Parameters
		}
		
		#region Global functions
		
		public static string[] GlobalFunctions ()
		{
			return (string[])GlobalList.Clone();
		}
		
		private static readonly string[] GlobalList = new string[]{
			"=Globals!PageNumber",
			"=Globals!TotalPages",
			"=Globals!ExecutionTime",
			"=Globals!ReportFolder",
			"=Globals!ReportName"};
		
		#endregion
		
		#region User Functions
		
		public static string[] UserFunctions ()
		{
			return (string[])UserList.Clone();
		}
		
		
		private static readonly string[] UserList = new string[] {
																	  "=User!UserID",
																	  "=User!Language"};
		
		#endregion
		
		#region Aggregates
		
		public static string[] AggregateFunctions ()
		{
			return (string[])AggrFunctionList.Clone();
		}
		
		private static  readonly string[] AggrFunctionList = new string[] {"Sum(number [, scope])",
																		"Aggregate(number [, scope])",
																		"Avg(number [, scope])",
																		"Min(expr [, scope])",
																		"Max(expr [, scope])",
																		"First(expr [, scope])",
																		"Last(expr [, scope])",
																		"Next(expr [, scope])",
																		"Previous(expr [, scope])",
																		"Level([scope])",
																		"Count(expr [, scope])",
																		"Countrows(expr [, scope])",
																		"Countdistinct(expr [, scope])",
																		"RowNumber()",
																		"Runningvalue(expr, sum [, scope])",
																		"Runningvalue(expr, avg [, scope])",
																		"Runningvalue(expr, count [, scope])",
																		"Runningvalue(expr, max [, scope])",
																		"Runningvalue(expr, min [, scope])",
																		"Runningvalue(expr, stdev [, scope])",
																		"Runningvalue(expr, stdevp [, scope])",
																		"Runningvalue(expr, var [, scope])",
																		"Runningvalue(expr, varp [, scope])",
																		"Stdev(expr [, scope])",
																		"Stdevp(expr [, scope])",
																		"Var(expr [, scope])",
																		"Varp(expr [, scope])"};
		
		#endregion
		
		/// </summary>
		/// 
		#region Zoom
		
		public static string[] ZoomValues ()
		{
			return (string[])ZoomList.Clone();
		}
		
		private static readonly string[] ZoomList = new string[] {
									"Actual Size",
//									"Fit Page",
//									"Fit Width",
									"800%",
									"400%",
									"200%",
									"150%",
									"125%",
									"100%",
									"75%",
									"50%",
									"25%"};
		
		
		#endregion
		
		 
		#region Formats
      
		public static string[] Formats ()
		{
			return (string[])FormatList.Clone();
		}
        
        private static readonly string[] FormatList = new string[] { "",
            "#,##0", "#,##0.00", "0", "0.00", "", "MM/dd/yyyy",
            "dddd, MMMM dd, yyyy", "dddd, MMMM dd, yyyy HH:mm",
            "dddd, MMMM dd, yyyy HH:mm:ss", "MM/dd/yyyy HH:mm",
            "MM/dd/yyyy HH:mm:ss", "MMMM dd",
            "Ddd, dd MMM yyyy HH\':\'mm\'\"ss \'GMT\'",
            "yyyy-MM-dd HH:mm:ss",
            "yyyy-MM-dd HH:mm:ss GMT",
            "HH:mm", "HH:mm:ss",
            "yyyy-MM-dd HH:mm:ss", "html"};
 
       #endregion
       
       
        #region Gradienst
        public static string[] Gradients ()
		{
			return (string[])GradientList.Clone();
		}
        
        
        private static readonly string[] GradientList = new string[] {
        "None", "LeftRight", "TopBottom", "Center", "DiagonalLeft",
        "DiagonalRight", "HorizontalCenter", "VerticalCenter"};
        
        #endregion
        
	}
}
