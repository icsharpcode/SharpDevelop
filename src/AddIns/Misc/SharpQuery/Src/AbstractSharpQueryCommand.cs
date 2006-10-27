// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Luc Morin" email=""/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.Core;
using SharpQuery.Gui.TreeView;

namespace SharpQuery.Commands
{
	/// <summary>
	/// Base class of all commands of SharpQuery Addin
	/// </summary>
	public abstract class AbstractSharpQueryCommand : AbstractMenuCommand
	{
		protected ISharpQueryNode sharpQueryNode = null;

		/// <summary>
		/// get the selected <see cref="SharpQuery.Gui.TreeView.ISharpQueryNode"> SharpQuery node </see>
		/// and Enabled or disabled the command
		/// <remarks> If the selected node is <code>null</code> or this is not a <see cref="SharpQuery.Gui.TreeView.ISharpQueryNode"> SharpQuery node </see>, return <code>false</code> (disable the menu)</remarks>
		/// </summary>
		public override bool IsEnabled
		{
			get
			{
				SharpQueryTree sharpQueryTree;
				sharpQueryTree = this.Owner as SharpQueryTree;

				if ((sharpQueryTree != null) && (sharpQueryTree.SelectedNode != null))
				{
					this.sharpQueryNode = sharpQueryTree.SelectedNode as ISharpQueryNode;
				}
				else
				{
					this.sharpQueryNode = null;
				}

				return (this.sharpQueryNode != null);
			}
			set { }

		}

		/// <summary>
		/// Create a new SharpQueryCommand
		/// </summary>
		public AbstractSharpQueryCommand()
			: base()
		{
		}

	}
}
