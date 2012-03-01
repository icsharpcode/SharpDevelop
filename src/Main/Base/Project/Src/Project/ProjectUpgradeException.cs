// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Runtime.Serialization;

namespace ICSharpCode.SharpDevelop.Project
{
	/// <summary>
	/// This exception occurs when upgrading a project fails.
	/// </summary>
	[Serializable]
	public class ProjectUpgradeException : Exception
	{
		public ProjectUpgradeException() : base()
		{
		}
		
		public ProjectUpgradeException(string message) : base(message)
		{
		}
		
		public ProjectUpgradeException(string message, Exception innerException) : base(message, innerException)
		{
		}
		
		protected ProjectUpgradeException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
