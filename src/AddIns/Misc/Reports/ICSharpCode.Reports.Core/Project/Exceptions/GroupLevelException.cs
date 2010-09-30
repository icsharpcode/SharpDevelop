// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

/// <summary>
/// This exception is throw'n when something is wrong with the File Format
/// </summary>
/// <remarks>
/// 	created by - Forstmeier Peter
/// 	created on - 25.04.2005 14:29:20
/// </remarks>
namespace ICSharpCode.Reports.Core {
	
	[Serializable()]
	public class GroupLevelException : System.Exception {
		
		public GroupLevelException():base ()
		{
		}
		
		public GroupLevelException(string errorMessage) :base (errorMessage)
		{
		}
		
		public GroupLevelException(string errorMessage,
		                           Exception exception):base (errorMessage,exception)
		{
		}

		
		protected GroupLevelException(SerializationInfo info,
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
