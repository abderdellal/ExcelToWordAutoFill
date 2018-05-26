using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Models
{
    public class KeyColumnPair : ModelBase
    {
        public int id { get; set; }

        private string _key;
        [Required(ErrorMessage = "The place holder is mandatory !")]
        public string Key {
            get
            {
                return _key;
            }
            set
            {
                _key = value;
                NotifyPropertyChanged();
            }
        }

        private int _column;
        [Required(ErrorMessage = "The column is mandatory !")]
        [Range(minimum: 1, maximum: int.MaxValue, ErrorMessage = "Column must be greater than 0")]
        public int Column {
            get
            {
                return _column;
            }
            set
            {
                _column = value;
                NotifyPropertyChanged();
            }
        }

        public KeyColumnPair()
        {
        }

        public KeyColumnPair(string key, int column)
        {
            this.Key = key;
            this.Column = column;
        }

        public virtual Form form { get; set; }

    }
}
