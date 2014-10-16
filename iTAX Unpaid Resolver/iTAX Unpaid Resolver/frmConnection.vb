Imports System.IO
Imports System.Security.Cryptography
Imports FirebirdSql.Data.FirebirdClient
Public Class frmConnection
    
 

    Private Sub btnUpdate_Click(sender As Object, e As EventArgs) Handles btnUpdate.Click
        Dim FILE_NAME As String = My.Application.Info.DirectoryPath & " \connection.info"
        Dim EncrypText As String = ""

        Dim sPlainText As String = txtPassword.Text
        If Not String.IsNullOrEmpty(sPlainText) Then
            Dim memoryStream As MemoryStream = New MemoryStream()
            Dim cryptoStream As CryptoStream = New CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write)
            cryptoStream.Write(enc.GetBytes(sPlainText), 0, sPlainText.Length)
            cryptoStream.FlushFinalBlock()
            EncrypText = Convert.ToBase64String(memoryStream.ToArray())
            memoryStream.Close()
            cryptoStream.Close()
        End If

        If System.IO.File.Exists(FILE_NAME) = True Then

            Dim objWriter As New System.IO.StreamWriter(FILE_NAME)
           

            objWriter.WriteLine("HOST='" & txtServer.Text & "'")
            objWriter.WriteLine("DATABASE='" & txtDatabase.Text & "'")
            objWriter.WriteLine("PASSWORD='" & EncrypText & "'")
            objWriter.Close()


        Else
            Dim sw As StreamWriter = File.CreateText(FILE_NAME)
            sw.WriteLine("HOST='" & txtServer.Text & "'")
            sw.WriteLine("DATABASE='" & txtDatabase.Text & "'")
            sw.WriteLine("PASSWORD='" & EncrypText & "'")
            sw.Flush()
            sw.Close()

        End If


        MsgBox("Connection info updated. Will take effect on App restart.", MsgBoxStyle.Information + MsgBoxStyle.OkOnly)



    End Sub


    Private Sub btnTest_Click(sender As Object, e As EventArgs) Handles btnTest.Click
        Dim Conntest As New FbConnection
        Conntest.ConnectionString = "User=SYSDBA;Password=" & txtPassword.Text & ";Database=" & txtServer.Text & ":" & txtDatabase.Text & ";" _
                                        & "Port=3050;Dialect=3;Charset=utf8;Role=;Connection lifetime=15;Pooling=true;" _
                                        & "MinPoolSize=0;MaxPoolSize=50;Packet Size=8192;ServerType=0;"

        Try
            Conntest.Open()
            MsgBox("Connection Successful.", MsgBoxStyle.Information + MsgBoxStyle.OkOnly)
        Catch ex As Exception
            MsgBox("Connection Failed. " & ex.Message, MsgBoxStyle.Information + MsgBoxStyle.OkOnly)
        Finally
            Conntest.Close()
        End Try


    End Sub

    Private Sub frmConnection_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.txtServer.Text = FirebirdIP
        Me.txtPassword.Text = FirebirdPassword
        Me.txtDatabase.Text = FirebirdDbase
    End Sub

End Class