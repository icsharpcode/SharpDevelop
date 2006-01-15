// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Luc Morin" email=""/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.Core;
using SharpQuery.SchemaClass;

namespace SharpQuery.Exceptions
{
	public class ConnectionStringException : Exception
	{
		public ConnectionStringException()
			: base(StringParser.Parse("${res:SharpQuery.Error.WrongConnectionString}"))
		{
		}

		public ConnectionStringException(ISchemaClass schema)
			: base(StringParser.Parse("${res:SharpQuery.Error.WrongConnectionString}")
			       + "\n\r"
			       + "-----------------"
			       + "\n\r"
			       + "(" + schema.Connection.ConnectionString + ")"
			       + "\n\r"
			       + "(" + schema.Connection.Name + ")"
			      )
		{
		}

		public ConnectionStringException(string message)
			: base(StringParser.Parse("${res:SharpQuery.Error.WrongConnectionString}")
			       + "\n\r"
			       + "-----------------"
			       + "\n\r"
			       + message)
		{
		}
	}

}
