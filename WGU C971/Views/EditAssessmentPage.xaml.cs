using System;
using SQLite;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WGU_C971.Models;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace WGU_C971.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditAssessmentPage : ContentPage
    {
        public MainPage MainPage;
        public SchoolCourse Course;
        public Assessment Assessment;
        public List<string> AssessmentTypes = new List<string> { "Performance", "Objective" };

        public EditAssessmentPage(MainPage mainPage, SchoolCourse course, Assessment assessment)
        {
            InitializeComponent();
            MainPage = mainPage;
            Course = course;
            Assessment = assessment;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            TxtAssessmentName.Text = Assessment.Name;
            PickerAssessmentType.ItemsSource = AssessmentTypes;
            PickerAssessmentType.SelectedItem = AssessmentTypes.Find(assessmentType => assessmentType == Assessment.Type);
            DatePickerStartDate.Date = Assessment.StartDate;
            DatePickerEndDate.Date = Assessment.EndDate;
            Title = Assessment.Name;
        }

        private async void BtnSave_Clicked(object sender, EventArgs e)
        {
            try
            {
                bool assessmentTypeModified = Assessment.Type != PickerAssessmentType.SelectedItem.ToString().Trim();
                bool assessmentDatesModified = (Assessment.StartDate != DatePickerStartDate.Date) || (Assessment.EndDate != DatePickerEndDate.Date.AddHours(2.5));
                string perfAssessmentQuery = $"SELECT * FROM Assessment WHERE CourseId = {Course.Id} AND Type = 'Performance';";
                string objAssessmentQuery = $"SELECT * FROM Assessment WHERE CourseId = {Course.Id} AND Type = 'Objective';";

                Assessment.Name = TxtAssessmentName.Text;
                Assessment.StartDate = DatePickerStartDate.Date;
                Assessment.EndDate = DatePickerEndDate.Date;

                if (assessmentTypeModified || assessmentDatesModified)
                {
                    using (SQLiteConnection connection = new SQLiteConnection(App.FilePath))
                    {
                        List<Assessment> perfAssessments = connection.Query<Assessment>(perfAssessmentQuery);
                        List<Assessment> objAssessments = connection.Query<Assessment>(objAssessmentQuery);
                        bool isPerfAssessment = Assessment.Type.ToString().Trim() == "Performance";
                        bool isObjAssessment = Assessment.Type.ToString().Trim() == "Objective";
                        bool hasAssessmentId = !String.IsNullOrWhiteSpace(Assessment.Id.ToString());

                        if (isPerfAssessment && perfAssessments.Count == 0)
                        {
                            UpdateAssessmentType();
                            connection.Update(Assessment);
                            await Navigation.PopModalAsync();
                        }
                        else if (isObjAssessment && objAssessments.Count == 0)
                        {
                            UpdateAssessmentType();
                            connection.Update(Assessment);
                            await Navigation.PopModalAsync();
                        }
                        else if (((isPerfAssessment && perfAssessments.Count == 1) || (isObjAssessment && objAssessments.Count == 1))
                               && hasAssessmentId && !assessmentTypeModified)
                        {
                            connection.Update(Assessment);
                            await Navigation.PopModalAsync();
                        }
                        else
                        {
                            string warningMessage = $"You can't add more that one {Assessment.Type} Assessment Type per Course.";
                            await DisplayAlert(Title, warningMessage, "OK");
                        }
                    }
                }
                else
                {
                    await DisplayAlert("Info!", "No Changes were made.", "OK");
                    await Navigation.PopModalAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An Error has occured: {ex.Message}");
                await DisplayAlert("Error", "An Error Occured.", "OK");
            }
        }

        private async void BtnCancel_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }

        private async void BtnDelete_Clicked(object sender, EventArgs e)
        {
            try
            {
                string alertMessage = $"You are about to delete {Assessment.Name} Assessment. Are you sure you want to delete it?";
                bool response = await DisplayAlert("Warning!", alertMessage, "Yes", "No");

                if (response)
                {
                    using (SQLiteConnection connection = new SQLiteConnection(App.FilePath))
                    {
                        connection.Delete(Assessment);
                    }
                    await Navigation.PopModalAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception Occured: {ex.Message}");
                await DisplayAlert("Error Message", "An Error has occured.", "OK");
            }
        }

        private void UpdateAssessmentType()
        {
            Assessment.Type = PickerAssessmentType.SelectedItem.ToString().Trim();
        }
    }
}