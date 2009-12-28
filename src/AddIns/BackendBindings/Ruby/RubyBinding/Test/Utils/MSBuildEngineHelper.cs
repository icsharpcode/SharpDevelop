// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using ICSharpCode.RubyBinding;
using ICSharpCode.SharpDevelop.Project;

namespace RubyBinding.Tests.Utils
{
	public sealed class MSBuildEngineHelper
	{
		MSBuildEngineHelper()
		{
		}
		
		/// <summary>
		/// The MSBuildEngine sets the RubyBinPath so if
		/// the Ruby.Build.Tasks assembly is shadow copied it refers
		/// to the shadow copied assembly not the original. This
		/// causes problems for Ruby projects that refer to the
		/// SharpDevelop.Ruby.Build.targets import via $(RubyBinPath) 
		/// so here we change it so it points to the real RubyBinPath 
		/// binary.
		/// </summary>
		public static void InitMSBuildEngine()
		{
			// Remove existing RubyBinPath property.
			MSBuildEngine.MSBuildProperties.Remove("RubyBinPath");

			// Set the RubyBinPath property so it points to
			// the actual bin path where the Ruby.Build.Tasks was built not
			// to the shadow copy folder.
			string codeBase = typeof(RubyParser).Assembly.CodeBase.Replace("file:///", String.Empty);
			string folder = Path.GetDirectoryName(codeBase);
			folder = Path.GetFullPath(Path.Combine(folder, @"..\..\AddIns\AddIns\BackendBindings\RubyBinding\"));
			MSBuildEngine.MSBuildProperties["RubyBinPath"] = folder;
		}		
	}
}
