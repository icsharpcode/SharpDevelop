// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.PackageManagement.Scripting
{
	public class PowerShellSessionEnvironmentPath
	{
		IPackageScriptSession session;
		
		public PowerShellSessionEnvironmentPath(IPackageScriptSession session)
		{
			this.session = session;
		}
		
		public void Append(string path)
		{
			string environmentPath = GetEnvironmentPath();
			environmentPath = AppendPathSeparatorIfMissing(environmentPath);
			SetEnvironmentPath(environmentPath + path);
		}
		
		string GetEnvironmentPath()
		{
			return session.GetEnvironmentPath();
		}
		
		string AppendPathSeparatorIfMissing(string path)
		{
			if (String.IsNullOrEmpty(path)) {
				return String.Empty;
			}
			
			if (path.EndsWith(";")) {
				return path;
			}
			return path + ";";
		}
		
		void SetEnvironmentPath(string path)
		{
			session.SetEnvironmentPath(path);
		}
	}
}
