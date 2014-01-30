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
