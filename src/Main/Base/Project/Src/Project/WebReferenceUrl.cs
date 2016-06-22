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
		
		public override FileName FileName {
			get {
				if (Project != null && RelPath != null) {
					return FileName.Create(Path.Combine(Project.Directory, RelPath.Trim('\\')));
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
