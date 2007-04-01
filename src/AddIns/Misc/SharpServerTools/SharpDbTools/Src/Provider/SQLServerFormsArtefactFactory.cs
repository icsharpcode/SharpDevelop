// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Dickon Field" email=""/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Data;
using System.Windows.Forms;

using ICSharpCode.Core;
using SharpDbTools.Data;
using SharpDbTools.Forms;

namespace SharpDbTools.SQLServer.Forms
{
	/// <summary>
	/// Creates a TreeNode that displays the metadata for a SQLServer database
	/// Uses:
	/// <item>DbModelInfo and DbModelInfoService: to access the metadata</item>
	/// <item>TableTreeNode: to display Table metadata - this has 'Describe' 
	/// behaviour associated with it, accessed via a right mouse menu</item>
	/// Going forward this should reflect the structure and relationship
	/// of SQLServer objects - for now it is generic
	/// </summary>
	public class SQLServerFormsArtefactFactory : FormsArtefactFactory
	{
		public SQLServerFormsArtefactFactory()
		{
		}
		
		public override TreeNode CreateMetaDataNode(string logicalConnectionName)
		{
			LoggingService.Debug(this.GetType().ToString() 
			                     + ": creating MetaDataNode for: " + logicalConnectionName);
			// create root node of the metadata collections tree
			
			string nodeName = ResourceService.GetString("SharpDbTools.Forms.DbObjectNodeName");
			TreeNode metaNode = new TreeNode(nodeName);
			
			// retrieve the metadata for this logical connection name
			
			DbModelInfo info = DbModelInfoService.GetDbModelInfo(logicalConnectionName);
			
			// retrieve the table listing the metadata collections
			
			DataTable metadataCollectionsTable = info.Tables[MetadataNames.MetaDataCollections];
			
			// if it exists then populate the tree
			
			if (metadataCollectionsTable != null) {
				LoggingService.Debug(this.GetType().ToString() + ": found metadata collections table, " +
				                     " building node...");
				for (int i = 0; i < MetadataNames.PrimaryObjects.Length; i++) {
					string metadataCollectionName = MetadataNames.PrimaryObjects[i];
					LoggingService.Debug("looking for metadata: " + metadataCollectionName);
					DataTable metaCollectionTable = info.Tables[metadataCollectionName];
					if (metaCollectionTable == null) continue;
					LoggingService.Debug("found metadata collection: " + metadataCollectionName);
					string nodeDisplayNameKey = "SharpDbTools.Data.PrimaryObjects." + metadataCollectionName;
					string nodeDisplayName = ResourceService.GetString(nodeDisplayNameKey);
					TreeNode collectionNode = new TreeNode(nodeDisplayName);
					metaNode.Nodes.Add(collectionNode);
					
					if (metaCollectionTable != null) {
						foreach (DataRow dbObjectRow in metaCollectionTable.Rows) {
							TreeNode objectNode = null;
							
							// if there is only one field in the metadata table then it is almost certainly
							// the name of the item - so if not we need to then figure out what it is
							if (dbObjectRow.ItemArray.Length > 1) {
								
								// if it is a table metadata collection then create a node
								// with the option to invoke the DescribeTableViewContent -
								// that's what a TableTreeNode gives us right now
								// TODO: provide describe functions amongst others for
								// other metadata types
								
								switch (metadataCollectionName) {
									case "Tables":
										objectNode = new TableTreeNode((string)dbObjectRow[2], logicalConnectionName);	
										break;
									case "Functions":
										// do nothing - there are no functions in SQLServer
										break;
									case "Users":
										objectNode = new TreeNode((string)dbObjectRow[1]);
										break;
									default:
										objectNode = new TreeNode((string)dbObjectRow[2]);
										break;
								}
							} else {
								objectNode = new TreeNode((string)dbObjectRow[0]);
							}
							collectionNode.Nodes.Add(objectNode);
						}
					}
				}	
			}
			return metaNode;
		}
		
		public override string[] GetDescribeTableFieldNames() 
		{
				return tableFieldsToDisplay;
		}
		public override string[] GetDescribeTableColumnHeaderNames() 
		{
				return tableFieldsColumnHeaders;
		}
		
		private static string[] tableFieldsToDisplay = 
			new string [] {"COLUMN_NAME", "DATA_TYPE", 
			"CHARACTER_OCTET_LENGTH", "NUMERIC_PRECISION", "NUMERIC_SCALE", "IS_NULLABLE"};
		private static string[] tableFieldsColumnHeaders =
			new string[] { "Column", "Type", "Length", "Precision", "Scale", "Nullable" };


	}
	
}
