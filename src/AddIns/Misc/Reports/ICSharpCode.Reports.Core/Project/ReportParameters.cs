// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Peter Forstmeier" email="peter.forstmeier@t-online.de"/>
//     <version>$Revision$</version>
// </file>

using System;

/// <summary>
/// This Class holds all the Paramters needed to customize a Report
/// like <see cref="Connectionobject"></see>,<see cref="SqwlParameter"></see>
/// </summary>
/// <remarks>
/// 	created by - Forstmeier Peter
/// 	created on - 17.11.2005 22:41:26
/// </remarks>
namespace ICSharpCode.Reports.Core {
	public class ReportParameters : object {
		
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
