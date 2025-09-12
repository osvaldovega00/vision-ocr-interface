'**************************************************************************************************************************************************************
'Project            : Visoin-Based OCR Reading System
'
'Program Name       : ocr-reading.sln
'
'Author             : O. Vega-Cancel
'
'Purpose            : Communicate with a Cognex In-Sight camera to perform an OCR vision reading.
'
'Revision Hystory   :
'
'Date (MM/DD/YY)        Author             Rev     Modification Reason
'12/03/19               O. Vega-Cancel     1       Initial Revision.
'02/22/20               O. Vega-Cancel     2       Include Product Selection
'06/22/21               O. Vega-Cancel     3       Include Additional Products
'10/12/21               O. Vega-Cancel     4       Include automatic trigger for guardsoft configuration download
'
'**************************************************************************************************************************************************************

Imports System.ComponentModel
Imports System.Drawing
Imports System.Windows.Forms
Imports System.Xml
Imports System.IO
Imports System.Threading
Imports Cognex.InSight
Imports Cognex.InSight.Cell


Public Class Form1
    Inherits Form

    Public WithEvents panel1 As Panel
    Public WithEvents panel2 As Panel
    Public WithEvents panel3 As Panel
    Public WithEvents panel4 As Panel
    Public WithEvents panel5 As Panel
    Public WithEvents panel6 As Panel
    Public WithEvents label1 As Label
    Public WithEvents label2 As Label
    Public WithEvents label3 As Label
    Public WithEvents label4 As Label
    Public WithEvents label5 As Label
    Public WithEvents label6 As Label
    Public WithEvents label7 As Label
    Public WithEvents label8 As Label
    Public WithEvents label9 As Label
    Public WithEvents label10 As Label
    Public WithEvents label11 As Label    'Displays OCR reading value
    Public WithEvents label12 As Label    'Displays OCR pass/fail result
    Public WithEvents label13 As Label    'Displays output messages status
    Public WithEvents label14 As Label    'Displays output messages status
    Public WithEvents label15 As Label    'Displays output messages status
    Public WithEvents label16 As Label
    Public WithEvents label17 As Label    'ComboBox Header
    Public WithEvents button1 As Button   'Start OCR Reading
    Public WithEvents button2 As Button   'Stop OCR Reading
    Public WithEvents button3 As Button   'Single Camera Trigger
    Public WithEvents button4 As Button   'Live Acquisition Start
    Public WithEvents button5 As Button   'Live Acquisition Stop
    Public WithEvents button6 As Button   'OCR/Focus Tools Start
    Public WithEvents button7 As Button   'OCR/Focus Tools Stop
    Public WithEvents button8 As Button   'User/Pass Submit for OCR/Focus tools access window
    Public WithEvents button9 As Button   'User/Pass Cancel for OCR/Focus tools access window
    Public WithEvents button10 As Button  'Final reading results Copy
    Public WithEvents textbox1 As TextBox
    Public WithEvents textbox2 As TextBox
    Public ovalshape1 As New PowerPacks.OvalShape
    Public ovalshape2 As New PowerPacks.OvalShape
    Public rect1 As New PowerPacks.RectangleShape
    Public WithEvents CvsInSightDisplay1 As Cognex.InSight.Controls.Display.CvsInSightDisplay  'Camera form display - interaction with In-Sight
    Public WithEvents backgroundWorker1 As New BackgroundWorker
    Public WithEvents ComboBox1 As New ComboBox

    'Keuboard hook for "Enter" key interaction during OCR reading
    Public WithEvents KeyboardHook As New InputHelper.Hooks.KeyboardHook()

    'Connection Variable Declarations
    Dim str_IPAddress As String           'Cmaera IP Address for connection
    Dim str_Username As String            'Camera Username for connection
    Dim str_Password As String            'Camera Password for connection
    Dim int_CharMax As Integer            'Limits characters maximum qty in output messages
    Dim int_TriggerDelay As Integer       'Holds time value (in mSec) to delay between continous triggers
    Dim int_LightTriggerDelay As Integer  'Holds time value (in mSec) to delay between Guardsoft high/low value
    Dim str_ocrUsername As String         'OCR/Focus tools access username
    Dim str_ocrPassword As String         'OCR/Focus tools access username
    Dim int_loadOnes As Integer           'Oneshot method to identify main form load
    Dim str_jobName As String             'In-Sight job file (XXX.job) name

    'XML File Reading Variable Declarations
    Dim str_xmlFile As String
    Dim xmldoc As XmlDocument = New XmlDocument

    'Password Dialog Box Declaration
    Dim form2 As New Form

    'Result Variables Declaration
    Dim result1 As CvsCell            'Camera result - OCR reading
    Dim result2 As CvsCell            'Camera result - OCR status
    Dim result3 As CvsCell            'Camera result - Focus value
    Dim trigControl As Integer        'To control trigger & results
    Dim str_finalResult As String     'Holds final OCR reading value
    Dim int_msgBoxes As Integer       'To hold message boxes user selections
    Dim int_focusMinValue As Integer  'Minimum allowed focus value during continuous focus monitoring
    Dim int_focusEnable As Integer    'To enable/disable continuous focus monitoring

    'Log Files Variables Declaration
    Dim str_Directory As String                                'Log File main directory
    Dim str_LogDirectory As String                             'Log file sub-directory
    Dim str_LogFileDirectory As String                         'Log file name
    Dim date_Dictionary As New Dictionary(Of Integer, String)  'Dictionary (key-int / value-string) for months of the year
    Dim int_enableLogs As Integer                              'To enable/display log recording
    Dim str_Month As String                                    'Holds current month of the year for log file name
    Dim str_Year As String                                     'Holds current year for log file name


    Public Sub New()

        'Main Form Properties
        Me.Size = New Size(930, 715)
        Me.Text = New String("BSC Dorado OCR Vision Reading")
        Me.FormBorderStyle = Windows.Forms.FormBorderStyle.FixedSingle
        Me.BackColor = Color.DarkGray
        Me.Icon = New Icon(AppDomain.CurrentDomain.BaseDirectory + "icon.ico")

        'Form 2 Properties
        form2.Size = New Size(280, 195)
        form2.Text = New String("Login Window")
        form2.FormBorderStyle = Windows.Forms.FormBorderStyle.FixedSingle
        form2.BackColor = Color.DarkGray
        form2.ControlBox = False

        'Panel1 Properties
        panel1 = New Panel()
        panel1.Size = New Size(275, 90)
        panel1.Location = New Point(629, 48)
        panel1.BackColor = Color.LightGray
        panel1.AutoSize = True
        panel1.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        panel1.BorderStyle = BorderStyle.Fixed3D
        Me.Controls.Add(panel1)

        'Panel 2 Properties
        panel2 = New Panel()
        panel2.Size = New Size(275, 60)
        panel2.Location = New Point(629, 171)
        panel2.BackColor = Color.LightGray
        panel2.AutoSize = True
        panel2.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        panel2.BorderStyle = BorderStyle.Fixed3D
        Me.Controls.Add(panel2)

        'Panel 3 Properties
        panel3 = New Panel()
        panel3.Size = New Size(230, 34)
        panel3.Location = New Point(19, 8)
        panel3.BackColor = Color.LightGray
        panel3.AutoSize = True
        panel3.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        panel3.BorderStyle = BorderStyle.None
        Me.panel1.Controls.Add(panel3)

        'Panel 4 Properties
        panel4 = New Panel()
        panel4.Size = New Size(230, 34)
        panel4.Location = New Point(19, 48)
        panel4.BackColor = Color.LightGray
        panel4.AutoSize = True
        panel4.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        panel4.BorderStyle = BorderStyle.None
        Me.panel1.Controls.Add(panel4)

        'Panel 5 Properties
        panel5 = New Panel()
        panel5.Size = New Size(601, 70)
        panel5.Location = New Point(11, 597)
        panel5.BackColor = Color.Transparent
        panel5.AutoSize = True
        panel5.Anchor = AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right
        panel5.BorderStyle = BorderStyle.None
        Me.Controls.Add(panel5)

        'Panel 6 Properties
        panel6 = New Panel()
        panel6.Size = New Size(230, 150)
        panel6.Location = New Point(10, 10)
        panel6.BackColor = Color.LightGray
        panel6.AutoSize = True
        panel6.BorderStyle = BorderStyle.FixedSingle
        Me.Controls.Add(panel6)

        'Label 1 Properties
        label1 = New Label()
        label1.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        label1.AutoSize = True
        label1.BackColor = Color.Yellow
        label1.ForeColor = Color.Black
        label1.Location = New Point(667, 22)
        label1.Size = New Size(198, 20)
        label1.Text = New String("In-Sight Camera Status")
        label1.Font = New Font("Microsoft Sans Serif", 12, FontStyle.Bold)
        Me.Controls.Add(label1)

        'Label 2 Properties
        label2 = New Label()
        label2.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        label2.AutoSize = True
        label2.BackColor = Color.Yellow
        label2.ForeColor = Color.Black
        label2.Location = New Point(724, 145)
        label2.Size = New Size(70, 20)
        label2.Text = New String("Results")
        label2.Font = New Font("Microsoft Sans Serif", 12, FontStyle.Bold)
        Me.Controls.Add(label2)

        'Label 3 Properties
        label3 = New Label()
        label3.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        label3.AutoSize = True
        label3.BackColor = Color.Transparent
        label3.ForeColor = Color.Black
        label3.Location = New Point(5, 5)
        label3.Size = New Size(118, 20)
        label3.Text = New String("Online Status")
        label3.Font = New Font("Microsoft Sans Serif", 12, FontStyle.Bold)
        Me.panel3.Controls.Add(label3)

        'Label 4 Properties
        label4 = New Label()
        label4.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        label4.AutoSize = True
        label4.BackColor = Color.Transparent
        label4.ForeColor = Color.Black
        label4.Location = New Point(5, 5)
        label4.Size = New Size(118, 20)
        label4.Text = New String("Reading Status")
        label4.Font = New Font("Microsoft Sans Serif", 12, FontStyle.Bold)
        Me.panel4.Controls.Add(label4)

        'Label 5 Properties
        label5 = New Label()
        label5.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        label5.AutoSize = True
        label5.BackColor = Color.LightGray
        label5.ForeColor = Color.Black
        label5.Location = New Point(4, 3)
        label5.Size = New Size(118, 20)
        label5.Text = New String("Marking Number:")
        label5.Font = New Font("Microsoft Sans Serif", 11, FontStyle.Bold)
        Me.panel2.Controls.Add(label5)

        'Label 6 Properties
        label6 = New Label()
        label6.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        label6.AutoSize = True
        label6.BackColor = Color.LightGray
        label6.ForeColor = Color.Black
        label6.Location = New Point(3, 33)
        label6.Size = New Size(118, 20)
        label6.Text = New String("OCR Reading Result:")
        label6.Font = New Font("Microsoft Sans Serif", 11, FontStyle.Bold)
        Me.panel2.Controls.Add(label6)

        'Label 7 Properties
        label7 = New Label()
        label7.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        label7.AutoSize = True
        label7.BackColor = Color.Yellow
        label7.ForeColor = Color.Black
        label7.Location = New Point(685, 549)
        label7.Size = New Size(118, 20)
        label7.Text = New String("Technical Support")
        label7.Font = New Font("Microsoft Sans Serif", 12, FontStyle.Bold)
        Me.Controls.Add(label7)

        'Label 8 Properties
        label8 = New Label()
        label8.BackColor = Color.Yellow
        label8.ForeColor = Color.Black
        label8.Location = New Point(10, 10)
        label8.Size = New Size(57, 24)
        label8.Text = New String("Login")
        label8.Font = New Font("Microsoft Sans Serif", 12, FontStyle.Bold)
        form2.Controls.Add(label8)

        'Label 9 Properties
        label9 = New Label()
        label9.Location = New Point(10, 47)
        label9.Size = New Size(74, 16)
        label9.Text = New String("Username")
        label9.Font = New Font("Microsoft Sans Serif", 10, FontStyle.Regular)
        form2.Controls.Add(label9)

        'Label 10 Properties
        label10 = New Label()
        label10.Location = New Point(10, 77)
        label10.Size = New Size(74, 16)
        label10.Text = New String("Password")
        label10.Font = New Font("Microsoft Sans Serif", 10, FontStyle.Regular)
        form2.Controls.Add(label10)

        'Label 11 Properties
        label11 = New Label()
        label11.Location = New Point(140, 4)
        label11.Size = New Size(80, 18)
        label11.BackColor = Color.Transparent
        label11.Text = New String("")
        label11.Font = New Font("Microsoft Sans Serif", 11, FontStyle.Bold)
        Me.panel2.Controls.Add(label11)

        'Label 12 Properties
        label12 = New Label()
        label12.Location = New Point(170, 34)
        label12.Size = New Size(80, 18)
        label12.BackColor = Color.Transparent
        label12.Text = New String("")
        label12.Font = New Font("Microsoft Sans Serif", 11, FontStyle.Bold)
        Me.panel2.Controls.Add(label12)

        'Label 13 Properties
        label13 = New Label()
        label13.Location = New Point(4, 4)
        label13.Size = New Size(39, 13)
        label13.AutoSize = True
        label13.BackColor = Color.Transparent
        label13.ForeColor = Color.Yellow
        label13.Text = New String("")
        label13.Font = New Font("Microsoft Sans Serif", 10, FontStyle.Bold)
        Me.panel5.Controls.Add(label13)

        'Label 14 Properties
        label14 = New Label()
        label14.Location = New Point(4, 28)
        label14.Size = New Size(39, 13)
        label14.AutoSize = True
        label14.BackColor = Color.Transparent
        label14.ForeColor = Color.Yellow
        label14.Text = New String("")
        label14.Font = New Font("Microsoft Sans Serif", 10, FontStyle.Bold)
        Me.panel5.Controls.Add(label14)

        'Label 15 Properties
        label15 = New Label()
        label15.Location = New Point(4, 52)
        label15.Size = New Size(39, 13)
        label15.AutoSize = True
        label15.BackColor = Color.Transparent
        label15.ForeColor = Color.Yellow
        label15.Text = New String("")
        label15.Font = New Font("Microsoft Sans Serif", 10, FontStyle.Bold)
        Me.panel5.Controls.Add(label15)

        'Label 16 Properties
        label16 = New Label()
        label16.BackColor = Color.Yellow
        label16.ForeColor = Color.Black
        label16.Location = New Point(16, 45)
        label16.Size = New Size(198, 48)
        label16.TextAlign = ContentAlignment.MiddleCenter
        label16.Text = New String("Press Enter to Transfer" + Environment.NewLine + "Reading to MES")
        label16.Font = New Font("Microsoft Sans Serif", 12, FontStyle.Bold)
        Me.panel6.Controls.Add(label16)

        'Label 17 Properties
        label17 = New Label()
        label17.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        label17.AutoSize = True
        label17.BackColor = Color.Yellow
        label17.ForeColor = Color.Black
        label17.Location = New Point(685, 488)
        label17.Size = New Size(198, 20)
        label17.Text = New String("Product Selection")
        label17.Font = New Font("Microsoft Sans Serif", 12, FontStyle.Bold)
        Me.Controls.Add(label17)

        'ComboBox Properties
        ComboBox1 = New ComboBox
        ComboBox1.BackColor = System.Drawing.Color.FromArgb(224, 224, 224)
        ComboBox1.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        ComboBox1.AutoSize = True
        ComboBox1.Location = New Point(623, 518)
        ComboBox1.Size = New Size(280, 25)
        ComboBox1.FlatStyle = FlatStyle.System
        ComboBox1.Font = New Font("Microsoft Sans Serif", 8, FontStyle.Regular)
        ComboBox1.DropDownStyle = ComboBoxStyle.DropDownList
        Me.Controls.Add(ComboBox1)

        'Button 1 Properties
        button1 = New Button()
        button1.Size = New Size(280, 70)
        button1.Location = New Point(623, 253)
        button1.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        button1.AutoSize = True
        button1.FlatStyle = FlatStyle.Popup
        button1.BackColor = System.Drawing.Color.FromArgb(224, 224, 224)
        button1.Font = New Font("Microsoft Sans Serif", 12, FontStyle.Bold)
        button1.Text = "Start Vision" + Environment.NewLine + "Reading"
        Me.Controls.Add(button1)

        'Button 2 Properties
        button2 = New Button()
        button2.Size = New Size(280, 70)
        button2.Location = New Point(623, 253)
        button2.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        button2.AutoSize = True
        button2.FlatStyle = FlatStyle.Popup
        button2.BackColor = System.Drawing.Color.FromArgb(224, 224, 224)
        button2.Font = New Font("Microsoft Sans Serif", 12, FontStyle.Bold)
        button2.Text = "Stop Vision" + Environment.NewLine + "Reading"
        Me.Controls.Add(button2)

        'Button 3 Properties
        button3 = New Button()
        button3.Size = New Size(280, 70)
        button3.Location = New Point(623, 330)
        button3.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        button3.AutoSize = True
        button3.FlatStyle = FlatStyle.Popup
        button3.BackColor = System.Drawing.Color.FromArgb(224, 224, 224)
        button3.Font = New Font("Microsoft Sans Serif", 12, FontStyle.Bold)
        button3.Text = "Single Camera" + Environment.NewLine + "Trigger"
        Me.Controls.Add(button3)

        'Button 4 Properties
        button4 = New Button()
        button4.Size = New Size(280, 70)
        button4.Location = New Point(623, 408)
        button4.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        button4.AutoSize = True
        button4.FlatStyle = FlatStyle.Popup
        button4.BackColor = System.Drawing.Color.FromArgb(224, 224, 224)
        button4.Font = New Font("Microsoft Sans Serif", 12, FontStyle.Bold)
        button4.Text = "Live Camera" + Environment.NewLine + "Acquisition - Start"
        Me.Controls.Add(button4)

        'Button 5 Properties
        button5 = New Button()
        button5.Size = New Size(280, 70)
        button5.Location = New Point(623, 408)
        button5.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        button5.AutoSize = True
        button5.FlatStyle = FlatStyle.Popup
        button5.BackColor = System.Drawing.Color.FromArgb(224, 224, 224)
        button5.Font = New Font("Microsoft Sans Serif", 12, FontStyle.Bold)
        button5.Text = "Live Camera" + Environment.NewLine + "Acquisition - Stop"
        Me.Controls.Add(button5)

        'Button 6 Properties
        button6 = New Button()
        button6.Size = New Size(280, 70)
        button6.Location = New Point(623, 580)
        button6.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        button6.AutoSize = True
        button6.FlatStyle = FlatStyle.Popup
        button6.BackColor = System.Drawing.Color.FromArgb(224, 224, 224)
        button6.Font = New Font("Microsoft Sans Serif", 12, FontStyle.Bold)
        button6.Text = "OCR / Focus" + Environment.NewLine + "Tools - Start"
        Me.Controls.Add(button6)

        'Button 7 Properties
        button7 = New Button()
        button7.Size = New Size(280, 70)
        button7.Location = New Point(623, 580)
        button7.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        button7.AutoSize = True
        button7.FlatStyle = FlatStyle.Popup
        button7.BackColor = System.Drawing.Color.FromArgb(224, 224, 224)
        button7.Font = New Font("Microsoft Sans Serif", 12, FontStyle.Bold)
        button7.Text = "OCR / Focus" + Environment.NewLine + "Tools - Stop"
        Me.Controls.Add(button7)

        'Button 8 Properties
        button8 = New Button()
        button8.Size = New Size(107, 23)
        button8.Location = New Point(10, 110)
        button8.FlatStyle = FlatStyle.System
        button8.BackColor = System.Drawing.Color.Yellow
        button8.Font = New Font("Microsoft Sans Serif", 9, FontStyle.Regular)
        button8.Text = "Submit"
        form2.Controls.Add(button8)

        'Button 9 Properties
        button9 = New Button()
        button9.Size = New Size(107, 23)
        button9.Location = New Point(139, 110)
        button9.FlatStyle = FlatStyle.System
        button9.BackColor = System.Drawing.Color.FromArgb(224, 224, 224)
        button9.Font = New Font("Microsoft Sans Serif", 9, FontStyle.Regular)
        button9.Text = "Cancel"
        form2.Controls.Add(button9)

        'Button 10 Properties
        button10 = New Button()
        button10.Size = New Size(107, 30)
        button10.Location = New Point(60, 95)
        button10.FlatStyle = FlatStyle.Popup
        button10.BackColor = Color.Yellow
        button10.ForeColor = Color.Black
        button10.Font = New Font("Microsoft Sans Serif", 9, FontStyle.Bold)
        button10.Text = "Copy"
        button10.Visible = False
        Me.panel6.Controls.Add(button10)

        'TextBox 1 Properties
        textbox1 = New TextBox
        textbox1.Size = New Size(155, 20)
        textbox1.Location = New Point(91, 47)
        form2.Controls.Add(textbox1)

        'TextBox 2 Properties
        textbox2 = New TextBox
        textbox2.Size = New Size(155, 20)
        textbox2.Location = New Point(91, 77)
        textbox2.PasswordChar = "*"
        form2.Controls.Add(textbox2)

        'LED 1 Properties
        ovalshape1 = New PowerPacks.OvalShape
        ovalshape1.Size = New Size(27, 28)
        ovalshape1.Location = New Point(177, 2)
        ovalshape1.BackColor = Color.Black
        ovalshape1.BackStyle = PowerPacks.BackStyle.Opaque
        ovalshape1.BorderStyle = Drawing2D.DashStyle.Solid
        ovalshape1.BorderWidth = 1
        ovalshape1.FillColor = Color.LightGray
        Dim sc As New PowerPacks.ShapeContainer
        ovalshape1.Parent = sc
        Me.panel3.Controls.Add(sc)

        'LED 2 Properties
        ovalshape2 = New PowerPacks.OvalShape
        ovalshape2.Size = New Size(27, 28)
        ovalshape2.Location = New Point(177, 2)
        ovalshape2.BackColor = Color.Black
        ovalshape2.BackStyle = PowerPacks.BackStyle.Opaque
        ovalshape2.BorderStyle = Drawing2D.DashStyle.Solid
        ovalshape2.BorderWidth = 1
        ovalshape2.FillColor = Color.LightGray
        Dim sc1 As New PowerPacks.ShapeContainer
        ovalshape2.Parent = sc1
        Me.panel4.Controls.Add(sc1)

        'Rectangle 1 Properties
        rect1 = New PowerPacks.RectangleShape
        rect1.Size = New Size(291, 227)
        rect1.Location = New Point(-145, 13)
        rect1.BackStyle = PowerPacks.BackStyle.Transparent
        rect1.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        rect1.BorderColor = Color.Yellow
        rect1.BorderWidth = 5
        rect1.BorderStyle = Drawing2D.DashStyle.Solid
        rect1.FillColor = Color.Black
        rect1.FillGradientColor = Color.LightGray
        rect1.BringToFront()
        rect1.Visible = True
        Dim sc2 As New PowerPacks.ShapeContainer
        rect1.Parent = sc2
        Me.Controls.Add(sc2)

        'Camera Display Properties
        CvsInSightDisplay1 = New Cognex.InSight.Controls.Display.CvsInSightDisplay
        CvsInSightDisplay1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        CvsInSightDisplay1.DefaultTextScaleMode = Cognex.InSight.Controls.Display.CvsInSightDisplay.TextScaleModeType.Proportional
        CvsInSightDisplay1.DialogIcon = Nothing
        CvsInSightDisplay1.Location = New System.Drawing.Point(11, 11)
        CvsInSightDisplay1.Name = "CvsInSightDisplay1"
        CvsInSightDisplay1.Size = New System.Drawing.Size(600, 580)
        CvsInSightDisplay1.TabIndex = 0
        CvsInSightDisplay1.ImageZoomMode = Cognex.InSight.Controls.Display.CvsDisplayZoom.Fit
        CvsInSightDisplay1.AutoReconnect = True
        CvsInSightDisplay1.ShowGraphics = True
        CvsInSightDisplay1.Visible = True
        Me.Controls.Add(CvsInSightDisplay1)

        'Background Worker Properties
        backgroundWorker1 = New BackgroundWorker
        backgroundWorker1.WorkerReportsProgress = True
        backgroundWorker1.WorkerSupportsCancellation = True

        'Calendar Dictionary Creation
        date_Dictionary.Add(1, "January")
        date_Dictionary.Add(2, "February")
        date_Dictionary.Add(3, "March")
        date_Dictionary.Add(4, "April")
        date_Dictionary.Add(5, "May")
        date_Dictionary.Add(6, "June")
        date_Dictionary.Add(7, "July")
        date_Dictionary.Add(8, "August")
        date_Dictionary.Add(9, "September")
        date_Dictionary.Add(10, "October")
        date_Dictionary.Add(11, "November")
        date_Dictionary.Add(12, "December")


    End Sub


    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        trigControl = 0

        'Verifies if XML file exists and if not triggers a message and closes
        str_xmlFile = "C:\Vision-Based OCR Reading System Project\Settings\OCR Vision Reading Settings.xml"
        If Not File.Exists(str_xmlFile) Then
            Try
                MsgBox("Unable to open XML settings file, contact support.", MsgBoxStyle.Critical)
                Me.Close()
            Catch ex As Exception
            End Try
        End If

        Me.WindowState = FormWindowState.Maximized
        panel6.Visible = False
        label11.Text = ""
        label12.Text = ""
        button1.Enabled = False
        button2.Enabled = False
        button2.Visible = False
        button3.Enabled = False
        button4.Enabled = False
        button5.Enabled = False
        button5.Visible = False
        button6.Enabled = False
        button7.Enabled = False
        button7.Visible = False
        ComboBox1.Enabled = False
        label13.Text = Now + " - In-Sight sensor connecting..."

        'Releases keyboard hook
        KeyboardHook.Dispose()

        'XML File Load and Variable Assignment
        Try
            xmldoc.Load(str_xmlFile)
            str_IPAddress = xmldoc.SelectSingleNode("Data/Camera/IP").InnerText
            str_Username = xmldoc.SelectSingleNode("Data/Camera/Username").InnerText
            str_Password = xmldoc.SelectSingleNode("Data/Camera/Password").InnerText
            int_TriggerDelay = xmldoc.SelectSingleNode("Data/Camera/TriggerDelay").InnerText
            'int_LightTriggerDelay = xmldoc.SelectSingleNode("Data/Camera/LightTriggerDelay").InnerText
            str_jobName = xmldoc.SelectSingleNode("Data/Camera/JobName").InnerText
            int_CharMax = xmldoc.SelectSingleNode("Data/Output/Lenght").InnerText
            str_ocrUsername = xmldoc.SelectSingleNode("Data/Access/OCR_Username").InnerText
            str_ocrPassword = xmldoc.SelectSingleNode("Data/Access/OCR_Password").InnerText
            str_Directory = xmldoc.SelectSingleNode("Data/LogFiles/LogDirectory").InnerText
            int_enableLogs = xmldoc.SelectSingleNode("Data/LogFiles/LogEnable").InnerText
            int_focusEnable = xmldoc.SelectSingleNode("Data/Focus/FocusEnable").InnerText
            int_focusMinValue = xmldoc.SelectSingleNode("Data/Focus/FocusMinValue").InnerText
        Catch ex As Exception
            Call OutputMessages(Now + " - " + ex.Message)
        End Try

        'Load Product Names from XML file to ComboBox List
        Try
            Dim xelement As XElement = xelement.Load(str_xmlFile)
            For Each xEle As XElement In xelement.Descendants("Name")
                ComboBox1.Items.Add(CStr(xEle))
                Debug.Print(CStr(xEle))
            Next xEle
        Catch ex As Exception
            Call OutputMessages(Now + " - " + ex.Message)
        End Try

        'To connect the form camera display to In-Sight
        Try
            CvsInSightDisplay1.Connect(str_IPAddress, str_Username, str_Password, True)
        Catch ex As Exception
            Call OutputMessages(Now + " - " + ex.Message)
        End Try

        'Oneshot method to identify form loaded
        int_loadOnes = 1

        'Call made with empty strings only verifies if directory exists
        Call LogFiles("")

    End Sub


    Private Sub Form1_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing

        'To disconnect/dispose camera display & release keyboard
        Try
            CvsInSightDisplay1.Disconnect()
            CvsInSightDisplay1.Dispose()
            KeyboardHook.Dispose()
        Catch ex As Exception
            Call OutputMessages(Now + " - " + ex.Message)
        End Try

        'Logs when form is closed
        Call OutputMessages(Now + " - BSC Dorado OCR reading application closed")

    End Sub


    Private Sub button3_Click(ByVal sender As Object, ByVal e As EventArgs) Handles button3.Click

        'To control when to display OCR results in windows form
        If trigControl = 0 Then
            label11.Text = ""
            label12.Text = ""
        End If

        'Single Camera Trigger
        Try
            CvsInSightDisplay1.InSight.SoftOnline = False
            CvsInSightDisplay1.InSight.ManualAcquire(1)
        Catch ex As Exception
            Call OutputMessages(Now + " - " + ex.Message)
        End Try

    End Sub


    <STAThread()> _
    Shared Sub Main()
        Application.EnableVisualStyles()
        Application.Run(New Form1())

    End Sub


    Private Sub CvsInSightDisplay1_ConnectedChanged(sender As Object, e As EventArgs) Handles CvsInSightDisplay1.ConnectedChanged
        'Controls form behaviour and informs user of camera connection status

        If CvsInSightDisplay1.Connected Then
            panel3.BackColor = System.Drawing.Color.FromArgb(192, 255, 192)
            ovalshape1.BackColor = System.Drawing.Color.FromArgb(0, 192, 0)
            button1.Enabled = True
            button3.Enabled = True
            button4.Enabled = True
            button6.Enabled = True
            ComboBox1.Enabled = True
            Call OutputMessages(Now + " - In-Sight sensor connected")
            ComboBox1.SelectedItem = ComboBox1.Items.Item(0)

            If int_loadOnes = 1 Then
                Try
                    Call button3_Click(Nothing, Nothing)
                Catch ex As Exception
                    Call OutputMessages(Now + " - " + ex.Message)
                End Try
                int_loadOnes = 0
            End If

        Else
            panel3.BackColor = System.Drawing.Color.FromArgb(255, 192, 192)
            ovalshape1.BackColor = System.Drawing.Color.Red
            button1.Enabled = False
            button3.Enabled = False
            button4.Enabled = False
            button6.Enabled = False
            ComboBox1.Enabled = False
            Call OutputMessages(Now + " - In-Sight sensor not connected")
        End If

    End Sub


    Private Sub CvsInSightDisplay1_ResultsChanged(sender As Object, e As EventArgs) Handles CvsInSightDisplay1.ResultsChanged

        'Clears previous results
        label11.Text = ""
        label12.Text = ""

        If CvsInSightDisplay1.Connected = True Then
            Try
                'Acquires results from In-Sight spreadsheet
                result1 = CvsInSightDisplay1.Results.Cells.Item(5)
                result2 = CvsInSightDisplay1.Results.Cells.Item(4)
                result3 = CvsInSightDisplay1.Results.Cells.Item(6)
            Catch ex As Exception
            End Try

            If trigControl = 1 Then
                Try
                    'Verifies focus result against minimum value if enabled and responds accordingly
                    If int_focusEnable = 1 And result3.Text < int_focusMinValue Then
                        panel4.BackColor = System.Drawing.Color.FromArgb(255, 192, 192)
                        ovalshape2.BackColor = System.Drawing.Color.Red
                        Call OutputMessages(Now + " - Focus measurement below minimum setpoint of " + int_focusMinValue.ToString)
                        MsgBox("Focus measurement below " + int_focusMinValue.ToString + ", contact support.", MsgBoxStyle.Exclamation)
                        Call button2_Click(Nothing, Nothing)
                        Exit Sub
                    End If

                    'Sets form behaviour depending on OCR status result
                    If result2.ToString = "1" Then
                        panel4.BackColor = System.Drawing.Color.FromArgb(192, 255, 192)
                        ovalshape2.BackColor = System.Drawing.Color.FromArgb(0, 192, 0)
                    Else
                        panel4.BackColor = System.Drawing.Color.FromArgb(255, 192, 192)
                        ovalshape2.BackColor = System.Drawing.Color.Red
                    End If
                Catch ex As Exception
                    Call OutputMessages(Now + " - " + ex.Message)
                End Try

                'Makes "copy result panel" visible and hooks/unhooks keyboard depending on OCR reading status result
                If result2.ToString = "1" And panel6.Visible = False And trigControl = 1 Then
                    trigControl = 0
                    panel6.Visible = True
                    Try
                        KeyboardHook = New InputHelper.Hooks.KeyboardHook()
                    Catch ex As Exception
                        Call OutputMessages(Now + " - " + ex.Message)
                    End Try

                ElseIf result2.ToString <> "1" And panel6.Visible = True And trigControl = 1 Then
                    panel6.Visible = False
                    Try
                        KeyboardHook.Dispose()
                    Catch ex As Exception
                        Call OutputMessages(Now + " - " + ex.Message)
                    End Try

                End If

            End If

        End If

    End Sub


    Private Sub button1_Click(sender As Object, e As EventArgs) Handles button1.Click

        trigControl = 1

        'Sets form behaviour during continous trigger inspection
        button1.Enabled = False
        button1.Visible = False
        button2.Enabled = True
        button2.Visible = True
        button3.Enabled = False
        button4.Enabled = False
        button6.Enabled = False
        ComboBox1.Enabled = False
        Me.ControlBox = False

        'Clears clipboard before using it during continous trigger
        Clipboard.Clear()

        'Starts background worker thread used for continous camera trigger
        backgroundWorker1.RunWorkerAsync()

    End Sub


    Private Sub button2_Click(sender As Object, e As EventArgs) Handles button2.Click

        trigControl = 0

        Try
            CvsInSightDisplay1.InSight.SoftOnline = True
        Catch ex As Exception
            Call OutputMessages(Now + " - " + ex.Message)
        End Try

        'Sets form behaviour after continous trigger is stoped
        panel6.Visible = False
        button2.Enabled = False
        button2.Visible = False
        button1.Enabled = True
        button1.Visible = True
        button3.Enabled = True
        button4.Enabled = True
        button6.Enabled = True
        ComboBox1.Enabled = True
        panel4.BackColor = Color.LightGray
        ovalshape2.BackColor = Color.Black
        Me.ControlBox = True

        KeyboardHook.Dispose()
        backgroundWorker1.CancelAsync()

    End Sub


    Private Sub button4_Click(sender As Object, e As EventArgs) Handles button4.Click

        'Starts camera live acquisition
        Try
            CvsInSightDisplay1.InSight.LiveAcquisition = True
        Catch ex As Exception
            Call OutputMessages(Now + " - " + ex.Message)
        End Try

        'Sets form behaviour during live camera acquisition
        button1.Enabled = False
        button3.Enabled = False
        button4.Enabled = False
        button4.Visible = False
        button5.Visible = True
        button5.Enabled = True
        button6.Enabled = False
        ComboBox1.Enabled = False

    End Sub


    Private Sub button5_Click(sender As Object, e As EventArgs) Handles button5.Click

        'Stops live camera qcquisition
        Try
            CvsInSightDisplay1.InSight.LiveAcquisition = False
        Catch ex As Exception
            Call OutputMessages(Now + " - " + ex.Message)
        End Try

        'Sets form behaviour after live camera acquisition is stoped
        button1.Enabled = True
        button3.Enabled = True
        button4.Enabled = True
        button4.Visible = True
        button5.Visible = False
        button6.Enabled = True
        ComboBox1.Enabled = True

    End Sub


    Private Sub button6_Click(sender As Object, e As EventArgs) Handles button6.Click

        'OCR/Focus tools access
        button1.Enabled = False
        button3.Enabled = False
        button4.Enabled = False
        button6.Enabled = False
        ComboBox1.Enabled = False
        form2.Show()
        textbox1.Focus()

    End Sub


    Private Sub button7_Click(sender As Object, e As EventArgs) Handles button7.Click

        'Asks user to save In-Sight job file after spreadseet is closed 
        int_msgBoxes = MsgBox("Would you like to save Job File?", MsgBoxStyle.YesNo)
        If int_msgBoxes = 6 Then
            Try
                'Saves Job
                CvsInSightDisplay1.InSight.File.SaveJobFile(str_jobName)
                Call OutputMessages(Now + " - " + str_jobName + " saved.")
            Catch ex As Exception
                Call OutputMessages(Now + " - " + ex.Message)
            End Try

        End If

        'Closes spreadsheet grid in window form
        Try
            CvsInSightDisplay1.ShowGrid = False
        Catch ex As Exception
            Call OutputMessages(Now + " - " + ex.Message)
        End Try

        'Sets form behaviour after spreadsheet is closed
        button7.Enabled = False
        button7.Visible = False
        button1.Enabled = True
        button4.Enabled = True
        button6.Enabled = True
        button6.Visible = True
        ComboBox1.Enabled = True

    End Sub


    Private Sub button8_Click(sender As Object, e As EventArgs) Handles button8.Click

        'Verifies username and password provided by user and sets form accordingly
        If textbox1.Text.ToLower = str_ocrUsername And textbox2.Text = str_ocrPassword Then
            form2.Hide()
            textbox1.Clear()
            textbox2.Clear()
            button3.Enabled = True
            button6.Visible = False
            button7.Enabled = True
            button7.Visible = True
            ComboBox1.Enabled = True

            'If user/pass are correct displays spreadsheet grid to power-user
            Try
                CvsInSightDisplay1.ShowGrid = True
            Catch ex As Exception
                Call OutputMessages(Now + " - " + ex.Message)
            End Try

        Else
            MsgBox("Sorry, incorrect login details. Please try again.", MsgBoxStyle.Exclamation)
            textbox1.Clear()
            textbox2.Clear()
        End If

    End Sub


    Private Sub button9_Click(sender As Object, e As EventArgs) Handles button9.Click

        'Sets form behaviour after Cancel was pressed on Login window
        form2.Hide()
        textbox1.Clear()
        textbox2.Clear()
        button1.Enabled = True
        button3.Enabled = True
        button4.Enabled = True
        button6.Enabled = True
        ComboBox1.Enabled = True

    End Sub


    Sub OutputMessages(statusMessages As String)

        'Displays log messages to user
        label15.Text = label14.Text
        label14.Text = label13.Text

        'Verifes if the lenght of the messages exceeds minimum and removes excess characters to avoid label misalignment on windows form
        If statusMessages.Length > int_CharMax Then
            label13.Text = statusMessages.Remove(int_CharMax)
            Call LogFiles(label13.Text)
        Else
            label13.Text = statusMessages
            Call LogFiles(label13.Text)
        End If

    End Sub


    Sub LogFiles(logMessage As String)

        If int_enableLogs = 1 Then

            'Creates directory name depending on the current month and year
            str_Month = date_Dictionary.Item(Now.Month)
            str_Year = CStr(Now.Year)
            str_LogDirectory = str_Directory + "\" + str_Month + "_" + str_Year

            'Creates directory if it does not exists
            If Not Directory.Exists(str_LogDirectory) Then
                Try
                    Directory.CreateDirectory(str_LogDirectory)
                Catch ex As Exception
                    Call OutputMessages(Now + " - " + ex.Message)
                End Try
            End If

            If logMessage.Length >= 1 Then

                'Creates log file name depending on the current month and year
                str_LogFileDirectory = str_LogDirectory + "\" + CStr(Now.Day) + str_Month + str_Year + ".txt"

                'Creates log file if it does not exists and append message
                If File.Exists(str_LogFileDirectory) Then
                    Try
                        File.AppendAllText(str_LogFileDirectory, vbNewLine + logMessage)
                    Catch ex As Exception
                        Call OutputMessages(Now + " - " + ex.Message)
                    End Try

                    'If file exists, only appends message
                Else
                    Try
                        File.AppendAllText(str_LogFileDirectory, "Date                   Message" + vbNewLine + logMessage)
                    Catch ex As Exception
                        Call OutputMessages(Now + " - " + ex.Message)
                    End Try

                End If

            End If

        End If

    End Sub


    Private Sub backgroundWorker1_DoWork(ByVal sender As System.Object, ByVal e As DoWorkEventArgs) Handles backgroundWorker1.DoWork

        'Continous trigger start using background worker
        While trigControl = 1
            Call button3_Click(Nothing, Nothing)
            Thread.Sleep(int_TriggerDelay)
        End While

    End Sub


    Private Sub button10_Click(sender As Object, e As EventArgs) Handles button10.Click

        'Sets form behaviour during OCR continous reading results recording
        label11.Text = result1.Text
        label12.Text = "Pass"
        str_finalResult = result1.ToString

        'Sends copy/paste and records values
        Try
            Clipboard.SetText(str_finalResult)
            My.Computer.Keyboard.SendKeys("^v")
            Call OutputMessages(Now + " - " + "OCR reading value " + str_finalResult + " recorded.")
        Catch ex As Exception
            Call OutputMessages(Now + " - " + ex.Message)
        End Try

        label11.Text = result1.Text
        label12.Text = "Pass"

        'Continous trigger end
        Try
            Call button2_Click(Nothing, Nothing)
        Catch ex As Exception
            Call OutputMessages(Now + " - " + ex.Message)
        End Try


    End Sub


    Private Sub KeyboardHook_KeyDown(sender As System.Object, e As InputHelper.EventArgs.KeyboardHookEventArgs) Handles KeyboardHook.KeyDown

        'Keyboard hook monitors enter key during continous trigger OCR reading
        If e.KeyCode = Keys.Enter Then

            'Call is made to copy button when "Enter" key is pressed
            Try
                Call button10_Click(Nothing, Nothing)
            Catch ex As Exception
                Call OutputMessages(Now + " - " + ex.Message)
            End Try
        End If

    End Sub


    Private Sub ComboBox1_SelectedValueChanged(sender As Object, e As EventArgs) Handles ComboBox1.SelectedValueChanged

        Call OutputMessages(Now + " - " + ComboBox1.SelectedItem + " product selected.")

        Try
            CvsInSightDisplay1.InSight.SoftOnline = True
            CvsInSightDisplay1.InSight.SetInteger(15, 0, ComboBox1.SelectedIndex)
            CvsInSightDisplay1.InSight.SetInteger(15, 1, 1)
            CvsInSightDisplay1.InSight.SetInteger(15, 1, 0)
        Catch ex As Exception
            Call OutputMessages(Now + " - " + "Product Selection / " + ex.Message)
        Finally
            CvsInSightDisplay1.InSight.SoftOnline = False
        End Try




    End Sub

End Class