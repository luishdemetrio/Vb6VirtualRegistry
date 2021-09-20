VERSION 5.00
Object = "{3B7C8863-D78F-101B-B9B5-04021C009402}#1.2#0"; "RICHTX32.OCX"
Begin VB.Form Form1 
   Caption         =   "VB6 with MSIX"
   ClientHeight    =   4545
   ClientLeft      =   120
   ClientTop       =   465
   ClientWidth     =   7935
   LinkTopic       =   "Form1"
   ScaleHeight     =   4545
   ScaleWidth      =   7935
   StartUpPosition =   3  'Windows Default
   Begin RichTextLib.RichTextBox RichTextBox1 
      Height          =   1095
      Left            =   480
      TabIndex        =   3
      Top             =   120
      Width           =   6975
      _ExtentX        =   12303
      _ExtentY        =   1931
      _Version        =   393217
      Enabled         =   -1  'True
      TextRTF         =   $"Form1.frx":0000
   End
   Begin RichTextLib.RichTextBox rtbTextFile 
      Height          =   1215
      Left            =   480
      TabIndex        =   2
      Top             =   1440
      Width           =   6975
      _ExtentX        =   12303
      _ExtentY        =   2143
      _Version        =   393217
      Enabled         =   -1  'True
      TextRTF         =   $"Form1.frx":00AF
   End
   Begin VB.CommandButton Command2 
      Caption         =   "Exit"
      Height          =   975
      Left            =   5400
      TabIndex        =   1
      Top             =   3000
      Width           =   2055
   End
   Begin VB.CommandButton Command1 
      Caption         =   "Click me"
      Height          =   975
      Left            =   480
      TabIndex        =   0
      Top             =   3000
      Width           =   2055
   End
End
Attribute VB_Name = "Form1"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = True
Attribute VB_Exposed = False
Private Sub Command1_Click()
    MsgBox "Hello from MSIX!"
End Sub

Private Sub Command2_Click()
    End
End Sub

Private Sub Form_Load()

    Dim iFile
    
    Dim strFilename As String
    
    iFile = FreeFile
    'strFilename = "C:\Program Files (x86)\MyApp\myfile.txt"
    
    strFilename = "C:\github\Vb6VirtualRegistry\Sample\unpackaged\VFS\ProgramFilesX86\MyApp\myfile.txt"
    
  Open strFilename For Input As #iFile
    rtbTextFile.Text = StrConv(InputB(LOF(iFile), iFile), vbUnicode)
  Close #iFile
  
End Sub
