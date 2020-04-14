Module Helpers

    Public Enum MouseState As Byte
        None = 0
        Over = 1
        Down = 2
    End Enum

    Public Function FullRectangle(ByVal S As Size, ByVal Subtract As Boolean) As Rectangle
        If Subtract Then
            Return New Rectangle(0, 0, S.Width - 1, S.Height - 1)
        Else
            Return New Rectangle(0, 0, S.Width, S.Height)
        End If
    End Function

    Public Function GreyColor(ByVal G As UInteger) As Color
        Return Color.FromArgb(G, G, G)
    End Function

    Public Sub CenterString(ByVal G As Graphics, ByVal T As String, ByVal F As Font, ByVal C As Color, ByVal R As Rectangle)
        Dim TS As SizeF = G.MeasureString(T, F)

        Using B As New SolidBrush(C)
            G.DrawString(T, F, B, New Point(R.Width / 2 - (TS.Width / 2), R.Height / 2 - (TS.Height / 2)))
        End Using

    End Sub

    Public Sub FillRoundRect(ByVal G As Graphics, ByVal R As Rectangle, ByVal Curve As Integer, ByVal B As Brush)
        G.FillPie(B, R.X, R.Y, Curve, Curve, 180, 90)
        G.FillPie(B, R.X + R.Width - Curve, R.Y, Curve, Curve, 270, 90)
        G.FillPie(B, R.X, R.Y + R.Height - Curve, Curve, Curve, 90, 90)
        G.FillPie(B, R.X + R.Width - Curve, R.Y + R.Height - Curve, Curve, Curve, 0, 90)
        G.FillRectangle(B, CInt(R.X + Curve / 2), R.Y, R.Width - Curve, CInt(Curve / 2))
        G.FillRectangle(B, R.X, CInt(R.Y + Curve / 2), R.Width, R.Height - Curve)
        G.FillRectangle(B, CInt(R.X + Curve / 2), CInt(R.Y + R.Height - Curve / 2), R.Width - Curve, CInt(Curve / 2))
    End Sub

    Public Sub DrawRoundRect(ByVal G As Graphics, ByVal R As Rectangle, ByVal Curve As Integer, ByVal PP As Pen)
        G.DrawArc(PP, R.X, R.Y, Curve, Curve, 180, 90)
        G.DrawLine(PP, CInt(R.X + Curve / 2), R.Y, CInt(R.X + R.Width - Curve / 2), R.Y)
        G.DrawArc(PP, R.X + R.Width - Curve, R.Y, Curve, Curve, 270, 90)
        G.DrawLine(PP, R.X, CInt(R.Y + Curve / 2), R.X, CInt(R.Y + R.Height - Curve / 2))
        G.DrawLine(PP, CInt(R.X + R.Width), CInt(R.Y + Curve / 2), CInt(R.X + R.Width), CInt(R.Y + R.Height - Curve / 2))
        G.DrawLine(PP, CInt(R.X + Curve / 2), CInt(R.Y + R.Height), CInt(R.X + R.Width - Curve / 2), CInt(R.Y + R.Height))
        G.DrawArc(PP, R.X, R.Y + R.Height - Curve, Curve, Curve, 90, 90)
        G.DrawArc(PP, R.X + R.Width - Curve, R.Y + R.Height - Curve, Curve, Curve, 0, 90)
    End Sub

    Public Enum Direction As Byte
        Up = 0
        Down = 1
        Left = 2
        Right = 3
    End Enum

    Public Sub DrawTriangle(ByVal G As Graphics, ByVal Rect As Rectangle, ByVal D As Direction, ByVal C As Color)
        Dim halfWidth As Integer = Rect.Width / 2
        Dim halfHeight As Integer = Rect.Height / 2
        Dim p0 As Point = Point.Empty
        Dim p1 As Point = Point.Empty
        Dim p2 As Point = Point.Empty

        Select Case D
            Case Direction.Up
                p0 = New Point(Rect.Left + halfWidth, Rect.Top)
                p1 = New Point(Rect.Left, Rect.Bottom)
                p2 = New Point(Rect.Right, Rect.Bottom)

            Case Direction.Down
                p0 = New Point(Rect.Left + halfWidth, Rect.Bottom)
                p1 = New Point(Rect.Left, Rect.Top)
                p2 = New Point(Rect.Right, Rect.Top)

            Case Direction.Left
                p0 = New Point(Rect.Left, Rect.Top + halfHeight)
                p1 = New Point(Rect.Right, Rect.Top)
                p2 = New Point(Rect.Right, Rect.Bottom)

            Case Direction.Right
                p0 = New Point(Rect.Right, Rect.Top + halfHeight)
                p1 = New Point(Rect.Left, Rect.Bottom)
                p2 = New Point(Rect.Left, Rect.Top)

        End Select

        Using B As New SolidBrush(C)
            G.FillPolygon(B, New Point() {p0, p1, p2})
        End Using

    End Sub

    Public Function ColorFromHex(ByVal Hex As String) As Color
        Dim R, G, B As String
        Hex = Replace(Hex, "#", "")
        R = Val("&H" & Mid(Hex, 1, 2))
        G = Val("&H" & Mid(Hex, 3, 2))
        B = Val("&H" & Mid(Hex, 5, 2))
        Return Color.FromArgb(R, G, B)
    End Function

End Module
<System.ComponentModel.DefaultEvent("CheckedChanged")>
Public Class AetherCheckBox
    Inherits Control
    Public Event CheckedChanged(ByVal sender As Object, ByVal e As EventArgs)
    Private _Checked As Boolean
    Private _EnabledCalc As Boolean
    Private G As Graphics
    Private State As MouseState
    Public Property Checked As Boolean
        Get
            Return _Checked
        End Get
        Set(ByVal value As Boolean)
            _Checked = value
            Invalidate()
        End Set
    End Property
    Public Shadows Property Enabled As Boolean
        Get
            Return EnabledCalc
        End Get
        Set(ByVal value As Boolean)
            _EnabledCalc = value

            If Enabled Then
                Cursor = Cursors.Hand
            Else
                Cursor = Cursors.Default
            End If

            Invalidate()
        End Set
    End Property
    Public Property EnabledCalc As Boolean
        Get
            Return _EnabledCalc
        End Get
        Set(ByVal value As Boolean)
            Enabled = value
            Invalidate()
        End Set
    End Property
    Sub New()
        DoubleBuffered = True
        Enabled = True
        Cursor = Cursors.Hand
        ForeColor = Color.FromArgb(255, 255, 255)
        SetStyle(ControlStyles.SupportsTransparentBackColor, True)
        BackColor = Color.Transparent
    End Sub
    Protected Overrides Sub OnPaint(ByVal e As PaintEventArgs)
        G = e.Graphics
        G.SmoothingMode = Drawing2D.SmoothingMode.AntiAlias
        G.TextRenderingHint = Drawing.Text.TextRenderingHint.SystemDefault
        MyBase.OnPaint(e)
        If Enabled Then
            Using P1 As New Pen(ColorFromHex("#FF1E00")), B1 As New SolidBrush(ColorFromHex("#FFFFFF")), F1 As New Font("candara", 11.5, FontStyle.Regular)
                DrawRoundRect(G, New Rectangle(0, 0, 18, 18), 6, P1)
                G.DrawString(Text, F1, B1, New Point(25, 0))
            End Using
            If State = MouseState.Over Or State = MouseState.Down Then
                Using P1 As New Pen(ColorFromHex("#FF1E00"))
                    DrawRoundRect(G, New Rectangle(0, 0, 18, 18), 6, P1)
                End Using
            End If
        Else
            Using P1 As New Pen(ColorFromHex("#FFFFFF")), B1 As New SolidBrush(ColorFromHex("#FF1E00")), F1 As New Font("candara", 11.5)
                DrawRoundRect(G, New Rectangle(0, 0, 18, 18), 6, P1)
                G.DrawString(Text, F1, B1, New Point(25, 0))
            End Using
        End If
        If Checked Then
            If Enabled Then
                Using B1 As New Drawing2D.HatchBrush(Drawing2D.HatchStyle.WideUpwardDiagonal, ColorFromHex("#FF1E00"), ColorFromHex("#FF1E00")), P1 As New Pen(ColorFromHex("#FF1E00"))
                    G.FillRectangle(B1, New Rectangle(4, 4, 10, 10))
                    DrawRoundRect(G, New Rectangle(4, 4, 10, 10), 3, P1)
                End Using
            Else
                Using B1 As New Drawing2D.HatchBrush(Drawing2D.HatchStyle.WideUpwardDiagonal, ColorFromHex("#FF1E00"), ColorFromHex("#FF1E00")), P1 As New Pen(ColorFromHex("#FF1E00"))
                    G.FillRectangle(B1, New Rectangle(4, 4, 10, 10))
                    DrawRoundRect(G, New Rectangle(4, 4, 10, 10), 3, P1)
                End Using
            End If
        End If
    End Sub
    Protected Overrides Sub OnResize(ByVal e As EventArgs)
        MyBase.OnResize(e)
        Size = New Size(Width, 19)
    End Sub
    Protected Overrides Sub OnMouseUp(ByVal e As MouseEventArgs)
        MyBase.OnMouseUp(e)
        If Enabled Then
            State = MouseState.Over
            Checked = Not Checked
            RaiseEvent CheckedChanged(Me, e)
            Invalidate()
        End If
    End Sub
    Protected Overrides Sub OnMouseDown(ByVal e As MouseEventArgs)
        MyBase.OnMouseDown(e)
        If Enabled Then
            State = MouseState.Down : Invalidate()
        End If
    End Sub
    Protected Overrides Sub OnMouseEnter(ByVal e As EventArgs)
        MyBase.OnMouseEnter(e)
        If Enabled Then
            State = MouseState.Over : Invalidate()
        End If
    End Sub
    Protected Overrides Sub OnMouseLeave(ByVal e As EventArgs)
        MyBase.OnMouseLeave(e)
        If Enabled Then
            State = MouseState.None : Invalidate()
        End If
    End Sub
End Class
<System.ComponentModel.DefaultEvent("CheckedChanged")>
Public Class AetherRadioButton
    Inherits Control

    Public Event CheckedChanged(ByVal sender As Object, ByVal e As EventArgs)

    Private G As Graphics
    Private State As MouseState

    Private _Checked As Boolean
    Private _EnabledCalc As Boolean
    Sub New()
        DoubleBuffered = True
        Enabled = True
        Cursor = Cursors.Hand
        ForeColor = Color.FromArgb(110, 114, 118)
        SetStyle(ControlStyles.SupportsTransparentBackColor, True)
        BackColor = Color.Transparent
    End Sub

    Public Property Checked As Boolean
        Get
            Return _Checked
        End Get
        Set(ByVal value As Boolean)
            _Checked = value
            Invalidate()
        End Set
    End Property

    Public Shadows Property Enabled As Boolean
        Get
            Return EnabledCalc
        End Get
        Set(ByVal value As Boolean)
            _EnabledCalc = value

            If Enabled Then
                Cursor = Cursors.Hand
            Else
                Cursor = Cursors.Default
            End If

            Invalidate()
        End Set
    End Property


    Public Property EnabledCalc As Boolean
        Get
            Return _EnabledCalc
        End Get
        Set(ByVal value As Boolean)
            Enabled = value
            Invalidate()
        End Set
    End Property

    Protected Overrides Sub OnPaint(ByVal e As PaintEventArgs)

        G = e.Graphics
        G.SmoothingMode = Drawing2D.SmoothingMode.HighQuality
        G.TextRenderingHint = Drawing.Text.TextRenderingHint.AntiAlias

        MyBase.OnPaint(e)

        If Enabled Then

            Using P As New Pen(ColorFromHex("#6E7276"))
                G.DrawEllipse(P, New Rectangle(0, 0, 18, 18))
            End Using

            Using B As New SolidBrush(ColorFromHex("#6E7276"))
                G.DrawString(Text, New Font("Consolas", 11.6, FontStyle.Bold), B, New Point(25, 1))
            End Using

            If State = MouseState.Over Or State = MouseState.Down Then

                Using P1 As New Pen(ColorFromHex("#6E7276"))
                    G.DrawEllipse(P1, New Rectangle(0, 0, 18, 18))
                End Using

            End If

        Else

            Using P As New Pen(ColorFromHex("#6E7276"))
                G.DrawEllipse(P, New Rectangle(0, 0, 18, 18))
            End Using

            Using B As New SolidBrush(ColorFromHex("#6E7276"))
                G.DrawString(Text, New Font("Segoe UI", 9), B, New Point(25, 1))
            End Using

        End If

        If Checked Then

            If Enabled Then

                Using B1 As New Drawing2D.HatchBrush(Drawing2D.HatchStyle.WideUpwardDiagonal, ColorFromHex("#5B606F"), ColorFromHex("#525766")), P1 As New Pen(ColorFromHex("#474C5A"))
                    G.FillEllipse(B1, New Rectangle(4, 4, 10, 10))
                    G.DrawEllipse(P1, New Rectangle(4, 4, 10, 10))
                End Using

            Else

                Using B1 As New Drawing2D.HatchBrush(Drawing2D.HatchStyle.WideUpwardDiagonal, ColorFromHex("#8C92A1"), ColorFromHex("#7A7F8E")), P1 As New Pen(ColorFromHex("#797E8C"))
                    G.FillEllipse(B1, New Rectangle(4, 4, 10, 10))
                    G.DrawEllipse(P1, New Rectangle(4, 4, 10, 10))
                End Using

            End If

        End If

    End Sub

    Protected Overrides Sub OnResize(ByVal e As EventArgs)
        MyBase.OnResize(e)
        Size = New Size(Width, 19)
    End Sub

    Protected Overrides Sub OnMouseUp(ByVal e As MouseEventArgs)
        MyBase.OnMouseUp(e)

        If Enabled Then

            For Each C As Control In Parent.Controls
                If TypeOf C Is AetherRadioButton Then
                    DirectCast(C, AetherRadioButton).Checked = False
                End If
            Next

            Checked = Not Checked
            RaiseEvent CheckedChanged(Me, e)
            State = MouseState.Over : Invalidate()

        End If

    End Sub

    Protected Overrides Sub OnMouseDown(ByVal e As MouseEventArgs)
        MyBase.OnMouseDown(e)
        If Enabled Then
            State = MouseState.Down : Invalidate()
        End If
    End Sub

    Protected Overrides Sub OnMouseEnter(ByVal e As EventArgs)
        MyBase.OnMouseEnter(e)
        If Enabled Then
            State = MouseState.Over : Invalidate()
        End If
    End Sub

    Protected Overrides Sub OnMouseLeave(ByVal e As EventArgs)
        MyBase.OnMouseLeave(e)
        If Enabled Then
            State = MouseState.None : Invalidate()
        End If
    End Sub

End Class
Public Class AetherTag
    Inherits Control

    Private G As Graphics

    Public Property Background As Color = ColorFromHex("#424452")
    Public Property Border As Color = ColorFromHex("#323541")
    Public Property TextColor As Color = ColorFromHex("#7C8290")

    Sub New()
        DoubleBuffered = True
        Text = "Tag"
    End Sub

    Protected Overrides Sub OnPaint(ByVal e As PaintEventArgs)
        G = e.Graphics
        G.SmoothingMode = Drawing2D.SmoothingMode.HighQuality
        G.TextRenderingHint = Drawing.Text.TextRenderingHint.ClearTypeGridFit

        MyBase.OnPaint(e)

        Using B1 As New SolidBrush(Background), P1 As New Pen(Border), B2 As New SolidBrush(TextColor)
            G.FillRectangle(B1, FullRectangle(Size, True))
            DrawRoundRect(G, FullRectangle(Size, True), 3, P1)

            If IsNumeric(Text) Then

                Using F1 As New Font("Segoe UI", 8, FontStyle.Bold)
                    G.DrawString(Text, F1, B2, New Point(2, 0))
                End Using

            Else

                Using F1 As New Font("Segoe UI", 7, FontStyle.Bold)
                    G.DrawString(Text.ToUpper, F1, B2, New Point(2, 1))
                End Using

            End If

        End Using

    End Sub

    Protected Overrides Sub OnResize(ByVal e As EventArgs)
        MyBase.OnResize(e)
        Size = New Size(Width, 15)
    End Sub

End Class
