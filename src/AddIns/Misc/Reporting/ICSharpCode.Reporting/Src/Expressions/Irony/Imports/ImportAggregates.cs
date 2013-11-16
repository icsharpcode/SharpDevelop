// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)
using System;
using System.Linq;
using ICSharpCode.Reporting.DataManager.Listhandling;
using ICSharpCode.Reporting.DataSource;
using ICSharpCode.Reporting.Interfaces.Data;
using Irony.Interpreter;
using Irony.Interpreter.Ast;

namespace ICSharpCode.Reporting.Expressions.Irony.Imports
{
	/// <summary>
	/// Description of ImportAggregates.
	/// </summary>
	public static class ImportExtensions
	{
		public static IDataSource GetDataSource (this ScriptThread thread){
			return (IDataSource)thread.App.Globals["DataSource"];
		}
	}
	
	public static class ImportAggregates
	{
		public static object Sum(ScriptThread thread, AstNode[] childNodes) {
			double sum =  0;
			var fieldName = childNodes[0].Evaluate(thread).ToString();
			
			var dataSource = thread.GetDataSource();

			if (FieldExist(dataSource.CurrentList[0],fieldName)) {
				
				sum = dataSource.CurrentList.Sum(o => {
				                                 	var propertyPath = o.ParsePropertyPath(fieldName);
				                                 	var val = propertyPath.Evaluate(o);
				                                 	return TypeNormalizer.EnsureType<double>(val);
				                                 });
			}
			return sum;
		}
	
		
		static bool  FieldExist (object current,string fieldName) {
			var property1 = current.ParsePropertyPath(fieldName);
			if (property1 == null) {
				Console.WriteLine(String.Format("Aggregate <Sum> Field '{0}' not found",fieldName));
				return false;
			}
			return true;
		}
	}
}
