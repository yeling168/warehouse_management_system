﻿Imports System.Configuration
Imports System.Data.SqlClient
Public Class Form_log_in
    Dim sqlcs As String = ConfigurationManager.ConnectionStrings("connect").ConnectionString 'string connection command adaptor table
    Dim cs As New SqlConnection(sqlcs)
    Dim sql_select As New SqlCommand("SELECT * from warehouse_password", cs) 'get the password table
    Dim sqladptor As New SqlDataAdapter(sql_select)
    Dim table As New DataTable
    Dim password_low As String 'manager password
    Dim password_high As String 'boss password
    Dim level As Boolean 'boss is true, manager is false
    Dim num_check As Integer = 3 'only three chance
    Private Sub Form_log_in_Load(sender As Object, e As EventArgs) Handles MyBase.Load 'when the form load
        sqladptor.Fill(table)
        password_high = table.Rows(0).Item("password") 'first one is boss password
        password_low = table.Rows(1).Item("password") 'second one is manager password
    End Sub
    Private Sub BT_log_in_Click(sender As Object, e As EventArgs) Handles BT_log_in.Click
        password_check() 'check the password
    End Sub

    Private Sub password_check()
        If TB_password.Text = password_low And RB_manager.Checked Then
            LB_hint.Text = ""
            level = False
            Me.Enabled = False
            Main_Form.Show()
            Me.Hide()
        ElseIf TB_password.Text = password_high And RB_boss.Checked Then
            LB_hint.Text = ""
            level = True
            Me.Enabled = False
            Main_Form.Show()
            Me.Hide()
        ElseIf TB_password.Text = "" Then
            LB_hint.Text = "please enter password"
        ElseIf num_check = 0 Then
            Me.Close()
        Else
            LB_hint.Text = "you have " + num_check.ToString() + " more time to enter the password"
            num_check -= 1
        End If
        TB_password.Clear()
    End Sub

    Public Sub password_change(new_password As String)
        Dim sql_update As New SqlCommand("UPDATE warehouse_password SET password = @password WHERE ID = @ID", cs)
        sql_update.Parameters.Add("password", SqlDbType.BigInt).Value = new_password
        If level Then
            sql_update.Parameters.Add("ID", SqlDbType.TinyInt).Value = 1
            password_high = new_password
        Else
            sql_update.Parameters.Add("ID", SqlDbType.TinyInt).Value = 2
            password_low = new_password
        End If
        cs.Open()
        sql_update.ExecuteNonQuery()
        cs.Close()
    End Sub

    Public Function password_get() As String
        If level Then
            Return password_high
        Else
            Return password_low
        End If
    End Function

    Public Function level_get() As Boolean
        Return level
    End Function
End Class