// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.WixBinding
{
	public static class WixItemType
	{
		public const string LibExtensionName = "LibExtension";
		public static readonly ItemType LibExtension = new ItemType(LibExtensionName);
		
		public const string LinkExtensionName = "LinkExtension";
		public static readonly ItemType LinkExtension = new ItemType(LinkExtensionName);
		
		public const string CompileExtensionName = "CompileExtension";
		public static readonly ItemType CompileExtension = new ItemType(CompileExtensionName);
		
		public const string LibraryName = "WixLibrary";
		public static readonly ItemType Library = new ItemType(LibraryName);
	}
	
	/// <summary>
	/// Base class for all Wix compiler extension project items.
	/// </summary>
	public abstract class WixExtensionProjectItem : ProjectItem
	{
		public WixExtensionProjectItem(IProject project, ItemType itemType)
			: base(project, itemType)
		{
		}
		
		public WixExtensionProjectItem(IProject project, Microsoft.Build.BuildEngine.BuildItem item)
			: base(project, item)
		{
		}
		
		/// <summary>
		/// Gets or sets the Wix extension class name.
		/// </summary>
		/// <remarks>
		/// This is the fully qualified class name.
		/// </remarks>
		public string ClassName {
			get {
				return GetEvaluatedMetadata("Class");
			}
			set {
				SetEvaluatedMetadata("Class", value);
			}
		}
		
		/// <summary>
		/// Gets or sets the qualified name. This updates the ClassName and the
		/// Include property.
		/// </summary>
		/// <returns>Returns "ClassName, Include" as the qualified name.</returns>
		public string QualifiedName {
			get {
				WixCompilerExtensionName name = new WixCompilerExtensionName(Include, ClassName);
				return name.QualifiedName;
			}
			set {
				WixCompilerExtensionName name = new WixCompilerExtensionName(value);
				ClassName = name.ClassName;
				Include = name.AssemblyName;
			}
		}
	}
}
