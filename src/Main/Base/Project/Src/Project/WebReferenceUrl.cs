using System;
using System.IO;
using System.Reflection;
using System.Net;
using System.Web.Services.Description;

namespace ICSharpCode.SharpDevelop.Project
{
	public class WebReferenceUrl : ProjectItem
	{
		public override ItemType ItemType {
			get {
				return ItemType.WebReferenceUrl;
			}
		}
		
		public string UrlBehavior {
			get {
				return base.Properties["UrlBehavior"];
			}
			set {
				base.Properties["UrlBehavior"] = value;
			}
		}
		
		public string RelPath {
			get {
				return base.Properties["RelPath"];
			}
			set {
				base.Properties["RelPath"] = value;
			}
		}
		
		public string UpdateFromURL {
			get {
				return base.Properties["UpdateFromURL"];
			}
			set {
				base.Properties["UpdateFromURL"] = value;
			}
		}
		
		public string ServiceLocationURL {
			get {
				return base.Properties["ServiceLocationURL"];
			}
			set {
				base.Properties["ServiceLocationURL"] = value;
			}
		}
		
		public string CachedDynamicPropName {
			get {
				return base.Properties["CachedDynamicPropName"];
			}
			set {
				base.Properties["CachedDynamicPropName"] = value;
			}
		}
		
		public string CachedAppSettingsObjectName {
			get {
				return base.Properties["CachedAppSettingsObjectName"];
			}
			set {
				base.Properties["CachedAppSettingsObjectName"] = value;
			}
		}
		
		public string CachedSettingsPropName {
			get {
				return base.Properties["CachedSettingsPropName"];
			}
			set {
				base.Properties["CachedSettingsPropName"] = value;
			}
		}
		
		public WebReferenceUrl(IProject project) : base(project)
		{
		}
		
		/// <summary>
		/// Creates a ServiceDescription object from a valid URI
		/// </summary>
		public static ServiceDescription ReadServiceDescription(string uri) 
		{
			ServiceDescription desc = null;
			
			try {
				HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
				WebResponse response  = request.GetResponse();
			
				desc = ServiceDescription.Read(response.GetResponseStream());
				response.Close();
				desc.RetrievalUrl = uri;
			} catch (Exception) {				
				// possibly error reading WSDL?
				return null;
			} 		
			if(desc.Services.Count == 0)
				return null;
			
			return desc;
		}
		
	}
}
