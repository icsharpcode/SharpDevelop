// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

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
