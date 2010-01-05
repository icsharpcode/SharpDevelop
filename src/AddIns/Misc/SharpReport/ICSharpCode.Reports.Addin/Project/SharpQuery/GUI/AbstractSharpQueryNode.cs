// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Luc Morin" email=""/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using SharpQuery.Collections;
using SharpQuery.Connection;
using SharpQuery.Gui.DataView;
using SharpQuery.SchemaClass;

namespace SharpQuery.Gui.TreeView
{
	public abstract class AbstractSharpQueryNode : System.Windows.Forms.TreeNode, ISharpQueryNode
	{
		internal static SQLParameterInput inputform = null;

		///<summary>
		/// this variable force to have a "plus" near the node.
		/// </summary>
		protected Assembly ass = null;

		protected ISchemaClass pSchemaClass = null;

		///<summary> force to displayed a "+" for the node</summary>
		protected virtual bool NullChildAllowed
		{
			get
			{
				return true;
			}
		}

		public virtual string AddinContextMenu
		{
			get
			{
				return "";
			}
		}

		public virtual string entityNormalizedName
		{
			get
			{
				if (this.SchemaClass != null)
				{
					return AbstractSharpQuerySchemaClass.RemoveBracket(this.SchemaClass.NormalizedName);
				}
				else
				{
					return "";
				}
			}
		}

		public virtual string entityName
		{
			get
			{
				if (this.SchemaClass != null)
				{
					return this.SchemaClass.InternalName;
				}
				else
				{
					return "";
				}
			}
		}

		public ISchemaClass SchemaClass
		{
			get
			{
				if (this.pSchemaClass != null)
				{
					return this.pSchemaClass;
				}
				else
				{
					return null;
				}
			}
		}

		public virtual IConnection Connection
		{
			get
			{
				if (this.SchemaClass != null)
				{
					return this.SchemaClass.Connection;
				}
				else
				{
					return null;
				}
			}
		}

		public virtual SharpQueryListDictionary Entities
		{
			get
			{
				if (this.SchemaClass != null)
				{
					return this.SchemaClass.Entities;
				}
				else
				{
					return null;
				}
			}
		}

		public AbstractSharpQueryNode()
			: base()
		{
			ass = System.Reflection.Assembly.GetExecutingAssembly();
		}

		public AbstractSharpQueryNode(ISchemaClass schemaclass)
			: this()
		{
			this.pSchemaClass = schemaclass;
		}

		///<summary>
		/// called by <see cref=".Refresh()">Refresh</see> just after the <see cref=".Clear()">Clear</see> and before <see cref=".Refresh()">childs'refresh</see>.
		/// In this, you could change the <see cref=".Entities">Entities dicntionnary.</see>
		///</summary>
		protected virtual void OnRefresh()
		{
			// Nothing !
		}

		public virtual void Refresh()
		{
			try
			{
				if (this.TreeView != null)
				{
					this.TreeView.BeginUpdate();
				}

				this.Clear();

				this.OnRefresh();

				if (this.Connection.IsOpen)
				{
					this.Text = this.entityName;

					if (this.IsExpanded == true)
					{
						if (this.SchemaClass != null)
						{
							this.SchemaClass.Refresh();
						}

						this.BuildsChilds();
					}
				}
			}
			finally
			{
				if (this.TreeView != null)
				{
					this.TreeView.EndUpdate();
				}
			}
		}

		public virtual void Clear()
		{
			if (this.SchemaClass != null)
			{
				this.SchemaClass.Clear();
			}

			this.Nodes.Clear();

			if ((this.IsExpanded == false) && (this.NullChildAllowed == true))
			{
				this.Nodes.Add(new TreeNode());
			}
		}

		///<summary>
		/// allow the user to add some parameters while executing an SQL command
		/// </summary>
		protected virtual SharpQuerySchemaClassCollection OnExecute(CancelEventArgs e)
		{
			return null;
		}

		///<summary>
		/// For a Table or a View extract data.
		/// For a stocked procedure, execute it :o).
		/// <param name="rows">Number of row to extract. if "0", extract all rows.</param>
		/// </summary>
		public void Execute(int rows)
		{
			try
			{
				if (this.SchemaClass != null)
				{
					CancelEventArgs e = new CancelEventArgs();
					SharpQuerySchemaClassCollection ret = this.OnExecute(e);
					if (e.Cancel == false)
					{
						WorkbenchSingleton.Workbench.ShowView(new SharpQueryDataView(this.SchemaClass, rows, ret));
					}
				}
			}
			catch (Exception e)
			{
				MessageService.ShowError(e.Message);
			}
		}

		public virtual void BuildsChilds()
		{
			string childclass = "";
			ISharpQueryNode ChildNode = null;

			if (this.Entities != null)
			{
				foreach ( KeyValuePair<string, SharpQuerySchemaClassCollection> DicEntry in this.Entities)
				{
					if (DicEntry.Value != null)
					{
						SharpQuerySchemaClassCollection entitieslist = DicEntry.Value as SharpQuerySchemaClassCollection;

						foreach (ISchemaClass entity in entitieslist)
						{
							childclass = SharpQueryTree.SchemaClassDict[entity.GetType().FullName];
							if ((childclass != null) && (childclass != ""))
							{
								ChildNode = (ISharpQueryNode)ass.CreateInstance(childclass, false, BindingFlags.CreateInstance, null, new object[] { entity }, null, null);
								if (ChildNode != null)
								{
									bool addNode = true;

									if (ChildNode is SharpQueryNodeNotSupported)
									{
										addNode = this.ShowUnsupported();
									}

									if (addNode == true)
									{
										this.Nodes.Add(ChildNode as TreeNode);
										ChildNode.Refresh();
									}
								}
							}
						}
					}
				}
			}
		}

		protected bool ShowUnsupported()
		{
			//			AddInTreeNode AddinNode;
			bool ret = true;

			//			AddinNode = AddInTree.GetTreeNode("/SharpQuery/Connection");
			//			foreach ( DictionaryEntry entryChild in AddinNode.ChildNodes)
			//			{
			//				AddInTreeNode ChildNode = entryChild.Value as AddInTreeNode;
			//				if ( ChildNode != null )
			//				{
			//					SharpQueryConnectionCodon codon = ChildNode.Codon as SharpQueryConnectionCodon;
			//					if ( codon != null )
			//					{
			//						if ( codon.Node == this.GetType().FullName )
			//						{
			//							ret = bool.Parse( codon.ShowUnsuported );
			//						}
			//					}
			//				}
			//			}

			return ret;
		}

	}

}
