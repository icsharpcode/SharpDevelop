// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using Hornung.ResourceToolkit.ResourceFileContent;
using ICSharpCode.SharpDevelop.Dom;

namespace Hornung.ResourceToolkit.Resolver
{
	/// <summary>
	/// Describes a reference to a resource.
	/// </summary>
	public class ResourceResolveResult : ResolveResult
	{
		
		readonly ResourceSetReference resourceSetReference;
		readonly string key;
		
		/// <summary>
		/// Gets the <see cref="ResourceSetReference"/> that describes the resource set being referenced.
		/// </summary>
		public ResourceSetReference ResourceSetReference {
			get { return this.resourceSetReference; }
		}
		
		/// <summary>
		/// Gets the <see cref="IResourceFileContent"/> for the referenced resource set.
		/// May be <c>null</c>.
		/// </summary>
		public IResourceFileContent ResourceFileContent {
			get {
				if (this.ResourceSetReference == null ||
				    this.ResourceSetReference.FileName == null) {
					return null;
				}
				return this.ResourceSetReference.ResourceFileContent;
			}
		}
		
		/// <summary>
		/// Gets the resource key being referenced. May be null if the key is unknown/not yet typed.
		/// </summary>
		public virtual string Key {
			get { return this.key; }
		}
		
		/// <summary>
		/// Gets the resource file name that contains the resource being referenced.
		/// Only valid if <see cref="ResourceSetReference"/> is not <c>null</c>
		/// and the <see cref="ResourceSetReference"/> contains a valid file name.
		/// </summary>
		public string FileName {
			get {
				
				IMultiResourceFileContent mrfc = this.ResourceFileContent as IMultiResourceFileContent;
				if (mrfc != null && this.Key != null) {
					return mrfc.GetFileNameForKey(this.Key);
				} else if (this.ResourceFileContent != null) {
					return this.ResourceFileContent.FileName;
				} else if (this.ResourceSetReference != null) {
					return this.ResourceSetReference.FileName;
				}
				
				return null;
			}
		}
		
		/// <summary>
		/// Initializes a new instance of the <see cref="ResourceResolveResult"/> class.
		/// </summary>
		/// <param name="callingClass">The class that contains the reference to the resource.</param>
		/// <param name="callingMember">The member that contains the reference to the resource.</param>
		/// <param name="returnType">The type of the resource being referenced.</param>
		/// <param name="resourceSetReference">The <see cref="ResourceSetReference"/> that describes the resource set being referenced.</param>
		/// <param name="key">The resource key being referenced.</param>
		public ResourceResolveResult(IClass callingClass, IMember callingMember, IReturnType returnType, ResourceSetReference resourceSetReference, string key)
			: base(callingClass, callingMember, returnType)
		{
			this.resourceSetReference = resourceSetReference;
			this.key = key;
		}
		
		public override ResolveResult Clone()
		{
			return new ResourceResolveResult(this.CallingClass, this.CallingMember, this.ResolvedType,
			                                 this.ResourceSetReference, this.Key);
		}
	}
	
	/// <summary>
	/// Describes a reference to a group of resource keys with a common prefix.
	/// </summary>
	public sealed class ResourcePrefixResolveResult : ResourceResolveResult
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ResourceResolveResult"/> class.
		/// </summary>
		/// <param name="callingClass">The class that contains the reference to the resource.</param>
		/// <param name="callingMember">The member that contains the reference to the resource.</param>
		/// <param name="returnType">The type of the resource being referenced.</param>
		/// <param name="resourceSetReference">The <see cref="ResourceSetReference"/> that describes the resource set being referenced.</param>
		/// <param name="prefix">The prefix of the resource keys being referenced.</param>
		public ResourcePrefixResolveResult(IClass callingClass, IMember callingMember, IReturnType returnType, ResourceSetReference resourceSetReference, string prefix)
			: base(callingClass, callingMember, returnType, resourceSetReference, prefix)
		{
		}
		
		public override string Key {
			get { return null; }
		}
		
		public string Prefix {
			get { return base.Key; }
		}
		
		public override ResolveResult Clone()
		{
			return new ResourcePrefixResolveResult(this.CallingClass, this.CallingMember, this.ResolvedType,
			                                       this.ResourceSetReference, this.Prefix);
		}
	}
}
