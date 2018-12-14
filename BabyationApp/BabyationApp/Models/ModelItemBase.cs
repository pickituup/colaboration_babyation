using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BabyationApp.Common;

namespace BabyationApp.Models
{
    public class ModelItemBase : ObservableObject
    {
        public ModelItemBase()
        {
            _isFocused = false;
			TagBool1 = false;
        }

        public int _index;
        public int Index
        {
            get
            {
                return _index;
            }
            set
            {
                if (SetPropertyChanged(ref _index, value))
                {
                    SetPropertyChanged("IsOddIndex");
                }
            }
        }

        public bool IsOddIndex
        {
            get
            {
                return _index % 2 == 1;
            }
        }

        private double _tagDouble1;
        public double TagDouble1
        {
            get { return _tagDouble1; }
            set => SetPropertyChanged(ref _tagDouble1, value);
        }

		private bool _tagBool1;
		public bool TagBool1
		{
			get { return _tagBool1; }
            set => SetPropertyChanged(ref _tagBool1, value);
		}

        private bool _isFocused;
        public bool IsFocused
        {
            get { return _isFocused; }
            set => SetPropertyChanged(ref _isFocused, value);
        }


        private bool _isSelected;
        public bool IsSelected
        {
            get { return _isSelected; }
            set => SetPropertyChanged(ref _isSelected, value);
        }
    }
}
