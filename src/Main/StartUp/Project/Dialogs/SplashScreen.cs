// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using System.Reflection;
using System.Resources;

namespace ICSharpCode.SharpDevelop
{
	public class SplashScreenForm : Form
	{
		public const string VersionText = "Corsavy Beta 1, rev. " + RevisionClass.Revision;
		
		static SplashScreenForm splashScreen = new SplashScreenForm();
		static ArrayList requestedFileList = new ArrayList();
		static ArrayList parameterList = new ArrayList();
		Bitmap bitmap;
		
		public static SplashScreenForm SplashScreen {
			get {
				return splashScreen;
			}
		}
		
		public SplashScreenForm()
		{
			#if !DEBUG
			TopMost         = true;
			#endif
			FormBorderStyle = FormBorderStyle.None;
			StartPosition   = FormStartPosition.CenterScreen;
			ShowInTaskbar   = false;
			#if DEBUG
			string versionText = VersionText + " (debug)";
			#else
			string versionText = VersionText;
			#endif
			using (Stream stream = Assembly.GetEntryAssembly().GetManifestResourceStream("Resources.SplashScreen.jpg")) {
				bitmap = new Bitmap(stream);
			}
			this.ClientSize = bitmap.Size;
			using (Font font = new Font("Sans Serif", 4)) {
				using (Graphics g = Graphics.FromImage(bitmap)) {
					g.DrawRectangle(Pens.Black, 0, 0, bitmap.Size.Width - 1, bitmap.Size.Height - 1);
					g.DrawString(versionText, font, Brushes.Black, 106, 142);
				}
			}
			BackgroundImage = bitmap;
		}
		
		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				if (bitmap != null) {
					bitmap.Dispose();
					bitmap = null;
				}
			}
			base.Dispose(disposing);
		}
		
		public static string[] GetParameterList()
		{
			return GetStringArray(parameterList);
		}
		
		public static string[] GetRequestedFileList()
		{
			return GetStringArray(requestedFileList);
		}
		
		static string[] GetStringArray(ArrayList list)
		{
			return (string[])list.ToArray(typeof(string));
		}
		
		public static void SetCommandLineArgs(string[] args)
		{
			requestedFileList.Clear();
			parameterList.Clear();
			
			foreach (string arg in args) {
				if (arg[0] == '-' || arg[0] == '/') {
					int markerLength = 1;
					
					if (arg.Length >= 2 && arg[0] == '-' && arg[1] == '-') {
						markerLength = 2;
					}
					
					parameterList.Add(arg.Substring(markerLength));
				} else {
					requestedFileList.Add(arg);
				}
			}
		}
	}
}
