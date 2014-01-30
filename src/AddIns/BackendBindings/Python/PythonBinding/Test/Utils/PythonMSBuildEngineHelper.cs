// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
