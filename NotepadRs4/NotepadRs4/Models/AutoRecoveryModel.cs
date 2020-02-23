using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace NotepadRs4.Models
{
    public class AutoRecoveryModel
    {
        StorageFile CurrentFile { get; set; }
        StorageFile TempRecoveryFile { get; set; }
        DateTimeOffset LastSavedTempFile { get; set; }
    }
}
