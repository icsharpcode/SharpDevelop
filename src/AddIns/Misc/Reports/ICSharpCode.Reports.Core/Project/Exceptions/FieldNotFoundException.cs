// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace ICSharpCode.Reports.Core
{
	/// <summary>
	/// Description of FieldNotFoundException.
	/// </summary>
	[Serializable()]
	public class FieldNotFoundException : System.Exception {
		
		public FieldNotFoundException():base ()
		{
		}
		
		public FieldNotFoundException(string errorMessage) :base (errorMessage)
		{
		}
		
		public FieldNotFoundException(string errorMessage,
		                           Exception exception):base (errorMessage,exception)
		{
		}

		
		protected FieldNotFoundException(SerializationInfo info,
		                              StreamingContext context) : base(info, context)
		{
			// Implement type-specific serialization constructor logic.
		}

		
		[SecurityPermissionAttribute(SecurityAction.Demand,
		                             SerializationFormatter = true)]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
