/*
 * Created by SharpDevelop.
 * User: Omnibrain
 * Date: 20.01.2005
 * Time: 16:07
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Drawing;

namespace ICSharpCode.SharpDevelop.Dom.NRefactoryResolver
{
	/// <summary>
	/// Description of SearchDefinition.
	/// </summary>
	public class FilePosition
	{
		string filename = "";
		Point position = new Point(0, 0);
		
		public FilePosition(string filename, Point position)
		{
			this.filename = filename;
			this.position = position;
		}
	}
}
