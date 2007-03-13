// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Dickon Field" email=""/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Forms;
using ICSharpCode.Core;

namespace SharpDbTools.Forms
{
	/// <summary>
	/// Base class for one of the classes that each SharpDbTools plugin must support. Subclasses
	/// provide the UI artefacts required to build the metadata node for a particular
	/// datasource such as Oracle, SQLServer, MySQL etc.
	/// It makes sense to have a separate derived class for each datasource since the structure and
	/// relationship of db objects supported by each server is quite different, and therefore merits
	/// quite a different presentation and layout in the metadata tree.
	/// </summary>
	public abstract class FormsArtefactFactory
	{
		public FormsArtefactFactory()
		{
		}
		
		static FormsArtefactFactory()
		{
			ResourceService.RegisterStrings("SharpDbTools.Resources.Strings", typeof(FormsArtefactFactory).Assembly);
		}
		
		public abstract TreeNode CreateMetaDataNode(string name);
		public abstract string[] GetDescribeTableFieldNames();
		public abstract string[] GetDescribeTableColumnHeaderNames();
	}
}
