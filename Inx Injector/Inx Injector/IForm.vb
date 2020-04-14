Imports InjectionLibrary
Imports JLibrary.PortableExecutable
Imports System.IO
Imports Microsoft.VisualBasic.CompilerServices
Public Class IForm
#Region "Declares"
    Dim procname As String
    Dim DllPath As String
#End Region
#Region "Injection Types"
    Public Function MMInject(ByVal pID As Integer, ByVal DLL As Byte()) As Boolean
        Try
            Dim injector As InjectionMethod = InjectionMethod.Create(InjectionMethodType.ManualMap)
            Dim hModule As IntPtr = IntPtr.Zero
            Using img As New PortableExecutable(DLL)
                hModule = injector.Inject(img, pID)
            End Using
            If hModule <> IntPtr.Zero Then
                DLL = Nothing
                Label11.Text = "DLL Injected Successfully !"
            Else
                DLL = Nothing
                Label11.Text = "Injection Failed !"
            End If
        Catch ex As Exception
            EForm.Label6.Text = (ex.Message.ToString & 47)
            EForm.ShowDialog()
            ProjectData.ClearProjectError()
        End Try
    End Function
    Public Function THInject(ByVal pID As Integer, ByVal DLL As Byte()) As Boolean
        Try
            Dim injector As InjectionMethod = InjectionMethod.Create(InjectionMethodType.ThreadHijack)
            Dim hModule As IntPtr = IntPtr.Zero
            Using img As New PortableExecutable(DLL)
                hModule = injector.Inject(img, pID)
            End Using
            If hModule <> IntPtr.Zero Then
                'DLL = Nothing
                Label11.Text = "DLL Injected Successfully !"
            Else
                'DLL = Nothing
                Label11.Text = "Injected & Known Error !"
            End If
        Catch ex As Exception
            EForm.Label6.Text = (ex.Message.ToString & 47)
            EForm.ShowDialog()
            ProjectData.ClearProjectError()
        End Try
    End Function
    Public Function LLInject(ByVal pID As Integer, ByVal DLL As Byte()) As Boolean
        Try
            Dim injector As InjectionMethod = InjectionMethod.Create(InjectionMethodType.Standard)
            Dim hModule As IntPtr = IntPtr.Zero
            Using img As New PortableExecutable(DLL)
                hModule = injector.Inject(img, pID)
            End Using
            If hModule <> IntPtr.Zero Then
                DLL = Nothing
                Label11.Text = "DLL Injected Successfully !"
                Return True
            Else
                DLL = Nothing
                Label11.Text = "Injection Failed !"
                Return False
            End If
        Catch ex As Exception
            EForm.Label6.Text = (ex.Message.ToString & 47)
            EForm.ShowDialog()
            ProjectData.ClearProjectError()
        End Try
    End Function
#End Region
#Region "Drag"
    Private newpoint As System.Drawing.Point
    Private xpos1 As Integer
    Private ypos1 As Integer
    Private Sub Panel1_MouseDown(sender As Object, e As MouseEventArgs) Handles Panel1.MouseDown, Label1.MouseDown
        xpos1 = Control.MousePosition.X - Me.Location.X
        ypos1 = Control.MousePosition.Y - Me.Location.Y
    End Sub
    Private Sub Panel1_MouseMove(sender As Object, e As MouseEventArgs) Handles Panel1.MouseMove, Label1.MouseMove
        If e.Button = Windows.Forms.MouseButtons.Left Then
            newpoint = Control.MousePosition
            newpoint.X -= (xpos1)
            newpoint.Y -= (ypos1)
            Me.Location = newpoint
        End If
    End Sub
#End Region
    Private Sub PictureBox2_Click(sender As Object, e As EventArgs) Handles PictureBox2.Click
        ProjectData.EndApp()
    End Sub
    Private Sub PictureBox3_Click(sender As Object, e As EventArgs) Handles PictureBox3.Click
        Me.WindowState = FormWindowState.Minimized
    End Sub
    Private Sub me_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ListBox1.AllowDrop = True
        Dim allProc As Process() = Process.GetProcesses()
        For Each p As Process In allProc
            ComboBox1.Items.Add(p.ProcessName)
        Next
        ComboBox1.SelectedIndex = 1
    End Sub
    Private Sub ListBox1_DragDrop(sender As Object, e As DragEventArgs) Handles ListBox1.DragDrop
        Try
            Dim array As String() = CType(e.Data.GetData(DataFormats.FileDrop, False), String())
            For Each str As String In array
                If str.EndsWith("dll") Then
                    Me.ListBox1.Items.Add(str)
                    ListBox1.SelectedIndex = 0
                    DllPath = ListBox1.SelectedItem
                End If
            Next
        Catch ex As Exception
            MsgBox(ex.ToString)
            ProjectData.ClearProjectError()
            MessageBox.Show("Please Select .DLL !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
    Private Sub ListBox1_DragEnter(sender As Object, e As DragEventArgs) Handles ListBox1.DragEnter
        e.Effect = DragDropEffects.All
    End Sub
    Private Sub Label5_Click(sender As Object, e As EventArgs) Handles PictureBox6.Click, Panel5.Click, Label5.Click
        ListBox1.Items.Clear()
    End Sub
    Private Sub Label4_Click(sender As Object, e As EventArgs) Handles PictureBox5.Click, Panel4.Click, Label4.Click
        ListBox1.Items.Remove(ListBox1.SelectedItem)
    End Sub
    Private Sub Label3_Click(sender As Object, e As EventArgs) Handles PictureBox4.Click, Panel3.Click, Label3.Click
        Try
            Dim Browse As New OpenFileDialog
            With Browse
                .FileName = Nothing
                .Tag = "Browse for dll"
                .Title = "Browser for dll"
                .FileName = Nothing
                .Multiselect = True
                If .ShowDialog = Windows.Forms.DialogResult.OK Then
                    If .FileName.EndsWith("dll") Then
                        Me.ListBox1.Items.Add(.FileName)
                        ListBox1.SelectedIndex = 0
                        DllPath = ListBox1.SelectedItem
                    End If
                End If
            End With
        Catch ex As Exception
            ProjectData.EndApp()
            MessageBox.Show("Please Select .DLL !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
    Private Sub Inject_Tick(sender As Object, e As EventArgs) Handles Inject.Tick
        Try
            If ComboBox1.Enabled = True Then
                procname = ComboBox1.SelectedItem
            Else
                procname = TextBox1.Text
            End If
            Dim targeted As Process() = Process.GetProcessesByName(procname)
            If targeted.Length = 1 Then
                DllPath = ListBox1.SelectedItem
                Dim pid As Integer = targeted(0).Id
                Dim DLL As Byte() = File.ReadAllBytes(DllPath)
                Inject.Stop()
                Inject.Enabled = False
                If ManualMap.Checked Then
                    MMInject(pid, DLL)
                    Exit Sub
                End If
                If LoadLibrary.Checked Then
                    LLInject(pid, DLL)
                    Exit Sub
                End If
                If ThreadHijack.Checked Then
                    THInject(pid, DLL)
                    Exit Sub
                End If
            End If
        Catch ex As Exception
            Inject.Stop()
            Inject.Enabled = False
            EForm.Label6.Text = ex.Message.ToString
            EForm.ShowDialog()
            ProjectData.ClearProjectError()
        End Try
    End Sub
    Private Sub Label6_Click(sender As Object, e As EventArgs) Handles PictureBox7.Click, Panel6.Click, Label6.Click
        Try
            If ComboBox1.Enabled = True Then
                procname = ComboBox1.SelectedItem
            Else
                procname = TextBox1.Text
            End If
            DllPath = ListBox1.SelectedItem
            Dim targeted As Process() = Process.GetProcessesByName(procname)
            Dim pid As Integer = targeted(0).Id
            Dim DLL As Byte() = File.ReadAllBytes(DllPath)
            If ManualMap.Checked Then
                MMInject(pid, DLL)
                Exit Sub
            End If
            If LoadLibrary.Checked Then
                LLInject(pid, DLL)
                Exit Sub
            End If
            If ThreadHijack.Checked Then
                THInject(pid, DLL)
                Exit Sub
            End If
        Catch ex As Exception
            EForm.Label6.Text = ex.Message.ToString
            EForm.ShowDialog()
            ProjectData.ClearProjectError()
        End Try
    End Sub
    Private Sub AetherCheckBox4_CheckedChanged(sender As Object, e As EventArgs) Handles AetherCheckBox4.CheckedChanged
        If AetherCheckBox4.Checked Then
            Inject.Enabled = False
            Panel6.Enabled = True
            PictureBox7.Enabled = True
            Label6.Enabled = True
            Label6.ForeColor = Color.White
            AetherCheckBox5.Checked = False
        End If
    End Sub
    Private Sub AetherCheckBox5_CheckedChanged(sender As Object, e As EventArgs) Handles AetherCheckBox5.CheckedChanged
        If AetherCheckBox5.Checked Then
            Inject.Enabled = True
            Panel6.Enabled = False
            PictureBox7.Enabled = False
            Label6.Enabled = False
            Label6.ForeColor = Color.White
            AetherCheckBox4.Checked = False
        End If
    End Sub
    Dim change As Boolean = False
    Private Sub Label13_Click(sender As Object, e As EventArgs) Handles Label13.Click
        If change = False Then
            Label13.Text = "Disable"
            TextBox1.Enabled = False
            ComboBox1.Enabled = True
            change = True
        Else
            Label13.Text = "Enable"
            TextBox1.Enabled = True
            ComboBox1.Enabled = False
            change = False
        End If
    End Sub
    Private Sub ManualMap_CheckedChanged(sender As Object, e As EventArgs) Handles ManualMap.CheckedChanged
        If ManualMap.Checked Then
            LoadLibrary.Checked = False
            ThreadHijack.Checked = False
        End If
    End Sub
    Private Sub LoadLibrary_CheckedChanged(sender As Object, e As EventArgs) Handles LoadLibrary.CheckedChanged
        If LoadLibrary.Checked Then
            ManualMap.Checked = False
            ThreadHijack.Checked = False
        End If
    End Sub
    Private Sub ThreadHijack_CheckedChanged(sender As Object, e As EventArgs) Handles ThreadHijack.CheckedChanged
        If ThreadHijack.Checked Then
            ManualMap.Checked = False
            LoadLibrary.Checked = False
        End If
    End Sub
    Private Sub Label12_Click(sender As Object, e As EventArgs) Handles Label12.Click
        ComboBox1.Items.Clear()
        Dim allProc As Process() = Process.GetProcesses()
        For Each p As Process In allProc
            ComboBox1.Items.Add(p.ProcessName)
        Next
        ComboBox1.SelectedIndex = 1
    End Sub
End Class
