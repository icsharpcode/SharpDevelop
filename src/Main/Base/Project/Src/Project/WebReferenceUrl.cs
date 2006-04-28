// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using System;
using System.ComponentModel;
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
		
		[ReadOnly(true)]
		[LocalizedProperty("${res:ICSharpCode.SharpDevelop.Internal.Project.ProjectReference.UrlBehaviour}", 
		                   Description="${res:ICSharpCode.SharpDevelop.Internal.Project.ProjectReference.UrlBehaviour.Description}")]
		public string UrlBehavior {
			get {
				return base.Properties["UrlBehavior"];
			}
			set {
				base.Properties["UrlBehavior"] = value;
			}
		}
		
		[Browsable(false)]
		public string RelPath {
			get {
				return base.Properties["RelPath"];
			}
			set {
				base.Properties["RelPath"] = value;
			}
		}
		
		[ReadOnly(true)]
		[LocalizedProperty("${res:ICSharpCode.SharpDevelop.Internal.Project.ProjectReference.WebReferenceUrl}", 
		                   Description="${res:ICSharpCode.SharpDevelop.Internal.Project.ProjectReference.WebReferenceUrl.Description}")]
		public string UpdateFromURL {
			get {
				return base.Properties["UpdateFromURL"];
			}
			set {
				base.Properties["UpdateFromURL"] = value;
			}
		}
		
		[Browsable(false)]
		public string ServiceLocationURL {
			get {
				return base.Properties["ServiceLocationURL"];
			}
			set {
				base.Properties["ServiceLocationURL"] = value;
			}
		}
		
		[Browsable(false)]
		public string CachedDynamicPropName {
			get {
				return base.Properties["CachedDynamicPropName"];
			}
			set {
				base.Properties["CachedDynamicPropName"] = value;
			}
		}
		
		[Browsable(false)]
		public string CachedAppSettingsObjectName {
			get {
				return base.Properties["CachedAppSettingsObjectName"];
			}
			set {
				base.Properties["CachedAppSettingsObjectName"] = value;
			}
		}
		
		[Browsable(false)]
		public string CachedSettingsPropName {
			get {
				return base.Properties["CachedSettingsPropName"];
			}
			set {
				base.Properties["CachedSettingsPropName"] = value;
			}
		}
		
		public override string FileName {
			get {
				if (Project != null && RelPath != null) {
					return Path.Combine(Project.Directory, RelPath.Trim('\\'));
				}
				return null;
			}
			set {
				if (Project != null) {
					RelPath = FileUtility.GetRelativePath(Project.Directory, value);
				}
			}
		}
		
		public WebReferenceUrl(IProject project) : base(project)
		{
			UrlBehavior = "Static";
		}
		
		public override ProjectItem Clone()
		{
			ProjectItem n = new WebReferenceUrl(this.Project);
			n.Include = this.Include;
			this.CopyExtraPropertiesTo(n);
			return n;
		}
	}
}
