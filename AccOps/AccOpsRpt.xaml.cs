using System.Windows.Controls;

namespace AccOps
{
    /// <summary>
    /// Interaction logic for AccOpsRpt.xaml
    /// </summary>
    public partial class AccOpsRpt : Page
    {
        public AccOpsRpt()
        {
            InitializeComponent();
        }

        // Custom constructor to pass expense report data
        public AccOpsRpt(object data) : this()
        {
            // Bind to expense report data.
            this.DataContext = data;
        }
    }
}
