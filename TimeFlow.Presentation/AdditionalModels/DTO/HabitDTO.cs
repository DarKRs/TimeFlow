using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TimeFlow.Domain.Entities;

namespace TimeFlow.Presentation.AdditionalModels.DTO
{
    public class HabitDTO : INotifyPropertyChanged
    {
        private string _name;
        private string _description;

        public int Id { get; set; }

        public string Name
        {
            get => _name;
            set { if (_name != value) { _name = value; OnPropertyChanged(); } }
        }

        public string Description
        {
            get => _description;
            set { if (_description != value) { _description = value; OnPropertyChanged(); } }
        }

        public DateTime CreatedDate { get; set; }
        public bool IsActive { get; set; }
        public int AllowedMissedDays { get; set; }

        public ObservableCollection<HabitRecordDTO> DisplayedRecords { get; set; }
            = new ObservableCollection<HabitRecordDTO>();

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public class HabitRecordDTO : INotifyPropertyChanged
    {
        private CompletionStatus _status;

        public int Id { get; set; }
        public int HabitId { get; set; }
        public DateTime Date { get; set; }

        public CompletionStatus Status
        {
            get => _status;
            set { if (_status != value) { _status = value; OnPropertyChanged(); } }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public class HabitPeriodicityDTO
    {
        public Frequency FrequencyType { get; set; }
        public string DaysOfWeek { get; set; }
    }

    public class HabitStageDTO
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public CompletionStatus Status { get; set; }
        public int Order { get; set; }
    }

}
