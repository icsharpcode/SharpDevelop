// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Luc Morin" email=""/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Runtime.Serialization;
using ICSharpCode.Core;
using SharpQuery.SchemaClass;

namespace SharpQuery.Exceptions
{
	[Serializable()]
	public class ExecuteSQLException : Exception
	{
		public ExecuteSQLException()
			: base(StringParser.Parse("${res:SharpQuery.Error.SQLExecution}"))
		{
		}

		public ExecuteSQLException(ISchemaClass schema)
			: base(StringParser.Parse("${res:SharpQuery.Error.SQLExecution}")
			       + "\n\r"
			       + "-----------------"
			       + "\n\r"
			       + "(" + schema.Connection.ConnectionString + ")"
			       + "\n\r"
			       + "(" + schema.Connection.Name + ")"
			      )
		{
		}

		public ExecuteSQLException(string message)
			: base(StringParser.Parse("${res:SharpQuery.Error.SQLExecution}")
			       + "\n\r"
			       + "-----------------"
			       + "\n\r"
			       + message)
		{
		}

		public ExecuteSQLException(string message, Exception innerException) : base(message, innerException)
		{
		}
		
		protected ExecuteSQLException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}

}
