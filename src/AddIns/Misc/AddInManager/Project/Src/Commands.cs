/*
 * Created by SharpDevelop.
 * User: Daniel Grunwald
 * Date: 26.11.2005
 * Time: 14:53
 */

using System;
using ICSharpCode.Core;

namespace ICSharpCode.AddInManager
{
	public class ShowCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			ManagerForm.ShowForm();
		}
	}
}
