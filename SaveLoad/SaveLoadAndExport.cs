using Microsoft.Win32;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MissionAssistant
{
    static class SaveLoadAndExport
    {
        public static void LoadData(ref ObservableCollection<object> drawnItems)
        {
            OpenFileDialog openbox = new OpenFileDialog
            {
                Filter = "Mission Assistant Project File (*.pma) | *.pma",
                CheckFileExists = true,
            };

            if (openbox.ShowDialog().GetValueOrDefault())
            {
                drawnItems.Clear();
                var items = ObjectSerialization.DeSerializeData(openbox.FileName);
                foreach (var item in items) drawnItems.Add(item);
            }
        }

        public static void SaveData(List<object> Items)
        {
            SaveFileDialog savebox = new SaveFileDialog
            {
                Filter = "Mission Assistant Project File (*.pma) | *.pma"
            };
            if (savebox.ShowDialog().GetValueOrDefault())
            {
                ObjectSerialization.SerializeData(Items, savebox.FileName);
            }
        }
    }
}
