using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Logic.Messages;
using Logic.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.ViewModels
{
    public class FormsListViewModel : ViewModelBase
    {
        public FormsListViewModel()
        {
            PopulateView();

            DeleteItemCommand = new RelayCommand<Form>(item =>
            {
                using (var ctx = new Model1())
                {
                    ctx.Forms.Attach(item);
                    foreach(var pair in item.keyColumnPairs)
                    {
                        ctx.KeyColumnPairs.Attach(pair);
                    }
                    if (item.keyColumnPairs != null && item.keyColumnPairs.Count > 0)
                    {
                        ctx.KeyColumnPairs.RemoveRange(item.keyColumnPairs);
                    }
                    ctx.Forms.Remove(item);
                    //ctx.Entry(item).State = EntityState.Deleted;
                    ctx.SaveChanges();
                    ItemList.Remove(item);
                }
                MessengerInstance.Send(new ItemDeletedMessage());
            });

            MessengerInstance.Register<ItemAddedMessage>(this, m => { PopulateView(); });
        }

        public ObservableCollection<Form> ItemList { get; set; }
        public RelayCommand<Form> DeleteItemCommand { get; set; }

        private void PopulateView()
        {
            ItemList = new ObservableCollection<Form>();
            using (var ctx = new Model1())
            {
                foreach (var item in ctx.Forms.Include(f => f.keyColumnPairs))
                    ItemList.Add(item);
            }
        }
    }
}

