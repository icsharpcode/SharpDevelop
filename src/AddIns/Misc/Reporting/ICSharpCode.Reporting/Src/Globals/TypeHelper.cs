// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)
using System;
using System.Data;

namespace ICSharpCode.Reporting.Globals
{
	/// <summary>
	/// Description of TypeHelper.
	/// </summary>
	class TypeHelper
	{
		public static DbType DbTypeFromStringRepresenation(string type)
		{
			switch (type.ToLower())
			{
				case "int":
					return DbType.Int16;
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
			if (type == null)
				type = "System.String";
			var x = Type.GetTypeCode( Type.GetType(type));
		
			return x;
		}
	}
}
