// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)
using System;

namespace ICSharpCode.Reporting.BaseClasses
{
	/// <summary>
	/// Description of BasicParameter.
	/// </summary>
	public class BasicParameter
	{
		public BasicParameter (){
		}
		
		public BasicParameter(string parameterName,string parameterValue){
			this.ParameterName = parameterName;
			this.ParameterValue = parameterValue;
		}
		
		public string ParameterName {get;set;}
		
		public string Type {get;set;}
		
		public string ParameterValue {get;set;}
	}
}
