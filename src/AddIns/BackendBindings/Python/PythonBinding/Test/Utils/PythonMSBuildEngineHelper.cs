// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using ICSharpCode.PythonBinding;
using ICSharpCode.Scripting.Tests.Utils;
using ICSharpCode.SharpDevelop.Project;

namespace PythonBinding.Tests.Utils
{
	public sealed class PythonMSBuildEngineHelper
	{
		PythonMSBuildEngineHelper()
		{
		}
		
		/// <summary>
		/// The MSBuildEngine sets the PythonBinPath so if
		/// the Python.Build.Tasks assembly is shadow copied it refers
		/// to the shadow copied assembly not the original. This
		/// causes problems for Python projects that refer to the
		/// SharpDevelop.Python.Build.targets import via $(PythonBinPath) 
		/// so here we change it so it points to the real PythonBinPath 
		/// binary.
		/// </summary>
		public static void InitMSBuildEngine()
		{
			string relativePath = @"..\..\AddIns\BackendBindings\PythonBinding\";
			MSBuildEngineHelper.InitMSBuildEngine("PythonBinPath", relativePath, typeof(PythonParser));
		}
	}
}
