using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Logic.Messages;
using Logic.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Excel = Microsoft.Office.Interop.Excel;
using System.Text;
using System.Threading.Tasks;
using Xceed.Words.NET;
using System.Data.Entity;

namespace Logic.ViewModels
{
    public class FillFormViewModel : ViewModelBase
    {
        public ObservableCollection<Form> FormsList { get; set; }

        private string _ErrorMessage;
        public string ErrorMessage
        {
            get
            {
                return _ErrorMessage;
            }
            set
            {
                _ErrorMessage = value;
                RaisePropertyChanged();
            }
        }

        private string _ResultMessage;
        public string ResultMessage
        {
            get
            {
                return _ResultMessage;
            }
            set
            {
                _ResultMessage = value;
                RaisePropertyChanged();
            }
        }

        private string _searchFor;
        public string searchFor {
            get
            {
                return _searchFor;
            }
            set
            {
                _searchFor = value;
                AutoFillCommand.RaiseCanExecuteChanged();
            }
        }

        public RelayCommand AutoFillCommand { get; set; }

        private Form _selectedForm;
        public Form SelectedForm {
            get
            {
                return _selectedForm;
            }
            set
            {
                _selectedForm = value;
                AutoFillCommand.RaiseCanExecuteChanged();
            }
        }

        public FillFormViewModel()
        {
            PopulateComboBox();
            AutoFillCommand = new RelayCommand(AutoFill, canAutoFill);
            MessengerInstance.Register<ItemAddedMessage>(this, m => { PopulateComboBox(); });
            MessengerInstance.Register<ItemDeletedMessage>(this, m => { PopulateComboBox(); });
        }

        public void PopulateComboBox()
        {
            FormsList = new ObservableCollection<Form>();
            try {
            using (var ctx = new Model1())
            {
                foreach (var item in ctx.Forms.Include(f => f.keyColumnPairs))
                    FormsList.Add(item);
            }
            }
            catch(Exception e)
            {
                ErrorMessage = e.Message;
            }
        }


        public void AutoFill()
        {
            if (SelectedForm == null || SelectedForm.keyColumnPairs == null || !SelectedForm.keyColumnPairs.Any())
                return;

            ErrorMessage = "";
            ResultMessage = "";

            Excel.Application xlApp;
            Excel.Workbook xlWorkBook;
            Excel.Worksheet xlWorkSheet;
            Excel.Range range;

            string str;
            int rCnt;
            int cCnt;
            int rw = 0;
            int cl = 0;

            xlApp = new Excel.Application();
            xlWorkBook = xlApp.Workbooks.Open(SelectedForm.sourceExcelFile, 0, true, 5, "", "", true, Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);
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
                    str = x.ToString();
                    if (!string.IsNullOrWhiteSpace(str))
                    {
                        tableFirstLine = rCnt;
                        tableFirstColumn = cCnt;
                        found = true;
                    }
                }
            }

            int replaced = 0;
            found = false;
            bool error = false;
            for (rCnt = tableFirstLine; rCnt <= rw && !error; rCnt++)
            {


                dynamic x = (range.Cells[rCnt, SelectedForm.SearchColumn] as Excel.Range).Value2;

                str = x != null ? x.ToString() : "";
                
                if (str == searchFor)
                {
                    found = true;
                    Dictionary<string, string> replaceDict = new Dictionary<string, string>();

                    foreach (var p in SelectedForm.keyColumnPairs)
                    {
                        try
                        {
                            dynamic x2 = (range.Cells[rCnt, p.Column] as Excel.Range).Value2;
                            str = str = x2 != null ? x2.ToString() : "";
                            replaceDict.Add(p.Key, str);
                            replaced++;
                        }
                        catch
                        {
                            ErrorMessage = "Error occured while reading the source file!";
                            error = true;
                            break;
                        }
                    }

                    try
                    {
                        using (DocX document = DocX.Load(SelectedForm.formModel))
                        {
                            foreach (KeyValuePair<string, string> kvp in replaceDict)
                            {
                                document.ReplaceText(kvp.Key, kvp.Value);
                            }
                            document.SaveAs(SelectedForm.OutputFolder + "\\" + searchFor + ".docx");
                        }
                    }
                    catch
                    {
                        ErrorMessage = "Error occured while filling form !\nTemplate file mustn't be used by an other process.";
                        error = true;
                        break;
                    }
                    ResultMessage = "All work done !";
                    break;
                }
            }
            if (found == false)
            {
                ErrorMessage = "couldn't found the row that you are looking for !";
            }
        }

        public bool canAutoFill()
        {
            return SelectedForm != null && !string.IsNullOrWhiteSpace(searchFor);
        }
    }
}
