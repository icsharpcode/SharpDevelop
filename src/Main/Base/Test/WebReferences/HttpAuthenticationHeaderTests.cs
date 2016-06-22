// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
