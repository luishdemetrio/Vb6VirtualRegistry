namespace VB6VirtualRegistry.WinForm
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            lblUsage.Text = @"For now, this tool is supported by command line.
Actions:
    pack
    unpack
    regsvr32
    regasm

Create virtual registry for VB6 components (run as admin):
    Vb6VirtualRegistry.exe regsvr32 c:\MyApp\unpackagedFiles c:\MyApp\unpackagedFiles\registry.dat

Create virtual registry for .NET components (run as admin):
    Vb6VirtualRegistry.exe regasm c:\MyApp\unpackagedFiles\VFS\SystemX86\CrystalReport13.dll c:\vb6\registry.dat

Package the folder
    Vb6VirtualRegistry.exe pack c:\MyApp\unpackagedFiles c:\MyApp\myapp.msix

Unpack a MSIX file
    Vb6VirtualRegistry.exe unpack c:\MyApp\myapp.msix c:\MyApp\unpackagedFiles";

        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}