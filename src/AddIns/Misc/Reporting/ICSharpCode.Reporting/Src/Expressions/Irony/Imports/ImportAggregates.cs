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
			return sum.ToString();
		}

		
		static object ReadValueFromObject(string fieldName, object currentObject)
		{
			var propertyPath = currentObject.ParsePropertyPath(fieldName);
			var evaluated = propertyPath.Evaluate(currentObject);
			return evaluated;
		}
	}
}
