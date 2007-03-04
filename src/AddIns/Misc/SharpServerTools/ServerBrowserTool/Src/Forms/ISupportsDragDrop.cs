/*
 * Created by SharpDevelop.
 * User: dickon
 * Date: 04/03/2007
 * Time: 09:14
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Windows.Forms;

namespace SharpServerTools.Forms
{
	/// <summary>
	/// This interface is implemented by any plugin to ServerExplorer that supports drag and drop
	/// of some of its data
	/// </summary>
	public interface ISupportsDragDrop
	{
		void HandleMouseDownEvent(object sender, MouseEventArgs e);
	}
}
