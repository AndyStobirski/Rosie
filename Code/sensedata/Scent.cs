using Rosie.Code.Misc;
using System.Linq;

namespace Rosie.Code.sensedata
{
    public class Scent : SenseDatum
    {

        public int DegradeVal { get; set; }
        public int ScentValue { get; set; }

        public Scent(int pScentValue, int pDegrade, int pX, int pY)
        {
            ScentValue = pScentValue;
            DegradeVal = pDegrade;
            X = pX;
            Y = pY;
        }

        /// <summary>
        /// Generate a new set of neighbours with a degraded scent value
        /// </summary>
        /// <returns></returns>
        public Scent[] Propogate()
        {
            return Library.Directions2
                .Select(p => new Scent(ScentValue, DegradeVal, p.X + this.X, p.Y + this.Y))
                .Where(p => MapUtils.IsCellValid(p.X, p.Y))
                .ToArray();
        }

        /// <summary>
        /// Degrade the scent, returning if it still exist
        /// </summary>
        /// <returns></returns>
        public bool Degrade()
        {
            ScentValue -= DegradeVal;
            return ScentValue < 1;
        }



    }
}
