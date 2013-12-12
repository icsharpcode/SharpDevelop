// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.WpfDesign.Designer
{
	/// <summary>
	/// Description of Translations.
	/// </summary>
	public class Translations
	{
		private static Translations _instance;
		public static Translations Instance { 
			get {
				if (_instance == null)
					_instance = new Translations();
				return _instance; 
			} protected set {
				_instance = value;
			}
		}
		
		public virtual string SendToFrontText {
			get {
				return "Bring to front";
			}
		}
		
		public virtual string SendForward {
			get {
				return "Forward";
			}
		}
		
		public virtual string SendBackward {
			get {
				return "Backward";
			}
		}
		
		public virtual string SendToBack {
			get {
				return "Send to back";
			}
		}
		
		public virtual string PressAltText {
			get {
				return "Press \"Alt\" to Enter Container";
			}
		}
	}
}
