// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

using ICSharpCode.Reporting.DataManager.Listhandling;
using ICSharpCode.Reporting.DataSource;
using Irony.Interpreter;
using Irony.Interpreter.Ast;


namespace ICSharpCode.Reporting.Expressions.Irony.Imports
{
	/// <summary>
	/// Description of ImportAggregates.
	/// </summary>
	public static class ImportAggregates
	{
		public static object Sum(ScriptThread thread, AstNode[] childNodes) {
			double sum =  0;
			var fieldName = childNodes[0].Evaluate(thread).ToString();
			
			var dataSource = (CollectionSource)thread.App.Globals["Current"];
			
			var curpos = dataSource.CurrentPosition;
			
			dataSource.CurrentPosition = 0;
		
			if (FieldExist(dataSource.Current,fieldName)) {
				do {
					var current = dataSource.Current;
					var property = current.ParsePropertyPath(fieldName);
					var val = property.Evaluate(current);
					var nextVal = TypeNormalizer.EnsureType<double>(val);
					sum = sum + nextVal;
				}
				while (dataSource.MoveNext());
			}

			dataSource.CurrentPosition = curpos;
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
