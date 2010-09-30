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
		string parameterName;
		string parameterValue;
		
		public BasicParameter ()
		{
		}
		
		
		public BasicParameter(string parameterName,string parameterValue)
		{
			this.parameterName = parameterName;
			this.parameterValue = parameterValue;
		}
		
		/// <summary>
		/// Name of the Parameter
		/// </summary>
		/// 
		public string ParameterName {
			get {return parameterName;}
			set {this.parameterName = value;}
		}
		
		
		public string ParameterValue {
			get { return parameterValue; }
			set { parameterValue = value; }
		}
	}
}
