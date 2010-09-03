// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Runtime.Serialization;

namespace ICSharpCode.WpfDesign.XamlDom
{
	/// <summary>
	/// Exception class used for xaml loading failures.
	/// </summary>
	[Serializable]
	public class XamlLoadException : Exception
	{
		/// <summary>
		/// Create a new XamlLoadException instance.
		/// </summary>
		public XamlLoadException()
		{
		}
		
		/// <summary>
		/// Create a new XamlLoadException instance.
		/// </summary>
		public XamlLoadException(string message) : base(message)
		{
		}
		
		/// <summary>
		/// Create a new XamlLoadException instance.
		/// </summary>
		public XamlLoadException(string message, Exception innerException) : base(message, innerException)
		{
		}
		
		/// <summary>
		/// Create a new XamlLoadException instance.
		/// </summary>
		protected XamlLoadException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
