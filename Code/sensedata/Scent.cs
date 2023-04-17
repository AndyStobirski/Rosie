using Rosie.Code.Misc;
using System.Linq;

namespace Rosie.Code.sensedata
{
    public class Scent : SenseDatum
    {

        public int DegradeVal { get; set; }
        public int ScentValue { get; set; }
        public Scent(int pDuration, int pDegrade)
        {
            ScentValue = pDuration;
            DegradeVal = pDegrade;
        }

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
            if ((ScentValue - DegradeVal) > 0)
            {

                return Library.Directions1
                    .Select(p => new Scent(ScentValue, DegradeVal, p.X + this.X, p.Y + this.Y))
                    .Where(p => MapUtils.IsCellValid(p.X, p.Y))
                    .ToArray();
            }
            else
            {
                return new Scent[] { };
            }
        }

        /// <summary>
        /// Degrade the scent, returning if it's still existing
        /// </summary>
        /// <returns></returns>
        public bool Degrade()
        {
            ScentValue -= DegradeVal;
            return ScentValue < 0;
        }



    }
}
