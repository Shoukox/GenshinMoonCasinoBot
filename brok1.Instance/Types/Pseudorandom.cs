namespace brok1.Instance.Types
{
    public class Pseudorandom
    {
        /// <summary>
        /// a and b are inclusive
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>

        List<int> log;
        public double native_chance;
        public double chance;
        public double crystalChance = 0.001; //5%
        public int success, loss;
        public int rowLostCount;
        public bool mustWin
        {
            get
            {
                return chance >= 100;
            }
        }
        private bool wasLost;
        private bool isTwisted;
        private int listSize = 50;
        private List<int[]> lastRndGrenzen;
        private Random random = new Random();

        public Pseudorandom(double chance)
        {
            native_chance = chance;
            this.chance = chance;
            lastRndGrenzen = new List<int[]>();
            log = new List<int>();
            success = 0;
            loss = 0;
            rowLostCount = 0;
        }

        private void _EditChance(double chance)
        {
            this.chance = chance;
        }
        public void EditChance(double chance, bool isTwisted = true)
        {
            this.isTwisted = isTwisted;
            this.chance = chance;
        }
        public static int GetRandom(int a, int b) => Random.Shared.Next(a, b + 1);
        private int _GetRandom(int a, int b) => random.Next(a, b + 1);
        public bool ProcessChance(bool hasPayed, bool crystals = false, double chance = -1)
        {
            if (chance == -1)
                chance = this.chance;

            if (isTwisted)
            {
                isTwisted = false;
            }
            else
            {
                if (chance != -1)
                {
                    _EditChance(chance);
                }
            }
            if (mustWin)
            {
                success += 1;
                wasLost = false;
                rowLostCount = 0;

                return true;
            }


            int num = _GetRandom(1, 100000);
            bool hasWon;
            if (!crystals)
                hasWon = num <= chance * 1000;
            else
                hasWon = num >= 727 && num <= 800;

            if (hasWon)
            {
                success += 1;
                wasLost = false;
                rowLostCount = 0;
                EditChance(native_chance, false);
            }
            else
            {
                wasLost = true;
                rowLostCount += 1;
                loss += 1;
            }

            return hasWon;
        }

        public string SaveData()
        {
            string result = "";
            result += string.Join(",", log) + "=";
            result += $"{native_chance},{chance},{chance}" + "=";
            result += $"{success},{loss},{rowLostCount}" + "=";
            result += $"{rowLostCount}, {wasLost}";
            return result;
        }
        public void LoadData(string pseudorandomString)
        {
            string[] splittedData = pseudorandomString.Split("=");
            string[] spl0 = splittedData[0].Split(",");
            string[] spl1 = splittedData[1].Split(",");
            string[] spl2 = splittedData[2].Split(",");
            string[] spl3 = splittedData[3].Split(",");

            //0
            if (!int.TryParse(spl0[0], out _))
                log = new List<int>();
            else
                log = spl0.Select(m => int.Parse(m)).ToList();

            //1
            native_chance = double.Parse(spl1[0]);
            chance = double.Parse(spl1[1]);
            //chance = double.Parse(spl1[2]);

            //2
            success = int.Parse(spl2[0]);
            loss = int.Parse(spl2[1]);
            rowLostCount = int.Parse(spl2[2]);

            //3
            rowLostCount = int.Parse(spl3[0]);
            wasLost = bool.Parse(spl3[1]);
        }
    }
}
