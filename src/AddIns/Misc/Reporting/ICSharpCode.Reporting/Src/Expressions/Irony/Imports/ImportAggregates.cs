// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)
using System;
using System.Collections.Generic;
using System.Linq;

using ICSharpCode.Reporting.DataSource;
using Irony.Interpreter;
using Irony.Interpreter.Ast;

namespace ICSharpCode.Reporting.Expressions.Irony.Imports
{
	/// <summary>
	/// Description of ImportAggregates.
	/// </summary>
	public static class ImportExtensions
	{
		public static IEnumerable<object> GetDataSource (this ScriptThread thread){
			return (IEnumerable<object>)thread.App.Globals["DataSource"];
		}
		
		
	}
	
	static class ImportAggregateHelper {
		
		public static bool  FieldExist (object current,string fieldName) {
			var property1 = current.ParsePropertyPath(fieldName);
			if (property1 == null) {
				Console.WriteLine(String.Format("Aggregate <Sum> Field '{0}' not found",fieldName));
				return false;
			}
			return true;
		}
		
		public static IEnumerable<IGrouping<object, object>> IsGrouped (IEnumerable<object> listToConvert) {
			var grouped = listToConvert as IEnumerable<IGrouping<object, object>>;
			if (grouped != null) {
				return grouped;
			}
			return null;
		}
	}
	
	
	public static class ImportAggregates
	{
		public static object Sum(ScriptThread thread, AstNode[] childNodes) {
			double sum =  0;
			var fieldName = childNodes[0].Evaluate(thread).ToString();
			
			var dataSource = thread.GetDataSource();
			
			var grouped = ImportAggregateHelper.IsGrouped(dataSource);
			
			if (grouped != null) {
				
				foreach (var element in grouped) {
					var s = element.Sum(o => {
					                    	var v = ReadValueFromObject(fieldName, o);
					                    	return TypeNormalizer.EnsureType<double>(v);
					                    });
					sum = sum + s;
				}
			} else {
				if (ImportAggregateHelper.FieldExist(dataSource.FirstOrDefault(),fieldName)) {
					sum = dataSource.Sum(o => {
					                     	var v = ReadValueFromObject(fieldName, o);
					                     	return TypeNormalizer.EnsureType<double>(v);
					                     });
				}
			}
			return sum;
		}

		
		static object ReadValueFromObject(string fieldName, object currentObject)
		{
			var propertyPath = currentObject.ParsePropertyPath(fieldName);
			var evaluated = propertyPath.Evaluate(currentObject);
			return evaluated;
		}
	}
}
