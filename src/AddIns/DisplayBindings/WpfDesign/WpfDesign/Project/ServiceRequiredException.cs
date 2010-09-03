// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace ICSharpCode.WpfDesign
{
	/// <summary>
	/// Exception class used for designer failures.
	/// </summary>
	[Serializable]
	public class ServiceRequiredException : DesignerException
	{
		/// <summary>
		/// Gets the missing sevice.
		/// </summary>
		public Type ServiceType { get; private set; }
		
		/// <summary>
		/// Create a new ServiceRequiredException instance.
		/// </summary>
		public ServiceRequiredException(Type serviceType)
			: base("Service " + serviceType.FullName + " is required.")
		{
			this.ServiceType = serviceType;
		}
		
		/// <summary>
		/// Create a new ServiceRequiredException instance.
		/// </summary>
		public ServiceRequiredException()
		{
		}
		
		/// <summary>
		/// Create a new ServiceRequiredException instance.
		/// </summary>
		public ServiceRequiredException(string message) : base(message)
		{
		}
		
		/// <summary>
		/// Create a new ServiceRequiredException instance.
		/// </summary>
		public ServiceRequiredException(string message, Exception innerException) : base(message, innerException)
		{
		}
		
		/// <summary>
		/// Create a new ServiceRequiredException instance.
		/// </summary>
		protected ServiceRequiredException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			if (info == null)
				throw new ArgumentNullException("info");
			this.ServiceType = (Type)info.GetValue("ServiceType", typeof(Type));
		}
		
		/// <inheritdoc/>
		[SecurityPermission(SecurityAction.Demand, SerializationFormatter=true)]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
				throw new ArgumentNullException("info");
			base.GetObjectData(info, context);
			info.AddValue("ServiceType", this.ServiceType);
		}
	}
}
