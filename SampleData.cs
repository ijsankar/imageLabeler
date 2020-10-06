using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Graphics.Imaging;

namespace imageLabeler
{
    public class SampleData
    {
        public StorageFile File { get; set; }
        public SampleData SampleDataHandle { get
            {
                return this;
            }
        }
        public List<Tuple<String, double, double, double, double, String>> SampleBoundsList;
        public SampleData(StorageFile file)
        {
            File = file;
        }
    }
}
