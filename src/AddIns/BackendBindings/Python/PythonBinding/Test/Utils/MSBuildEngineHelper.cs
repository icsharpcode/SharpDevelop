// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using ICSharpCode.PythonBinding;
using ICSharpCode.SharpDevelop.Project;

namespace PythonBinding.Tests.Utils
{
	/// <summary>
	/// Description of MSBuildEngineHelper.
	/// </summary>
	public sealed class MSBuildEngineHelper
	{
		MSBuildEngineHelper()
		{
		}
		
		/// <summary>
		/// The MSBuildEngine sets the PythonBinPath so if
		/// the Python.Build.Tasks assembly is shadow copied it refers
		/// to the shadow copied assembly not the original. This
		/// causes problems for python projects that refer to the
		/// SharpDevelop.Python.Build.targets import via $(PythonBinPath) 
		/// so here we change it so it points to the real PythonBinPath 
		/// binary.
		/// </summary>
		public static void InitMSBuildEngine()
		{
			// Remove existing PythonBinPath property.
			MSBuildEngine.MSBuildProperties.Remove("PythonBinPath");

			// Set the PythonBinPath property so it points to
			// the actual bin path where the Python.Build.Tasks was built not
			// to the shadow copy folder.
			string codeBase = typeof(PythonParser).Assembly.CodeBase.Replace("file:///", String.Empty);
			string folder = Path.GetDirectoryName(codeBase);
			folder = Path.GetFullPath(Path.Combine(folder, @"..\..\AddIns\AddIns\BackendBindings\PythonBinding\"));
			MSBuildEngine.MSBuildProperties["PythonBinPath"] = folder;
		}		
	}
}
