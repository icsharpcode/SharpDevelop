// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace ICSharpCode.Core
{
	/// <summary>
	/// Is thrown when the GlobalResource manager can't find a requested
	/// resource.
	/// </summary>
	public class ResourceNotFoundException : CoreException
	{
		public ResourceNotFoundException(string resource) : base("Resource not found : " + resource)
		{
		}
	}
}
