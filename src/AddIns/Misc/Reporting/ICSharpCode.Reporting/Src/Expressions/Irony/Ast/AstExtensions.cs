// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using ICSharpCode.Reporting.DataManager.Listhandling;
using ICSharpCode.Reporting.Interfaces;
using ICSharpCode.Reporting.Interfaces.Export;
using ICSharpCode.Reporting.Items;
using ICSharpCode.Reporting.PageBuilder.ExportColumns;
using Irony.Interpreter;

namespace ICSharpCode.Reporting.Expressions.Irony.Ast
{
	/// <summary>
	/// Description of AstExtensions.
	/// </summary>
	public static class AstExtensions
	{
		#region Globals
		public static void AddPageInfo (this ReportingExpressionEvaluator app,IPageInfo pageInfo) {
			if (pageInfo == null)
				throw new ArgumentNullException("pageInfo");
			
			if (!app.Globals.ContainsKey("PageInfo")) {
				app.Globals.Add("PageInfo",pageInfo);
			} else {
				app.Globals["PageInfo"] = pageInfo;
			}
		}
		
		
		public static IPageInfo GetPageInfo (this ScriptThread thread){
			var pi = (IPageInfo)thread.App.Globals["PageInfo"];
			return pi;
		}
		#endregion
		
		
		#region Parameters
		
		public static ParameterCollection GetParametersCollection (this ScriptThread thread){
			var rs =  (ReportSettings)thread.App.Globals["ReportSettings"];
			return rs.ParameterCollection;
		}
		
		
		public static void  AddReportSettings (this ReportingExpressionEvaluator app,IReportSettings reportSettings) {
			if (reportSettings == null)
				throw new ArgumentNullException("reportSettings");
			app.Globals.Add("ReportSettings",reportSettings);
		}
		
		#endregion
		
		#region current Container
		
		public static void AddCurrentContainer (this ReportingExpressionEvaluator app,ExportContainer container){
			if (container == null)
				throw new ArgumentNullException("container");
			if (!app.Globals.ContainsKey("CurrentContainer")) {
				app.Globals.Add("CurrentContainer",container);
			} else {
				app.Globals["CurrentContainer"] = container;
			}
		}
		
		public static ExportContainer GetCurrentContainer (this ScriptThread thread){
			return (ExportContainer)thread.App.Globals["CurrentContainer"];
		}
		
		#endregion
		
		#region DataSource
		
		public static void SetCurrentDataSource (this ReportingExpressionEvaluator app,IEnumerable<object> dataSource){
			if (dataSource == null)
				throw new ArgumentNullException("dataSource");
			if (!app.Globals.ContainsKey("DataSource")) {
			    	app.Globals.Add("DataSource",dataSource);
			    } else {
			    	app.Globals["DataSource"] = dataSource;
			    }
		}
	
		#endregion
	}
}
