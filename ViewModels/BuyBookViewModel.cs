using BookSeller.Models;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BookSeller.ViewModels
{
    public class BuyBookViewModel:Screen
    {
        private string _selectedBookType = "";
        private int _pageNumber = 1;
        private int _currentProgress = 0;
        private BindableCollection<BookModel> _books = new();
        private string aux = "";


        #region === Button clicks ===

        public async void SearchType_ButtonAsync()
        {
            WebScraper webScraper = new WebScraper();
            Progress<ProgressReportModel> progress = new();
            progress.ProgressChanged += RepoprtProgress;

            if (SelectedBookType == "All")
            {
                Books = await Task.Run(() => new BindableCollection<BookModel>(webScraper.GetAllBooksOfaNonSpecificGenre(PageNumber , progress)));
            }
            else
            {
                Books = await Task.Run(() => new BindableCollection<BookModel>(webScraper.GetBooksOfaSpecificGenre(SelectedBookType, PageNumber , progress)));
            }
        }
        public void CheckOut_Buttom()
        {
            MessageBox.Show(aux);
        }

        public void AddToCart_Buttom()
        {
            aux += SelectedBooks.BookTitle;
        }

        public void NextPage_Button()
        {
            PageNumber = PageNumber + 1;
        }

        public void PreviosPage_Buton()
        {
            if(PageNumber != 1)
            {
                PageNumber = PageNumber - 1;
            }
            
        }
        #endregion


        #region === Getters and Setters ===
        public List<string> BookType
        {
            get { return WebScraper.GetBookTypes(); }
        }

        public string SelectedBookType
        {
            get { return _selectedBookType; }
            set 
            { 
                _selectedBookType = value; 
                NotifyOfPropertyChange(nameof(this.SelectedBookType));
            }
        }

        public BindableCollection<BookModel> Books
        {
            get { return _books; }
            set 
            {
                _books = value;
                NotifyOfPropertyChange();
            }
        }

        public int PageNumber
        {
            get { return _pageNumber; }
            set 
            {
                _pageNumber = value; 
                NotifyOfPropertyChange(()=> this.PageNumber);
            }
        }

        #region === Progress Bar Stuf ===
        private void RepoprtProgress(object? sender, ProgressReportModel e)
        {
            CurrentProgress = e.PercentageComplete;
        }

        public int CurrentProgress
        {
            get { return _currentProgress; }
            set
            {
                if (_currentProgress == value) return;

                _currentProgress = value;
                NotifyOfPropertyChange(() => CurrentProgress);
                NotifyOfPropertyChange();
            }
        }
        #endregion

        private BookModel _selectedBook;

        public BookModel SelectedBooks
        {
            get { return _selectedBook; }
            set 
            { 
                _selectedBook = value;
                NotifyOfPropertyChange(() => SelectedBooks);
            }
        }
        #endregion
    }
}
