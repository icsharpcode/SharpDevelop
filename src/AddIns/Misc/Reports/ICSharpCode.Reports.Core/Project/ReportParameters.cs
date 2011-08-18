// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

/// <summary>
/// This Class holds all the Paramters needed to customize a Report
/// like <see cref="Connectionobject"></see>,<see cref="SqwlParameter"></see>
/// </summary>
/// <remarks>
/// 	created by - Forstmeier Peter
/// 	created on - 17.11.2005 22:41:26
/// </remarks>
namespace ICSharpCode.Reports.Core 
{
	
	public class ReportParameters 
	{
		
		private ConnectionObject connectionObject;
		private ParameterCollection sqlParameters;
		private SortColumnCollection sortColumnCollection;
		
		
		public ReportParameters() 
		{
		}
		
		
		public ReportParameters(ConnectionObject connectionObject)
		{
			this.connectionObject = connectionObject;
		}
		
		public ConnectionObject ConnectionObject
		{
			get {
				return connectionObject;
			}
			set {
				connectionObject = value;
			}
		}
		
		public ParameterCollection SqlParameters
		{
			get {
				if (this.sqlParameters == null) {
					this.sqlParameters = new ParameterCollection();
				}
				return sqlParameters;
			}
		}
		
		public SortColumnCollection SortColumnCollection 
		{
			get {
				if (this.sortColumnCollection == null) {
					this.sortColumnCollection = new SortColumnCollection();
				}
				return sortColumnCollection;
			}
		}
		
	}
}
