using System;
using SQLite;
using WGU_C971.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace WGU_C971.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddCourseAssessment : ContentPage
    {
        public SchoolCourse Course;
        public MainPage MainPage;

        public AddCourseAssessment(SchoolCourse course, MainPage mainPage)
        {
            InitializeComponent();
            Course = course;
            MainPage = mainPage;
        }
        private async void BtnSave_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (RequiredAssessmentFieldPopulated())
                {
                    Assessment assessment = new Assessment()
                    {
                        Name = TxtAssessmentName.Text,
                        Type = PickerAssessmentType.SelectedItem.ToString(),
                        StartDate = DatePickerStartDate.Date,
                        EndDate = DatePickerEndDate.Date.AddHours(2.5),
                        CourseId = Course.Id,
                        Notify = true
                    };

                    string perfAssessmentQuery = $"SELECT * FROM Assessment WHERE CourseId = {Course.Id} AND Type = 'Performance';";
                    string objAssessmentQuery = $"SELECT * FROM Assessment WHERE CourseId = {Course.Id} AND Type = 'Objective';";

                    using (SQLiteConnection connection = new SQLiteConnection(App.FilePath))
                    {
                        List<Assessment> perfAssessments = connection.Query<Assessment>(perfAssessmentQuery);
                        List<Assessment> objAssessments = connection.Query<Assessment>(objAssessmentQuery);
                        bool isPerfAssessment = assessment.Type.ToString().Trim() == "Performance";
                        bool isObjAssessment = assessment.Type.ToString().Trim() == "Objective";

                        if (isPerfAssessment && perfAssessments.Count == 0)
                        {
                            connection.Insert(assessment);
                            MainPage.AssessmentList.Add(assessment);
                            await Navigation.PopModalAsync();
                        }
                        else if (isObjAssessment && objAssessments.Count == 0)
                        {
                            connection.Insert(assessment);
                            MainPage.AssessmentList.Add(assessment);
                            await Navigation.PopModalAsync();
                        }
                        else
                        {
                            string warningMessage = $"You can't add more that one {assessment.Type} Assessment Type per Course.";
                            await DisplayAlert("Warning", warningMessage, "OK");
                        }
                    }
                }
                else
                {
                    if (String.IsNullOrWhiteSpace(TxtAssessmentName.Text))
                    {
                        throw new ApplicationException("Assessment Name is REQUIRED.");
                    }

                    if (PickerAssessmentType.SelectedItem == null)
                    {
                        throw new ApplicationException("Assessment Type is REQUIRED.");
                    }

                    if (String.IsNullOrWhiteSpace(DatePickerStartDate.Date.ToString()))
                    {
                        throw new ApplicationException("Start Date is REQUIRED.");
                    }

                    if (String.IsNullOrWhiteSpace(DatePickerEndDate.Date.ToString()))
                    {
                        throw new ApplicationException("End Date is REQUIRED.");
                    }
                }
            }
            catch (ApplicationException ex)
            {
                await DisplayAlert("Warning", ex.Message, "OK");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An Exception Occured: {ex.Message}");
                await DisplayAlert("Error", "An Exception Occured.", "OK");
            }
        }

        private async void BtnCancel_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }

        bool RequiredAssessmentFieldPopulated()
        {
            return !String.IsNullOrWhiteSpace(TxtAssessmentName.Text) &&
                PickerAssessmentType.SelectedItem != null &&
                !String.IsNullOrWhiteSpace(DatePickerStartDate.Date.ToString()) &&
                !String.IsNullOrWhiteSpace(DatePickerEndDate.Date.ToString());
        }
    }
}