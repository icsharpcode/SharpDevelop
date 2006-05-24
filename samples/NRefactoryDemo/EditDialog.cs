/*
 * Created by SharpDevelop.
 * User: Daniel Grunwald
 * Date: 07.05.2006
 * Time: 19:57
 */

using System;
using System.Drawing;
using System.Windows.Forms;

namespace NRefactoryDemo
{
	public partial class EditDialog
	{
		public EditDialog(object element)
		{
			InitializeComponent();
			propertyGrid.SelectedObject = element;
		}
	}
}
