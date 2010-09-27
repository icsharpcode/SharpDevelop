// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

using SimpleExpressionEvaluator.Evaluation;

namespace ICSharpCode.Reports.Expressions.ReportingLanguage
{
	/// <summary>
	/// Description of UnknownFunctionException.
	/// </summary>
	[Serializable()]
	public class UnknownFunctionException: Exception
	{
		
		public UnknownFunctionException():base()
		{
		}
		public UnknownFunctionException(string errorMessage) :base (errorMessage)
		{
		}
		public UnknownFunctionException(string errorMessage,
		                             Exception exception):base (errorMessage,exception)
		{
		}
		
		protected UnknownFunctionException(SerializationInfo info,
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
