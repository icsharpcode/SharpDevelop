// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.SharpDevelop.Gui;
using NUnit.Framework;
using System;
using System.Net;

namespace ICSharpCode.SharpDevelop.Tests.WebReferences
{
	[TestFixture]
	public class HttpAuthenticationHeaderTests
	{
		[Test]
		public void IsBasicAuthentication()
		{
			WebHeaderCollection headers = new WebHeaderCollection();
			headers.Add(HttpResponseHeader.WwwAuthenticate, "Basic realm='localhost'");
			HttpAuthenticationHeader authenticationHeader = new HttpAuthenticationHeader(headers);

			Assert.AreEqual("Basic", authenticationHeader.AuthenticationType);
		}
		
		[Test]
		public void NoHeadersAdded()
		{
			WebHeaderCollection headers = new WebHeaderCollection();
			HttpAuthenticationHeader authenticationHeader = new HttpAuthenticationHeader(headers);

			Assert.AreEqual(String.Empty, authenticationHeader.AuthenticationType);
		}
		
		[Test]
		public void NonStandardAuthenticationHeaderAdded()
		{
			WebHeaderCollection headers = new WebHeaderCollection();
			headers.Add(HttpResponseHeader.WwwAuthenticate, "Foo");
			HttpAuthenticationHeader authenticationHeader = new HttpAuthenticationHeader(headers);

			Assert.AreEqual("Foo", authenticationHeader.AuthenticationType);
		}
		
		[Test]
		public void IsWindowsAuthentication()
		{
			WebHeaderCollection headers = new WebHeaderCollection();
			headers.Add(HttpResponseHeader.WwwAuthenticate, "Negotiate");
			headers.Add(HttpResponseHeader.WwwAuthenticate, "NTLM");
			HttpAuthenticationHeader authenticationHeader = new HttpAuthenticationHeader(headers);

			Assert.AreEqual("Negotiate,NTLM", authenticationHeader.AuthenticationType);
		}
		
		[Test]
		public void ManyAuthenticationSchemesAdded()
		{
			WebHeaderCollection headers = new WebHeaderCollection();
			headers.Add(HttpResponseHeader.WwwAuthenticate, "Negotiate");
			headers.Add(HttpResponseHeader.WwwAuthenticate, "NTLM");
			headers.Add(HttpResponseHeader.WwwAuthenticate, "Basic realm='test'");
			HttpAuthenticationHeader authenticationHeader = new HttpAuthenticationHeader(headers);

			Assert.AreEqual("Negotiate,NTLM,Basic", authenticationHeader.AuthenticationType);
		}
		
		[Test]
		public void DigestAuthenticationSchemeAdded()
		{
			WebHeaderCollection headers = new WebHeaderCollection();
			headers.Add(HttpResponseHeader.WwwAuthenticate, "Digest realm='test'");
			headers.Add(HttpResponseHeader.WwwAuthenticate, "NTLM");
			HttpAuthenticationHeader authenticationHeader = new HttpAuthenticationHeader(headers);

			Assert.AreEqual("Digest,NTLM", authenticationHeader.AuthenticationType);
		}
	}
}
