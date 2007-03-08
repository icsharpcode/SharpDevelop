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
						switch(metadataCollectionName) {
						       case "Tables":
								LoggingService.Debug("found table row");
						       	objectNode = new TableTreeNode((string)dbObjectRow[1], logicalConnectionName);
						       	break;
						       case "Users":
						       	LoggingService.Debug("found users row");
						       	objectNode = new TreeNode((string)dbObjectRow[0]);
						       	break;
						       default:
						       	LoggingService.Debug("found " + metadataCollectionName + " row");
						       	if (dbObjectRow.ItemArray.Length > 1) {
						       		objectNode = new TreeNode((string)dbObjectRow[1]);
						       	} else {
						       		objectNode = new TreeNode((string)dbObjectRow[0]);
						       	}
						       	break;
						}
						collectionNode.Nodes.Add(objectNode);
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
			new string [] {"COLUMN_NAME", "DATATYPE", 
			"LENGTH", "PRECISION", "SCALE", "NULLABLE"};
		private static string[] tableFieldsColumnHeaders =
			new string[] { "Column", "Type", "Length", "Precision", "Scale", "Nullable" };


	}
	
}
