using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using WGU_C971.Models;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace WGU_C971.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddTermPage : ContentPage
    {
        public MainPage MainPage;

        public AddTermPage(MainPage mainPage)
        {
            InitializeComponent();
            MainPage = mainPage;
        }
        private async void SaveBtn_Clicked(object sender, EventArgs e)
        {
            SchoolTerm term = new SchoolTerm()
            {
                Name = newTermName.Text,
                StartDate = DatePickerStartDate.Date,
                EndDate = DatePickerEndDate.Date
            };

            using (SQLiteConnection connection = new SQLiteConnection(App.FilePath))
            {
                try
                {
                    if (RequiredTermFieldPopulated())
                    {
                        if (term.StartDate > term.EndDate)
                        {
                            throw new ApplicationException("Start Date cannot be Greater than the End Date");
                        }

                        connection.CreateTable<SchoolTerm>();
                        int insertedRows = connection.Insert(term);
                        MainPage.TermList.Add(term);

                        if (insertedRows < 1)
                        {
                            throw new ApplicationException();
                        }

                        string successMessage = $"{term.Name} has been added successfully.";
                        await DisplayAlert("Success", successMessage, "OK");

                        await Navigation.PopModalAsync();
                    }
                    else
                    {
                        if (String.IsNullOrWhiteSpace(term.Name)) throw new ApplicationException("Term Name is Required");
                        if (String.IsNullOrWhiteSpace(term.StartDate.ToString())) throw new ApplicationException("Start Date is Required");
                        if (String.IsNullOrWhiteSpace(term.EndDate.ToString())) throw new ApplicationException("End Date is Required");
                    }
                }
                catch (ApplicationException ex)
                {
                    await DisplayAlert("Error", ex.Message, "OK");
                }
                catch (Exception ex)
                {
                    await DisplayAlert(Title, ex.Message, "OK");
                }
            }
        }

        // Helper methods
        private bool RequiredTermFieldPopulated()
        {
            return !String.IsNullOrWhiteSpace(newTermName.Text)
                && !String.IsNullOrWhiteSpace(DatePickerStartDate.Date.ToString())
                && !String.IsNullOrWhiteSpace(DatePickerEndDate.Date.ToString());
        }

        private async void CancelBtn_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }
    }
}