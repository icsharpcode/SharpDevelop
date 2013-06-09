// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Net;
using NuGet;

namespace ICSharpCode.PackageManagement
{
	public class SharpDevelopCredentialProvider : ICredentialProvider
	{
		public ICredentials GetCredentials(Uri uri, IWebProxy proxy, CredentialType credentialType, bool retrying)
		{
			return null;
		}
	}
}

