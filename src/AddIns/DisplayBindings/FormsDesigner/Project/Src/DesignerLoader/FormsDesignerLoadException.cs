// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Runtime.Serialization;

namespace ICSharpCode.FormsDesigner
{
	[Serializable()]
	public class FormsDesignerLoadException : ApplicationException
	{
		public FormsDesignerLoadException() : base()
		{
		}
		
		public FormsDesignerLoadException(string message) : base(message)
		{
		}
		
		public FormsDesignerLoadException(string message, Exception innerException) : base(message, innerException)
		{
		}
		
		protected FormsDesignerLoadException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
