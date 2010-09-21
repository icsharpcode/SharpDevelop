// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Runtime.Serialization;

namespace ICSharpCode.Reports.Core
{
	/// <summary>
	/// Description of WrongSectionException.
	/// </summary>
	[Serializable()]
	public class WrongSectionException: Exception
	{
		
		public WrongSectionException():base()
		{
		}
		
		
		public WrongSectionException(string errorMessage) :base (errorMessage)
		{
		}
		
		public WrongSectionException(string errorMessage,
		                             Exception exception):base (errorMessage,exception)
		{
		}
		
		protected WrongSectionException(SerializationInfo info,
		                                StreamingContext context) : base(info, context)
		{
			// Implement type-specific serialization constructor logic.
		}
	}
}
