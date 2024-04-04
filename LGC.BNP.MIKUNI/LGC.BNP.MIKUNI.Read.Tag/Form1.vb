Imports System
Imports System.Collections.Generic
Imports System.Drawing
Imports System.Threading.Tasks
Imports System.Windows.Forms

Imports System.Net.Http
Imports System.Threading
Imports Org.LLRP.LTK.LLRPV1
Imports Org.LLRP.LTK.LLRPV1.DataType
Imports Org.LLRP.LTK.LLRPV1.Impinj
Imports Microsoft.AspNetCore.SignalR.Client

Namespace LGC.BNP.MIKUNI.Read.Tag

    Public Partial Class Form1
        Inherits Form

        Private Shared reportCount As Integer = 0
        Private Shared eventCount As Integer = 0
        Private hubConnection As HubConnection

        Public Sub New()
            InitializeComponent()
            'InitializeSignalRConnection();
            button2.Enabled = False
            status_segnalr.Enabled = False
            status_reader.Enabled = False
            btn_start.Enabled = True
            btn_con_signalr.Enabled = False

            'myTimer.Tick += new EventHandler(TimerEventProcessor);
            ' Sets the timer interval to 5 seconds.
            'myTimer.Interval = Properties.Settings.Default.intervalTime;

        End Sub

        Private Async Sub InitializeSignalRConnection()
            Dim host = Properties.Settings.Default.Socket
            hubConnection = New HubConnectionBuilder().WithUrl(host & "notifications").Build()

            hubConnection.[On](Of String)("onReceiveAlarm", Async Function(message)
                                                                ' Handle received message asynchronously if needed
                                                                Await AppendMessage(message)
                                                            End Function)

            Try
                Await hubConnection.StartAsync()
                If hubConnection.State = HubConnectionState.Connected Then
                    listBox2.Items.Add("Connect  WebSocket seccess")
                    btn_con_signalr.Enabled = False
                    btn_start.Enabled = True
                    status_segnalr.Text = "Connected"
                    status_segnalr.BackColor = Color.YellowGreen
                    status_segnalr.ForeColor = Color.White

                End If
            Catch ex As Exception
                ' MessageBox.Show($"Connection error: {ex.Message}", "Error");
                listBox2.Items.Add(ex.Message)
                btn_con_signalr.Enabled = True
                btn_start.Enabled = False
                status_segnalr.Text = "Disconnect"
                status_segnalr.BackColor = Color.Red
                status_segnalr.ForeColor = Color.White

            End Try
        End Sub
        Private Async Function AppendMessage(message As String) As Task
            ' Update your UI with the received message
            If InvokeRequired Then
                Invoke(New Action(Sub() AppendMessage(message)))
            Else
                Try
                    listBox1.Items.Add(message)
                    If reader.IsConnected Then
                        Await Task.Run(Function() GPO1_ON())
                    Else
                        reader.Dispose()
                    End If
                Catch ex As Exception
                    listBox1.Items.Add("Fail" & message)
                    reader.Dispose()
                End Try
            End If
        End Function

        Private Sub Form1_Resize(sender As Object, e As EventArgs)
            If WindowState = FormWindowState.Minimized Then
                Hide()
                notifyIcon.Visible = True
            End If
        End Sub


        Private reader As LLRPClient = New LLRPClient()

        Private Async Sub button1_Click(sender As Object, e As EventArgs) 'start
            button2.Enabled = True
            btn_start.Enabled = False
            status_reader.Text = "Connecting.."
            status_reader.BackColor = Color.Thistle
            status_reader.ForeColor = Color.White

            If Not Equals(reader.ReaderName, Nothing) Then
                reader.Close()
                reader = New LLRPClient()
            End If
            ' string readerName = "impinj-15-50-20";
            Dim readerName = Properties.Settings.Default.Hostname

            listBox1.Items.Clear()
            listBox2.Items.Clear()
            Call Impinj_Installer.Install()

#Region "EventHandlers"
            If True Then
                Console.WriteLine("Adding Event Handlers" & Microsoft.VisualBasic.Constants.vbLf)
                ' reader.OnReaderEventNotification += new delegateReaderEventNotification(Reader_OnReaderEventNotification);
                AddHandler reader.OnRoAccessReportReceived, New delegateRoAccessReport(AddressOf Reader_OnRoAccessReportReceived)
            End If
            ' Open the reader connection.  Timeout after 5 seconds
            Dim status As ENUM_ConnectionAttemptStatusType = Nothing
#End Region

#Region "Connecting"
            If True Then

                listBox2.Items.Add("Connecting.. Reader")
                Dim ret = reader.Open(readerName, 5000, status)

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
            End If

            ' Send the custom message and wait for 8 seconds

            Dim msg_err As MSG_ERROR_MESSAGE = Nothing, msg_rsp As MSG_IMPINJ_ENABLE_EXTENSIONS_RESPONSE = Nothing
#End Region

#Region "EnableExtensions"
            If True Then
                Console.WriteLine("Enabling Impinj Extensions" & Microsoft.VisualBasic.Constants.vbLf)

                Dim imp_msg As MSG_IMPINJ_ENABLE_EXTENSIONS = New MSG_IMPINJ_ENABLE_EXTENSIONS() With {
    .MSG_ID = 1 ' note this doesn't need to be set as the library will default
}
                Dim cust_rsp = reader.CUSTOM_MESSAGE(imp_msg, msg_err, 8000)

                If CSharpImpl.__Assign(msg_rsp, TryCast(cust_rsp, MSG_IMPINJ_ENABLE_EXTENSIONS_RESPONSE)) IsNot Nothing Then
                    If msg_rsp.LLRPStatus.StatusCode <> ENUM_StatusCode.M_Success Then
                        Console.WriteLine(msg_rsp.LLRPStatus.StatusCode.ToString())
                        listBox2.Items.Add(msg_rsp.LLRPStatus.StatusCode.ToString())
                        reader.Close()
                        Return
                    End If
                ElseIf msg_err IsNot Nothing Then
                    Console.WriteLine(msg_err.ToString())
                    listBox2.Items.Add(msg_err.ToString())
                    reader.Close()
                    Return
                Else
                    Console.WriteLine("Enable Extensions Command Timed out" & Microsoft.VisualBasic.Constants.vbLf)
                    listBox2.Items.Add("Enable Extensions Command Timed out")
                    reader.Close()
                    Return
                End If
            End If

            ' if SET_READER_CONFIG affects antennas it could take several seconds to return
            Dim msg_err As MSG_ERROR_MESSAGE = Nothing
#End Region

#Region "FactoryDefault"
            If True Then
                Console.WriteLine("Factory Default the Reader" & Microsoft.VisualBasic.Constants.vbLf)

                ' factory default the reader
                Dim msg_cfg As MSG_SET_READER_CONFIG = New MSG_SET_READER_CONFIG() With {
    .ResetToFactoryDefault = True,
    .MSG_ID = 2 ' this doesn't need to be set as the library will default
}
                Dim rsp_cfg = reader.SET_READER_CONFIG(msg_cfg, msg_err, 12000)

                If rsp_cfg IsNot Nothing Then
                    If rsp_cfg.LLRPStatus.StatusCode <> ENUM_StatusCode.M_Success Then
                        Console.WriteLine(rsp_cfg.LLRPStatus.StatusCode.ToString())
                        reader.Close()
                        Return
                    End If
                ElseIf msg_err IsNot Nothing Then
                    Console.WriteLine(msg_err.ToString())
                    reader.Close()
                    Return
                Else
                    Console.WriteLine("SET_READER_CONFIG Command Timed out" & Microsoft.VisualBasic.Constants.vbLf)
                    reader.Close()
                    Return
                End If
            End If

            Dim msg_err As MSG_ERROR_MESSAGE = Nothing
#End Region

#Region "ADDRoSpecWithObjects"
            If True Then
                Console.WriteLine("Adding RoSpec" & Microsoft.VisualBasic.Constants.vbLf)

                ' set up the basic parameters in the ROSpec. Use all the defaults from the reader
                Dim msg As MSG_ADD_ROSPEC = New MSG_ADD_ROSPEC() With {
                        .ROSpec = New PARAM_ROSpec With {
        .CurrentState = ENUM_ROSpecState.Disabled,
        .Priority = &H00,
        .ROSpecID = 1111,

                                ' setup the start and stop triggers in the Boundary Spec
                                .ROBoundarySpec = New PARAM_ROBoundarySpec With {
                                        .ROSpecStartTrigger = New PARAM_ROSpecStartTrigger With {
                .ROSpecStartTriggerType = ENUM_ROSpecStartTriggerType.Null
            },

                                        .ROSpecStopTrigger = New PARAM_ROSpecStopTrigger With {
                .ROSpecStopTriggerType = ENUM_ROSpecStopTriggerType.Null,
                .DurationTriggerValue = 0 ' ignored by reader
            }
        },

        ' Add a single Antenna Inventory to the ROSpec
        .SpecParameter = New UNION_SpecParameter()
    }
}
                Dim aiSpec As PARAM_AISpec = New PARAM_AISpec() With {
    .AntennaIDs = New UInt16Array(),
                        .AISpecStopTrigger = New PARAM_AISpecStopTrigger With {
        .AISpecStopTriggerType = ENUM_AISpecStopTriggerType.Null
    },

    ' use all the defaults from the reader.  Just specify the minimum required
    .InventoryParameterSpec = New PARAM_InventoryParameterSpec(0) {}
}
                aiSpec.InventoryParameterSpec(0) = New PARAM_InventoryParameterSpec With {
    .InventoryParameterSpecID = 1234,
    .ProtocolID = ENUM_AirProtocols.EPCGlobalClass1Gen2
}
                aiSpec.AntennaIDs.Add(0)       ' all antennas

                msg.ROSpec.SpecParameter.Add(aiSpec)
                Dim rsp = reader.ADD_ROSPEC(msg, msg_err, 12000)
                If rsp IsNot Nothing Then
                    If rsp.LLRPStatus.StatusCode <> ENUM_StatusCode.M_Success Then
                        Console.WriteLine(rsp.LLRPStatus.StatusCode.ToString())
                        reader.Close()
                        Return
                    End If
                ElseIf msg_err IsNot Nothing Then
                    Console.WriteLine(msg_err.ToString())
                    reader.Close()
                    Return
                Else
                    Console.WriteLine("ADD_ROSPEC Command Timed out" & Microsoft.VisualBasic.Constants.vbLf)
                    reader.Close()
                    Return
                End If
            End If
            Dim msg_err As MSG_ERROR_MESSAGE = Nothing
#End Region

#Region "EnableRoSpec"
            If True Then
                Console.WriteLine("Enabling RoSpec" & Microsoft.VisualBasic.Constants.vbLf)
                Dim msg As MSG_ENABLE_ROSPEC = New MSG_ENABLE_ROSPEC() With {
    .ROSpecID = 1111 ' this better match the ROSpec we created above
}
                Dim rsp = reader.ENABLE_ROSPEC(msg, msg_err, 12000)
                If rsp IsNot Nothing Then
                    If rsp.LLRPStatus.StatusCode <> ENUM_StatusCode.M_Success Then
                        Console.WriteLine(rsp.LLRPStatus.StatusCode.ToString())
                        reader.Close()
                        Return
                    End If
                ElseIf msg_err IsNot Nothing Then
                    Console.WriteLine(msg_err.ToString())
                    reader.Close()
                    Return
                Else
                    Console.WriteLine("ENABLE_ROSPEC Command Timed out" & Microsoft.VisualBasic.Constants.vbLf)
                    reader.Close()
                    Return
                End If
            End If
            Dim msg_err As MSG_ERROR_MESSAGE = Nothing
#End Region

#Region "StartRoSpec"
            If True Then
                Console.WriteLine("Starting RoSpec" & Microsoft.VisualBasic.Constants.vbLf)
                listBox2.Items.Add("Running")
                Dim msg As MSG_START_ROSPEC = New MSG_START_ROSPEC() With {
    .ROSpecID = 1111 ' this better match the RoSpec we created above
}
                Dim rsp = reader.START_ROSPEC(msg, msg_err, 12000)
                If rsp IsNot Nothing Then
                    If rsp.LLRPStatus.StatusCode <> ENUM_StatusCode.M_Success Then
                        Console.WriteLine(rsp.LLRPStatus.StatusCode.ToString())
                        reader.Close()
                        Return
                    End If
                ElseIf msg_err IsNot Nothing Then
                    Console.WriteLine(msg_err.ToString())
                    listBox2.Items.Add(msg_err.ToString())
                    reader.Close()
                    Return
                Else
                    Console.WriteLine("START_ROSPEC Command Timed out" & Microsoft.VisualBasic.Constants.vbLf)
                    listBox2.Items.Add("START_ROSPEC Command Timed out")
                    reader.Close()
                    Return
                End If
            End If
#End Region
            ' wait around to collect some data
            Call myTimer.Start()
        End Sub

        Private Sub button2_Click(sender As Object, e As EventArgs) 'stop

            ' Note that if SET_READER_CONFIG affects antennas it could take several seconds to return
            Dim msg_err As MSG_ERROR_MESSAGE = Nothing, msg_err As MSG_ERROR_MESSAGE = Nothing
            Try
                Call myTimer.Stop()
                If reader.IsConnected Then
                    button2.Enabled = False
                    btn_start.Enabled = True
                    listBox2.Items.Clear()
                    listBox2.Items.Add("Waiting")

                    status_reader.Text = "Disconnect"
                    status_reader.BackColor = Color.Red
                    status_reader.ForeColor = Color.White

#Region "StopRoSpec"
                    If True Then
                        Console.WriteLine("Stopping RoSpec" & Microsoft.VisualBasic.Constants.vbLf)
                        Dim msg As MSG_STOP_ROSPEC = New MSG_STOP_ROSPEC() With {
    .ROSpecID = 1111 ' this better match the RoSpec we created above
}
                        Dim rsp = reader.STOP_ROSPEC(msg, msg_err, 12000)
                        If rsp IsNot Nothing Then
                            If rsp.LLRPStatus.StatusCode <> ENUM_StatusCode.M_Success Then
                                Console.WriteLine(rsp.LLRPStatus.StatusCode.ToString())
                                reader.Close()
                                Return
                            End If
                        ElseIf msg_err IsNot Nothing Then
                            Console.WriteLine(msg_err.ToString())
                            reader.Close()
                            Return
                        Else
                            Console.WriteLine("STOP_ROSPEC Command Timed out" & Microsoft.VisualBasic.Constants.vbLf)
                            reader.Close()
                            reader.Dispose()
                            Return
                        End If
                    End If
#End Region

#Region "Clean Up Reader Configuration"
                    If True Then
                        Console.WriteLine("Factory Default the Reader" & Microsoft.VisualBasic.Constants.vbLf)

                        ' factory default the reader
                        Dim msg_cfg As MSG_SET_READER_CONFIG = New MSG_SET_READER_CONFIG() With {
    .ResetToFactoryDefault = True,
    .MSG_ID = 2 ' note this doesn't need to be set as the library will default
}
                        Dim rsp_cfg = reader.SET_READER_CONFIG(msg_cfg, msg_err, 12000)

                        If rsp_cfg IsNot Nothing Then
                            If rsp_cfg.LLRPStatus.StatusCode <> ENUM_StatusCode.M_Success Then
                                Console.WriteLine(rsp_cfg.LLRPStatus.StatusCode.ToString())
                                listBox1.Items.Add(rsp_cfg.LLRPStatus.StatusCode.ToString())
                                reader.Close()
                                Return
                            End If
                        ElseIf msg_err IsNot Nothing Then
                            Console.WriteLine(msg_err.ToString())
                            listBox2.Items.Add(msg_err.ToString())
                            reader.Close()
                            Return
                        Else
                            Console.WriteLine("SET_READER_CONFIG Command Timed out" & Microsoft.VisualBasic.Constants.vbLf)
                            listBox2.Items.Add("SET_READER_CONFIG Command Timed out")
                            reader.Close()
                            Return
                        End If
                    End If
#End Region
                    reader.Dispose()
                    reader = New LLRPClient()
                    listBox2.Items.Add("Stop")
                Else
                    reader.Dispose()
                End If

            Catch
                reader.Dispose()
            End Try

        End Sub
        Private Sub setText(Text As String)
            Try
                Invoke(New MethodInvoker(Sub()
                                             listBox1.Items.Add(Text)
                                             Dim nItems As Integer = listBox1.Height / listBox1.ItemHeight
                                             listBox1.TopIndex = listBox1.Items.Count - nItems
                                         End Sub))
            Catch err As Exception
                Console.WriteLine(err.Message)
            End Try
        End Sub

        Async Public Sub Reader_OnRoAccessReportReceived(msg As MSG_RO_ACCESS_REPORT)
            Try

                If msg.TagReportData Is Nothing Then Return

                For i = 0 To msg.TagReportData.Length - 1
                    reportCount += 1
                    Dim epc As String
                    Dim antenna As Long


                    If msg.TagReportData(i).EPCParameter(0).GetType() Is GetType(PARAM_EPC_96) Then
                        epc = CType(msg.TagReportData(i).EPCParameter(0), PARAM_EPC_96).EPC.ToHexString()
                        antenna = msg.TagReportData(i).AntennaID.AntennaID
                    Else
                        epc = CType(msg.TagReportData(i).EPCParameter(0), PARAM_EPCData).EPC.ToHexString()
                        antenna = msg.TagReportData(i).AntennaID.AntennaID

                    End If
                    'var CovertToSting = "";
                    'CovertToSting = getTag_id(epc);
                    Await Task.Run(Sub() setText(antenna.ToString() & " | " & epc))
                    Await Task.Run(Function() SendToSocket(epc, antenna))

                Next

            Catch
                reader.Dispose()

            End Try
        End Sub

        Private Sub notifyIcon_DoubleClick(sender As Object, e As EventArgs)
            Show()
            WindowState = FormWindowState.Normal
            notifyIcon.Visible = False
        End Sub
        Private Async Function GPO1_ON() As Task
            Try
                ' this routine turns on 3.3v power in LLRP GPO1
                ' LLRP GPO1 = Hardware PIN 14 / GPOUT0MSG_SET_READER_CONFIG 
                If reader.IsConnected Then
                    Dim msg_err As MSG_ERROR_MESSAGE = New MSG_ERROR_MESSAGE()
                    Dim msg As MSG_SET_READER_CONFIG = New MSG_SET_READER_CONFIG()

                    msg.GPOWriteData = New PARAM_GPOWriteData(0) {}
                    msg.GPOWriteData(0) = New PARAM_GPOWriteData()
                    msg.GPOWriteData(0).GPOData = True
                    msg.GPOWriteData(0).GPOPortNumber = 1

                    Dim rsp = reader.SET_READER_CONFIG(msg, msg_err, 12000)
                    If rsp IsNot Nothing Then
                        Dim time = Properties.Settings.Default.Worktime
                        Await Task.Run(Sub() Thread.Sleep(time))
                        GPO1_OFF()
                    ElseIf msg_err IsNot Nothing Then
                        listBox2.Items.Add(rsp.ToString())
                    Else
                        listBox2.Items.Add("Commmand time out!")
                    End If
                Else
                    reader.Dispose()
                End If
            Catch ex As Exception
                reader.Dispose()
                listBox2.Items.Add(ex.Message)
            End Try
        End Function
        Private Sub GPO1_OFF()
            Try
                If reader.IsConnected Then
                    ' this routine turns off power in LLRP GPO1
                    ' LLRP GPO1 = Hardware PIN 14 / GPOUT0MSG_SET_READER_CONFIG 
                    Dim msg As MSG_SET_READER_CONFIG = New MSG_SET_READER_CONFIG()
                    msg.GPOWriteData = New PARAM_GPOWriteData(0) {}
                    msg.GPOWriteData(0) = New PARAM_GPOWriteData()
                    msg.GPOWriteData(0).GPOData = False
                    msg.GPOWriteData(0).GPOPortNumber = 1
                    Dim msg_err As MSG_ERROR_MESSAGE = New MSG_ERROR_MESSAGE()
                    Dim rsp = reader.SET_READER_CONFIG(msg, msg_err, 12000)
                    'listBox2.Items.Add("GPO1 OFF");
                    If rsp IsNot Nothing Then
                    ElseIf msg_err IsNot Nothing Then
                        listBox2.Items.Add(rsp.ToString())
                    Else
                        listBox2.Items.Add("Commmand time out!")
                    End If
                Else
                    reader.Dispose()
                End If

            Catch
                reader.Dispose()
            End Try
        End Sub

        Private Async Sub btn_clear_DoubleClick(sender As Object, e As EventArgs)
            listBox1.Items.Clear()
            'await Task.Run(() => ClearText());
        End Sub
        Private Function getTag_id(text As String) As String
            Dim tag_code = ""
            If Not String.IsNullOrEmpty(text) Then
                Select Case text
                    Case "E2801191A5030062E59558C1"
                        tag_code = "DMIT10033"
                    Case "E2801191A5030062E59558C2"
                        tag_code = "DMIT10030"
                    Case "E2801191A5030062E59676DA"
                        tag_code = "DMIT10346"
                    Case "E2801191A5030062E59676D8"
                        tag_code = "DMIT10409"
                    Case "E280689400005029A3B58C3D"
                        tag_code = "DMIT10079"
                    Case Else
                        tag_code = text
                End Select
            End If
            Return tag_code
        End Function

        Private Sub btn_con_signalr_Click(sender As Object, e As EventArgs)
            status_segnalr.Text = "Connecting.."
            status_segnalr.ForeColor = Color.White
            status_segnalr.BackColor = Color.Thistle
            InitializeSignalRConnection()
        End Sub


        Private Sub Form1_FormClosing(sender As Object, e As FormClosingEventArgs)
            Dim msg_err As MSG_ERROR_MESSAGE = Nothing
            If reader.IsConnected Then
                GPO1_OFF()
#Region "StopRoSpec"
                If True Then
                    Console.WriteLine("Stopping RoSpec" & Microsoft.VisualBasic.Constants.vbLf)
                    Dim msg As MSG_STOP_ROSPEC = New MSG_STOP_ROSPEC() With {
    .ROSpecID = 1111 ' this better match the RoSpec we created above
}
                    Dim rsp = reader.STOP_ROSPEC(msg, msg_err, 12000)
                    If rsp IsNot Nothing Then
                        If rsp.LLRPStatus.StatusCode <> ENUM_StatusCode.M_Success Then
                            Console.WriteLine(rsp.LLRPStatus.StatusCode.ToString())
                            reader.Close()
                            Return
                        End If
                    ElseIf msg_err IsNot Nothing Then
                        Console.WriteLine(msg_err.ToString())
                        reader.Close()
                        Return
                    Else
                        Console.WriteLine("STOP_ROSPEC Command Timed out" & Microsoft.VisualBasic.Constants.vbLf)
                        reader.Close()
                        Return
                    End If
                End If
#End Region
            End If
        End Sub

        Public Function ConvertStringToHex(asciiString As String) As String
            Dim hex = ""
            For Each c In asciiString
                Dim tmp As Integer = c
                hex += String.Format("{0:x2}", CUInt(Convert.ToUInt32(tmp.ToString())))
            Next
            Return hex
        End Function

        Public Function ConvertHexToString(hexString As String) As String
            Dim ascii = String.Empty

            For i = 0 To hexString.Length - 1 Step 2
                Dim hs = String.Empty

                hs = hexString.Substring(i, 2)
                Dim decval = Convert.ToUInt32(hs, 16)
                Dim character = Convert.ToChar(decval)
                ascii += character

            Next

            Return ascii
        End Function

        Private Async Function SendToSocket(Tag As String, atenna_id As Long) As Task
            Try
                Dim atenta_port_in = Properties.Settings.Default.AtentaPortIN
                Dim atenta_port_out = Properties.Settings.Default.AtentaPortOut
                Dim type = String.Empty

                If atenna_id = atenta_port_in Then
                    type = "in"
                ElseIf atenna_id = atenta_port_out Then
                    type = "out"
                End If
                Dim host = Properties.Settings.Default.Socket
                Dim url = host & "Notification/autobank/pushnoti"
                Dim client = New HttpClient()
                Dim data = New Dictionary(Of String, String) From {
    {"tag_code", Tag},
    {"type", type}
}
                Dim res = client.PostAsync(url, New FormUrlEncodedContent(data))
            Catch e As Exception
                Throw e
            End Try
        End Function
        Private Async Function RandomTag() As Task
            Try
                Dim tag_code = {"DMIT10081", "DMIT10117", "DMIT10346", "DMIT10409", "DMIT10079"}
                Dim rnd As Random = New Random()
                Dim random = rnd.Next(0, 5)
                Dim ran12 = rnd.Next(1, 3)
                Dim tag_ran = tag_code(random)
                Await Task.Run(Sub() setText(tag_ran))
                Await Task.Run(Function() SendToSocket(tag_ran, ran12))
            Catch e As Exception
                Throw e
            End Try



        End Function
        Private Shared myTimer As Windows.Forms.Timer = New Windows.Forms.Timer()

        Private Async Sub TimerEventProcessor(myObject As Object, myEventArgs As EventArgs)
            Call myTimer.Stop()

            ' Displays a message box asking whether to continue running the timer.
            Await Task.Run(Function() RandomTag())
            myTimer.Enabled = True


        End Sub

        Private Sub Form1_Load(sender As Object, e As EventArgs)

        End Sub

        Private Class CSharpImpl
            <Obsolete("Please refactor calling code to use normal Visual Basic assignment")>
            Shared Function __Assign(Of T)(ByRef target As T, value As T) As T
                target = value
                Return value
            End Function
        End Class
    End Class
End Namespace
