using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhisperWpfWindows
{
    public static class CommonExtentions
    {
        public static string extentionWhisperModelEnums(this MainWindow.whisperModels model)
        {
            switch (model)
            {
                case MainWindow.whisperModels.tiny:
                    return "tiny";
                    break;
                case MainWindow.whisperModels._base:
                    return "base";
                    break;
                case MainWindow.whisperModels.small:
                    return "small";
                    break;
                case MainWindow.whisperModels.medium:
                    return "medium";
                    break;
                case MainWindow.whisperModels.large:
                    return "large";
                    break;
                default:
                    return "tiny";
                    break;
            }
        }
    }
}
