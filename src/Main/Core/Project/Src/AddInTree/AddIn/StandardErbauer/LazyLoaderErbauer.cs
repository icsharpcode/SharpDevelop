using System;
using System.Collections;

namespace ICSharpCode.Core
{
	/// <summary>
	/// Description of ClassErbauer.
	/// </summary>
	public class LazyLoadErbauer : IErbauer
	{
		AddIn addIn;
		string name;
		string className;
		
		public string Name {
			get {
				return name;
			}
		}
		
		public string ClassName {
			get {
				return className;
			}
		}
		
		public LazyLoadErbauer(AddIn addIn, Properties properties)
		{
			this.addIn      = addIn;
			this.name       = properties["name"];
			this.className  = properties["class"];
			
		}
		
		public object BuildItem(object caller, Codon codon, ArrayList subItems)
		{
			IErbauer erbauer = (IErbauer)addIn.CreateObject(className);
			AddInTree.Erbauer[name] = erbauer;
			return erbauer.BuildItem(caller, codon, subItems);
		}
		
		public override string ToString() 
		{
			return String.Format("[LazyLoadErbauer: className = {0}, name = {1}]",
			                     className,
			                     name);
		}
		
	}
}
