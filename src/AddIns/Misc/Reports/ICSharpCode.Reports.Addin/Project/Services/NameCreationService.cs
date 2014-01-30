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
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Diagnostics;

namespace ICSharpCode.Reports.Addin
{
    
	public class NameCreationService : INameCreationService
	{
		public NameCreationService()
		{
		}

		string INameCreationService.CreateName(IContainer container, Type type)
		{
			
			ComponentCollection cc = container.Components;
			int min = Int32.MaxValue;
			int max = Int32.MinValue;
			int count = 0;

			for (int i = 0; i < cc.Count; i++)
			{
				Component comp = cc[i] as Component;
				if (comp.GetType() == type)
				{
					count++;

					string name = comp.Site.Name;
					if(name.StartsWith(type.Name,StringComparison.InvariantCultureIgnoreCase))
					{
						try
						{
							int value = Int32.Parse(name.Substring(type.Name.Length));

							if (value < min)
								min = value;

							if (value > max)
								max = value;
						}
                        catch (Exception ex)
                        {
                            Trace.WriteLine(ex.ToString());
                        }
                    }
				}
			}// for

			if (count == 0)
				return type.Name + "1";

			else if (min > 1)
			{
				int j = min - 1;

				return type.Name + j.ToString();
			}
			else 
			{
				int j = max + 1;

				return type.Name + j.ToString();
			}
		}
		
		bool INameCreationService.IsValidName(string name)
		{
			return true;
		}
		void INameCreationService.ValidateName(string name)
		{
			return;
		}

	}// class
}// namespace
