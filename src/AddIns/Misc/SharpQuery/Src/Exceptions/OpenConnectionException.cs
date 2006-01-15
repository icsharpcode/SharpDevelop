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


	}

}
