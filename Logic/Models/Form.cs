using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Logic.Models
{
    public class Form : ModelBase
    {
        public long id { get; set; }

        private string _FormName;
        [Required(ErrorMessage = "Form name is mandatory !")]
        [MinLength(length:3, ErrorMessage ="Name length must be greater than 3")]
        public string FormName
        {
            get
            {
                return _FormName;
            }
            set
            {
                _FormName = value;
                this.NotifyPropertyChanged();
            }
        }

        private string _sourceExcelFile;
        [Required(ErrorMessage = "Source Excel file is mandatory !")]
        public string sourceExcelFile
        {
            get
            {
                return _sourceExcelFile;
            }
            set
            {
                _sourceExcelFile = value;
                this.NotifyPropertyChanged();
            }
        }

        private string _formModel;
        [Required(ErrorMessage = "Form template file is mandatory !")]
        public string formModel
        {
            get
            {
                return _formModel;
            }
            set
            {
                _formModel = value;
                this.NotifyPropertyChanged();
            }
        }

        private string _OutputFolder;
        [Required(ErrorMessage = "Output folder is mandatory !")]
        public string OutputFolder
        {
            get
            {
                return _OutputFolder;
            }
            set
            {
                _OutputFolder = value;
                this.NotifyPropertyChanged();
            }
        }

        private int _SearchColumn;
        [Required(ErrorMessage = "Search column is mandatory !")]
        [Range(minimum:1, maximum:int.MaxValue,ErrorMessage = "Search column must be greater than 0")]
        public int SearchColumn
        {
            get
            {
                return _SearchColumn;
            }
            set
            {
                _SearchColumn = value;
                this.NotifyPropertyChanged();
            }
        }


        private bool _firstLineIsHeader;
        public bool firstLineIsHeader
        {
            get
            {
                return _firstLineIsHeader;
            }
            set
            {
                _firstLineIsHeader = value;
                this.NotifyPropertyChanged();
            }
        }

        public virtual List<KeyColumnPair> keyColumnPairs { get; set; }

        public Form()
        {
            keyColumnPairs = new List<KeyColumnPair>();
            SearchColumn = 1;
        }

        public Form(string sourceExcelFile, string formModel, string OutputFolder, int searchColumn,
                    bool firstLineIsHeader, List<KeyColumnPair> keyColumnPairs)
        {
            this.SearchColumn = searchColumn;
            this.sourceExcelFile = sourceExcelFile;
            this.formModel = formModel;
            this.OutputFolder = OutputFolder;
            this.firstLineIsHeader = firstLineIsHeader;
            this.keyColumnPairs = keyColumnPairs;
        }
    }
}
