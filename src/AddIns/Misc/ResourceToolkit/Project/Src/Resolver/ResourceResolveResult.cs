// <file>
//     <copyright see="prj:///Doc/copyright.txt"/>
//     <license see="prj:///Doc/license.txt"/>
//     <owner name="Christian Hornung" email="c-hornung@gmx.de"/>
//     <version>$Revision$</version>
// </file>

using System;

using ICSharpCode.SharpDevelop.Dom;

using Hornung.ResourceToolkit.ResourceFileContent;

namespace Hornung.ResourceToolkit.Resolver
{
	/// <summary>
	/// Describes a reference to a resource.
	/// </summary>
	public class ResourceResolveResult : ResolveResult
	{
		
		IResourceFileContent resourceFileContent;
		string key;
		
		/// <summary>
		/// Gets the <see cref="IResourceFileContent" /> of the resource being referenced.
		/// </summary>
		public IResourceFileContent ResourceFileContent {
			get {
				return this.resourceFileContent;
			}
		}
		
		/// <summary>
		/// Gets the resource key being referenced. May be null if the key is unknown/not yet typed.
		/// </summary>
		public string Key {
			get {
				return this.key;
			}
		}
		
		/// <summary>
		/// Gets the resource file name that contains the resource being referenced.
		/// Only valid if both <see cref="ResourceFileContent"/> and <see cref="Key"/> are not <c>null</c>.
		/// </summary>
		public string FileName {
			get {
				
				if (this.ResourceFileContent == null || this.Key == null) {
					return null;
				}
				
				IMultiResourceFileContent mrfc = this.ResourceFileContent as IMultiResourceFileContent;
				if (mrfc != null) {
					return mrfc.GetFileNameForKey(this.Key);
				} else {
					return this.ResourceFileContent.FileName;
				}
				
			}
		}
		
		/// <summary>
		/// Initializes a new instance of the <see cref="ResourceResolveResult"/> class.
		/// </summary>
		/// <param name="callingClass">The class that contains the reference to the resource.</param>
		/// <param name="callingMember">The member that contains the reference to the resource.</param>
		/// <param name="returnType">The type of the resource being referenced.</param>
		/// <param name="resourceFileContent">The <see cref="IResourceFileContent"/> that contains the resource being referenced.</param>
		/// <param name="key">The resource key being referenced.</param>
		public ResourceResolveResult(IClass callingClass, IMember callingMember, IReturnType returnType, IResourceFileContent resourceFileContent, string key)
			: base(callingClass, callingMember, returnType)
		{
			this.resourceFileContent = resourceFileContent;
			this.key = key;
		}
		
	}
}
