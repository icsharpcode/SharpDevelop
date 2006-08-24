/*
 * Created by SharpDevelop.
 * User: Forstmeier Helmut
 * Date: 09.08.2006
 * Time: 22:13
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Data.OleDb;
using System.ComponentModel;
using SharpQuery.SchemaClass;
using SharpQuery.Collections;
using SharpReportCore;

namespace ReportGenerator
{
	/// <summary>
	/// Description of SqlTypeConverter.
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
			if (destinationType == typeof(SqlParametersCollection)) {
				return true;
			}
			return base.CanConvertFrom(context, destinationType);
		}
		

		public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType){
			SharpQuerySchemaClassCollection tmp= value as SharpQuerySchemaClassCollection;
			
			if (destinationType == typeof (SqlParametersCollection)) {
				SqlParametersCollection a = new SqlParametersCollection();
				foreach (SharpQueryParameter par in tmp){
					SqlParameter reportPar = new SqlParameter();
					reportPar =  new SqlParameter (par.Name,
					                               par.DataType,					                
					                               String.Empty,
					                               par.Type);
					
					reportPar.ParameterValue = par.Value;
					a.Add(reportPar);
				}
				return a;
			}
			
			return base.ConvertTo(context, culture, value, destinationType);
		}
		
	}
}
