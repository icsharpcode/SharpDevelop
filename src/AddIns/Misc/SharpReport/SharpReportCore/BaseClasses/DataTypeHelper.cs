/*
 * Created by SharpDevelop.
 * User: Forstmeier Helmut
 * Date: 27.06.2006
 * Time: 09:12
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;

namespace SharpReportCore
{
	/// <summary>
	/// Description of DataType.
	/// </summary>
	internal class DataTypeHelper{
		
		//TODO why not use 
		// TypeCode tc = Type.GetTypeCode( Type.GetType("System.String"));
		private DataTypeHelper () {
			
		}
		internal static TypeCode TypeCodeFromString (string type) {
			TypeCode tc;
			
			if (type == null) {
				type = "System.String";
			}
			
			if (type.StartsWith("System.")){
				type = type.Substring(7);
			}
			
			switch (type){
				case "DateTime":
					tc = TypeCode.DateTime;
					break;
					
				case "Boolean":
					tc = TypeCode.Boolean;
					break;
				
					
				case "String":
				case "Char":
					tc = TypeCode.String;
					break;
					
				case "Decimal":
					tc = TypeCode.Decimal;
					break;
				case "Integer":
				case "Int16":
				case "Int32":
					tc = TypeCode.Int32;
					break;
				case "Float":
				case "Single":
				case "Double":
					tc = TypeCode.Double;
					break;
				 
				default:
					tc = TypeCode.Object;
					break;
			}
			return tc;
		}
		
		internal static  bool IsNumber(TypeCode tc){
			
			switch (tc){
				case TypeCode.Int32:
				case TypeCode.Double:
				case TypeCode.Decimal:
					return true;
				default:		// user error
					return false;
			}
		}
	}
}
