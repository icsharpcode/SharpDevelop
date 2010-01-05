namespace Org.BouncyCastle.Math.EC.Multiplier
{
	/**
	* Class holding precomputation data for the WTNAF (Window
	* <code>&#964;</code>-adic Non-Adjacent Form) algorithm.
	*/
	internal class WTauNafPreCompInfo
		: PreCompInfo
	{
		/**
		* Array holding the precomputed <code>F2mPoint</code>s used for the
		* WTNAF multiplication in <code>
		* {@link org.bouncycastle.math.ec.multiplier.WTauNafMultiplier.multiply()
		* WTauNafMultiplier.multiply()}</code>.
		*/
		private readonly F2mPoint[] preComp;

		/**
		* Constructor for <code>WTauNafPreCompInfo</code>
		* @param preComp Array holding the precomputed <code>F2mPoint</code>s
		* used for the WTNAF multiplication in <code>
		* {@link org.bouncycastle.math.ec.multiplier.WTauNafMultiplier.multiply()
		* WTauNafMultiplier.multiply()}</code>.
		*/
		internal WTauNafPreCompInfo(F2mPoint[] preComp)
		{
			this.preComp = preComp;
		}

		/**
		* @return the array holding the precomputed <code>F2mPoint</code>s
		* used for the WTNAF multiplication in <code>
		* {@link org.bouncycastle.math.ec.multiplier.WTauNafMultiplier.multiply()
		* WTauNafMultiplier.multiply()}</code>.
		*/
		internal F2mPoint[] GetPreComp()
		{
			return preComp;
		}
	}
}
