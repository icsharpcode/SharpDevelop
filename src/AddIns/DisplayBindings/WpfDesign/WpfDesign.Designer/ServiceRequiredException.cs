// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision: 2222 $</version>
// </file>

using System;
using System.Runtime.Serialization;

namespace ICSharpCode.WpfDesign.Designer
{
	/// <summary>
	/// Exception class used for designer failures.
	/// </summary>
	[Serializable]
	public class ServiceRequiredException : DesignerException
	{
		/// <summary>
		/// Create a new ServiceRequiredException instance.
		/// </summary>
		public ServiceRequiredException(Type serviceType)
			: this("Service " + serviceType.FullName + " is required.")
		{
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
		public ServiceRequiredException(string message)
			: base(message)
		{
		}

		/// <summary>
		/// Create a new ServiceRequiredException instance.
		/// </summary>
		public ServiceRequiredException(string message, Exception innerException)
			: base(message, innerException)
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
