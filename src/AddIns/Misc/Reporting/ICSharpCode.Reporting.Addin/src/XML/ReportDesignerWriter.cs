/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 16.03.2014
 * Time: 18:28
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace ICSharpCode.Reporting.Addin.XML
{
	/// <summary>
	/// Description of ReportDesignerWriter.
	/// </summary>
	class ReportDesignerWriter:MycroWriter
	{
		public ReportDesignerWriter() {
			Console.WriteLine("ReportDesignerWriter");
			Console.WriteLine();
		}
		protected override string GetTypeName(Type t)
		{
			if (t.BaseType != null && t.BaseType.Name.StartsWith("Base",StringComparison.InvariantCultureIgnoreCase)) {
//				return t.BaseType.Name;
			}
			return t.Name;
		}
	}
}
