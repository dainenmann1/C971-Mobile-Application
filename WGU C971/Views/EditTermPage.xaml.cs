using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WGU_C971.Models;
using SQLite;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace WGU_C971.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditTermPage : ContentPage
    {
        public SchoolTerm Term;
        public MainPage MainPage;

        public EditTermPage(SchoolTerm term, MainPage mainPage)
        {
            InitializeComponent();
            MainPage = mainPage;
            Term = term;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            TxtTermName.Text = Term.Name;
            DatePickerStartDate.Date = Term.StartDate;
            DatePickerEndDate.Date = Term.EndDate;
        }

        private async void BtnSave_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (RequiredTermInputsPopulated())
                {
                    Term.Name = TxtTermName.Text;
                    Term.StartDate = DatePickerStartDate.Date;
                    Term.EndDate = DatePickerEndDate.Date;
                    using (SQLiteConnection connection = new SQLiteConnection(App.FilePath))
                    {
                        connection.Update(Term);
                        await Navigation.PopModalAsync();
                    }
                }
                else
                {
                    if (String.IsNullOrWhiteSpace(TxtTermName.Text))
                    {
                        throw new ApplicationException("Term Name is REQUIRED");
                    }

                    if (String.IsNullOrWhiteSpace(DatePickerStartDate.Date.ToString()))
                    {
                        throw new ApplicationException("Term Start Date is REQUIRED");
                    }

                    if (String.IsNullOrWhiteSpace(DatePickerEndDate.Date.ToString()))
                    {
                        throw new ApplicationException("Term End Date is REQUIRED");
                    }
                }
            }
            catch (ApplicationException ex)
            {
                await DisplayAlert("Warning", ex.Message, "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }

        }

        private async void BtnCancel_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }

        private bool RequiredTermInputsPopulated()
        {
            return !String.IsNullOrWhiteSpace(TxtTermName.Text) &&
                !String.IsNullOrWhiteSpace(DatePickerStartDate.Date.ToString()) &&
                !String.IsNullOrWhiteSpace(DatePickerEndDate.Date.ToString());
        }
    }
}