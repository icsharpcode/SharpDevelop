// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Runtime.Serialization;

namespace ICSharpCode.Core
{
	/// <include file='ICSharpCode.Core.doc' path='Documentation/Type[@name="ICSharpCode.Core.CoreException"]/Description/*' />
	[Serializable()]
	public class CoreException : ApplicationException
	{
		/// <include file='ICSharpCode.Core.doc' path='Documentation/Type[@name="ICSharpCode.Core.CoreException"]/Member[@name="#ctor()"]/*' />
		public CoreException() : base()
		{
		}
		
		/// <include file='ICSharpCode.Core.doc' path='Documentation/Type[@name="ICSharpCode.Core.CoreException"]/Member[@name="#ctor(String)"]/*' />
		public CoreException(string message) : base(message)
		{
		}
		
		/// <include file='ICSharpCode.Core.doc' path='Documentation/Type[@name="ICSharpCode.Core.CoreException"]/Member[@name="#ctor(String, Exception)"]/*' />
		public CoreException(string message, Exception innerException) : base(message, innerException)
		{
		}
		
		/// <include file='ICSharpCode.Core.doc' path='Documentation/Type[@name="ICSharpCode.Core.CoreException"]/Member[@name="#ctor(SerializationInfo, StreamingContext)"]/*' />
		protected CoreException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
