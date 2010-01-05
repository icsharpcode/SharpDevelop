/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Peter Forstmeier
 * Datum: 29.08.2008
 * Zeit: 19:44
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */
using System;
using System.ComponentModel;
using ICSharpCode.Reports.Core;
using SharpQuery.Collections;
using SharpQuery.SchemaClass;

namespace ICSharpCode.Reports.Addin.ReportWizard
{
	/// <summary>
	/// Description of SqlParameterConverter.
	/// </summary>
	public class SqlParameterConverter:TypeConverter
	{
		public SqlParameterConverter()
		{
		}
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			if (sourceType == typeof(SharpQuerySchemaClassCollection)) {
				return true;
			}
			return base.CanConvertFrom(context, sourceType);
		}
		
		public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
		{
			return base.ConvertFrom(context, culture, value);
		}
		
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			if (destinationType == typeof(ParameterCollection)) {
				return true;
			}
			return base.CanConvertFrom(context, destinationType);
		}
		

		public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
		{
			SharpQuerySchemaClassCollection tmp= value as SharpQuerySchemaClassCollection;
			
			if (destinationType == typeof (ParameterCollection)) {
				ParameterCollection a = new ParameterCollection();
				foreach (SharpQueryParameter par in tmp){
					SqlParameter reportPar =  new SqlParameter (par.Name,
					                               par.DataType,					                
					                               String.Empty,
					                               par.Type);
					reportPar.ParameterValue = par.Value.ToString();
					a.Add(reportPar);
				}
				return a;
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
		
	}
}
