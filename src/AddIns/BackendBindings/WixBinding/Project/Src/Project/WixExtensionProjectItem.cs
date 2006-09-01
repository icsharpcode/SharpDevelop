// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.SharpDevelop.Project;
using System;

namespace ICSharpCode.WixBinding
{
	/// <summary>
	/// Base class for all Wix compiler extension project items.
	/// </summary>
	public abstract class WixExtensionProjectItem : ProjectItem
	{
		public WixExtensionProjectItem(IProject project) : base(project)
		{
		}
		
		public override ItemType ItemType {
			get {
				return ItemType.None;
			}
		}
		
		/// <summary>
		/// Gets or sets the Wix extension class name. 
		/// </summary>
		/// <remarks>
		/// This is the fully qualified class name.
		/// </remarks>
		public string ClassName {
			get {
				return base.Properties["Class"];
			}
			set {
				base.Properties["Class"] = value;
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
		
		public override ProjectItem Clone()
		{
			ProjectItem n = CreateNewInstance(Project);
			n.Include = Include;
			this.CopyExtraPropertiesTo(n);
			return n;
		}
		
		/// <summary>
		/// Derived Wix compiler extensions need to create a new instance of
		/// themselves when this method is called. This helps to have one
		/// common Clone method.
		/// </summary>
		protected abstract ProjectItem CreateNewInstance(IProject project);
	}
}
