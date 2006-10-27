// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Luc Morin" email=""/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Forms;
using ICSharpCode.SharpDevelop.Gui;
using SharpQuery.Gui.TreeView;

//TODO : dans les SharpQueryList faire correspondre les restrictions vec les objets ajoutés
//TODO : dans les SharpQueryList faire correspondre les dataconnection avec les objets ajoutés
//TODO : ajout statistiques.

namespace SharpQuery.Pads
{
	/// <summary>
	/// This Pad Show a tree where you can add/remove databases connections.
	/// You can administrate databases from this tree.
	/// </summary>
	public class SharpQueryPad : AbstractPadContent
	{
		private static SharpQueryTree sharpQueryTree = null;
		#region AbstractPadContent requirements
		/// <summary>
		/// The <see cref="System.Windows.Forms.Control"/> representing the pad
		/// </summary>
		public override Control Control
		{
			get
			{
				return sharpQueryTree;
			}
		}

		/// <summary>
		/// Creates a new SharpQueryPad object
		/// </summary>
		public SharpQueryPad()
		{
			CreateDefaultSharpQuery();
			sharpQueryTree.Dock = DockStyle.Fill;
		}

		void CreateDefaultSharpQuery()
		{
			sharpQueryTree = new SharpQueryTree();
		}

		public void SaveSharpQueryView()
		{
		}

		/// <summary>
		/// Refreshes the pad
		/// </summary>
		public override void RedrawContent()
		{
			//			OnTitleChanged(null);
			//			OnIconChanged(null);
			sharpQueryTree.Refresh();
		}

		/// <summary>
		/// Cleans up all used resources
		/// </summary>
		public override void Dispose()
		{
			this.SaveSharpQueryView();
			sharpQueryTree.Dispose();
		}
		#endregion
	}
}
