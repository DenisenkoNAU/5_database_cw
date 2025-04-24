using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CourseWork
{
    public class EventBus
    {
        private static readonly Lazy<EventBus> instance = new Lazy<EventBus>(() => new EventBus());
        public static EventBus Instance => instance.Value;

        public event EventHandler? SpecialitiesUpdated;
        public event EventHandler? DisciplineUpdated;
        public event EventHandler? GroupUpdated;
        public event EventHandler? StudentUpdated;
        public event EventHandler? TeacherUpdated;

        public void OnSpecialitiesUpdated()
        {
            SpecialitiesUpdated?.Invoke(this, EventArgs.Empty);
        }

        public void OnDisciplineUpdated()
        {
            DisciplineUpdated?.Invoke(this, EventArgs.Empty);
        }

        public void OnGroupUpdated()
        {
            GroupUpdated?.Invoke(this, EventArgs.Empty);
        }

        public void OnStudentUpdated()
        {
            StudentUpdated?.Invoke(this, EventArgs.Empty);
        }

        public void OnTeacherUpdated()
        {
            TeacherUpdated?.Invoke(this, EventArgs.Empty);
        }
    }
}
