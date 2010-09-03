// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.ComponentModel;
using System.IO;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.Project
{
	public sealed class WebReferenceUrl : ProjectItem
	{
		[ReadOnly(true)]
		[LocalizedProperty("${res:ICSharpCode.SharpDevelop.Internal.Project.ProjectReference.WebReferenceUrl}",
		                   Description="${res:ICSharpCode.SharpDevelop.Internal.Project.ProjectReference.WebReferenceUrl.Description}")]
		public string UpdateFromURL {
			get {
				return GetEvaluatedMetadata("UpdateFromURL");
			}
			set {
				SetEvaluatedMetadata("UpdateFromURL", value);
			}
		}
		
		[Browsable(false)]
		public string ServiceLocationURL {
			get {
				return GetEvaluatedMetadata("ServiceLocationURL");
			}
			set {
				SetEvaluatedMetadata("ServiceLocationURL", value);
			}
		}
		
		[Browsable(false)]
		public string CachedDynamicPropName {
			get {
				return GetEvaluatedMetadata("CachedDynamicPropName");
			}
			set {
				SetEvaluatedMetadata("CachedDynamicPropName", value);
			}
		}
		
		[Browsable(false)]
		public string CachedAppSettingsObjectName {
			get {
				return GetEvaluatedMetadata("CachedAppSettingsObjectName");
			}
			set {
				SetEvaluatedMetadata("CachedAppSettingsObjectName", value);
			}
		}
		
		[Browsable(false)]
		public string CachedSettingsPropName {
			get {
				return GetEvaluatedMetadata("CachedSettingsPropName");
			}
			set {
				SetEvaluatedMetadata("CachedSettingsPropName", value);
			}
		}
		
		[Browsable(false)]
		public string Namespace {
			get {
				string ns = GetEvaluatedMetadata("Namespace");
				if (ns.Length > 0) {
					return ns;
				}
				return Project.RootNamespace;
			}
			set {
				SetEvaluatedMetadata("Namespace", value);
			}
		}
		
		[Browsable(false)]
		public string RelPath {
			get {
				return GetEvaluatedMetadata("RelPath");
			}
			set {
				SetEvaluatedMetadata("RelPath", value);
			}
		}
		
		[ReadOnly(true)]
		[LocalizedProperty("${res:ICSharpCode.SharpDevelop.Internal.Project.ProjectReference.UrlBehaviour}",
		                   Description="${res:ICSharpCode.SharpDevelop.Internal.Project.ProjectReference.UrlBehaviour.Description}")]
		public string UrlBehavior {
			get {
				return GetEvaluatedMetadata("UrlBehavior");
			}
			set {
				SetEvaluatedMetadata("UrlBehavior", value);
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
		
		public WebReferenceUrl(IProject project)
			: base(project, ItemType.WebReferenceUrl)
		{
			UrlBehavior = "Static";
		}
		
		public WebReferenceUrl(IProject project, IProjectItemBackendStore buildItem) : base(project, buildItem)
		{
			UrlBehavior = "Static";
		}
	}
}
