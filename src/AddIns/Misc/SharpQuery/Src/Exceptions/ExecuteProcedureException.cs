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
	public class ExecuteProcedureException : Exception
	{
		public ExecuteProcedureException()
			: base(StringParser.Parse("${res:SharpQuery.Error.ProcedureExecution}"))
		{
		}

		public ExecuteProcedureException(ISchemaClass schema)
			: base(StringParser.Parse("${res:SharpQuery.Error.ProcedureExecution}")
			       + "\n\r"
			       + "-----------------"
			       + "\n\r"
			       + "(" + schema.Connection.ConnectionString + ")"
			       + "\n\r"
			       + "(" + schema.Connection.Name + ")"
			      )
		{
		}

		public ExecuteProcedureException(string message)
			: base(StringParser.Parse("${res:SharpQuery.Error.ProcedureExecution}")
			       + "\n\r"
			       + "-----------------"
			       + "\n\r"
			       + message)
		{
		}
		
		public ExecuteProcedureException(string message, Exception innerException) : base(message, innerException)
		{
		}
		
		protected ExecuteProcedureException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}

}
