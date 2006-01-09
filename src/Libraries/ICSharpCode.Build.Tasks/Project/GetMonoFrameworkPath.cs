// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System;

namespace ICSharpCode.Build.Tasks
{
	/// <summary>
	/// Gets the path to the Mono Framework assemblies.
	/// </summary>
	public class GetMonoFrameworkPath : Task
	{
		public const string TargetMonoFrameworkVersion11 = "Mono v1.1";
		public const string TargetMonoFrameworkVersion20 = "Mono v2.0";
		
		string path = String.Empty;
		TargetMonoFrameworkVersion targetFrameworkVersion = TargetMonoFrameworkVersion.VersionLatest;
		
		public GetMonoFrameworkPath()
		{
		}
		
		[Output]
		public string Path { 
			get {
				return path;
			}
			set {
				path = value;
			}
		}
		
		public string TargetFrameworkVersion {
			get {
				return ConvertToString(targetFrameworkVersion);
			}
			set {
				targetFrameworkVersion = ConvertToEnum(value);
			}
		}

		public override bool Execute()
		{
			if (MonoToolLocationHelper.IsMonoInstalled) {
				System.Diagnostics.Debug.WriteLine("TargetFrameworkVersion: " + targetFrameworkVersion.ToString());
				path = MonoToolLocationHelper.GetPathToMonoFramework(targetFrameworkVersion);
				System.Diagnostics.Debug.WriteLine("MonoFrameworkPath: " + path);
				return true;
			}
			Log.LogError("Mono is not installed.");
			return false;
		}
		
		static string ConvertToString(TargetMonoFrameworkVersion frameworkVersion)
		{
			switch (frameworkVersion) {
				case TargetMonoFrameworkVersion.Version11:
					return TargetMonoFrameworkVersion11;
				case TargetMonoFrameworkVersion.Version20:
					return TargetMonoFrameworkVersion20;
			}
			return null;
		}
		
		static TargetMonoFrameworkVersion ConvertToEnum(string frameworkVersion)
		{
			if (frameworkVersion == TargetMonoFrameworkVersion11) {
				return TargetMonoFrameworkVersion.Version11;
			} else if (frameworkVersion == TargetMonoFrameworkVersion20) {
				return TargetMonoFrameworkVersion.Version20;
			}
			throw new ArgumentException(String.Concat("Unknown Target Mono Framework Version: ", frameworkVersion));
		}
	}
}
