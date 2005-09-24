// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Runtime.Serialization;

namespace ICSharpCode.FormDesigner
{
	[Serializable()]
	public class FormDesignerLoadException : ApplicationException
	{
		public FormDesignerLoadException() : base()
		{
		}
		
		public FormDesignerLoadException(string message) : base(message)
		{
		}
		
		public FormDesignerLoadException(string message, Exception innerException) : base(message, innerException)
		{
		}
		
		protected FormDesignerLoadException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
