/*
 * Created by SharpDevelop.
 * User: Daniel Grunwald
 * Date: 08.10.2005
 * Time: 19:47
 */
using System;
using System.Net;

namespace StringResourceTool
{
	public class CookieAwareWebClient : WebClient
	{
		CookieContainer container;
		
		public CookieAwareWebClient(CookieContainer container)
		{
			if (container == null)
				throw new ArgumentNullException("container");
			this.container = container;
		}
	
		protected override WebRequest GetWebRequest(Uri address)
		{
			WebRequest request = base.GetWebRequest(address);
			if (request is HttpWebRequest) {
				(request as HttpWebRequest).CookieContainer = container;
			}
			return request;
		}
	}
}
