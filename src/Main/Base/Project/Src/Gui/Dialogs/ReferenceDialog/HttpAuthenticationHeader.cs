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

using System;
using System.Net;
using System.Text;

namespace ICSharpCode.SharpDevelop.Gui
{
	/// <summary>
	/// Represents the WWW-Authenticate HTTP response header.
	/// </summary>
	public class HttpAuthenticationHeader
	{
		string[] authenticationSchemes;
		
		public HttpAuthenticationHeader(WebHeaderCollection headers)
		{
			authenticationSchemes = headers.GetValues("WWW-Authenticate");
		}
		
		public override string ToString()
		{
			if (HasAuthenticationSchemes) {
				StringBuilder schemes = new StringBuilder();
				foreach (string scheme in authenticationSchemes) {
					schemes.Append("WWW-Authenticate: ");
					schemes.Append(scheme);
					schemes.Append("\r\n");
				}
				return schemes.ToString();
			}
			return String.Empty;
		}
		
		/// <summary>
		/// Gets a comma separated list of authentication types.
		/// </summary>
		public string AuthenticationType {
			get {
				if (HasAuthenticationSchemes) {
					int schemesAdded = 0;
					StringBuilder authenticationType = new StringBuilder();
					for (int i = 0; i < authenticationSchemes.Length; ++i) {
						string scheme = authenticationSchemes[i];
						int index = scheme.IndexOf(' ');
						if (index > 0) {
							scheme = scheme.Substring(0, index);
						}
						if (schemesAdded > 0) {
							authenticationType.Append(",");
						}
						authenticationType.Append(scheme);
						schemesAdded++;
					}
					return authenticationType.ToString();
				}
				return String.Empty;
			}
		}
		
		bool HasAuthenticationSchemes {
			get {
				return authenticationSchemes != null && authenticationSchemes.Length > 0;
			}
		}
	}
}
