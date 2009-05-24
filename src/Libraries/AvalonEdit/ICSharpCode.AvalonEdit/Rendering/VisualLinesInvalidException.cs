// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Runtime.Serialization;

namespace ICSharpCode.AvalonEdit.Rendering
{
	/// <summary>
	/// A VisualLinesInvalidException indicates that you accessed the <see cref="TextView.VisualLines"/> property
	/// of the <see cref="TextView"/> while the visual lines were invalid.
	/// </summary>
	[Serializable]
	public class VisualLinesInvalidException  : Exception
	{
		/// <summary>
		/// Creates a new VisualLinesInvalidException instance.
		/// </summary>
		public VisualLinesInvalidException() : base()
		{
		}
		
		/// <summary>
		/// Creates a new VisualLinesInvalidException instance.
		/// </summary>
		public VisualLinesInvalidException(string message) : base(message)
		{
		}
		
		/// <summary>
		/// Creates a new VisualLinesInvalidException instance.
		/// </summary>
		public VisualLinesInvalidException(string message, Exception innerException) : base(message, innerException)
		{
		}
		
		/// <summary>
		/// Creates a new VisualLinesInvalidException instance.
		/// </summary>
		protected VisualLinesInvalidException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
