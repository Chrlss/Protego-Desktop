using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace Protego.Class
{
    class GPUdata
    {
        public class GpuData : INotifyPropertyChanged
        {
            public double GpuTemp { get; set; }
            public double GpuTempProgress { get; set; } // Normalized temperature (0-1)
            public double GpuClock { get; set; }
            public double GpuClockProgress { get; set; } // Normalized clock speed (0-1)
            public double GpuPower { get; set; }
            public double GpuPowerProgress { get; set; } // Normalized power consumption (0-1)
            public double GpuLoad { get; set; }
            public double GpuLoadProgress { get; set; } // Normalized load (0-1)

            public event PropertyChangedEventHandler PropertyChanged;

            protected virtual void OnPropertyChanged(string propertyName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
