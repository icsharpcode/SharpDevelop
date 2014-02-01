// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
