// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Russell Wilkins" email=""/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Runtime.Serialization;

namespace WorkflowDesigner.Loaders
{
	/// <summary>
	/// Description of WorkflowDesignerLoaderException.
	/// </summary>
	[Serializable()]
	public class WorkflowDesignerLoadException : Exception
	{
		public WorkflowDesignerLoadException() : base()
		{
		}
		
		public WorkflowDesignerLoadException(string message) : base(message)
		{
		}
		
		public WorkflowDesignerLoadException(string message, Exception innerException) : base(message, innerException)
		{
		}
		
		protected WorkflowDesignerLoadException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}	
}
