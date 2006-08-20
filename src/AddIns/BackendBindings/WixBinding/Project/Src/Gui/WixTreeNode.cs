// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using System;
using System.Collections.Specialized;
using System.Xml;

namespace ICSharpCode.WixBinding
{
	/// <summary>
	/// Base class for all Wix tree nodes.
	/// </summary>
	public class WixTreeNode : ExtTreeNode, IOwnerState
	{
		XmlElement element;
		
		public WixTreeNode(XmlElement element)
		{
			this.element = element;
		}
		
		public Enum InternalState {
			get {
				return WixPackageFilesTreeView.InternalState;
			}
		}
		
		/// <summary>
		/// Can delete all Wix tree nodes.
		/// </summary>
		public override bool EnableDelete {
			get {
				return true;
			}
		}
		
		public override void Delete()
		{
			RemoveElementCommand command = new RemoveElementCommand();
			command.Run();
		}
		
		/// <summary>
		/// Gets the XmlElement associated with this tree node.
		/// </summary>
		public XmlElement XmlElement {
			get {
				return element;
			}
		}
		
		public WixPackageFilesTreeView WixPackageFilesTreeView {
			get {
				return (WixPackageFilesTreeView)TreeView;
			}
		}
	}
}
