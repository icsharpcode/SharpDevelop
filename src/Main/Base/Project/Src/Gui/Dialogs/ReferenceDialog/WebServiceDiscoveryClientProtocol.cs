// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Net;
using System.Web.Services.Discovery;

namespace ICSharpCode.SharpDevelop.Gui
{
	/// <summary>
	/// Custom DiscoveryClientProtocol that determines whether user authentication
	/// is required.
	/// </summary>
	public class WebServiceDiscoveryClientProtocol : DiscoveryClientProtocol
	{
		HttpWebResponse lastResponseReceived;
		
		public WebServiceDiscoveryClientProtocol()
		{
		}
		
		public HttpAuthenticationHeader GetAuthenticationHeader()
		{
			if (lastResponseReceived != null) {
				return new HttpAuthenticationHeader(lastResponseReceived.Headers);
			}
			return null;
		}
		
		public bool IsAuthenticationRequired {
			get {
				if (lastResponseReceived != null) {
					return lastResponseReceived.StatusCode == HttpStatusCode.Unauthorized;
				}
				return false;
			}
		}
		
		protected override WebResponse GetWebResponse(WebRequest request)
		{
			WebResponse response = base.GetWebResponse(request);
			lastResponseReceived = response as HttpWebResponse;
			return response;
		}
	}
}
