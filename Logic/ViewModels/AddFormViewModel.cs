using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Logic.Models;
using System.Collections.ObjectModel;
using Excel = Microsoft.Office.Interop.Excel;
using Xceed.Words.NET;
using System.Linq;
using System.Collections.Generic;
using Logic.Messages;

namespace Logic.ViewModels
{
    public class AddFormViewModel : ViewModelBase
    {
        private Form _form;

        public ObservableCollection<KeyColumnPair> keyColumnPairs { get; set; }
        public int addedPairs { get; set; }
        public string addedPairsMessageVisible { get; set; }
        private KeyColumnPair _keyColumnPair;
        public KeyColumnPair keyColumnPair
        {
            get
            {
                return _keyColumnPair;
            }
            set
            {
                _keyColumnPair = value;
                _keyColumnPair.PropertyChanged += (s, e) => AddKeyColumnPairCommand.RaiseCanExecuteChanged();
                RaisePropertyChanged();
                AddKeyColumnPairCommand.RaiseCanExecuteChanged();
            }
        }

        public Form form { get
            {
                return _form;
            }
            set
            {
                _form = value;
                _form.PropertyChanged += (s, e) => {
                    SaveFormCommand.RaiseCanExecuteChanged();
                    AutoFetchPairsCommand.RaiseCanExecuteChanged();
                };
                RaisePropertyChanged();
                SaveFormCommand.RaiseCanExecuteChanged();
                AutoFetchPairsCommand.RaiseCanExecuteChanged();
            }
        }
        public RelayCommand SaveFormCommand { get; set; }
        public RelayCommand AddKeyColumnPairCommand { get; set; }
        public RelayCommand<KeyColumnPair> DeleteKeyColumnPair { get; set; }
        public RelayCommand AutoFetchPairsCommand { get; set; }


        public string stringSearchColumn
        {
            get
            {
                return form.SearchColumn + "";
            }
            set
            {
                int temp;
                if (int.TryParse(value, out temp))
                {
                    form.SearchColumn = temp;
                }
                else
                {
                    form.SearchColumn = 0;
                }
            }
        }

        

        public string IsPairsListVisible
        {
            get
            {
                if(keyColumnPairs.Count > 0)
                {
                    return "Visible";
                }
                else
                {
                    return "Collapsed";
                }
            }
        }

        public AddFormViewModel()
        {
            SaveFormCommand = new RelayCommand(SaveForm, CanSaveForm);
            AddKeyColumnPairCommand = new RelayCommand(AddKeyColumnPair, CanAddKeyColumnPair);
            DeleteKeyColumnPair = new RelayCommand<KeyColumnPair>((KeyColumnPair p) => keyColumnPairs.Remove(p));
            AutoFetchPairsCommand = new RelayCommand(AutoFetchPairs, CanAutoFetchPairs);

            keyColumnPairs = new ObservableCollection<KeyColumnPair>();
            form = new Form();
            keyColumnPair = new KeyColumnPair();
            keyColumnPairs.CollectionChanged += (s,e) => {
                SaveFormCommand.RaiseCanExecuteChanged();
                RaisePropertyChanged("IsPairsListVisible");
                };
            addedPairsMessageVisible = "Collapsed";
        }

        public void SaveForm()
        {
            if(CanSaveForm())
            {
                using (var ctx = new Model1())
                {
                    form.keyColumnPairs = keyColumnPairs.ToList();
                    ctx.Forms.Add(form);
                    ctx.SaveChanges();
                }
                form = new Form();
                keyColumnPairs = new ObservableCollection<KeyColumnPair>();
                RaisePropertyChanged("keyColumnPairs");
                RaisePropertyChanged("IsPairsListVisible");
                keyColumnPairs.CollectionChanged += (s, e) => SaveFormCommand.RaiseCanExecuteChanged();
                addedPairsMessageVisible = "Collapsed";
                RaisePropertyChanged("addedPairsMessageVisible");
                MessengerInstance.Send(new ItemAddedMessage());

            }
        }

        public bool CanSaveForm()
        {
            return form.IsValid() && keyColumnPairs.Count > 0;
        }

        public void AddKeyColumnPair()
        {
            if(CanAddKeyColumnPair() && !keyColumnPairs.Any(p => p.Key.Equals(keyColumnPair.Key)))
            {
                keyColumnPairs.Add(keyColumnPair);
                keyColumnPair = new KeyColumnPair();
                RaisePropertyChanged("IsPairsListVisible");
                addedPairsMessageVisible = "Collapsed";
                RaisePropertyChanged("addedPairsMessageVisible");
            }
        }
        public bool CanAddKeyColumnPair()
        {
            return keyColumnPair.IsValid();
        }

        public void AutoFetchPairs()
        {
            Excel.Application xlApp;
            Excel.Workbook xlWorkBook;
            Excel.Worksheet xlWorkSheet;
            Excel.Range range;

            string str;
            int cCnt;
            int rCnt;
            int rw;
            int cl;

            xlApp = new Excel.Application();
            xlWorkBook = xlApp.Workbooks.Open(form.sourceExcelFile, 0, true, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);
            xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);

            range = xlWorkSheet.UsedRange;
            rw = range.Rows.Count;
            cl = range.Columns.Count;

            int tableFirstColumn = 1;
            int tableFirstLine = 1;

            bool found = false;
            for (rCnt = 1; rCnt <= rw && found == false; rCnt++)
            {
                for (cCnt = 1; cCnt <= cl && found == false; cCnt++)
                {
                    dynamic x = (range.Cells[rCnt, cCnt] as Excel.Range).Value2;
                    str = x != null ? x.ToString() : "";
                    if (!string.IsNullOrWhiteSpace(str))
                    {
                        tableFirstLine = rCnt;
                        tableFirstColumn = cCnt;
                        found = true;
                    }
                }
            }
            addedPairs = 0;
            for (cCnt = tableFirstColumn; cCnt <= cl; cCnt++)
            {
                dynamic x = (range.Cells[tableFirstLine, cCnt] as Excel.Range).Value2;
                str = x != null ? x.ToString() : "";
                if (!string.IsNullOrWhiteSpace(str) && !keyAlreadyExists(str))
                {
                    keyColumnPairs.Add(new KeyColumnPair("#" + str + "#", cCnt));
                    addedPairs++;
                }
            }

            addedPairsMessageVisible = "Visible";
            RaisePropertyChanged("addedPairsMessageVisible");
            RaisePropertyChanged("IsPairsListVisible");
            RaisePropertyChanged("addedPairs");
        }

        public bool CanAutoFetchPairs()
        {
            return form.firstLineIsHeader && form.IsValid();
        }

        private bool keyAlreadyExists(string key)
        {
            return keyColumnPairs.Any(p => p.Key.Equals("#" + key + "#"));
        }
    }
}
