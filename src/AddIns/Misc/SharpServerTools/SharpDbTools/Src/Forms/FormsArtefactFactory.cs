// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Dickon Field" email=""/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Forms;

namespace SharpDbTools.Forms
{
	/// <summary>
	/// Description of FormsArtefactFactory.
	/// </summary>
	public abstract class FormsArtefactFactory
	{
		public FormsArtefactFactory()
		{
		}
		
		public abstract TreeNode CreateMetaDataNode(string name);
		public abstract string[] GetDescribeTableFieldNames();
		public abstract string[] GetDescribeTableColumnHeaderNames();
	}
}
