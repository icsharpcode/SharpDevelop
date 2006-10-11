/*
 * User: dickon
 * Date: 17/09/2006
 * Time: 23:47
 * 
 */

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
	}
}
