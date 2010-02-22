// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Runtime.Serialization;

namespace ICSharpCode.XamlBinding
{
	/// <summary>
	/// Exception thrown when XAML loading fails because there is a syntax error in a markup extension.
	/// </summary>
	[Serializable]
	public class MarkupExtensionParseException : Exception
	{
		/// <summary>
		/// Create a new XamlMarkupExtensionParseException instance.
		/// </summary>
		public MarkupExtensionParseException()
		{
		}
		
		/// <summary>
		/// Create a new XamlMarkupExtensionParseException instance.
		/// </summary>
		public MarkupExtensionParseException(string message) : base(message)
		{
		}
		
		/// <summary>
		/// Create a new XamlMarkupExtensionParseException instance.
		/// </summary>
		public MarkupExtensionParseException(string message, Exception innerException) : base(message, innerException)
		{
		}
		
		/// <summary>
		/// Create a new XamlMarkupExtensionParseException instance.
		/// </summary>
		protected MarkupExtensionParseException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
