// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
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
