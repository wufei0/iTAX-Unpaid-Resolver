Imports System
Imports System.IO
Imports System.Text
Imports FirebirdSql.Data.FirebirdClient
Imports System.Security.Cryptography

Module modDatabase
    Public FirebirdIP As String
    Public FirebirdDbase As String
    Public FirebirdPassword As String

    'Public FbDataAdapter As New FbDataAdapter
    Public FBCon As New FbConnection
    'Public odbccon As New Odbc.OdbcConnection
    ' Public FbCommand As FbCommand




    Public enc As System.Text.UTF8Encoding
    Public encryptor As ICryptoTransform
    Public decryptor As ICryptoTransform

    'CONNECTION PROCEDURE
    Public Sub dbConnection()


    End Sub

    Public Function FBConnectOpen() As Boolean
        FBCon = New FbConnection
        FBCon.ConnectionString = "User=SYSDBA;Password=" & FirebirdPassword & ";Database=" & FirebirdIP & ":" & FirebirdDbase & ";" _
                                        & "Port=3050;Charset=utf8"

        'odbccon.ConnectionString = "Driver=Firebird/InterBase(r) driver;Uid=SYSDBA; Pwd=" & FirebirdPassword & "; Dbname=" & FirebirdIP & ":" & FirebirdDbase & "; Charset=utf8"



        'odbccon.Open()
        Try
            FBCon.Open()
            frmMain.ToolStripStatusLabel8.Text = "Connected"
            frmMain.ToolStripStatusLabel8.BackColor = Color.Green
            frmMain.btnFind.Enabled = True
            Return True
        Catch ex As Exception
            FBCon.Close()
            frmMain.ToolStripStatusLabel8.Text = "Disconnected"
            frmMain.ToolStripStatusLabel8.BackColor = Color.LightCoral
            frmMain.btnFind.Enabled = False
            Return False
        End Try



    End Function

    'VIEW DATABASE CONNECTION INFO
    Public Sub dbConf()

        Dim ConfCategory As String
        Try

            'VIEW CONFIG.INF TO VIEW DATABASE CONFIG
            Dim COnfReader As StreamReader = New StreamReader(My.Application.Info.DirectoryPath & " \connection.info")
            ConfCategory = COnfReader.ReadLine()
            Do

                If InStr(ConfCategory.ToUpper, "HOST") Then    'SEARCH FOR IP KEYWORD
                    FirebirdIP = Microsoft.VisualBasic.Right(ConfCategory, Microsoft.VisualBasic.Len(ConfCategory) - InStr(ConfCategory, "'"))
                    FirebirdIP = Microsoft.VisualBasic.Left(FirebirdIP, Microsoft.VisualBasic.Len(FirebirdIP) - 1)
                    'txtIP.Text = FirebirdIP
                ElseIf InStr(ConfCategory.ToUpper, "DATABASE") Then   'SEARCH FOR FirebirdDbase KEYWORD
                    FirebirdDbase = Microsoft.VisualBasic.Right(ConfCategory, Microsoft.VisualBasic.Len(ConfCategory) - InStr(ConfCategory, "'"))
                    FirebirdDbase = Microsoft.VisualBasic.Left(FirebirdDbase, Microsoft.VisualBasic.Len(FirebirdDbase) - 1)
                ElseIf InStr(ConfCategory.ToUpper, "PASSWORD") Then   'SEARCH FOR PASSWORD KEYWORD
               



                    FirebirdPassword = Microsoft.VisualBasic.Right(ConfCategory, Microsoft.VisualBasic.Len(ConfCategory) - InStr(ConfCategory, "'"))
                    FirebirdPassword = Microsoft.VisualBasic.Left(FirebirdPassword, Microsoft.VisualBasic.Len(FirebirdPassword) - 1)

                   

                    Dim cypherTextBytes As Byte() = Convert.FromBase64String(FirebirdPassword)
                    Dim memoryStream As MemoryStream = New MemoryStream(cypherTextBytes)
                    Dim cryptoStream As CryptoStream = New CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read)
                    Dim plainTextBytes(cypherTextBytes.Length) As Byte
                    Dim decryptedByteCount As Integer = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length)
                    memoryStream.Close()
                    cryptoStream.Close()
                    FirebirdPassword = enc.GetString(plainTextBytes, 0, decryptedByteCount)


                End If
                ConfCategory = COnfReader.ReadLine()
            Loop Until ConfCategory Is Nothing
            COnfReader.Close()
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try

    End Sub

   


End Module
