namespace InheritStatements
{
	class A<T> : B where T : B
	{
		A() : base()
		{
			base();
		}
	}

	class C<T> 
		: D where T 
		: D
	{
		C()
			: this(this.ToString())
		{
			throw this;
		}

		C(object c)
			: base()
		{
			base();
		}
	}

	class E<T> : 
	F 
		where T : F
	{
		E() : 
		base()
		{
			base();
		}
	}

	class X
	{
		X()
		{
			var t = this.ToString();
			var b = t == "X"
				? true
				: false;

			if (b) goto x;

			switch (t)
			{
			default:
				y: throw null;
			}

			x: ;
		}
	}
}
