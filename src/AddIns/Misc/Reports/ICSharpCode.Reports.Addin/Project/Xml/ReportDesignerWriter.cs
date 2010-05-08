/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Peter Forstmeier
 * Datum: 29.11.2007
 * Zeit: 22:13
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */
 
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;


namespace ICSharpCode.Reports.Addin
{
	/// <summary>
	/// Description of ReportDesignerWriter.
	/// </summary>
	public class ReportDesignerWriter:MycroWriter
	{
		public ReportDesignerWriter()
		{
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
