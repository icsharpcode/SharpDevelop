/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 24.04.2013
 * Time: 19:53
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Drawing;

namespace ICSharpCode.Reporting.Globals
{
	/// <summary>
	/// Description of CreateGraphics.
	/// </summary>
	public class CreateGraphics
	{
		public static Graphics FromSize (Size size){
			if (size == null) {
				throw new ArgumentNullException("size");
			}
			Bitmap bitmap = new Bitmap(size.Width,size.Height);
			var graphics = Graphics.FromImage(bitmap);
			return graphics;
		}
	}
}
