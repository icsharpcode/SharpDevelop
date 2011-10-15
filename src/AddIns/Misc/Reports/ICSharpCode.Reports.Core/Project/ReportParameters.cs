// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Reports.Core.Interfaces;

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
	
	public interface IReportParameters
	{
		ConnectionObject ConnectionObject { get; set; }
		SqlParameterCollection SqlParameters { get; }
		ParameterCollection Parameters { get; }
		SortColumnCollection SortColumnCollection { get; }
	}
	
	
	public class ReportParameters : IReportParameters
	{

		private ParameterCollection parameters;
		private SqlParameterCollection sqlParameters;

		private SortColumnCollection sortColumnCollection;

		public ReportParameters()
		{
		}


		public ConnectionObject ConnectionObject { get; set; }

		public SqlParameterCollection SqlParameters {
			get {
				if (this.sqlParameters == null) {
					this.sqlParameters = new SqlParameterCollection();
				}
				return sqlParameters;
			}
		}


		public ParameterCollection Parameters {
			get {
				if (this.parameters == null) {
					this.parameters = new ParameterCollection();
				}
				return parameters;
			}
		}


		public SortColumnCollection SortColumnCollection {
			get {
				if (this.sortColumnCollection == null) {
					this.sortColumnCollection = new SortColumnCollection();
				}
				return sortColumnCollection;
			}
		}
	}
}
