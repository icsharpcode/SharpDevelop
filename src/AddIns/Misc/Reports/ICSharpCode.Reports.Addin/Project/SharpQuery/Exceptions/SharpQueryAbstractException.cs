// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Luc Morin" email=""/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Runtime.Serialization;

namespace SharpQuery.Exceptions
{
	[Serializable()]
	public abstract class SharpQueryAbstractException : Exception
	{
		public SharpQueryAbstractException() : base()
		{
		}
		
		public SharpQueryAbstractException(string message) : base(message)
		{
		}
		
		public SharpQueryAbstractException(string message, Exception innerException) : base(message, innerException)
		{
		}
		
		protected SharpQueryAbstractException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
