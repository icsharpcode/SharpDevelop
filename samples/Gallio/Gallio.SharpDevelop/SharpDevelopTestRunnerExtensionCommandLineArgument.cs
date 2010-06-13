// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using Gallio.Extension;

namespace Gallio.SharpDevelop
{
	public class SharpDevelopTestRunnerExtensionCommandLineArgument : TestRunnerExtensionCommandLineArgument
	{
		string testResultsFileName = String.Empty;
		
		public SharpDevelopTestRunnerExtensionCommandLineArgument()
			: base(GetFullyQualifiedTypeName())
		{
		}
		
		static string GetFullyQualifiedTypeName()
		{
			Type type = typeof(SharpDevelopTestRunnerExtension);
			string typeName = type.FullName;
			string assemblyFileName = type.Assembly.ManifestModule.ScopeName;
			return String.Format("{0},{1}", typeName, assemblyFileName);
		}
		
		public string TestResultsFileName {
			get { return testResultsFileName; }
			set {
				testResultsFileName = value;
				Parameters = testResultsFileName;
			}
		}
	}
}
