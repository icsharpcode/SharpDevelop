// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Runtime.Serialization;

namespace ICSharpCode.Core
{
	/// <summary>
	/// Is thrown when the ServiceManager cannot find a required service.
	/// </summary>
	[Serializable()]
	public class ServiceNotFoundException : CoreException
	{
		public ServiceNotFoundException() : base()
		{
		}
		
		public ServiceNotFoundException(Type serviceType) : base("Required service not found: " + serviceType.FullName)
		{
		}
		
		public ServiceNotFoundException(string message) : base(message)
		{
		}
		
		public ServiceNotFoundException(string message, Exception innerException) : base(message, innerException)
		{
		}
		
		protected ServiceNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
