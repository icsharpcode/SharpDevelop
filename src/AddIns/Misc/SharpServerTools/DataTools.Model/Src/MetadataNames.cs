// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Dickon Field" email=""/>
//     <version>$Revision: 1784 $</version>
// </file>

using System;

namespace SharpDbTools.Data
{
	/// <summary>
	/// Description of Tables.
	/// </summary>
	public sealed class MetadataNames
	{
		public const string MetaDataCollections = "MetaDataCollections";
		public const string ConnectionInfo = "ConnectionInfo";
		public static string[] PrimaryObjects = new string[] { "Tables", "Procedures",  "Functions", "Views", "Users" };
		public const string Columns = "Columns";
	}
}
