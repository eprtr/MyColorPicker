using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColorPicker.utils
{/// <summary>
/// 实现控件与控件的双向绑定
/// </summary>
    class ColorViewModel : INotifyPropertyChanged
    {
        private double _sliderRedValue;
        public double SliderRedValue
        {
            get { return _sliderRedValue; }
            set
            {
                if (_sliderRedValue != value)
                {
                    _sliderRedValue = value;
                    OnPropertyChanged(nameof(SliderRedValue));
                }
            }
        }

        private double _sliderGreenValue;
        public double SliderGreenValue
        {
            get { return _sliderGreenValue; }
            set
            {
                if (_sliderGreenValue != value)
                {
                    _sliderGreenValue = value;
                    OnPropertyChanged(nameof(SliderGreenValue));
                }
            }
        }

        private double _sliderBlueValue;
        public double SliderBlueValue
        {
            get { return _sliderBlueValue; }
            set
            {
                if (_sliderBlueValue != value)
                {
                    _sliderBlueValue = value;
                    OnPropertyChanged(nameof(SliderBlueValue));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
