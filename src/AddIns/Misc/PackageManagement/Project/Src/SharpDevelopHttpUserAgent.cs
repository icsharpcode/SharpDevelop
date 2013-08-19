// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using NuGet;

namespace ICSharpCode.PackageManagement
{
	public class SharpDevelopHttpUserAgent
	{
		public SharpDevelopHttpUserAgent()
		{
			CreateUserAgent();
		}
		
		public string Client { get; private set; }
		public string Host { get; private set; }
		public string UserAgent { get; private set; }
		
		void CreateUserAgent()
		{
			Client = "SharpDevelop";
			Host = GetHost();
			UserAgent = HttpUtility.CreateUserAgentString(Client, Host);
		}
		
		string GetHost()
		{
			return String.Format("SharpDevelop/{0}", RevisionClass.FullVersion);
		}
		
		public override string ToString()
		{
			return UserAgent;
		}
	}
}