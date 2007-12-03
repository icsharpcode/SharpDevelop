// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using System.Reflection;
using System.Xml;

namespace PythonBinding.Tests
{
	/// <summary>
	/// Utility class that reads the PythonBinding.addin file
	/// that has been embedded as a resource into the test assembly.
	/// </summary>
	public sealed class PythonBindingAddInFile
	{
		PythonBindingAddInFile()
		{
		}
		
		/// <summary>
		/// Returns the PythonBinding.addin file.
		/// </summary>
		public static TextReader ReadAddInFile()
		{
			Assembly assembly = Assembly.GetAssembly(typeof(PythonBindingAddInFile));
			string resourceName = String.Concat("PythonBinding.Tests.PythonBinding.addin");
			Stream resourceStream = assembly.GetManifestResourceStream(resourceName);
			if (resourceStream != null) {
				return new StreamReader(resourceStream);
			}
			return null;
		}
	}
}
