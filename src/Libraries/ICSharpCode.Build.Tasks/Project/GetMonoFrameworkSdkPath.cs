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
	/// Gets the path to the Mono SDK folder.
	/// </summary>
	public class GetMonoFrameworkSdkPath : Task
	{
		string path = String.Empty;
		
		public GetMonoFrameworkSdkPath()
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

		public override bool Execute()
		{
			if (MonoToolLocationHelper.IsMonoInstalled) {
				path = MonoToolLocationHelper.GetPathToMonoSdk();
				System.Diagnostics.Debug.WriteLine("MonoSdk: " + path);
				return true;
			}
			Log.LogError(Resources.MonoIsNotInstalled);
			return false;
	
		}
	}
}
