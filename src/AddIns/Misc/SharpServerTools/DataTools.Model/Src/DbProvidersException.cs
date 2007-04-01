/*
 * Created by SharpDevelop.
 * User: dickon
 * Date: 31/03/2007
 * Time: 15:57
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Runtime.Serialization;
using System.Collections.Generic;

namespace SharpDbTools.Data
{
	/// <summary>
	/// Thrown when the DbProvidersService cannot find the class for
	/// a DbProviderFactory that is found in a *.config file.
	/// </summary>
	
	[Serializable()]
	public class DbProvidersException : Exception
	{
		public DbProvidersException() : base()
		{
		}
		
		public DbProvidersException(string message) : base(message)
		{
		}
		
		public DbProvidersException(string message, Exception innerException) : base(message, innerException)
		{
		}
		
		protected DbProvidersException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}	
	}
}
