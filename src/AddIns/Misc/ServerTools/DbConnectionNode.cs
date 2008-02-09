/*
 * Created by SharpDevelop.
 * User: dickon
 * Date: 15/01/2008
 * Time: 18:15
 * 
 */

using System;
using System.Windows.Controls;

namespace ICSharpCode.ServerTools
{
	/// <summary>
	/// View element representing a connection to a db server
	/// </summary>
	public class DbConnectionNode : TreeViewItem
	{
		private TablesNode _tablesNode;
		private ViewsNode _viewsNode;
		private StoreProcsNode _storedProcsNode;
		private FunctionsNode _functionsNode;

		/// <summary>
		/// Dependent on the state this TreeViewItem should display an
		/// appropriate icon.
		/// TODO: select and embed appropriate resources from famfamfam
		/// </summary>
		private DbConnectionNodeState state;
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <param name="header"></param>
		public DbConnectionNode(string header)
		{
			this.Header = header;
			this.State = DbConnectionNodeState.Closed;
			this._tablesNode = new TablesNode();
			this._viewsNode = new ViewsNode();
			this._storedProcsNode = new StoreProcsNode();
			this._functionsNode = new FunctionsNode();
			this.Items.Add(_tablesNode);
			this.Items.Add(_viewsNode);
			this.Items.Add(_storedProcsNode);
			this.Items.Add(_functionsNode);
		}
		
		public TablesNode TablesNode {
			get {
				return _tablesNode;
			}
		}
		
		public ViewsNode ViewsNode {
			get {
				return _viewsNode;
			}
		}
		
		public StoreProcsNode StoredProcsNode {
			get {
				return _storedProcsNode;
			}
		}
		
		public FunctionsNode FunctionsNode {
			get {
				return _functionsNode;
			}
		}
		
		public DbConnectionNode(string header, DbConnectionNodeState state): this(header)
		{
			this.State = state;
		}
		
		public DbConnectionNodeState State {
			get {
				return this.state;
			}
			set {
				this.state = value;
			}
		}
	}
	
	public enum DbConnectionNodeState
	{
		Open,
		Closed
	}
}
