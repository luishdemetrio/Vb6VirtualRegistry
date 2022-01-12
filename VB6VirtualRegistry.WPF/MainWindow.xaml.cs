using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace VB6VirtualRegistry.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            txtUsage.Text = 
 @"Actions:
    pack
    unpack
    regsvr32
    regasm

Create virtual registry for VB6 components:
    Vb6VirtualRegistry.exe regsvr32 c:\MyApp\unpackagedFiles c:\MyApp\unpackagedFiles\registry.dat

Create virtual registry for .NET components:
    Vb6VirtualRegistry.exe regasm c:\MyApp\unpackagedFiles\VFS\SystemX86\CrystalReport13.dll c:\vb6\registry.dat

Package the folder
    Vb6VirtualRegistry.exe pack c:\MyApp\unpackagedFiles c:\MyApp\myapp.msix

Unpack a MSIX file
    Vb6VirtualRegistry.exe unpack c:\MyApp\myapp.msix c:\MyApp\unpackagedFiles";

        }
    }
}
