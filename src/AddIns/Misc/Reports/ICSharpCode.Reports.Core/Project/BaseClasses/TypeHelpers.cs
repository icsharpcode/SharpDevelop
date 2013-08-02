/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 16.01.2011
 * Time: 17:46
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Data;

namespace ICSharpCode.Reports.Core.Project.BaseClasses
{
	/// <summary>
	/// Description of .
	/// </summary>
	public class TypeHelpers
	{
		//http://social.msdn.microsoft.com/Forums/en-US/netfxbcl/thread/16e981bd-4fa1-4ad2-9f45-5f434489e1e2/
		
		public static DbType DbTypeFromStringRepresenation(string type)
		{
			switch (type.ToLower())
			{
				case "int16":
					return DbType.Int16;
				case "int32":
					return DbType.Int32;
				case "int64":
					return DbType.Int64;
				case "uint16":
					return DbType.UInt16;
				case "uint32":
					return DbType.UInt32;
				case "uint64":
					return DbType.UInt64;
				case "single":
					return DbType.Single;
				case "double":
					return DbType.Double;
				case "decimal":
					return DbType.Decimal;
				case "datetime" :
					return DbType.DateTime;
				case "datetime2" :
					return DbType.DateTime2;
				case "boolean" :
					return DbType.Boolean;
				case "nvarchar":
					return DbType.String;
				case "varchar":
					return DbType.AnsiString;
				case "binary":
					return DbType.Binary;
				case "currency":
					return DbType.Currency;
				case "guid":
					return DbType.Guid;
				case "xml":
					return DbType.Xml;
				default:
					return DbType.Object;
			}
		}
		
		public static TypeCode TypeCodeFromString (string type) {
			if (String.IsNullOrEmpty(type)) {
				throw new ArgumentNullException("type");
			}
//			var x = Type.GetType(type,false,true);
//			var s = Type.GetTypeCode( Type.GetType(type));
//			Console.WriteLine ("typeCode for {0} - {1}",type,s);
			return Type.GetTypeCode( Type.GetType(type));
		}
	}
}
