// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
		
		public virtual string WrapInCanvas {
			get {
				return "Wrap in Canvas";
			}
		}
		
		public virtual string WrapInGrid {
			get {
				return "Wrap in Grid";
			}
		}
		
		public virtual string WrapInBorder {
			get {
				return "Wrap in Border";
			}
		}
	}
}
