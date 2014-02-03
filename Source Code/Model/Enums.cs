using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace EnigmaBreaker.Model
{
    public enum GameResolution
    {
        [Description("800x600")]
        Small,
        [Description("1024x768")]
        Medium,
        [Description("Full Screen")]
        Large
    }

    public enum OptionSlider
    {
        VolumeSlider,
        None
    }
}
