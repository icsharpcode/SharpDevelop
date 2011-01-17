// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Data;

namespace ICSharpCode.Reports.Core
{

	/// <summary>
	/// According to the definition in
	/// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/rsrdl/htm/rsp_ref_rdl_elements_qz_629g.asp
	/// 
	/// This Class definies a Reportparameter
	/// </summary>
	/// <remarks>
	/// 	created by - Forstmeier Peter
	/// 	created on - 30.05.2005 22:20:41
	/// </remarks>
	public class SqlParameter : BasicParameter {
	
	//	ParameterDirection  parameterDirection = ParameterDirection.InputOutput;
	
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
