// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Data;
using System.Globalization;

namespace ICSharpCode.Reporting.Globals
{
	/// <summary>
	/// Description of TypeHelper.
	/// </summary>
	static class TypeHelper
	{
		public static DbType DbTypeFromStringRepresenation(string type)
		{
			switch (type.ToLower(CultureInfo.CurrentCulture)){
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
			return Type.GetTypeCode( Type.GetType(type));
		}
	}
}
