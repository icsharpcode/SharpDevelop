// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Luc Morin" email=""/>
//     <version>$Revision$</version>
// </file>

using System;
using System.ComponentModel;
using System.Windows.Forms;

using ICSharpCode.SharpDevelop.Gui;
using SharpQuery.Collections;
using SharpQuery.Gui.DataView;
using SharpQuery.SchemaClass;

namespace SharpQuery.Gui.TreeView
{
	///<summary>
	/// Column Node
	///</summary>
	public class SharpQueryNodeColumn : AbstractSharpQueryNode
	{
		///<summary> force to displayed a "+" for the node</summary>
		protected override bool NullChildAllowed
		{
			get
			{
				return false;
			}
		}

		public override string AddinContextMenu
		{
			get
			{
				return "/SharpQuery/ContextMenu/Column";
			}
		}

		public SharpQueryNodeColumn(SharpQueryColumn sharpQueryColumn)
			: base(sharpQueryColumn)
		{
			this.ImageIndex = 9;
			this.SelectedImageIndex = 9;
		}
	}

	///<summary>
	/// Parameter Node
	///</summary>
	public class SharpQueryNodeParameter : AbstractSharpQueryNode
	{

		///<summary> force to displayed a "+" for the node</summary>
		protected override bool NullChildAllowed
		{
			get
			{
				return false;
			}
		}

		public override string AddinContextMenu
		{
			get
			{
				return "/SharpQuery/ContextMenu/Parameter";
			}
		}

		public SharpQueryNodeParameter(SharpQueryParameter sharpQueryParameter)
			: base(sharpQueryParameter)
		{
			this.ImageIndex = 9;
			this.SelectedImageIndex = 9;
		}
	}

	///<summary>
	/// Table Node
	///</summary>
	public class SharpQueryNodeTable : AbstractSharpQueryNode
	{
		public override string AddinContextMenu
		{
			get
			{
				return "/SharpQuery/ContextMenu/Table";
			}
		}

		public SharpQueryNodeTable(SharpQueryTable sharpQueryTable)
			: base(sharpQueryTable)
		{
			this.ImageIndex = 6;
			this.SelectedImageIndex = 6;
		}
	}

	///<summary>
	/// View Node
	///</summary>
	public class SharpQueryNodeView : AbstractSharpQueryNode
	{
		public override string AddinContextMenu
		{
			get
			{
				return "/SharpQuery/ContextMenu/View";
			}
		}

		public SharpQueryNodeView(SharpQueryView sharpQueryView)
			: base(sharpQueryView)
		{
			this.ImageIndex = 7;
			this.SelectedImageIndex = 7;
		}
	}

	///<summary>
	/// Procedure Node
	///</summary>
	public class SharpQueryNodeProcedure : AbstractSharpQueryNode
	{

		public override string AddinContextMenu
		{
			get
			{
				return "/SharpQuery/ContextMenu/Procedure";
			}
		}

		public SharpQueryNodeProcedure(SharpQueryProcedure sharpQueryProcedure)
			: base(sharpQueryProcedure)
		{
			this.ImageIndex = 8;
			this.SelectedImageIndex = 8;
		}

		///<summary>
		/// allow the user to add some parameters while executing an SQL command
		/// </summary>
		protected override SharpQuerySchemaClassCollection OnExecute(CancelEventArgs e)
		{
			SharpQuerySchemaClassCollection tmp = this.SchemaClass.GetSchemaParameters();
			SharpQueryParameterCollection parameters = null;
			SharpQuerySchemaClassCollection returnValue = null;

			if (tmp.Count == 1 && tmp[0] is SharpQueryNotSupported)
			{
				parameters = new SharpQueryParameterCollection();
			}
			else
			{
				parameters = new SharpQueryParameterCollection();
				foreach (SharpQueryParameter par in tmp)
					parameters.Add(par);
			}

			if (parameters != null && parameters.Count > 0)
			{
				inputform = new SQLParameterInput(parameters);
				inputform.Owner = (Form)WorkbenchSingleton.Workbench;

				if (inputform.ShowDialog() != DialogResult.OK)
				{
					returnValue = null;
					e.Cancel = true;
				}
				else
				{
					returnValue = new SharpQuerySchemaClassCollection();
					foreach (SharpQueryParameter par in parameters)
					{
						returnValue.Add(par);
					}
				}
			}

			return returnValue;
		}
	}

	///<summary>
	/// Node displayed when a function is not supported by the provider!
	///</summary>
	public class SharpQueryNodeNotSupported : AbstractSharpQueryNode
	{

		///<summary> force to displayed a "+" for the node</summary>
		protected override bool NullChildAllowed
		{
			get
			{
				return false;
			}
		}

		public SharpQueryNodeNotSupported(SharpQueryNotSupported sharpQueryNotSupported)
			: base(sharpQueryNotSupported)
		{
			this.ImageIndex = 10;
			this.SelectedImageIndex = 10;
		}
	}


	///<summary>
	/// Schema Node
	///</summary>
	public class SharpQueryNodeSchema : AbstractSharpQueryNode
	{
		public override string AddinContextMenu
		{
			get
			{
				return "/SharpQuery/ContextMenu/Schema";
			}
		}

		public SharpQueryNodeSchema(SharpQuerySchema schema)
			: base(schema)
		{
			this.ImageIndex = 1;
			this.SelectedImageIndex = 1;
		}

	}

	///<summary>
	/// Catalog Node
	///</summary>
	public class SharpQueryNodeCatalog : AbstractSharpQueryNode
	{
		public override string AddinContextMenu
		{
			get
			{
				return "/SharpQuery/ContextMenu/Catalog";
			}
		}

		public SharpQueryNodeCatalog(SharpQueryCatalog catalog)
			: base(catalog)
		{
			this.ImageIndex = 1;
			this.SelectedImageIndex = 1;
		}
	}


}
