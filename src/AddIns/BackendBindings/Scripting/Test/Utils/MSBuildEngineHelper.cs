// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using ICSharpCode.Scripting;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.Scripting.Tests.Utils
{
	public sealed class MSBuildEngineHelper
	{
		MSBuildEngineHelper()
		{
		}
		
		/// <summary>
		/// The MSBuildEngine sets theBinPath so if
		/// the Build.Tasks assembly is shadow copied it refers
		/// to the shadow copied assembly not the original. This
		/// causes problems for python projects that refer to the
		/// SharpDevelop.*.Build.targets import via $(BinPath) 
		/// so here we change it so it points to the real BinPath 
		/// binary.
		/// </summary>
		public static void InitMSBuildEngine(string binPathName, string addInRelativePath, Type typeForCodeBase)
		{
			MSBuildEngine.MSBuildProperties.Remove(binPathName);

			// Set the bin path property so it points to
			// the actual bin path where the Build.Tasks was built not
			// to the shadow copy folder.
			string codeBase = typeForCodeBase.Assembly.CodeBase.Replace("file:///", String.Empty);
			string folder = Path.GetDirectoryName(codeBase);
			folder = Path.GetFullPath(Path.Combine(folder, addInRelativePath));
			MSBuildEngine.MSBuildProperties[binPathName] = folder;
		}
	}
}
