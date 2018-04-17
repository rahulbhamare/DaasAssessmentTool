using AssessmentLibrary;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AssessmentLibrary.TestCaseRunner;

namespace DaasAssessmentTool.ViewModel
{
    public class TestResultViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private ObservableCollection<TestCaseResult> _testResultObservableCollection;
        public ObservableCollection<TestCaseResult> TestResultObservableCollection
        {
            get
            {
                return _testResultObservableCollection;
            }
            set
            {
                _testResultObservableCollection = value;
                Notify("TestResultObservableCollection");
            }
        }

        public TestResultViewModel()
        {
            TestResultObservableCollection = new ObservableCollection<TestCaseResult>();
        }

        private void Notify(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
