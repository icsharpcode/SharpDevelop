// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Peter Forstmeier" email="peter.forstmeier@t-online.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Data.Common;

namespace ICSharpCode.Reports.Core
{
	/// <summary>
	/// This Class is a FactoryClass for <see cref="ConnectionObject"></see>
	/// </summary>
	public sealed class ConnectionObjectFactory
	{
		private ConnectionObjectFactory()
		{
		}
		
		public static ConnectionObject BuildConnectionObject (ReportSettings reportSettings)
		{
			if (reportSettings == null) {
				throw new ArgumentNullException("reportSettings");
			}
			return ConnectionObject.CreateInstance(reportSettings.ConnectionString,
			                                      DbProviderFactories.GetFactory("System.Data.OleDb"));
		}
		
		
		public  static ConnectionObject BuildConnectionObject (string connectionString)
		{
			if (String.IsNullOrEmpty(connectionString)) {
				throw new ArgumentNullException("connectionString");
			}
			return ConnectionObject.CreateInstance(connectionString,
			                                      DbProviderFactories.GetFactory("System.Data.OleDb"));
		}
		
		
		public static ConnectionObject BuildConnectionObject (ReportParameters reportParameters)
		{
			if (reportParameters == null) {
				throw new ArgumentNullException("reportParameters");
				                                
			}
			return reportParameters.ConnectionObject;
		}
	}
}
