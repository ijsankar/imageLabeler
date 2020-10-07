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
       
        public List<Tuple<double, double, double, double, String>> SampleBoundsList;
        public SampleData(StorageFile file, List<Tuple<double, double, double, double, String>> SampleBoundsList)
        {
            File = file;
            this.SampleBoundsList = SampleBoundsList;
        }
    }
}
