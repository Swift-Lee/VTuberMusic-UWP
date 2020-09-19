using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VTuberMusic.Tools
{
    class Collection
    {
        public static ObservableCollection<T> ListToObservableCollection<T>(T[] temp)
        {
            ObservableCollection<T> scheduleInProcessOwner = new ObservableCollection<T>();
            List<T> tempList = new List<T>();
            if (temp != null && temp.Count() > 0)
            {
                tempList = temp.ToList();
            }
            tempList.ForEach(p => scheduleInProcessOwner.Add(p));

            return scheduleInProcessOwner;
        }

        public static ObservableCollection<T> ArrayToObservableCollection<T>(T[] temp)
        {
            ObservableCollection<T> scheduleInProcessOwner = new ObservableCollection<T>();
            foreach(T tempItem in temp)
            {
                scheduleInProcessOwner.Add(tempItem);
            }
            return scheduleInProcessOwner;
        }
    }
}
