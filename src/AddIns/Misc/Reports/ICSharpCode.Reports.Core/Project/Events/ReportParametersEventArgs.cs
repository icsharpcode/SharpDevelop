// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

namespace ICSharpCode.Reports.Core {
	using System;
	
	/// <summary>
	/// This Event is fired when SharpReport need Parameters to run a Query
	/// as an example, have a look to SharpReportView, there we will use this event
	/// </summary>
	/// <remarks>
	/// 	created by - Forstmeier Peter
	/// 	created on - 23.06.2005 22:55:10
	/// </remarks>
	
	
	public class ReportParametersEventArgs : System.EventArgs {
		
		ParameterCollection sqlParametersCollection;
		string reportName;
		
		public ReportParametersEventArgs () {
			
		}
		
		/// <summary>
		/// The ParametersCollection
		/// </summary>
		public ParameterCollection SqlParametersCollection {
			get {
				if (this.sqlParametersCollection == null) {
					this.sqlParametersCollection = new ParameterCollection();
				}
				return sqlParametersCollection;
			}
		}
		
		/// <summary>
		/// The ReportName
		/// </summary>
		public string ReportName {
			get {
				return reportName;
			}
			set {
				reportName = value;
			}
		}
		
	}
}
