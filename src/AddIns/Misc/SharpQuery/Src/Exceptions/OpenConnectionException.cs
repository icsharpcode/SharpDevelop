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
	public class OpenConnectionException : Exception
	{
		public OpenConnectionException()
			: base(StringParser.Parse("${res:SharpQuery.Error.OpenError}"))
		{
		}

		public OpenConnectionException(ISchemaClass schema)
			: base(StringParser.Parse("${res:SharpQuery.Error.OpenError}")
			       + "\n\r"
			       + "-----------------"
			       + "\n\r"
			       + "(" + schema.Connection.ConnectionString + ")"
			       + "\n\r"
			       + "(" + schema.Connection.Name + ")"
			      )
		{
		}

		public OpenConnectionException(string message)
			: base(StringParser.Parse("${res:SharpQuery.Error.OpenError}")
			       + "\n\r"
			       + "-----------------"
			       + "\n\r"
			       + message)
		{
		}

		public OpenConnectionException(string message, Exception innerException) : base(message, innerException)
		{
		}
		
		protected OpenConnectionException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}

}
