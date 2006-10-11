/*
 * User: dickon
 * Date: 17/09/2006
 * Time: 09:10
 * 
 */

using System;
using System.Windows.Forms;
using System.Data;

using SharpDbTools.Forms;
using SharpDbTools.Data;

using SharpServerTools.Forms;

using ICSharpCode.Core;

namespace SharpDbTools.Oracle.Forms
{
	/// <summary>
	/// Description of MetaDataNodeBuilder.
	/// TODO: currently this is just a flat list - need to reflect ownership
	/// relationships such as schema etc
	/// </summary>
	public class OracleFormsArtefactFactory : FormsArtefactFactory
	{
		public OracleFormsArtefactFactory()
		{
		}
		
		public override TreeNode CreateMetaDataNode(string logicalConnectionName)
		{
			LoggingService.Debug(this.GetType().ToString() 
			                     + ": creating MetaDataNode for: " + logicalConnectionName);
			// create root node of the metadata collections tree
			
			TreeNode metaNode = new TreeNode("Db Objects");
			metaNode.Name = logicalConnectionName + ":MetaData";
			
			// retrieve the metadata for this logical connection name
			
			DbModelInfo info = DbModelInfoService.GetDbModelInfo(logicalConnectionName);
			
			// retrieve the table listing the metadata collections
			
			DataTable metadataCollectionsTable = info.Tables[TableNames.MetaDataCollections];
			
			// if it exists then populate the tree
			
			if (metadataCollectionsTable != null) {
				LoggingService.Debug(this.GetType().ToString() + ": found metadata collections table, " +
				                     " building node...");
				for (int i = 0; i < TableNames.PrimaryObjects.Length; i++) {
					string metadataCollectionName = TableNames.PrimaryObjects[i];
					LoggingService.Debug("looking for metadata: " + metadataCollectionName);
					DataTable metaCollectionTable = info.Tables[metadataCollectionName];
					LoggingService.Debug("found metadata collection: " + metadataCollectionName);
					TreeNode collectionNode = new TreeNode(metadataCollectionName);
					collectionNode.Name = logicalConnectionName + ":Collection:" + metadataCollectionName;
					metaNode.Nodes.Add(collectionNode);
					foreach (DataRow dbObjectRow in metaCollectionTable.Rows) {
						TreeNode objectNode = null;
						if (dbObjectRow.ItemArray.Length > 1) {
							
							// if it is a table metadata collection then create a node
							// with the option to invoke the DescribeTableViewContent
							
							if (metadataCollectionName.Equals("Tables")) {
								objectNode = new TableTreeNode((string)dbObjectRow[1], logicalConnectionName);
							} else {
								
								// TODO: describe other metadata collections
								
								objectNode = new TreeNode((string)dbObjectRow[1]);	
							}
							objectNode.Name = logicalConnectionName + ":Object:" + (string)dbObjectRow[1];
						} else {
							objectNode = new TreeNode((string)dbObjectRow[0]);
							objectNode.Name = logicalConnectionName + ":Object:" + (string)dbObjectRow[0];
						}
						collectionNode.Nodes.Add(objectNode);
					}
				}	
			}
			return metaNode;
		}
	}
	
}
