// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Peter Forstmeier" email="peter.forstmeier@t-online.de"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace ICSharpCode.Reports.Core
{
	/// <summary>
	/// Description of DataType.
	/// </summary>
	internal class DataTypeHelper{
		
		private DataTypeHelper () {
			
		}
		internal static TypeCode TypeCodeFromString (string type) {
			if (String.IsNullOrEmpty(type)) {
				throw new ArgumentNullException("type");
			}
			return Type.GetTypeCode( Type.GetType(type));
		}
		
		/*
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
		*/
	}
}
