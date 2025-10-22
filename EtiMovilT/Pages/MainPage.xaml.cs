using EtiMovilT.Models;
using EtiMovilT.PageModels;

namespace EtiMovilT.Pages
{
    public partial class MainPage : ContentPage
    {
        public MainPage(MainPageModel model)
        {
            InitializeComponent();
            BindingContext = model;
        }
    }
}