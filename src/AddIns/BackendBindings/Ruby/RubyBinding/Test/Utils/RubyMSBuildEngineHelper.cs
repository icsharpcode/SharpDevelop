// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using ICSharpCode.RubyBinding;
using ICSharpCode.Scripting.Tests.Utils;
using ICSharpCode.SharpDevelop.Project;

namespace RubyBinding.Tests.Utils
{
	public sealed class RubyMSBuildEngineHelper
	{
		RubyMSBuildEngineHelper()
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
			string relativePath = @"..\..\AddIns\BackendBindings\RubyBinding\";
			MSBuildEngineHelper.InitMSBuildEngine("RubyBinPath", relativePath, typeof(RubyParser));
		}		
	}
}
