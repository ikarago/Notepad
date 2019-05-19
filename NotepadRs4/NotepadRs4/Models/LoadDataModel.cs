using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace NotepadRs4.Models
{
    public class LoadDataModel
    {
        public bool LoadSuccessful { get; set; }
        public TextDataModel TextModel { get; set; }
        public StorageFile File { get; set; }
    }
}
