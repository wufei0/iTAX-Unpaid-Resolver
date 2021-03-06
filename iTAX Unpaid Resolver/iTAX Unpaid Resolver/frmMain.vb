﻿
Imports FirebirdSql.Data.FirebirdClient
Imports System.Data.Odbc
Public Class frmMain

    Private Sub frmMain_Deactivate(sender As Object, e As EventArgs) Handles Me.Deactivate

    End Sub

    Private Sub frmMain_FormClosed(sender As Object, e As FormClosedEventArgs) Handles Me.FormClosed
        End
    End Sub



    Private Sub frmMain_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Text = My.Application.Info.Title & " v." & FileVersionInfo.GetVersionInfo(Application.ExecutablePath).FileVersion
        Me.Show()
        

        Me.Cursor = Cursors.WaitCursor
        'Open connection.info
        Application.DoEvents()
        Call dbConf()
        Me.ToolStripStatusLabel2.Text = FirebirdIP
        Me.ToolStripStatusLabel5.Text = FirebirdDbase

        Me.ToolStripStatusLabel8.Text = "Connecting...."
        Me.ToolStripStatusLabel8.BackColor = Color.Orange
        Application.DoEvents()
        Dim Conntest As New FbConnection
        Conntest.ConnectionString = "User=SYSDBA;Password=" & FirebirdPassword & ";Database=" & FirebirdIP & ":" & FirebirdDbase & ";" _
                                        & "Port=3050;Dialect=3;Charset=utf8;Role=;Connection lifetime=15;Pooling=true;" _
                                        & "MinPoolSize=0;MaxPoolSize=50;Packet Size=8192;ServerType=0;"

        Try
            Conntest.Open()
            Me.ToolStripStatusLabel8.Text = "Connected"
            Me.ToolStripStatusLabel8.BackColor = Color.Green
            Me.btnFind.Enabled = True

        Catch ex As Exception
            Me.ToolStripStatusLabel8.Text = "Disconnected"
            Me.ToolStripStatusLabel8.BackColor = Color.LightCoral
            Me.btnFind.Enabled = False
        Finally
            Conntest.Close()
            Conntest.Dispose()

        End Try

        'AUTO COMPLETE
        Dim MySource As New AutoCompleteStringCollection()
        Dim lst As New List(Of String)
        Dim FBcmd As New FbCommand
        Dim FBdr As FbDataReader = Nothing



        If FBConnectOpen() = True Then
            Try
                FBcmd.Connection = FBCon
                FBcmd.CommandText = "SELECT * FROM RPTASSESSMENT join property  on rptassessment.prop_id = property.prop_id join propertyowner on property.prop_id = propertyowner.prop_id join taxpayer on propertyowner.local_tin = taxpayer.local_tin "
                FBdr = FBcmd.ExecuteReader

                While FBdr.Read
                    lst.Add(FBdr!tdno.ToString)
                    lst.Add(FBdr!pinno.ToString)
                    lst.Add(FBdr!cadastrallotno.ToString)
                    lst.Add(FBdr!ownername.ToString)

                End While
                MySource.AddRange(lst.ToArray)
                txtFind.AutoCompleteCustomSource = MySource
                txtFind.AutoCompleteMode = AutoCompleteMode.SuggestAppend
                txtFind.AutoCompleteSource = AutoCompleteSource.CustomSource

            Catch ex As Exception
                MsgBox(ex.Message)
            Finally
                FBdr.Close()
                FBcmd.Dispose()
                FBCon.Close()
                FBCon.Dispose()
            End Try

        End If

        Me.Cursor = Cursors.Default
    End Sub






    Private Sub ConnectionToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ConnectionToolStripMenuItem.Click
        frmConnection.ShowDialog()
    End Sub

    Private Sub btnFind_Click(sender As Object, e As EventArgs) Handles btnFind.Click

        If txtFind.Text = Nothing Then
            If MsgBox("Search all Records? Continue?", MsgBoxStyle.Information + MsgBoxStyle.YesNo) = MsgBoxResult.No Then
                Exit Sub
            End If
        End If

        Dim FBcmd As New FbCommand
        Dim FBdr As FbDataReader = Nothing
        Dim LVitem As ListViewItem


       

        'Dim odbccommand As New OdbcCommand
        'Dim odbcdr As OdbcDataReader

        lstSearch.Items.Clear()
        If FBConnectOpen() = True Then

            Try
               
                Me.Cursor = Cursors.WaitCursor
                FBcmd.Connection = FBCon
                'odbccommand.Connection = odbccon
                'FBcmd.CommandText = "SELECT * FROM RPTASSESSMENT JOIN PROPERTY" _
                '& "ON RPTASSESSMENT.PROP_ID = PROPERTY.PROP_ID join PROPERTYOWNER ON PROPERTY.PROP_ID = PROPERTYOWNER.PROP_ID" _
                '& "JOIN TAXPAYER ON PROPERTYOWNER.LOCAL_TIN = TAXPAYER.LOCAL_TIN WHERE RPTASSESSMENT.ENDED_BV=0 "

                FBcmd.CommandText = "SELECT * FROM RPTASSESSMENT join property  on rptassessment.prop_id = property.prop_id join propertyowner on property.prop_id = propertyowner.prop_id join taxpayer on propertyowner.local_tin = taxpayer.local_tin WHERE (TDNO LIKE '%" & Me.txtFind.Text & "%' or pinno like '%" & Me.txtFind.Text & "%' or ownername like '%" & Me.txtFind.Text & "%' or cadastrallotno like '%" & Me.txtFind.Text & "%') and ended_bv=0"
                'MsgBox(FBcmd.CommandText)
                FBdr = FBcmd.ExecuteReader

                'odbccommand.CommandText = "SELECT * FROM RPTASSESSMENT join property on rptassessment.prop_id = property.prop_id WHERE TDNO LIKE '%" & Me.txtFind.Text & "%' or pinno like '%" & Me.txtFind.Text & "%'"
                'odbcdr = odbccommand.ExecuteReader
                'While odbcdr.Read
                '    Me.Cursor = Cursors.WaitCursor
                '    LVitem = New ListViewItem(odbcdr!TAXTRANS_ID.ToString)
                '    LVitem.SubItems.Add(odbcdr!TDNO.ToString)
                '    LVitem.SubItems.Add(odbcdr!pinno.ToString)
                '    LVitem.SubItems.Add(odbcdr!propertykind_ct.ToString)
                '    LVitem.SubItems.Add(odbcdr!cadastrallotno.ToString)
                '    lstSearch.Items.Add(LVitem)
                '    'MsgBox(FBdr!TDNO)
                '    'Application.DoEvents()
                '    'Me.ToolStripStatusLabel9.Text = FBdr!TDNO.ToString

                'End While

                While FBdr.Read

                    LVitem = New ListViewItem(FBdr!TAXTRANS_ID.ToString)
                    LVitem.SubItems.Add(FBdr!TDNO.ToString)
                    LVitem.SubItems.Add(FBdr!pinno.ToString)
                    LVitem.SubItems.Add(FBdr!propertykind_ct.ToString)
                    LVitem.SubItems.Add(FBdr!cadastrallotno.ToString)
                    LVitem.SubItems.Add(FBdr!ownername.ToString)
                    LVitem.SubItems.Add(FBdr!owneraddress.ToString)
                    lstSearch.Items.Add(LVitem)
                    'MsgBox(FBdr!TDNO)
                    Application.DoEvents()
                    'Me.ToolStripStatusLabel9.Text = FBdr!TDNO.ToString

                End While


            Catch ex As Exception
                MsgBox(ex.Message)
            Finally
                Me.Cursor = Cursors.Default
                FBcmd.Dispose()
                FBdr.Close()
                FBCon.Close()
                FBCon.Dispose()
            End Try
          
        End If

    End Sub

    Private Sub lstSearch_DoubleClick(sender As Object, e As EventArgs) Handles lstSearch.DoubleClick

        'MsgBox(lstSearch.SelectedItems(0).Text)

        If FBConnectOpen() = False Then
            MsgBox("Error database connection. Check network connectivity.", MsgBoxStyle.Critical + MsgBoxStyle.OkOnly)
            Exit Sub
        End If

        Dim fbcmd As New FbCommand
        Dim fbrdr As FbDataReader = Nothing
        Dim FBCmd1 As New FbCommand
        Try
            'TPACCOUNT
            fbcmd.Connection = FBCon
            fbcmd.CommandText = "SELECT * FROM TPACCOUNT WHERE  TAXTRANS_ID = " & lstSearch.SelectedItems(0).Text & " "

            fbrdr = fbcmd.ExecuteReader

            While fbrdr.Read
                If fbrdr!earmark_ct = "OPN" Then
                    FBCmd1 = New FbCommand
                    FBCmd1.Connection = FBCon
                    FBCmd1.CommandText = "DELETE from TPACCOUNT WHERE POSTING_ID = " & fbrdr!POSTING_ID.ToString & " "
                    FBCmd1.ExecuteNonQuery()
                    FBCmd1.Dispose()
                End If


            End While

           

            MsgBox("Delete posting completed.", MsgBoxStyle.Information + MsgBoxStyle.OkOnly)



        Catch ex As Exception
            MsgBox(ex.Message)
        Finally
            fbrdr.Close()
            fbcmd.Dispose()
            FBCon.Close()
            FBCon.Dispose()

        End Try






    End Sub

    Private Sub lstSearch_SelectedIndexChanged(sender As Object, e As EventArgs) Handles lstSearch.SelectedIndexChanged

    End Sub

    Private Sub ExitToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ExitToolStripMenuItem.Click
        If MsgBox("Exit?", MsgBoxStyle.Question + MsgBoxStyle.YesNo) = MsgBoxResult.Yes Then
            End
        End If
    End Sub

    Private Sub AboutToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles AboutToolStripMenuItem.Click
        frmAbout.ShowDialog()
    End Sub

    Private Sub StatusStrip1_ItemClicked(sender As Object, e As ToolStripItemClickedEventArgs) Handles StatusStrip1.ItemClicked

    End Sub
End Class
