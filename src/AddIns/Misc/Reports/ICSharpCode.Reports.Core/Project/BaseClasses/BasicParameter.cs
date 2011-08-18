// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

namespace  ICSharpCode.Reports.Core
{
	using System;
	/// <summary>
	/// This Class is an AbstractParameter 
	/// </summary>
	/// <remarks>
	/// 	created by - Forstmeier Peter
	/// 	created on - 30.06.2005 10:23:42
	/// </remarks>
	
	
	public  class BasicParameter
	{

		
		public BasicParameter ()
		{
		}
		
		
		public BasicParameter(string parameterName,string parameterValue)
		{
			this.ParameterName = parameterName;
			this.ParameterValue = parameterValue;
		}
		public string ParameterName {get;set;}
		
		public string Type {get;set;}
		
		public string ParameterValue {get;set;}
		
		
	}
}
