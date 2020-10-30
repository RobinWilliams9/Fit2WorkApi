using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Fit2WorkImportUserService;

namespace FitToWorkImportServiceDebuggerForm
{
    public partial class Form1 : Form
    {

        private ImportUserService _service;

        public ImportUserService Service { 
            get {
                if (_service == null) {
                    _service = new ImportUserService();
                }
                return _service;
            }
        } 

        public Form1()
        {
            InitializeComponent();
        }

        private void btnStart_Click(object sender, EventArgs e) {
            Service.DoRun();
        }

        private void btnStop_Click(object sender, EventArgs e) {
            Service.DoStop();
        }
    }
}
