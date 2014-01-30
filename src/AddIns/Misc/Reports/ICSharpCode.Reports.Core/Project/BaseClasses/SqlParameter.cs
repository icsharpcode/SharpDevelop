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
using System.Data;

namespace ICSharpCode.Reports.Core
{

	/// <summary>
	/// This Class definies a Reportparameter
	/// </summary>
	/// <remarks>
	/// 	created by - Forstmeier Peter
	/// 	created on - 30.05.2005 22:20:41
	/// </remarks>
	public class SqlParameter : BasicParameter {
	
	
		#region Constructor
		
		public SqlParameter ()
			:this(String.Empty,DbType.String,String.Empty,ParameterDirection.Input)
		{
			
		}
		public SqlParameter(string parameterName,string parameterValue)
				:this(parameterName,DbType.String,parameterValue,ParameterDirection.Input)
		{
		}
		

		
		public SqlParameter(string parameterName,
		                    DbType dataType,
		                    string parameterValue)
			:this(parameterName,dataType,parameterValue,ParameterDirection.Input)
		{
		}
		
		
		public SqlParameter(string parameterName,
		                    DbType dataType,
		                    string parameterValue,
		                    ParameterDirection parameterDirection):base(parameterName,parameterValue)
		{
			this.DataType = dataType;
			this.ParameterDirection = parameterDirection;	
			base.Type = DataType.ToString();
		}
		
		#endregion
		
		
	
		/// <summary>
		/// DataType of the Parameter
		/// <see cref="System.Data.DbType">DbType</see>
		/// </summary>
		/// 
		public DbType DataType {get;set;}
		

		///<summary>
		/// Direction of Parameter 
		/// <see cref="System.Data.ParameterDirection">ParameterDirection</see>
		///</summary>
		
		public ParameterDirection ParameterDirection {get;set;}
		
	}
}
