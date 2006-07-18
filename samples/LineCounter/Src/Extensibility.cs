/*
 * Created by SharpDevelop.
 * User: Daniel Grunwald
 * Date: 18.07.2006
 * Time: 17:08
 */

using System;
using ICSharpCode.Core;

namespace LineCounterAddin
{
	// Improvement 2
	
	public interface ICountingAlgorithm
	{
		void CountLines(LineCountInfo info);
	}

	public class CountingAlgorithmGeneric : ICountingAlgorithm {
		public void CountLines(LineCountInfo info) {
			LineCounterBrowser.CountLinesGeneric(info);
		}
	}
	public class CountingAlgorithmCStyle : ICountingAlgorithm {
		public void CountLines(LineCountInfo info) {
			LineCounterBrowser.CountLinesCStyle(info);
		}
	}
	public class CountingAlgorithmVBStyle : ICountingAlgorithm {
		public void CountLines(LineCountInfo info) {
			LineCounterBrowser.CountLinesVBStyle(info);
		}
	}
	public class CountingAlgorithmXmlStyle : ICountingAlgorithm {
		public void CountLines(LineCountInfo info) {
			LineCounterBrowser.CountLinesXMLStyle(info);
		}
	}

	public class CountingAlgorithmDoozer : IDoozer
	{
		public bool HandleConditions {
			get {
				// our doozer cannot handle conditions, let SharpDevelop
				// do that for us
				return false;
			}
		}
		
		public object BuildItem(object caller, Codon codon, System.Collections.ArrayList subItems)
		{
			return new CountingAlgorithmDescriptor(codon.AddIn,
			                                       codon.Properties["extensions"],
			                                       codon.Properties["class"]);
		}
	}

	public class CountingAlgorithmDescriptor
	{
		AddIn addIn;
		internal string[] extensions;
		string className;
		
		public CountingAlgorithmDescriptor(AddIn addIn, string extensions, string className)
		{
			this.addIn = addIn;
			this.extensions = extensions.ToLowerInvariant().Split(';');
			this.className = className;
		}
		
		public bool CanCountLines(LineCountInfo info)
		{
			return (Array.IndexOf(extensions, info.FileType.ToLowerInvariant()) >= 0);
		}
		
		ICountingAlgorithm cachedAlgorithm;
		
		public ICountingAlgorithm GetAlgorithm()
		{
			if (cachedAlgorithm == null) {
				cachedAlgorithm = (ICountingAlgorithm)addIn.CreateObject(className);
			}
			return cachedAlgorithm;
		}
	}
}
