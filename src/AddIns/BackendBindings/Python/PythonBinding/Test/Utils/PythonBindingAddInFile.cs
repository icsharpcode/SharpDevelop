// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using System.Reflection;
using System.Xml;

namespace PythonBinding.Tests.Utils
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
