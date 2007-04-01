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

namespace SharpDbTools.SQLite.Forms
{
	/// <summary>
	/// Description of MetaDataNodeBuilder.
	/// TODO: currently this is just a flat list - need to reflect ownership
	/// relationships such as schema etc
	/// </summary>
	public class SQLiteFormsArtefactFactory : FormsArtefactFactory
	{
		public SQLiteFormsArtefactFactory()
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
					collectionNode.Name = logicalConnectionName + ":Collection:" + metadataCollectionName;
					metaNode.Nodes.Add(collectionNode);
					foreach (DataRow dbObjectRow in metaCollectionTable.Rows) {
						TreeNode objectNode = null;
						switch(metadataCollectionName) {
						       case "Tables":
								//LoggingService.Debug("found table row");
						       	objectNode = new TableTreeNode((string)dbObjectRow[2], logicalConnectionName);
						       	break;
						       default:
						       	objectNode = new TreeNode((string)dbObjectRow[2]);
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
