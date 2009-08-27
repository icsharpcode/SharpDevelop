// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Runtime.Serialization;

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
			: this("Service " + serviceType.FullName + " is required.")
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
		}
	}
}
