namespace Org.BouncyCastle.Math.EC.Multiplier
{
	/**
	* Class holding precomputation data for the WNAF (Window Non-Adjacent Form)
	* algorithm.
	*/
	internal class WNafPreCompInfo
		: PreCompInfo 
	{
		/**
		* Array holding the precomputed <code>ECPoint</code>s used for the Window
		* NAF multiplication in <code>
		* {@link org.bouncycastle.math.ec.multiplier.WNafMultiplier.multiply()
		* WNafMultiplier.multiply()}</code>.
		*/
		private ECPoint[] preComp = null;

		/**
		* Holds an <code>ECPoint</code> representing twice(this). Used for the
		* Window NAF multiplication in <code>
		* {@link org.bouncycastle.math.ec.multiplier.WNafMultiplier.multiply()
		* WNafMultiplier.multiply()}</code>.
		*/
		private ECPoint twiceP = null;

		internal ECPoint[] GetPreComp()
		{
			return preComp;
		}

		internal void SetPreComp(ECPoint[] preComp)
		{
			this.preComp = preComp;
		}

		internal ECPoint GetTwiceP()
		{
			return twiceP;
		}

		internal void SetTwiceP(ECPoint twiceThis)
		{
			this.twiceP = twiceThis;
		}
	}
}
