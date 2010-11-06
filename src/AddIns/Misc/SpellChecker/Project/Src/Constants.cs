// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using System.Reflection;

using ICSharpCode.Core;
using ICSharpCode.SpellChecker.Engine;
using NHunspell;

namespace ICSharpCode.SpellChecker
{
	/// <summary>
	/// Description of Constants.
	/// </summary>
	internal static class Constants
	{
		private static Hunspell currentSpell;
		
		private static object lockObject = new object();
		
		static Constants()
		{
			if (NHunspell.Hunspell.NativeDllPath == Constants.NHunpellDefaultPath)
				NHunspell.Hunspell.NativeDllPath = @"..\AddIns\Misc\SpellChecker\";
		
			CreateHunspell();
		}
		
		internal static void CreateHunspell() {
			
			string language = DefaultLanguage;
			
			lock(lockObject) {
				string aff = DictionariesPath + language + @"\" + language.Replace("-", "_") + ".aff";
				string dic = DictionariesPath + language + @"\" + language.Replace("-", "_") + ".dic";
				
				if (currentSpell == null) {
					currentSpell = new Hunspell(aff, dic);
				}
				else {
					currentSpell.Dispose();
					currentSpell = null;
					
					currentSpell = new Hunspell(aff, dic);
				}
			}
		}
		
		internal static Hunspell CurrentHunspell {
			get {
				return currentSpell; 
			}
		}
		
		internal static string DictionariesPath {
			get { return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\Dictionaries\"; }
		}
		
		internal static string NHunpellDefaultPath {
			get {
				return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, 
				             AppDomain.CurrentDomain.RelativeSearchPath ?? string.Empty);
			}
		}
		
		internal static string DefaultLanguage {
			get {
				if (string.IsNullOrEmpty(PropertyService.Get("SpellCheckerLanguage")))
					return "en-US";
				return PropertyService.Get("SpellCheckerLanguage");
			}
			set
				{
				PropertyService.Set("SpellCheckerLanguage", value);
			}
		}
			
	}
}
