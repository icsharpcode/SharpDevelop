// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
