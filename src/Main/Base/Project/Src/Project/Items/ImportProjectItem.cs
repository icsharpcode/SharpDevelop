/*
 * Created by SharpDevelop.
 * User: Daniel Grunwald
 * Date: 19.07.2005
 * Time: 23:34
 */

using System;

namespace ICSharpCode.SharpDevelop.Project
{
	/// <summary>
	/// Description of ImportProjectItem.
	/// </summary>
	public class ImportProjectItem : ProjectItem
	{
		public ImportProjectItem(IProject project) : base(project)
		{
		}
		
		public override ItemType ItemType {
			get {
				return ItemType.Import;
			}
		}
	}
}
