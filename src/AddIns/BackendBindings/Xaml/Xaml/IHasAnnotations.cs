using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ICSharpCode.Xaml
{
	public interface IHasAnnotations
	{
		void AnnotateWith<T>(T annotation) where T : class;
		T GetAnnotation<T>() where T : class;
	}
}
