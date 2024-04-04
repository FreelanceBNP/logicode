Imports System.Data.Common
Imports System.Diagnostics.Eventing
Imports Microsoft.AspNet.SignalR.Client
Imports Org.LLRP.LTK.LLRPV1
Imports Org.LLRP.LTK.LLRPV1.Impinj

Public Class Form1
    Private Shared reportCount As Integer = 0
    Private Shared eventCount As Integer = 0
    Private hubConnection As HubConnection

    Dim reader As LLRPClient = New LLRPClient()

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Console.WriteLine("Adding Event Handlers")

        ' reader.OnReaderEventNotification += New delegateReaderEventNotification(Reader_OnReaderEventNotification)
        reader.OnRoAccessReportReceived += New delegateRoAccessReport(Reader_OnRoAccessReportReceived)


    End Sub

    Private Sub setText(ByVal Text As String)
        Try
            Me.Invoke(New MethodInvoker(Sub()
                                            listBox1.Items.Add(Text)
                                            Dim nItems As Integer = CInt(listBox1.Height / listBox1.ItemHeight)
                                            listBox1.TopIndex = listBox1.Items.Count - nItems
                                        End Sub))
        Catch err As Exception
            Console.WriteLine(err.Message)
        End Try
    End Sub

    Public Async Sub Reader_OnRoAccessReportReceived(ByVal msg As MSG_RO_ACCESS_REPORT)
        Try
            If msg.TagReportData Is Nothing Then Return

            For i As Integer = 0 To msg.TagReportData.Length - 1
                reportCount += 1
                Dim epc As String
                Dim antenna As Long

                If msg.TagReportData(i).EPCParameter(0).GetType() Is GetType(PARAM_EPC_96) Then
                    epc = DirectCast(msg.TagReportData(i).EPCParameter(0), PARAM_EPC_96).EPC.ToHexString()
                    antenna = msg.TagReportData(i).AntennaID.AntennaID
                Else
                    epc = DirectCast(msg.TagReportData(i).EPCParameter(0), PARAM_EPCData).EPC.ToHexString()
                    antenna = msg.TagReportData(i).AntennaID.AntennaID
                End If

                Await Task.Run(Sub() setText(antenna & " | " & epc))
                'Await Task.Run(Sub() SendToSocket(epc, antenna))
            Next
        Catch
            reader.Dispose()
        End Try
    End Sub

    Private Sub btn_start_Click(sender As Object, e As EventArgs) Handles btn_start.Click
        button2.Enabled = True
        btn_start.Enabled = False
        status_reader.Text = "Connecting.."
        status_reader.BackColor = Color.Thistle
        status_reader.ForeColor = Color.White

        If reader.ReaderName IsNot Nothing Then
            reader.Close()
            reader = New LLRPClient()
        End If

        ' string readerName = "impinj-15-50-20"
        Dim readerName As String = "impinj-15-50-20" 'hostname

        listBox1.Items.Clear()
        listBox2.Items.Clear()
        Impinj_Installer.Install()

        ' #region EventHandlers
        Console.WriteLine("Adding Event Handlers
")
        '  reader.OnReaderEventNotification += new delegateReaderEventNotification(Reader_OnReaderEventNotification);
        AddHandler reader.OnRoAccessReportReceived, New delegateRoAccessReport(Reader_OnRoAccessReportReceived)

        ' #endregion

        listBox2.Items.Add("Connecting.. Reader")
        Dim status As ENUM_ConnectionAttemptStatusType = Nothing
        Dim ret As Boolean = reader.Open(readerName, 5000, status)

        If Not ret OrElse status <> ENUM_ConnectionAttemptStatusType.Success Then
            listBox2.Items.Add("Failed to Connect to Reader")
            status_reader.Text = "Disconnect"
            status_reader.BackColor = Color.Red
            status_reader.ForeColor = Color.White
            button2.Enabled = False
            btn_start.Enabled = True
            Return
        End If

        listBox2.Items.Add("Connect Success")
        status_reader.Text = "Connected"
        status_reader.BackColor = Color.YellowGreen
        status_reader.ForeColor = Color.White
        button2.Enabled = True

        '********



    End Sub
End Class
