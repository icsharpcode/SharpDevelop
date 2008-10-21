// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision: 2220 $</version>
// </file>

using System;
using System.Runtime.Serialization;

namespace ICSharpCode.WpfDesign
{
	/// <summary>
	/// Exception class used for designer failures.
	/// </summary>
	[Serializable]
	public class DesignerException : Exception
	{
		/// <summary>
		/// Create a new DesignerException instance.
		/// </summary>
		public DesignerException()
		{
		}

		/// <summary>
		/// Create a new DesignerException instance.
		/// </summary>
		public DesignerException(string message)
			: base(message)
		{
		}

		/// <summary>
		/// Create a new DesignerException instance.
		/// </summary>
		public DesignerException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		/// <summary>
		/// Create a new DesignerException instance.
		/// </summary>
		protected DesignerException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
