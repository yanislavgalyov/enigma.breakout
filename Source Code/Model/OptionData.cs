using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EnigmaBreaker.Model
{
    [Serializable]
    public struct OptionData
    {
        public string GameResolution;
        public int SoundVolume;

        public OptionData(bool IsCalled)
        {
            GameResolution = EnigmaBreaker.Model.GameResolution.Medium.ToString();
            SoundVolume = 50;
        }
    }
}
